using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaEdit;
using AvaloniaEdit.TextMate;
using HurlStudio.Common;
using HurlStudio.Common.Extensions;
using HurlStudio.Extensions;
using HurlStudio.Model.HurlFileTemplates;
using HurlStudio.Model.UserSettings;
using HurlStudio.Services.Editor;
using HurlStudio.Services.HurlFileTemplates;
using HurlStudio.Services.Notifications;
using HurlStudio.Services.UserSettings;
using HurlStudio.UI.Editor;
using HurlStudio.UI.ViewModels;
using HurlStudio.UI.Windows;
using HurlStudio.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HurlStudio.UI.Views
{
    public partial class AddFileView : ViewBase<AddFileViewViewModel>
    {
        private ILogger _log;
        private IConfiguration _configuration;
        private IUserSettingsService _userSettingsService;
        private IEditorService _editorService;
        private INotificationService _notificationService;
        private IHurlFileTemplateService _templateService;
        private TextEditor? _textEditor;
        private ServiceManager<Windows.WindowBase> _windowBuilder;

        public AddFileView(AddFileViewViewModel viewModel, ILogger<AddFileView> logger, IConfiguration configuration, IUserSettingsService userSettingsService, IEditorService editorService, ControlLocator controlLocator, INotificationService notificationService, IHurlFileTemplateService templateService, ServiceManager<Windows.WindowBase> windowBuilder) : base(viewModel, controlLocator)
        {
            _log = logger;
            _configuration = configuration;
            _userSettingsService = userSettingsService;
            _editorService = editorService;
            _notificationService = notificationService;
            _templateService = templateService;
            _windowBuilder = windowBuilder;

            this.InitializeComponent();
        }

        /// <summary>
        /// Filter type list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_TextBoxSearch_TextChanged(object? sender, Avalonia.Controls.TextChangedEventArgs e)
        {
            try
            {
                if (_viewModel == null) return;
                _viewModel.Templates.ForEach(x => x.IsVisible = true);

                if (_viewModel.Query == null) return;

                _viewModel.Templates
                    .Where(x => !_viewModel.Query.IsContainedInAnyNormalized(
                        x.Template.Content,
                        x.Template.Name))
                    .ForEach(x => x.IsVisible = false);
            }
            catch (Exception ex)
            {
                _notificationService.Notify(ex);
                _log.LogException(ex);
            }
        }

        /// <summary>
        /// On View loaded
        /// -> focus search textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_AddFileView_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                this.TextBoxFileName.Focus();
            }
            catch (Exception ex)
            {
                _notificationService.Notify(ex);
                _log.LogException(ex);
            }
        }

        /// <summary>
        /// Load available template list on initilization and set up editor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_AddFileView_Initialized(object? sender, System.EventArgs e)
        {
            if (_viewModel == null) return;
            try
            {
                await this.LoadTemplates();
                _viewModel.SelectedTemplate = _viewModel.Templates.FirstOrDefault();

                _textEditor = this.FindControl<TextEditor>("Editor");
                if (_textEditor == null) return;

                _textEditor.TextArea.RightClickMovesCaret = true;
                _textEditor.Options.EnableHyperlinks = false;
                _textEditor.Options.AllowScrollBelowDocument = true;
                _textEditor.Options.EnableRectangularSelection = true;
                _textEditor.Options.ShowBoxForControlCharacters = true;
                _textEditor.WordWrap = true;

                UserSettings userSettings = await _userSettingsService.GetUserSettingsAsync(false);
                LocalResourceGrammarRegistryOptions registryOptions = new LocalResourceGrammarRegistryOptions(userSettings.Theme, _log);

                TextMate.Installation textMateInstallation = _textEditor.InstallTextMate(registryOptions);
                textMateInstallation.SetGrammar(GlobalConstants.GRAMMAR_HURL_NAME);

            }
            catch (Exception ex)
            {
                _notificationService.Notify(ex);
                _log.LogException(ex);
            }
        }

        /// <summary>
        /// Loads the template list and sets it to the viewmodel
        /// </summary>
        private async Task LoadTemplates()
        {
            if (_viewModel == null) return;

            _viewModel.Templates.RemoveAll();
            _viewModel.Templates.AddRangeIfNotNull(await _templateService.GetTemplateContainersAsync());
            _viewModel.BindEvents();

            foreach(HurlFileTemplateContainer templateContainer in _viewModel.Templates)
            {
                templateContainer.HurlFileTemplateContainerDeleted += this.On_TemplateContainer_HurlFileTemplateContainerDeleted;
                templateContainer.HurlFileTemplateContainerEdited += this.On_TemplateContainer_HurlFileTemplateContainerEdited;
            }
        }

        /// <summary>
        /// Reload templates on edit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_TemplateContainer_HurlFileTemplateContainerEdited(object? sender, Model.EventArgs.HurlFileTemplateContainerEditedEventArgs e)
        {
            if (_viewModel == null) return;

            Guid? selectedID = _viewModel.SelectedTemplate?.Template.Id;
            await this.LoadTemplates();

            if (selectedID != null && _viewModel.Templates.Any(x => x.Template.Id == selectedID))
            {
                _viewModel.SelectedTemplate = _viewModel.Templates.First(x => x.Template.Id == selectedID);
            }
            else
            {
                _viewModel.SelectedTemplate = null;
            }
        }

        /// <summary>
        /// Reload templates on delete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_TemplateContainer_HurlFileTemplateContainerDeleted(object? sender, Model.EventArgs.HurlFileTemplateContainerDeletedEventArgs e)
        {
            if(_viewModel == null) return;

            Guid? selectedID = _viewModel.SelectedTemplate?.Template.Id;
            await this.LoadTemplates();

            if (selectedID != null && _viewModel.Templates.Any(x => x.Template.Id == selectedID))
            {
                _viewModel.SelectedTemplate = _viewModel.Templates.First(x => x.Template.Id == selectedID);
            }
            else
            {
                _viewModel.SelectedTemplate = null;
            }
        }

        /// <summary>
        /// Close the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void On_ButtonAddFile_Click(object sender, RoutedEventArgs e)
        {
            if(_viewModel == null) return;
            if(_window == null) return;

            try
            {
                _window.Close((_viewModel.FileName, _viewModel.SelectedTemplate));
            }
            catch (Exception ex)
            {
                _notificationService.Notify(ex);
                _log.LogException(ex);
            }
        }

        /// <summary>
        /// Creates a new template and opens the edit dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_Button_CreateTemplate_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_viewModel == null) return;
            if (_window == null) return;

            try
            {
                EditTemplateWindow editTemplateWindow = _windowBuilder.Get<EditTemplateWindow>();
                if (editTemplateWindow.ViewModel == null) return;

                HurlFileTemplate template = new HurlFileTemplate()
                {
                    CanDelete = true,
                    Content = string.Empty,
                    IsDefaultTemplate = false,
                    IsDeleted = false,
                    Name = string.Empty
                };

                HurlFileTemplateContainer? templateContainer = new HurlFileTemplateContainer(template, new Model.HurlSettings.HurlSettingSection(null, Model.Enums.HurlSettingSectionType.File, null));

                editTemplateWindow.ViewModel.TemplateContainer = templateContainer;
                HurlFileTemplateContainer? container = await editTemplateWindow.ShowDialog<HurlFileTemplateContainer?>(_window);

                if (container != null)
                {
                    await _templateService.CreateTemplateAsync(container.Template);
                    await this.LoadTemplates();
                }
            }
            catch (Exception ex)
            {
                _notificationService.Notify(ex);
                _log.LogException(ex);
            }
        }

    }
}