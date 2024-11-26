using AvaloniaEdit.Document;
using HurlStudio.Common.Extensions;
using HurlStudio.Common.UI;
using HurlStudio.Model.Enums;
using HurlStudio.Model.EventArgs;
using HurlStudio.Model.HurlContainers;
using HurlStudio.Model.HurlSettings;
using HurlStudio.Services.Editor;
using HurlStudio.UI.Dock;
using HurlStudio.UI.Windows;
using HurlStudio.Utility;
using System;
using System.IO;
using System.Threading.Tasks;
using HurlStudio.UI.MessageBox;

namespace HurlStudio.UI.ViewModels.Documents
{
    public class EnvironmentDocumentViewModel : DocumentBase, IExtendedAsyncDockable, IEditorDocument
    {
        public event EventHandler<SettingEvaluationChangedEventArgs>? SettingAdded;
        public event EventHandler<SettingEvaluationChangedEventArgs>? SettingRemoved;

        private HurlEnvironmentContainer? _environmentContainer;
        private EditorViewViewModel _editorViewViewModel;
        private IEditorService _editorService;
        private MainWindow _mainWindow;
        private bool _hasChanges;
        private OrderedObservableCollection<HurlSettingSection> _settingSections;

        public EnvironmentDocumentViewModel(EditorViewViewModel editorViewViewModel, IEditorService editorService, MainWindow mainWindow)
        {
            this.CanFloat = false;
            this.CanPin = false;
            this.HasChanges = false;

            _editorViewViewModel = editorViewViewModel;
            _settingSections = new OrderedObservableCollection<HurlSettingSection>();
            _editorService = editorService;
            _mainWindow = mainWindow;
        }

        public HurlEnvironmentContainer? EnvironmentContainer
        {
            get => _environmentContainer;
            set
            {
                this.SetProperty(ref _environmentContainer, value);
                if (_environmentContainer != null)
                {
                    _environmentContainer.PropertyChanged -= this.On_Environment_PropertyChanged;
                    _environmentContainer.PropertyChanged += this.On_Environment_PropertyChanged;

                    _environmentContainer.CollectionComponentPropertyChanged -= this.On_Environment_CollectionComponentPropertyChanged;
                    _environmentContainer.CollectionComponentPropertyChanged += this.On_Environment_CollectionComponentPropertyChanged;
                }

                this.RefreshTitle();
            }
        }

        /// <summary>
        /// When a component property changes, set hasChanges
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_Environment_CollectionComponentPropertyChanged(object? sender, HurlContainerPropertyChangedEventArgs e)
        {
            this.HasChanges = true;
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

        public HurlContainerBase? HurlContainer
        {
            get => _environmentContainer;
        }

        public HurlCollectionContainer? UnderlyingCollection
        {
            get => null;
        }

        /// <summary>
        /// Build the Title for display in the dock control tab strip
        /// </summary>
        private void RefreshTitle()
        {
            if (_environmentContainer != null)
            {
                string? title = _environmentContainer.Environment.Name;
                if(string.IsNullOrEmpty(title))
                {
                    title = Path.GetFileName(_environmentContainer.EnvironmentFileLocation);
                }

                this.Title = title +
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
        /// Refresh Title on Property change of underlying folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_Environment_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.EnvironmentContainer.Environment.Name))
            {
                this.RefreshTitle();
            }
        }

        /// <summary>
        /// Ask for an option which way to close this document
        /// </summary>
        /// <returns></returns>
        public async Task<DockableCloseMode> AskAllowClose()
        {
            if (!this.HasChanges) return DockableCloseMode.Close;

            MessageBox.MessageBoxResult decisionResult = await MessageBox.MessageBox.ShowDialog(
                _mainWindow,
                Localization.Localization.View_Editor_MessageBox_UnsavedChanges_Text + System.Environment.NewLine + this.EnvironmentContainer?.EnvironmentFileLocation,
                Localization.Localization.View_Editor_MessageBox_UnsavedChanges_Title,
                [MessageBoxResult.Save, MessageBoxResult.Discard, MessageBoxResult.Cancel],
                Icon.WarningColor
            );

            switch (decisionResult)
            {
                case MessageBoxResult.Cancel: return DockableCloseMode.Cancel;
                case MessageBoxResult.Save: return DockableCloseMode.Save;
                case MessageBoxResult.Discard: return DockableCloseMode.Discard;
            }

            return DockableCloseMode.Undefined;
        }

        /// <summary>
        /// Tell the editor service to save the folder
        /// </summary>
        /// <returns></returns>
        public async Task Save()
        {
            await _editorService.SaveEnvironment(this);
        }

        public async Task Discard()
        {
            // Nothing to do here
        }

        /// <summary>
        /// Adds a setting to the document 
        /// </summary>
        /// <param name="settingContainer">The setting to be added</param>
        public void AddSetting(HurlSettingContainer settingContainer, int idx = 0)
        {
            HurlSettingSection section = settingContainer.SettingSection;
            if (!_settingSections.Contains(section)) throw new ArgumentException($"{section} not in {nameof(_settingSections)}");

            section.SettingContainers.Insert(idx, settingContainer);
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
            HurlSettingSection section = settingContainer.SettingSection;
            if (!_settingSections.Contains(section)) throw new ArgumentException($"{section} not in {nameof(_settingSections)}");
            if (!section.SettingContainers.Contains(settingContainer)) throw new ArgumentException($"{settingContainer} not in {nameof(section.SettingContainers)}");

            section.SettingContainers.Remove(settingContainer);
            section.Document.HasChanges = true;

            this.SettingRemoved?.Invoke(this, new SettingEvaluationChangedEventArgs(settingContainer));
        }

        /// <summary>
        /// Returns a unique id for this document
        /// </summary>
        /// <returns></returns>
        public string GetId()
        {
            return this.Id;
        }

        public override string ToString()
        {
            return $"{this.GetType().Name} {_environmentContainer?.EnvironmentFileLocation}";
        }
    }
}
