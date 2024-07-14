using Avalonia;
using Avalonia.Controls;
using HurlStudio.Common.Extensions;
using HurlStudio.Model.UiState;
using HurlStudio.Services.UiState;
using HurlStudio.Services.UserSettings;
using HurlStudio.UI.Controls;
using HurlStudio.UI.ViewModels;
using HurlStudio.UI.ViewModels.Windows;
using HurlStudio.UI.Views;
using HurlStudio.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace HurlStudio.UI.Windows
{
    public partial class EditTemplateWindow : WindowBase<EditTemplateWindowViewModel>
    {
        private ILogger _log;
        private IConfiguration _configuration;
        private IUserSettingsService _userSettingsService;
        private IUiStateService _uiStateService;
        private ServiceManager<ViewModelBasedControl> _controlBuilder;
        private ControlLocator _controlLocator;
        

        public EditTemplateWindow(ILogger<EditTemplateWindow> logger, IConfiguration configuration, IUserSettingsService userSettingsService, IUiStateService uiStateService, EditTemplateWindowViewModel EditTemplateWindowViewModel, ServiceManager<ViewModelBasedControl> controlBuilder, ControlLocator controlLocator)
        {
            _log = logger;
            _configuration = configuration;
            _userSettingsService = userSettingsService;
            _uiStateService = uiStateService;
            _viewModel = EditTemplateWindowViewModel;
            _controlBuilder = controlBuilder;
            _controlLocator = controlLocator;

            this.DataContext = _viewModel;
            this.DataTemplates.Add(controlLocator);

            this.InitializeComponent();
        }

        /// <summary>
        /// On window initialization -> attach the view to the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected async void On_EditTemplateWindow_Initialized(object sender, EventArgs e)
        {
            if (_viewModel == null) return;

            try
            {
                ViewBase<EditTemplateViewViewModel>? view = _controlBuilder.Get<EditTemplateView>();
                if (view != null)
                {
                    // Bind the window offScreenMargin to the view margin
                    // -> this makes sure the window is displayed properly on full screen
                    var offscreenMarginBinding = this.GetObservable(OffScreenMarginProperty);
                    view.Bind(MarginProperty, offscreenMarginBinding);

                    this.Content = view;
                    view.SetWindow(this);

                    if (view.DataContext is EditTemplateViewViewModel viewModel)
                    {
                        viewModel.SetRoot(_viewModel);
                    }
                }
                else
                {
                    this.Content = new TextBlock() { Text = $"No entry view found: {typeof(EditTemplateWindow)}" };
                }
            }
            catch (Exception ex)
            {
                _log?.LogException(ex);
                await MessageBox.ShowError(ex.Message, Localization.Localization.MessageBox_ErrorTitle);
            }
        }

        /// <summary>
        /// Close window on ESC
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_EditTemplateWindow_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
        {
            try
            {
                if (e.Key == Avalonia.Input.Key.Escape)
                {
                    var element = this.FocusManager?.GetFocusedElement();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                _log?.LogException(ex);
                await MessageBox.ShowError(ex.Message, Localization.Localization.MessageBox_ErrorTitle);
            }
        }
    }
}
