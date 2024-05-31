using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
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

namespace HurlStudio.UI.Controls.EnvironmentExplorer
{
    public partial class Environment : ViewModelBasedControl<HurlEnvironmentContainer>
    {
        private HurlEnvironmentContainer? _environmentContainer;

        private IEditorService _editorService;
        private ILogger _log;
        private INotificationService _notificationService;
        private IUiStateService _uiStateService;


        public Environment(ILogger<Environment> logger, INotificationService notificationService, IEditorService editorService, IUiStateService uiStateService)
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
        protected override void SetViewModelInstance(HurlEnvironmentContainer viewModel)
        {
            _environmentContainer = viewModel;
            this.DataContext = viewModel;
        }

        /// <summary>
        /// Opens the folder containing the collection file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_MenuItem_RevealInExplorer_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_environmentContainer == null) return;
            if (_environmentContainer.EnvironmentFileLocation == null) return;
            try
            {
                OSUtility.RevealFileInExplorer(_environmentContainer.EnvironmentFileLocation);
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);
            }
        }

        /// <summary>
        /// Select item on header press
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected async void On_TitlePanel_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (_environmentContainer == null) return;
            try
            {

                if (e.ClickCount == 2 && _environmentContainer.Selected)
                {
                    await _editorService.OpenEnvironment(_environmentContainer.EnvironmentFileLocation);
                }

                _environmentContainer.Selected = true;

            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);
            }

            e.Handled = true;
        }
    }
}
