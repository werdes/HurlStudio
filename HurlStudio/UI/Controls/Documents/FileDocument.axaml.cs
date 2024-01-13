using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Styling;
using AvaloniaEdit;
using AvaloniaEdit.TextMate;
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
        }

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

                        UserSettings userSettings = await _userSettingsService.GetUserSettingsAsync(false);
                        LocalResourceGrammarRegistryOptions registryOptions = new LocalResourceGrammarRegistryOptions(userSettings.Theme);
                        
                        string? fileName = "csharp.json";
                        string? assemblyName = Assembly.GetExecutingAssembly()?.GetName()?.Name;

                        if (!string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(assemblyName))
                        {
                            TextMate.Installation installation = _textEditor.InstallTextMate(registryOptions);
                            installation.SetGrammar("hurl");
                        }

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
            if (_textEditor != null && _viewModel != null && _viewModel.File != null)
            {
                _textEditor.Document = new AvaloniaEdit.Document.TextDocument(File.ReadAllText(_viewModel.File.Location));
            }
        }
    }
}
