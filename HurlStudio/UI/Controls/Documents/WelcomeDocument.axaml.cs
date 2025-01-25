using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Dock.Model.Mvvm.Controls;
using HurlStudio.Common;
using HurlStudio.Common.Extensions;
using HurlStudio.Services.Editor;
using HurlStudio.Services.Notifications;
using HurlStudio.Services.UiState;
using HurlStudio.UI.Controls.CollectionExplorer;
using HurlStudio.UI.MessageBox;
using HurlStudio.UI.ViewModels.Documents;
using Microsoft.Extensions.Logging;

namespace HurlStudio.UI.Controls.Documents
{
    public partial class WelcomeDocument : ViewModelBasedControl<WelcomeDocumentViewModel>
    {
        private WelcomeDocumentViewModel? _viewModel;
        private ILogger _log;
        private IEditorService _editorService;
        private INotificationService _notificationService;

        public WelcomeDocument(IEditorService editorService, ILogger<WelcomeDocument> logger,
            INotificationService notificationService)
        {
            _editorService = editorService;
            _log = logger;
            _notificationService = notificationService;

            this.InitializeComponent();
        }

        protected override void SetViewModelInstance(WelcomeDocumentViewModel viewModel)
        {
            _viewModel = viewModel;
            this.DataContext = _viewModel;
        }

        private async void On_Button_NewCollection_Click(object? sender, RoutedEventArgs e)
        {
            if (_window == null) return;

            await _editorService.CreateCollection();
        }

        private async void On_Button_OpenCollection_Click(object? sender, RoutedEventArgs e)
        {
            if (_window == null) return;

            try
            {
                await _editorService.AddCollection();
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);
            }
        }
    }
}