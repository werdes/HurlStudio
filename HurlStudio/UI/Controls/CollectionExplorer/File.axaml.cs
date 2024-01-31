using Avalonia;
using Avalonia.Controls;
using HurlStudio.Model.CollectionContainer;
using HurlStudio.Services.Editor;
using HurlStudio.Services.Notifications;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace HurlStudio.UI.Controls.CollectionExplorer
{
    public partial class File : CollectionExplorerControlBase<CollectionFile>
    {
        private CollectionFile CollectionFile
        {
            get => (CollectionFile)GetValue(CollectionFileProperty);
            set => SetValue(CollectionFileProperty, value);
        }

        public static readonly StyledProperty<CollectionFile> CollectionFileProperty =
            AvaloniaProperty.Register<File, CollectionFile>(nameof(CollectionFile));

        private IEditorService _editorService;
        private ILogger _log;
        private INotificationService _notificationService;

        public File(ILogger<File> logger, INotificationService notificationService, IEditorService editorService)
            : base(notificationService, logger)
        {
            _editorService = editorService;
            _log = logger;
            _notificationService = notificationService;

            InitializeComponent();
        }

        /// <summary>
        /// Sets the view model
        /// </summary>
        /// <param name="viewModel"></param>
        protected override void SetViewModelInstance(CollectionFile viewModel)
        {
            CollectionFile = viewModel;
        }

        /// <summary>
        /// Set DataContext to provided avalonia property
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_File_Initialized(object? sender, System.EventArgs e)
        {
            this.DataContext = this.CollectionFile;
        }

        /// <summary>
        /// Returns the file as bound element
        /// </summary>
        /// <returns></returns>
        protected override CollectionComponentBase GetBoundCollectionComponent()
        {
            return this.CollectionFile;
        }

        /// <summary>
        /// Opens the file document
        /// </summary>
        /// <returns></returns>
        protected override async Task OpenComponentDocument()
        {
            try
            {
                await _editorService.OpenFile(CollectionFile);
            }
            catch (Exception ex)
            {
                _log.LogCritical(ex, nameof(OpenComponentDocument));
                _notificationService.Notify(ex);
            }
        }
    }
}
