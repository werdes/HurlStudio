using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using HurlUI.UI.ViewModels;
using HurlUI.UI.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using static System.Net.Mime.MediaTypeNames;

namespace HurlUI.UI.Controls
{
    public partial class ViewFrame : UserControl
    {
        private Type SelectedView
        {
            get => (Type)GetValue(SelectedViewProperty);
            set => SetValue(SelectedViewProperty, value);
        }

        public static readonly StyledProperty<Type> SelectedViewProperty =
            AvaloniaProperty.Register<ViewFrame, Type>(nameof(SelectedView));

        private ServiceManager<ViewBase>? _viewBuilder;
        private INavigatableViewModel? _navigatableViewModel;
        private Dictionary<Type, ViewBase> _views;

        public ViewFrame()
        {
            _viewBuilder = null;
            _navigatableViewModel = null;
            _views = new Dictionary<Type, ViewBase>();
        }

        public ViewFrame(INavigatableViewModel navigatableViewModel, ServiceManager<ViewBase> viewBuilder)
        {
            _views = new Dictionary<Type, ViewBase>();
            _navigatableViewModel = navigatableViewModel;
            _viewBuilder = viewBuilder;

            InitializeComponent();
        }

        /// <summary>
        /// Changes the view of the frame to the one associated with the given viewmodel
        /// </summary>
        /// <param name="view"></param>
        public void NavigateTo(ViewModelBase view)
        {
            SelectedView = view.GetType();
        }

        /// <summary>
        /// Fills the control/viewmodel association list
        /// </summary>
        private void Build()
        {
            if (_viewBuilder == null) throw new ArgumentNullException($"No viewBuilder was supplied to {nameof(ViewFrame)}");
            if (_navigatableViewModel == null) throw new ArgumentNullException($"No navigatable view model was supplied to {nameof(ViewFrame)}");

            IEnumerable<ViewModelBase> pages = _navigatableViewModel.GetNavigationTargets();
            foreach (ViewModelBase viewModelBase in pages)
            {
                _views.Add(viewModelBase.GetType(), _viewBuilder.Get(viewModelBase.AttachedViewType));
            }
        }

        /// <summary>
        /// Overridden PropertyChanged event for reacting to the SelectedView property change
        /// </summary>
        /// <param name="change"></param>
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == SelectedViewProperty)
            {
                Type? viewModelType = (Type?)change.NewValue;
                ViewBase? viewBase = null;
                if (viewModelType != null && _views.TryGetValue(viewModelType, out viewBase))
                {
                    this.Content = viewBase;
                }
                else
                {
                    this.Content = new TextBlock() { Text = $"No view registered for type {change.NewValue}" };
                }
            }
        }

        /// <summary>
        /// Once the viewFrame is initialized, building of the page list can be done as
        /// the viewBuilder has all views registered by now
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_ViewFrame_Initialized(object? sender, System.EventArgs e)
        {
            this.Build();
        }
    }
}
