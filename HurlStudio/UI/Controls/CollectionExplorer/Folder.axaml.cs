using Avalonia;
using Avalonia.Controls;
using HurlStudio.Common.Utility;
using HurlStudio.Model.HurlContainers;
using HurlStudio.Model.EventArgs;
using HurlStudio.Services.Editor;
using HurlStudio.Services.Notifications;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace HurlStudio.UI.Controls.CollectionExplorer
{
    public partial class Folder : CollectionExplorerControlBase<HurlFolderContainer>
    {
        private HurlFolderContainer CollectionFolder
        {
            get => (HurlFolderContainer)this.GetValue(CollectionFolderProperty);
            set => this.SetValue(CollectionFolderProperty, value);
        }

        public static readonly StyledProperty<HurlFolderContainer> CollectionFolderProperty =
            AvaloniaProperty.Register<Folder, HurlFolderContainer>(nameof(CollectionFolder));

        private ILogger _log;
        private IEditorService _editorService;
        private INotificationService _notificationService;

        public Folder(ILogger<Folder> logger, INotificationService notificationService, IEditorService editorService)
            : base(notificationService, logger)
        {
            _log = logger;
            _editorService = editorService;
            _notificationService = notificationService;

            this.InitializeComponent();

        }

        /// <summary>
        /// Sets the view model
        /// </summary>
        /// <param name="viewModel"></param>
        protected override void SetViewModelInstance(HurlFolderContainer viewModel)
        {
            this.CollectionFolder = viewModel;
        }

        /// <summary>
        /// Set DataContext to provided avalonia property
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_Folder_Initialized(object? sender, EventArgs e)
        {
            this.DataContext = this.CollectionFolder;
        }

        /// <summary>
        /// Toggle the folder's collapse state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_ButtonCollapse_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (this.DataContext == null) return;
            this.CollectionFolder.Collapsed = !this.CollectionFolder.Collapsed;
        }

        /// <summary>
        /// Returns the folder as bound element
        /// </summary>
        /// <returns></returns>
        protected override HurlContainerBase GetBoundCollectionComponent()
        {
            return this.CollectionFolder;
        }

        /// <summary>
        /// Opens a folder settings page
        /// </summary>
        /// <returns></returns>
        protected override async Task OpenComponentDocument()
        {
            try
            {
                await _editorService.OpenFolderSettings(this.CollectionFolder);
            }
            catch (Exception ex)
            {
                _log.LogCritical(ex, nameof(this.OpenComponentDocument));
                _notificationService.Notify(ex);
            }
        }

        /// <summary>
        /// Opens the folder containing the collection file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_MenuItem_RevealInExplorer_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (this.CollectionFolder == null) return;
            if (this.CollectionFolder.Location == null) return;

            try
            {
                OSUtility.RevealFileInExplorer(this.CollectionFolder.Location);
            }
            catch (Exception ex)
            {
                _log.LogCritical(ex, nameof(this.OpenComponentDocument));
                _notificationService.Notify(ex);
            }
        }
    }
}
