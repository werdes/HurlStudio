using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Styling;
using AvaloniaEdit;
using AvaloniaEdit.TextMate;
using HurlStudio.Collections.Settings;
using HurlStudio.Common;
using HurlStudio.Common.Extensions;
using HurlStudio.Model.Enums;
using HurlStudio.Model.HurlSettings;
using HurlStudio.Model.UserSettings;
using HurlStudio.Services.Editor;
using HurlStudio.Services.Notifications;
using HurlStudio.Services.UserSettings;
using HurlStudio.UI.Editor;
using HurlStudio.UI.ViewModels.Documents;
using HurlStudio.UI.Windows;
using HurlStudio.Utility;
using Microsoft.Extensions.Logging;
using SkiaSharp;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using TextMateSharp.Grammars;
using TextMateSharp.Internal.Grammars.Reader;
using TextMateSharp.Internal.Types;
using TextMateSharp.Registry;

namespace HurlStudio.UI.Controls.Documents
{
    public partial class CollectionDocument : ViewModelBasedControl<CollectionDocumentViewModel>
    {
        private CollectionDocumentViewModel? _viewModel;
        private MainWindow _mainWindow;
        private IEditorService _editorService;
        private ILogger _log;
        private INotificationService _notificationService;
        private IUserSettingsService _userSettingsService;
        private ServiceManager<Windows.WindowBase> _windowBuilder;
        private TextEditor? _textEditor;
        private bool _initializationCompleted = false;

        public CollectionDocument(IEditorService editorService, MainWindow mainWindow, ILogger<CollectionDocument> logger, INotificationService notificationService, IUserSettingsService userSettingsService, ServiceManager<Windows.WindowBase> windowBuilder)
        {
            _editorService = editorService;
            _notificationService = notificationService;
            _userSettingsService = userSettingsService;
            _log = logger;
            _windowBuilder = windowBuilder;
            _mainWindow = mainWindow;

            this.InitializeComponent();
        }

        protected override void SetViewModelInstance(CollectionDocumentViewModel viewModel)
        {
            _viewModel = viewModel;
            this.DataContext = _viewModel;
        }

        /// <summary>
        /// On Control initialize
        /// supply the text editor options
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_CollectionDocument_Initialized(object? sender, EventArgs e)
        {
            if (_viewModel == null) return;
            if (_viewModel.Collection == null) return;

            try
            {
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);
            }
        }

        private void On_CollectionDocument_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            _initializationCompleted = true;
        }

        /// <summary>
        /// Show a dialog to select a new setting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_ButtonAddSetting_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_viewModel == null) return;

            try
            {
                AddSettingWindow addSettingDialog = _windowBuilder.Get<AddSettingWindow>();
                BaseSetting? setting = await addSettingDialog.ShowDialog<BaseSetting?>(_mainWindow);
                if (setting == null) return;


                HurlSettingSection? collectionSection = _viewModel.SettingSections.FirstOrDefault(x => x.SectionType == HurlSettingSectionType.Collection);
                if (collectionSection == null) return;

                _viewModel.AddSetting(new HurlSettingContainer(_viewModel, collectionSection, setting, false, true, EnableType.Setting));
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);
            }
        }

        /// <summary>
        /// Adds an empty row to the collection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_ButtonAddAdditionalLocation_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_viewModel == null) return;
            if (_viewModel.Collection == null) return;

            try
            {
                _viewModel.Collection.Collection.AdditionalLocations.Add(new Collections.Model.Containers.AdditionalLocation(string.Empty, _viewModel.Collection.Collection));
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);
            }
        }
    }
}
