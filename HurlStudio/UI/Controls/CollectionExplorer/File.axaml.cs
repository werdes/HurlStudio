using Avalonia;
using Avalonia.Controls;
using HurlStudio.Common.Extensions;
using HurlStudio.Common.Utility;
using HurlStudio.Model.HurlContainers;
using HurlStudio.Services.Editor;
using HurlStudio.Services.Notifications;
using HurlStudio.Services.UiState;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace HurlStudio.UI.Controls.CollectionExplorer
{
    public partial class File : CollectionExplorerControlBase<HurlFileContainer>
    {
        private HurlFileContainer? _collectionFile;
        
        private IEditorService _editorService;
        private ILogger _log;
        private INotificationService _notificationService;
        private IUiStateService _uiStateService;

        public File(ILogger<File> logger, INotificationService notificationService, IEditorService editorService, IUiStateService uiStateService)
            : base(notificationService, logger)
        {
            _editorService = editorService;
            _log = logger;
            _notificationService = notificationService;
            _uiStateService = uiStateService;

            this.InitializeComponent();
        }

        /// <summary>
        /// Sets the view model
        /// </summary>
        /// <param name="viewModel"></param>
        protected override void SetViewModelInstance(HurlFileContainer viewModel)
        {
            _collectionFile = viewModel;
            this.DataContext = viewModel;
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_File_Initialized(object? sender, EventArgs e)
        {
        }

        /// <summary>
        /// Returns the file as bound element
        /// </summary>
        /// <returns></returns>
        protected override HurlContainerBase? GetBoundCollectionComponent()
        {
            return _collectionFile;
        }

        /// <summary>
        /// Opens the file document
        /// </summary>
        /// <returns></returns>
        protected override async Task OpenComponentDocument()
        {
            if (_collectionFile == null) return;
            if (_collectionFile.Collection.Collection.FileLocation == null) return;

            try
            {
                await _editorService.OpenFile(_collectionFile.Location, _collectionFile.Collection.Collection.FileLocation);
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
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
            if (_collectionFile == null) return;
            if (_collectionFile.Location == null) return;
            try
            {
                OSUtility.RevealFileInExplorer(_collectionFile.Location);
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);
            }
        }
    }
}
