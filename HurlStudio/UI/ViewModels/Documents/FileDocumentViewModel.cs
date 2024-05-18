using Avalonia.Controls;
using AvaloniaEdit.Document;
using Dock.Model.Mvvm.Controls;
using HurlStudio.Common.UI;
using HurlStudio.Model.CollectionContainer;
using HurlStudio.Model.HurlSettings;
using MsBox.Avalonia.Models;
using MsBox.Avalonia;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HurlStudio.UI.Dock;
using HurlStudio.Model.Enums;
using HurlStudio.Services.Editor;
using MsBox.Avalonia.Base;
using HurlStudio.UI.Windows;
using HurlStudio.Utility;
using HurlStudio.Model.EventArgs;

namespace HurlStudio.UI.ViewModels.Documents
{
    public class FileDocumentViewModel : DocumentBase, IExtendedAsyncDockable
    {
        public event EventHandler<SettingEvaluationChangedEventArgs>? SettingAdded;
        public event EventHandler<SettingEvaluationChangedEventArgs>? SettingRemoved;

        private CollectionFile? _file;
        private EditorViewViewModel _editorViewViewModel;
        private TextDocument? _document;
        private IEditorService _editorService;
        private MainWindow _mainWindow;
        private bool _hasChanges;

        private OrderedObservableCollection<HurlSettingSection> _settingSections;

        public FileDocumentViewModel(EditorViewViewModel editorViewViewModel, IEditorService editorService, MainWindow mainWindow)
        {
            this.CanFloat = false;
            this.CanPin = false;
            this.HasChanges = false;

            _editorViewViewModel = editorViewViewModel;
            _settingSections = new OrderedObservableCollection<HurlSettingSection>();
            _editorService = editorService;
            _mainWindow = mainWindow;
        }

        public CollectionFile? File
        {
            get => _file;
            set
            {
                this.SetProperty(ref _file, value);
                if (_file != null)
                {
                    _file.PropertyChanged -= this.On_File_PropertyChanged;
                    _file.PropertyChanged += this.On_File_PropertyChanged;
                }

                this.RefreshTitle();
            }
        }

        public EditorViewViewModel EditorViewViewModel
        {
            get => _editorViewViewModel;
            set
            {
                _editorViewViewModel = value;
                this.Notify();
            }
        }

        public TextDocument? Document
        {
            get => _document;
            set
            {
                _document = value;
                if (_document != null)
                {
                    _document.TextChanged -= this.On_Document_TextChanged;
                    _document.TextChanged += this.On_Document_TextChanged;
                }

                this.Notify();
            }
        }

        public OrderedObservableCollection<HurlSettingSection> SettingSections
        {
            get => _settingSections;
            set
            {
                _settingSections = value;
                this.Notify();
            }
        }

        public bool HasChanges
        {
            get => _hasChanges;
            set
            {
                _hasChanges = value;
                this.Notify();
                this.RefreshTitle();
            }
        }

        /// <summary>
        /// Build the Title for display in the dock control tab strip
        /// </summary>
        private void RefreshTitle()
        {
            if (_file != null)
            {
                this.Title = Path.GetFileName(_file.Location) +
                             (this.HasChanges ? "*" : string.Empty);
            }
            else
            {
                this.Title = Localization.Localization.Common_Undefined;
            }

            // somehow required?
            this.Notify(nameof(this.Title));
        }

        /// <summary>
        /// Refresh Title on Property change of underlying file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_File_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.File.Location))
            {
                this.RefreshTitle();
            }
        }

        /// <summary>
        /// On Document text change
        /// -> Reevaluate Redo/Undo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_Document_TextChanged(object? sender, EventArgs e)
        {
            if (_editorViewViewModel != null && _document != null)
            {
                _editorViewViewModel.CanUndo = _document.UndoStack.CanUndo;
                _editorViewViewModel.CanRedo = _document.UndoStack.CanRedo;
            }
        }

        /// <summary>
        /// Ask for an option which way to close this document
        /// </summary>
        /// <returns></returns>
        public async Task<DockableCloseMode> AskAllowClose()
        {
            if (!this.HasChanges) return DockableCloseMode.Close;

            MessageBox.ButtonType decisionResult = await MessageBox.Show(
                    Localization.Localization.View_Editor_MessageBox_UnsavedChanges_Text,
                    Localization.Localization.View_Editor_MessageBox_UnsavedChanges_Title,
                    [MessageBox.ButtonType.Save, MessageBox.ButtonType.Discard, MessageBox.ButtonType.Cancel],
                    Icon.MessageBoxWarning
                );

            switch (decisionResult)
            {
                case MessageBox.ButtonType.Cancel: return DockableCloseMode.Cancel;
                case MessageBox.ButtonType.Save: return DockableCloseMode.Save;
                case MessageBox.ButtonType.Discard: return DockableCloseMode.Discard;
            }

            return DockableCloseMode.Undefined;
        }

        public async Task Save()
        {
            await _editorService.SaveFile(this);
        }

        public async Task Discard()
        {
            // TODO: Discard
        }

        /// <summary>
        /// Adds a setting to the document 
        /// </summary>
        /// <param name="settingContainer">The setting to be added</param>
        /// <exception cref="ArgumentException">If the settings' section isn't part of this document</exception>
        public void AddSetting(HurlSettingContainer settingContainer)
        {
            HurlSettingSection section = settingContainer.Section;
            if (!_settingSections.Contains(section)) throw new ArgumentException($"{section} not in {nameof(_settingSections)}");

            section.SettingContainers.Add(settingContainer);
            section.Document.HasChanges = true;

            this.SettingAdded?.Invoke(this, new SettingEvaluationChangedEventArgs(settingContainer));
        }

        /// <summary>
        /// Removes a setting from the document
        /// </summary>
        /// <param name="settingContainer">The setting to be removed</param>
        /// <exception cref="ArgumentException">If the settings' section isn't part of this document</exception>
        public void RemoveSetting(HurlSettingContainer settingContainer)
        {
            HurlSettingSection section = settingContainer.Section;
            if (!_settingSections.Contains(section)) throw new ArgumentException($"{section} not in {nameof(_settingSections)}");
            if(!section.SettingContainers.Contains(settingContainer)) throw new ArgumentException($"{settingContainer} not in {nameof(section.SettingContainers)}");

            section.SettingContainers.Remove(settingContainer);
            section.Document.HasChanges = true;

            this.SettingRemoved?.Invoke(this, new SettingEvaluationChangedEventArgs(settingContainer));
        }
    }
}
