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

namespace HurlStudio.UI.ViewModels.Documents
{
    public class FolderDocumentViewModel : DocumentBase, IExtendedAsyncDockable, IEditorDocument
    {
        public event EventHandler<SettingEvaluationChangedEventArgs>? SettingAdded;
        public event EventHandler<SettingEvaluationChangedEventArgs>? SettingRemoved;

        private HurlFolderContainer? _folderContainer;
        private EditorViewViewModel _editorViewViewModel;
        private IEditorService _editorService;
        private MainWindow _mainWindow;
        private bool _hasChanges;
        private OrderedObservableCollection<HurlSettingSection> _settingSections;

        public FolderDocumentViewModel(EditorViewViewModel editorViewViewModel, IEditorService editorService, MainWindow mainWindow)
        {
            this.CanFloat = false;
            this.CanPin = false;
            this.HasChanges = false;

            _editorViewViewModel = editorViewViewModel;
            _settingSections = new OrderedObservableCollection<HurlSettingSection>();
            _editorService = editorService;
            _mainWindow = mainWindow;
        }

        public HurlFolderContainer? FolderContainer
        {
            get => _folderContainer;
            set
            {
                this.SetProperty(ref _folderContainer, value);
                if (_folderContainer != null)
                {
                    _folderContainer.PropertyChanged -= this.On_Folder_PropertyChanged;
                    _folderContainer.PropertyChanged += this.On_Folder_PropertyChanged;

                    _folderContainer.CollectionComponentPropertyChanged -= this.On_Folder_CollectionComponentPropertyChanged;
                    _folderContainer.CollectionComponentPropertyChanged += this.On_Folder_CollectionComponentPropertyChanged;
                }

                this.RefreshTitle();
            }
        }

        /// <summary>
        /// When a component property changes, set hasChanges
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_Folder_CollectionComponentPropertyChanged(object? sender, HurlContainerPropertyChangedEventArgs e)
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
            get => _folderContainer;
        }

        public HurlCollectionContainer? UnderlyingCollection
        {
            get => _folderContainer.CollectionContainer;
        }

        /// <summary>
        /// Build the Title for display in the dock control tab strip
        /// </summary>
        private void RefreshTitle()
        {
            if (_folderContainer != null)
            {
                string? collectionRootFolder = Path.GetDirectoryName(_folderContainer.CollectionContainer.Collection.CollectionFileLocation);

                if (collectionRootFolder != null) {
                    string folderPath = Path.TrimEndingDirectorySeparator(_folderContainer.Folder.FolderLocation).ConvertDirectorySeparator();

                    this.Title = folderPath +
                                 (this.HasChanges ? "*" : string.Empty);
                }
                else
                {
                    this.Title = Localization.Localization.Common_Undefined;
                }
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
        private void On_Folder_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.FolderContainer.AbsoluteLocation))
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

            MessageBox.ButtonType decisionResult = await MessageBox.ShowDialog(
                _mainWindow,
                Localization.Localization.View_Editor_MessageBox_UnsavedChanges_Text + Environment.NewLine + this.FolderContainer?.AbsoluteLocation,
                Localization.Localization.View_Editor_MessageBox_UnsavedChanges_Title,
                [MessageBox.ButtonType.Save, MessageBox.ButtonType.Discard, MessageBox.ButtonType.Cancel],
                Icon.WarningColor
            );

            switch (decisionResult)
            {
                case MessageBox.ButtonType.Cancel: return DockableCloseMode.Cancel;
                case MessageBox.ButtonType.Save: return DockableCloseMode.Save;
                case MessageBox.ButtonType.Discard: return DockableCloseMode.Discard;
            }

            return DockableCloseMode.Undefined;
        }

        /// <summary>
        /// Tell the editor service to save the folder
        /// </summary>
        /// <returns></returns>
        public async Task Save()
        {
            await _editorService.SaveFolder(this);
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
            return $"{this.GetType().Name} {_folderContainer?.AbsoluteLocation}";
        }
    }
}
