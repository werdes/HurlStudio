﻿using AvaloniaEdit.Document;
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
    public class CollectionDocumentViewModel : DocumentBase, IExtendedAsyncDockable, IEditorDocument
    {
        public event EventHandler<SettingEvaluationChangedEventArgs>? SettingAdded;
        public event EventHandler<SettingEvaluationChangedEventArgs>? SettingRemoved;

        private HurlCollectionContainer? _collectionContainer;
        private EditorViewViewModel _editorViewViewModel;
        private IEditorService _editorService;
        private MainWindow _mainWindow;
        private bool _hasChanges;
        private OrderedObservableCollection<HurlSettingSection> _settingSections;

        public CollectionDocumentViewModel(EditorViewViewModel editorViewViewModel, IEditorService editorService, MainWindow mainWindow)
        {
            this.CanFloat = false;
            this.CanPin = false;
            this.HasChanges = false;

            _editorViewViewModel = editorViewViewModel;
            _settingSections = new OrderedObservableCollection<HurlSettingSection>();
            _editorService = editorService;
            _mainWindow = mainWindow;
        }

        public HurlCollectionContainer? CollectionContainer
        {
            get => _collectionContainer;
            set
            {
                this.SetProperty(ref _collectionContainer, value);
                if (_collectionContainer != null)
                {
                    _collectionContainer.PropertyChanged -= this.On_Collection_PropertyChanged;
                    _collectionContainer.PropertyChanged += this.On_Collection_PropertyChanged;

                    _collectionContainer.CollectionComponentPropertyChanged -= this.On_Collection_CollectionComponentPropertyChanged;
                    _collectionContainer.CollectionComponentPropertyChanged += this.On_Collection_CollectionComponentPropertyChanged;
                }

                this.RefreshTitle();
            }
        }

        /// <summary>
        /// When a component property changes, set hasChanges
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_Collection_CollectionComponentPropertyChanged(object? sender, HurlContainerPropertyChangedEventArgs e)
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
            get => _collectionContainer;
        }

        public HurlCollectionContainer? UnderlyingCollection
        {
            get => _collectionContainer;
        }

        /// <summary>
        /// Build the Title for display in the dock control tab strip
        /// </summary>
        private void RefreshTitle()
        {
            if (_collectionContainer != null)
            {
                this.Title = (_collectionContainer.Collection.Name ?? Path.GetFileName(_collectionContainer.Collection.CollectionFileLocation)) +
                                 (this.HasChanges ? "*" : string.Empty); ;
            }
            else
            {
                this.Title = Localization.Localization.Common_Undefined;
            }

            // somehow required?
            this.Notify(nameof(this.Title));
        }

        /// <summary>
        /// Refresh Title on Property change of underlying collection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_Collection_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == nameof(this.Collection.Collection.FileLocation))
            //{
            //    //this.RefreshTitle();
            //}

            this.HasChanges = true;
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
                Localization.Localization.View_Editor_MessageBox_UnsavedChanges_Text + Environment.NewLine + this.CollectionContainer?.Collection.CollectionFileLocation,
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
        /// Tell the editor service to save the collection
        /// </summary>
        /// <returns></returns>
        public async Task Save()
        {
            await _editorService.SaveCollection(this);
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
            return $"{this.GetType().Name} {_collectionContainer?.Collection.CollectionFileLocation}";
        }
    }
}
