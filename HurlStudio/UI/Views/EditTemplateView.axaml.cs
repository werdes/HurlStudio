using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaEdit;
using AvaloniaEdit.TextMate;
using HurlStudio.Collections.Settings;
using HurlStudio.Common;
using HurlStudio.Common.Extensions;
using HurlStudio.Extensions;
using HurlStudio.Model.Enums;
using HurlStudio.Model.HurlFileTemplates;
using HurlStudio.Model.HurlSettings;
using HurlStudio.Model.UserSettings;
using HurlStudio.Services.Editor;
using HurlStudio.Services.HurlFileTemplates;
using HurlStudio.Services.Notifications;
using HurlStudio.Services.UserSettings;
using HurlStudio.UI.Editor;
using HurlStudio.UI.ViewModels;
using HurlStudio.UI.ViewModels.Windows;
using HurlStudio.UI.Windows;
using HurlStudio.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HurlStudio.UI.Views
{
    public partial class EditTemplateView : ViewBase<EditTemplateViewViewModel>
    {
        private EditTemplateViewViewModel? _viewModel;
        private ILogger _log;
        private IConfiguration _configuration;
        private IUserSettingsService _userSettingsService;
        private IEditorService _editorService;
        private INotificationService _notificationService;
        private IHurlFileTemplateService _templateService;
        private ControlLocator _controlLocator;
        private TextEditor? _textEditor;
        private ServiceManager<Windows.WindowBase> _windowBuilder;

        public EditTemplateView(EditTemplateViewViewModel viewModel, ILogger<EditTemplateView> logger, IConfiguration configuration, IUserSettingsService userSettingsService, IEditorService editorService, ControlLocator controlLocator, INotificationService notificationService, IHurlFileTemplateService templateService, ServiceManager<Windows.WindowBase> windowBuilder)
        {
            _viewModel = viewModel;

            _log = logger;
            _configuration = configuration;
            _userSettingsService = userSettingsService;
            _editorService = editorService;
            _controlLocator = controlLocator;
            _notificationService = notificationService;
            _templateService = templateService;
            _windowBuilder = windowBuilder;

            this.DataContext = viewModel;
            this.DataTemplates.Add(_controlLocator);

            this.InitializeComponent();
        }

        protected override void SetViewModelInstance(EditTemplateViewViewModel viewModel)
        {
            _viewModel = viewModel;
            this.DataContext = _viewModel;
        }

        /// <summary>
        /// On View loaded
        /// -> focus search textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_EditTemplateView_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_window == null) return;

            try
            {
                this.TextBoxName.Focus();
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
        private async void On_EditTemplateView_Initialized(object? sender, System.EventArgs e)
        {
            if (_viewModel == null) return;
            if (_window == null) return;

            try
            {
                // Pull template in from window
                if (_window.GetViewModel() is not EditTemplateWindowViewModel windowViewModel) return;
                _viewModel.TemplateContainer = windowViewModel.TemplateContainer;

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


        public override void SetWindow(Windows.WindowBase window)
        {
            base.SetWindow(window);
            _controlLocator.Window = window;
        }

        /// <summary>
        /// Close the window with the template container attached
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_Button_Save_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_viewModel == null) return;
            if (_viewModel.TemplateContainer == null) return;
            if (_window == null) return;

            try
            {
                _viewModel.TemplateContainer.Template.Content = _viewModel.TemplateContainer.Document.CreateSnapshot().Text;
                _viewModel.TemplateContainer.Template.Settings.RemoveAll();

                foreach (HurlSettingContainer setting in _viewModel.TemplateContainer.SettingSection.SettingContainers)
                {
                    _viewModel.TemplateContainer.Template.Settings.Add(setting.Setting.GetConfigurationString());
                }

                _window.Close(_viewModel.TemplateContainer);
            }
            catch (Exception ex)
            {
                _notificationService.Notify(ex);
                _log.LogException(ex);
            }
        }

        /// <summary>
        /// Close the window without attachment -> discard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_Button_Cancel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_window == null) return;

            try
            {
                _window.Close(null);
            }
            catch (Exception ex)
            {
                _notificationService.Notify(ex);
                _log.LogException(ex);
            }
        }

        /// <summary>
        /// Add Setting to template
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_ButtonAddSetting_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_window == null) return;
            if (_viewModel == null) return;
            if (_viewModel.TemplateContainer == null) return;

            try
            {
                AddSettingWindow addSettingDialog = _windowBuilder.Get<AddSettingWindow>();
                BaseSetting? setting = await addSettingDialog.ShowDialog<BaseSetting?>(_window);
                if (setting == null) return;

                //_viewModel.AddSetting(new HurlSettingContainer(_viewModel, fileSection, setting, false, true, EnableType.Setting));
                _viewModel.TemplateContainer.SettingSection.SettingContainers.Add(new HurlSettingContainer(null, _viewModel.TemplateContainer.SettingSection, setting, false, true, EnableType.Setting));
            }
            catch (Exception ex)
            {
                _notificationService.Notify(ex);
                _log.LogException(ex);
            }
        }
    }
}