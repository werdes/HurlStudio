using Avalonia;
using Avalonia.Controls;
using HurlStudio.Model.CollectionContainer;
using HurlStudio.Model.EventArgs;
using HurlStudio.Services.Editor;
using HurlStudio.Services.Notifications;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace HurlStudio.UI.Controls.CollectionExplorer
{
    public partial class Folder : CollectionExplorerControlBase<CollectionFolder>
    {
        private CollectionFolder CollectionFolder
        {
            get => (CollectionFolder)GetValue(CollectionFolderProperty);
            set => SetValue(CollectionFolderProperty, value);
        }

        public static readonly StyledProperty<CollectionFolder> CollectionFolderProperty =
            AvaloniaProperty.Register<Folder, CollectionFolder>(nameof(CollectionFolder));

        private ILogger _logger;
        private IEditorService _editorService;
        private INotificationService _notificationService;

        public Folder(ILogger<Folder> logger, INotificationService notificationService, IEditorService editorService)
            : base(notificationService, logger)
        {
            _logger = logger;
            _editorService = editorService;
            _notificationService = notificationService;

            InitializeComponent();

        }

        /// <summary>
        /// Sets the view model
        /// </summary>
        /// <param name="viewModel"></param>
        protected override void SetViewModelInstance(CollectionFolder viewModel)
        {
            CollectionFolder = viewModel;
        }

        /// <summary>
        /// Set DataContext to provided avalonia property
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_Folder_Initialized(object? sender, System.EventArgs e)
        {
            this.DataContext = CollectionFolder;
        }

        /// <summary>
        /// Toggle the folder's collapse state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_ButtonCollapse_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (this.DataContext == null) return;
            this.CollectionFolder.Collapsed = !CollectionFolder.Collapsed;
        }

        /// <summary>
        /// Returns the folder as bound element
        /// </summary>
        /// <returns></returns>
        protected override CollectionComponentBase GetBoundCollectionComponent()
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
                await _editorService.OpenFolderSettings(CollectionFolder);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, nameof(OpenComponentDocument));
                _notificationService.Notify(ex);
            }
        }
    }
}
