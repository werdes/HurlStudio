using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Styling;
using AvaloniaEdit;
using AvaloniaEdit.TextMate;
using HurlStudio.Common;
using HurlStudio.Model.Enums;
using HurlStudio.Model.UserSettings;
using HurlStudio.Services.Editor;
using HurlStudio.Services.Notifications;
using HurlStudio.Services.UserSettings;
using HurlStudio.UI.Editor;
using HurlStudio.UI.ViewModels.Documents;
using Microsoft.Extensions.Logging;
using SkiaSharp;
using System;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Security.Cryptography.X509Certificates;
using TextMateSharp.Grammars;
using TextMateSharp.Internal.Grammars.Reader;
using TextMateSharp.Internal.Types;
using TextMateSharp.Registry;

namespace HurlStudio.UI.Controls.Documents
{
    public partial class FileDocument : ViewModelBasedControl<FileDocumentViewModel>
    {
        private FileDocumentViewModel? _viewModel;
        private IEditorService _editorService;
        private ILogger _log;
        private INotificationService _notificationService;
        private IUserSettingsService _userSettingsService;
        private TextEditor? _textEditor;

        public FileDocument(IEditorService editorService, ILogger<FileDocument> logger, INotificationService notificationService, IUserSettingsService userSettingsService)
        {
            _editorService = editorService;
            _notificationService = notificationService;
            _userSettingsService = userSettingsService;
            _log = logger;


            InitializeComponent();
        }

        protected override void SetViewModelInstance(FileDocumentViewModel viewModel)
        {
            _viewModel = viewModel;
            this.DataContext = _viewModel;

            _viewModel.EditorViewViewModel.PropertyChanged += On_EditorViewViewModel_PropertyChanged;
        }

        /// <summary>
        /// Listen on property changes on the editor view model, since the editor doesn't support option binding
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_EditorViewViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_textEditor == null) return;
            if (_viewModel == null) return;

            if (e.PropertyName == nameof(_viewModel.EditorViewViewModel.ShowEndOfLine))
            {
                _textEditor.Options.ShowEndOfLine = _viewModel.EditorViewViewModel.ShowEndOfLine;
                _userSettingsService.GetUserSettings(false).ShowEndOfLine = _viewModel.EditorViewViewModel.ShowEndOfLine;
                //await _userSettingsService.StoreUserSettingsAsync();
            }
            else if (e.PropertyName == nameof(_viewModel.EditorViewViewModel.ShowWhitespace))
            {
                _textEditor.Options.ShowSpaces = _viewModel.EditorViewViewModel.ShowWhitespace;
                _textEditor.Options.ShowTabs = _viewModel.EditorViewViewModel.ShowWhitespace;

                _userSettingsService.GetUserSettings(false).ShowWhitespace = _viewModel.EditorViewViewModel.ShowWhitespace;
                //await _userSettingsService.StoreUserSettingsAsync();
            }
            else if (e.PropertyName == nameof(_viewModel.EditorViewViewModel.WordWrap))
            {
                _textEditor.WordWrap = _viewModel.EditorViewViewModel.WordWrap;

                _userSettingsService.GetUserSettings(false).WordWrap = _viewModel.EditorViewViewModel.WordWrap;
                //await _userSettingsService.StoreUserSettingsAsync();
            }
        }

        /// <summary>
        /// On Control initialize
        /// supply the text editor options
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_FileDocument_Initialized(object? sender, EventArgs e)
        {
            try
            {
                if (_viewModel != null && _viewModel.File != null)
                {
                    _textEditor = this.FindControl<TextEditor>("Editor");


                    if (_textEditor != null)
                    {
                        _textEditor.TextArea.RightClickMovesCaret = true;
                        _textEditor.Options.EnableHyperlinks = false;
                        _textEditor.Options.AllowScrollBelowDocument = true;
                        _textEditor.Options.EnableRectangularSelection = true;
                        _textEditor.Options.ShowBoxForControlCharacters = true;
                        _textEditor.WordWrap = _viewModel.EditorViewViewModel.WordWrap;
                        _textEditor.Options.ShowSpaces = _viewModel.EditorViewViewModel.ShowWhitespace;
                        _textEditor.Options.ShowTabs = _viewModel.EditorViewViewModel.ShowWhitespace;
                        _textEditor.Options.ShowEndOfLine = _viewModel.EditorViewViewModel.ShowEndOfLine;

                        UserSettings userSettings = await _userSettingsService.GetUserSettingsAsync(false);

                        LocalResourceGrammarRegistryOptions registryOptions = new LocalResourceGrammarRegistryOptions(userSettings.Theme);
                        TextMate.Installation textMateInstallation = _textEditor.InstallTextMate(registryOptions);
                        textMateInstallation.SetGrammar(GlobalConstants.GRAMMAR_HURL_NAME);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogCritical(ex, nameof(On_FileDocument_Initialized));
                _notificationService.Notify(ex);
            }
        }

        private void On_FileDocument_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            //if (_textEditor != null && _viewModel != null && _viewModel.File != null)
            //{
            //    _textEditor.Document = new AvaloniaEdit.Document.TextDocument(File.ReadAllText(_viewModel.File.Location));
            //}
        }
    }
}
