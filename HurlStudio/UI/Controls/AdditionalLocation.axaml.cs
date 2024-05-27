using Avalonia.Controls;
using HurlStudio.Common.Extensions;
using HurlStudio.Model.UiState;
using HurlStudio.Services.Editor;
using HurlStudio.Services.Notifications;
using HurlStudio.UI.Windows;
using Microsoft.Extensions.Logging;
using System;

namespace HurlStudio.UI.Controls
{
    public partial class AdditionalLocation : ViewModelBasedControl<Collections.Model.Containers.AdditionalLocation>
    {
        private Collections.Model.Containers.AdditionalLocation? _viewModel;
        private MainWindow _mainWindow;
        private ILogger _log;
        private IEditorService _editorService;
        private INotificationService _notificationService;

        public AdditionalLocation(ILogger<RecentFile> logger, IEditorService editorService, MainWindow mainWindow, INotificationService notificationService)
        {
            _editorService = editorService;
            _log = logger;
            _mainWindow = mainWindow;
            _notificationService = notificationService;

            this.InitializeComponent();
        }

        protected override void SetViewModelInstance(Collections.Model.Containers.AdditionalLocation viewModel)
        {
            _viewModel = viewModel;
            this.DataContext = viewModel;
        }

        /// <summary>
        /// Display file explorer to select a folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_ButtonSelectDirectory_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_viewModel == null) return;

            try
            {
                string? folder = await this.OpenDirectorySingle(
                    _mainWindow.StorageProvider,
                    Localization.Localization.Setting_FileRootSetting_FolderPicker_Title);

                if (!string.IsNullOrEmpty(folder))
                {
                    _viewModel.Path = folder;
                }
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);
            }
        }

        /// <summary>
        /// Remove a file from the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_ButtonRemoveRow_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_viewModel == null) return;
            if (_viewModel.Collection == null) return;

            try
            {
                if(_viewModel.Collection.AdditionalLocations.Contains(_viewModel))
                {
                    _viewModel.Collection.AdditionalLocations.Remove(_viewModel);
                }
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);
            }
        }
    }
}
