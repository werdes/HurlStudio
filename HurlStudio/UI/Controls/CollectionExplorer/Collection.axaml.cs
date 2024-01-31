using Avalonia;
using Avalonia.Controls;
using HurlStudio.Model.CollectionContainer;
using HurlStudio.Services.Editor;
using HurlStudio.Services.Notifications;
using HurlStudio.UI.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace HurlStudio.UI.Controls.CollectionExplorer
{
    public partial class Collection : CollectionExplorerControlBase<CollectionContainer>
    {
        private CollectionContainer CollectionContainer
        {
            get => (CollectionContainer)GetValue(CollectionContainerProperty);
            set => SetValue(CollectionContainerProperty, value);
        }

        public static readonly StyledProperty<CollectionContainer> CollectionContainerProperty =
            AvaloniaProperty.Register<Collection, CollectionContainer>(nameof(CollectionContainer));

        private ILogger _log;
        private IEditorService _editorService;
        private INotificationService _notificationService;

        public Collection(ILogger<Collection> logger, INotificationService notificationService, IEditorService editorService)
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
        protected override void SetViewModelInstance(CollectionContainer viewModel)
        {
            CollectionContainer = viewModel;
        }

        /// <summary>
        /// Set DataContext to provided avalonia property
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_Collection_Initialized(object? sender, System.EventArgs e)
        {
            this.DataContext = CollectionContainer;
        }

        /// <summary>
        /// Toggle collapse state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_ButtonCollapse_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if(this.DataContext == null) return;
            CollectionContainer.Collapsed = !CollectionContainer.Collapsed;
        }

        /// <summary>
        /// Returns the collection container as bound element
        /// </summary>
        /// <returns></returns>
        protected override CollectionComponentBase GetBoundCollectionComponent()
        {
            return this.CollectionContainer;
        }

        /// <summary>
        /// Opens a collection settings document
        /// </summary>
        /// <returns></returns>
        protected override async Task OpenComponentDocument()
        {
            try
            {
                await _editorService.OpenCollectionSettings(CollectionContainer);
            }
            catch (Exception ex)
            {
                _log.LogCritical(ex, nameof(OpenComponentDocument));
                _notificationService.Notify(ex);
            }
        }
    }
}
