using Avalonia;
using Avalonia.Controls;
using HurlStudio.Model.CollectionContainer;
using System;
using System.ComponentModel;

namespace HurlStudio.UI.Controls.CollectionExplorer
{
    public partial class Collection : UserControl
    {
        private CollectionContainer CollectionContainer
        {
            get => (CollectionContainer)GetValue(CollectionContainerProperty);
            set => SetValue(CollectionContainerProperty, value);
        }

        public static readonly StyledProperty<CollectionContainer> CollectionContainerProperty =
            AvaloniaProperty.Register<Collection, CollectionContainer>(nameof(CollectionContainer));

        public Collection()
        {
            InitializeComponent();
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

        private void On_ButtonCollapse_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if(this.DataContext == null) return;
            CollectionContainer.Collapsed = !CollectionContainer.Collapsed;
        }
    }
}
