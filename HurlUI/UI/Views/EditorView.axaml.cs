using Avalonia.Controls;
using HurlUI.UI.Dock;
using HurlUI.UI.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace HurlUI.UI.Views
{
    public partial class EditorView : ViewBase
    {
        private ILogger _log;
        private IConfiguration _configuration;
        private LayoutFactory _layoutFactory;
        private EditorViewViewModel _viewModel;
        private DockControlLocator _locator;

        public EditorView(EditorViewViewModel viewModel, ILogger<EditorView> logger, IConfiguration configuration, LayoutFactory layoutFactory, DockControlLocator dockControlLocator) : base(typeof(EditorViewViewModel))
        {
            this._viewModel = viewModel;
            this._log = logger;
            this._configuration = configuration;
            this._layoutFactory = layoutFactory;
            this._locator = dockControlLocator;

            this.DataContext = viewModel;
            this.DebugFactoryEvents(layoutFactory);

            this.DataTemplates.Add(this._locator);

            InitializeComponent();
        }

        /// <summary>
        /// Create the Dock layout on view load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_EditorView_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            this.DockControl.Layout = this._layoutFactory.CreateLayout();

            if (this.DockControl.Layout != null)
            {
                this._layoutFactory.InitLayout(this.DockControl.Layout);
            }
        }

        private void DebugFactoryEvents(LayoutFactory factory)
        {
            factory.ActiveDockableChanged += (_, args) =>
            {
                _log.LogDebug($"[ActiveDockableChanged] Title='{args.Dockable?.Title}'");
            };

            factory.FocusedDockableChanged += (_, args) =>
            {
                _log.LogDebug($"[FocusedDockableChanged] Title='{args.Dockable?.Title}'");
            };

            factory.DockableAdded += (_, args) =>
            {
                _log.LogDebug($"[DockableAdded] Title='{args.Dockable?.Title}'");
            };

            factory.DockableRemoved += (_, args) =>
            {
                _log.LogDebug($"[DockableRemoved] Title='{args.Dockable?.Title}'");
            };

            factory.DockableClosed += (_, args) =>
            {
                _log.LogDebug($"[DockableClosed] Title='{args.Dockable?.Title}'");
            };

            factory.DockableMoved += (_, args) =>
            {
                _log.LogDebug($"[DockableMoved] Title='{args.Dockable?.Title}'");
            };

            factory.DockableSwapped += (_, args) =>
            {
                _log.LogDebug($"[DockableSwapped] Title='{args.Dockable?.Title}'");
            };

            factory.DockablePinned += (_, args) =>
            {
                _log.LogDebug($"[DockablePinned] Title='{args.Dockable?.Title}'");
            };

            factory.DockableUnpinned += (_, args) =>
            {
                _log.LogDebug($"[DockableUnpinned] Title='{args.Dockable?.Title}'");
            };

            factory.WindowOpened += (_, args) =>
            {
                _log.LogDebug($"[WindowOpened] Title='{args.Window?.Title}'");
            };

            factory.WindowClosed += (_, args) =>
            {
                _log.LogDebug($"[WindowClosed] Title='{args.Window?.Title}'");
            };

            factory.WindowClosing += (_, args) =>
            {
                // NOTE: Set to True to cancel window closing.
#if false
                args.Cancel = true;
#endif
                _log.LogDebug($"[WindowClosing] Title='{args.Window?.Title}', Cancel={args.Cancel}");
            };

            factory.WindowAdded += (_, args) =>
            {
                _log.LogDebug($"[WindowAdded] Title='{args.Window?.Title}'");
            };

            factory.WindowRemoved += (_, args) =>
            {
                _log.LogDebug($"[WindowRemoved] Title='{args.Window?.Title}'");
            };

            factory.WindowMoveDragBegin += (_, args) =>
            {
                // NOTE: Set to True to cancel window dragging.
#if false
                args.Cancel = true;
#endif
                _log.LogDebug($"[WindowMoveDragBegin] Title='{args.Window?.Title}', Cancel={args.Cancel}, X='{args.Window?.X}', Y='{args.Window?.Y}'");
            };

            factory.WindowMoveDrag += (_, args) =>
            {
                _log.LogDebug($"[WindowMoveDrag] Title='{args.Window?.Title}', X='{args.Window?.X}', Y='{args.Window?.Y}");
            };

            factory.WindowMoveDragEnd += (_, args) =>
            {
                _log.LogDebug($"[WindowMoveDragEnd] Title='{args.Window?.Title}', X='{args.Window?.X}', Y='{args.Window?.Y}");
            };
        }
    }
}
