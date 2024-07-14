using Avalonia.Controls;
using Avalonia.Input;
using HurlStudio.Common.Extensions;
using HurlStudio.Model.HurlFileTemplates;
using HurlStudio.Model.HurlSettings;
using HurlStudio.Services.HurlFileTemplates;
using HurlStudio.Services.Notifications;
using HurlStudio.UI.ViewModels;
using HurlStudio.UI.ViewModels.Windows;
using HurlStudio.UI.Windows;
using HurlStudio.Utility;
using Microsoft.Extensions.Logging;
using System;

namespace HurlStudio.UI.Controls
{
    public partial class HurlFileTemplateListItem : ViewModelBasedControl<HurlFileTemplateContainer>
    {
        private HurlFileTemplateContainer? _viewModel;
        private ILogger _log;
        private INotificationService _notificationService;
        private IHurlFileTemplateService _templateService;
        private ServiceManager<Windows.WindowBase> _windowBuilder;

        public HurlFileTemplateListItem(ILogger<HurlFileTemplateListItem> logger, INotificationService notificationService, IHurlFileTemplateService templateService, ServiceManager<Windows.WindowBase> windowBuilder)
        {
            _notificationService = notificationService;
            _log = logger;
            _templateService = templateService;
            _windowBuilder = windowBuilder;

            this.InitializeComponent();
        }

        protected override void SetViewModelInstance(HurlFileTemplateContainer viewModel)
        {
            _viewModel = viewModel;
            this.DataContext = viewModel;
        }

        /// <summary>
        /// Select the container
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_TemplateContainer_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            if (_viewModel == null) return;

            _viewModel.NotifySelected();
        }

        /// <summary>
        /// Opens a window to edit the template
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_MenuItem_Edit_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_viewModel == null) return;
            if (_window == null) return;

            try
            {
                EditTemplateWindow editTemplateWindow = _windowBuilder.Get<EditTemplateWindow>();
                //if (editTemplateWindow.DataContext is not EditTemplateWindowViewModel editTemplateWindowViewModel) return;
                //if (editTemplateWindowViewModel.RootViewModel is not EditTemplateViewViewModel editTemplateViewViewModel) return;

                if(editTemplateWindow.ViewModel == null) return;

                // Get a copy of the viewmodel by reading it again
                HurlFileTemplateContainer? template = await _templateService.GetTemplateContainerAsync(_viewModel.Template.Id);
                if(template == null) return;
                template.SettingSection.SettingContainers.ForEach(x => x.IsReadOnly = false);

                editTemplateWindow.ViewModel.TemplateContainer = template;
                HurlFileTemplateContainer? container = await editTemplateWindow.ShowDialog<HurlFileTemplateContainer?>(_window);

                if(container != null)
                {
                    await _templateService.UpdateTemplateAsync(container.Template);
                    _viewModel.NotifyEdited();
                }
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);
            }
        }

        /// <summary>
        /// Removes the template
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_MenuItem_Delete_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_viewModel == null) return;
            if (_window == null) return;

            try
            {
                bool delete = await MessageBox.ShowQuestionYesNoDialog(_window, _viewModel.Template.Name ?? string.Empty, Localization.Localization.View_AddFileView_Templates_DeleteTemplate_MessageBox_Title) == MessageBox.ButtonType.Yes;

                if (delete && await _templateService.DeleteTemplateAsync(_viewModel.Template.Id))
                {
                    _viewModel.NotifyDeleted();
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
