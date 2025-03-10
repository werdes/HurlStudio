﻿using Avalonia;
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
    public partial class AddCollectionWindow : WindowBase<AddCollectionWindowViewModel>
    {
        private ILogger _log;
        private IConfiguration _configuration;
        private IUserSettingsService _userSettingsService;
        private IUiStateService _uiStateService;
        private ServiceManager<ViewModelBasedControl> _controlBuilder;
        private ControlLocator _controlLocator;
        

        public AddCollectionWindow(ILogger<AddCollectionWindow> logger, IConfiguration configuration, IUserSettingsService userSettingsService, IUiStateService uiStateService, AddCollectionWindowViewModel AddCollectionWindowViewModel, ServiceManager<ViewModelBasedControl> controlBuilder, ControlLocator controlLocator)
        {
            _log = logger;
            _configuration = configuration;
            _userSettingsService = userSettingsService;
            _uiStateService = uiStateService;
            _viewModel = AddCollectionWindowViewModel;
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
        /// <exception cref="ArgumentNullException">No control builder was supplied</exception>
        protected async void On_AddCollectionWindow_Initialized(object sender, EventArgs e)
        {
            try
            {
                ViewBase<AddCollectionViewViewModel>? view = _controlBuilder.Get<AddCollectionView>();
                if (view != null)
                {
                    // Bind the window offScreenMargin to the view margin
                    // -> this makes sure the window is displayed properly on full screen
                    var offscreenMarginBinding = this.GetObservable(OffScreenMarginProperty);
                    view.Bind(MarginProperty, offscreenMarginBinding);

                    this.Content = view;
                    view.SetWindow(this);

                    if (view.DataContext is AddCollectionViewViewModel viewModel)
                    {
                        viewModel.SetRoot(_viewModel);
                    }
                }
                else
                {
                    this.Content = new TextBlock() { Text = $"No entry view found: {typeof(AddCollectionWindow)}" };
                }
            }
            catch (Exception ex)
            {
                _log?.LogException(ex);
                await MessageBox.MessageBox.ShowErrorDialog(this, ex.Message, Localization.Localization.MessageBox_ErrorTitle);
            }
        }

        /// <summary>
        /// Close window on ESC
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_AddCollectionWindow_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
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
                await MessageBox.MessageBox.ShowErrorDialog(this, ex.Message, Localization.Localization.MessageBox_ErrorTitle);
            }
        }
    }
}
