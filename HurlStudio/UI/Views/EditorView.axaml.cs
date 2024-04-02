using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Dock.Model.Core;
using Dock.Model.Mvvm;
using Dock.Model.Mvvm.Controls;
using HurlStudio.Services.Editor;
using HurlStudio.Services.Notifications;
using HurlStudio.UI.Dock;
using HurlStudio.UI.ViewModels;
using HurlStudio.UI.ViewModels.Documents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace HurlStudio.UI.Views
{
    public partial class EditorView : ViewBase<EditorViewViewModel>
    {
        private EditorViewViewModel? _viewModel;
        private ILogger _log;
        private IConfiguration _configuration;
        private LayoutFactory _layoutFactory;
        private INotificationService _notificationService;
        private IEditorService _editorService;

        public EditorView(ILogger<EditorView> logger, IConfiguration configuration, LayoutFactory layoutFactory, INotificationService notificationService, IEditorService editorService)
        {
            _log = logger;
            _configuration = configuration;
            _layoutFactory = layoutFactory;
            _notificationService = notificationService;
            _editorService = editorService;

            this.DebugFactoryEvents(layoutFactory);

            this.InitializeComponent();
        }

        /// <summary>
        /// Prevent dock collapse by adding a welcome document once the last document is closed
        /// -> Also tell the editor service to close the file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_LayoutFactory_DockableRemoved(object? sender, global::Dock.Model.Core.Events.DockableRemovedEventArgs e)
        {
            if (sender == null || sender is not LayoutFactory layoutFactory) return;

            if (_viewModel?.Documents.Count == 0)
            {
                _layoutFactory.AddWelcomeDocument();
            }
        }

        /// <summary>
        /// Parameterless constructor for avalonia design
        /// </summary>
        public EditorView() { }

        /// <summary>
        /// Create the Dock layout on view load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_EditorView_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                _viewModel.Layout = _layoutFactory.CreateLayout();

                if (_viewModel.Layout != null)
                {
                    _layoutFactory.InitLayout(_viewModel.Layout);
                    _layoutFactory.ActiveDockableChanged += this.On_LayoutFactory_ActiveDockableChanged;
                    _layoutFactory.DockableRemoved += this.On_LayoutFactory_DockableRemoved;
                    _layoutFactory.DockableClosed += this.On_LayoutFactory_DockableClosed;
                }
            }
            catch (Exception ex)
            {
                _log.LogCritical(ex, nameof(this.On_EditorView_Loaded));
                await this.ShowErrorMessage(ex);
            }
        }

        /// <summary>
        /// On Dockable Closed 
        /// -> tell the editor service to close the file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_LayoutFactory_DockableClosed(object? sender, global::Dock.Model.Core.Events.DockableClosedEventArgs e)
        {
            // tell the editor service to close the file
            if (e.Dockable != null && e.Dockable is FileDocumentViewModel fileDocumentViewModel)
            {
                _editorService.CloseFileDocument(fileDocumentViewModel);
            }
        }

        /// <summary>
        /// On Tab change 
        /// -> Reevaluate Undo/Redo actions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_LayoutFactory_ActiveDockableChanged(object? sender, global::Dock.Model.Core.Events.ActiveDockableChangedEventArgs e)
        {
            _log.LogDebug($"[ActiveDockableChanged] Title='{e.Dockable?.Title}'");

            if (e.Dockable != null && e.Dockable is FileDocumentViewModel document)
            {
                if (document.Document != null)
                {
                    _viewModel.CanRedo = document.Document.UndoStack.CanRedo;
                    _viewModel.CanUndo = document.Document.UndoStack.CanUndo;
                }
                else
                {
                    _viewModel.CanRedo = false;
                    _viewModel.CanUndo = false;
                }
            }
            else
            {
                _viewModel.CanRedo = false;
                _viewModel.CanUndo = false;
            }
        }

        private void DebugFactoryEvents(LayoutFactory factory)
        {
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

        /// <summary>
        /// On Undo click -> call active document editor's undo action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_ButtonUndo_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_viewModel != null &&
               _viewModel.DocumentDock != null &&
               _viewModel.DocumentDock.ActiveDockable != null &&
               _viewModel.DocumentDock.ActiveDockable is FileDocumentViewModel file)
            {
                if (file.Document != null && file.Document.UndoStack.CanUndo)
                {
                    file.Document.UndoStack.Undo();
                    _viewModel.CanUndo = file.Document.UndoStack.CanUndo;
                    _viewModel.CanRedo = file.Document.UndoStack.CanRedo;
                }
            }
        }

        /// <summary>
        /// On Redo click -> call active document editor's redo action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_ButtonRedo_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_viewModel != null &&
               _viewModel.DocumentDock != null &&
               _viewModel.DocumentDock.ActiveDockable != null &&
               _viewModel.DocumentDock.ActiveDockable is FileDocumentViewModel file)
            {
                if (file.Document != null && file.Document.UndoStack.CanRedo)
                {
                    file.Document.UndoStack.Redo();
                    _viewModel.CanUndo = file.Document.UndoStack.CanUndo;
                    _viewModel.CanRedo = file.Document.UndoStack.CanRedo;
                }
            }
        }

        protected override void SetViewModelInstance(EditorViewViewModel viewModel)
        {
            _viewModel = viewModel;
            this.DataContext = _viewModel;
        }
    }
}
