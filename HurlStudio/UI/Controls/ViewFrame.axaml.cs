using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using HurlStudio.Common.Extensions;
using HurlStudio.UI.ViewModels;
using HurlStudio.UI.ViewModels.Controls;
using HurlStudio.UI.Views;
using HurlStudio.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using static System.Net.Mime.MediaTypeNames;

namespace HurlStudio.UI.Controls
{
    public partial class ViewFrame : ViewModelBasedControl<ViewFrameViewModel>
    {
        private ILogger<ViewFrame> _log;
        private ViewFrameViewModel? _viewModel;

        public ViewFrame(ILogger<ViewFrame> logger)
        {
            _log = logger;

            this.InitializeComponent();
        }

        protected override void SetViewModelInstance(ViewFrameViewModel viewModel)
        {
            _viewModel = viewModel;
            this.DataContext = _viewModel;
        }

        /// <summary>
        /// Swap view by setting the SelectedViewModel property of the viewmodel to the tagged
        /// viewmodel of the clicked button 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_ButtonChangeView_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_viewModel != null &&
                sender != null &&
                sender is Control control &&
                control.Tag is ViewModelBase targetViewModel)
            {
                _viewModel.SelectedViewModel = targetViewModel;

                //_viewModel.ViewModels.ForEach(x => x.IsActive =false);
                //targetViewModel.IsActive = true;    
            }
        }
    }
}
