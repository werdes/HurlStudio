using Avalonia;
using Avalonia.Controls;
using HurlStudio.Common.Utility;
using HurlStudio.Model.HurlContainers;
using HurlStudio.Services.Editor;
using HurlStudio.Services.Notifications;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace HurlStudio.UI.Controls.CollectionExplorer
{
    public partial class File : CollectionExplorerControlBase<HurlFileContainer>
    {
        private HurlFileContainer CollectionFile
        {
            get => (HurlFileContainer)this.GetValue(CollectionFileProperty);
            set => this.SetValue(CollectionFileProperty, value);
        }

        public static readonly StyledProperty<HurlFileContainer> CollectionFileProperty =
            AvaloniaProperty.Register<File, HurlFileContainer>(nameof(CollectionFile));

        private IEditorService _editorService;
        private ILogger _log;
        private INotificationService _notificationService;

        public File(ILogger<File> logger, INotificationService notificationService, IEditorService editorService)
            : base(notificationService, logger)
        {
            _editorService = editorService;
            _log = logger;
            _notificationService = notificationService;

            this.InitializeComponent();
        }

        /// <summary>
        /// Sets the view model
        /// </summary>
        /// <param name="viewModel"></param>
        protected override void SetViewModelInstance(HurlFileContainer viewModel)
        {
            this.CollectionFile = viewModel;
        }

        /// <summary>
        /// Set DataContext to provided avalonia property
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_File_Initialized(object? sender, EventArgs e)
        {
            this.DataContext = this.CollectionFile;
        }

        /// <summary>
        /// Returns the file as bound element
        /// </summary>
        /// <returns></returns>
        protected override HurlContainerBase GetBoundCollectionComponent()
        {
            return this.CollectionFile;
        }

        /// <summary>
        /// Opens the file document
        /// </summary>
        /// <returns></returns>
        protected override async Task OpenComponentDocument()
        {
            if (this.CollectionFile.Collection.Collection.FileLocation == null) return;

            try
            {

                await _editorService.OpenFile(this.CollectionFile.Location, this.CollectionFile.Collection.Collection.FileLocation);
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
            if (this.CollectionFile == null) return;
            if (this.CollectionFile.Location == null) return;
            try
            {
                OSUtility.RevealFileInExplorer(this.CollectionFile.Location);
            }
            catch (Exception ex)
            {
                _log.LogCritical(ex, nameof(this.OpenComponentDocument));
                _notificationService.Notify(ex);
            }
        }
    }
}
