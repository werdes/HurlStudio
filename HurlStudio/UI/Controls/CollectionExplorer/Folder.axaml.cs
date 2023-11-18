using Avalonia;
using Avalonia.Controls;
using HurlStudio.Model.CollectionContainer;
using System;
using System.ComponentModel;

namespace HurlStudio.UI.Controls.CollectionExplorer
{
    public partial class Folder : UserControl
    {
        private CollectionFolder CollectionFolder
        {
            get => (CollectionFolder)GetValue(CollectionFolderProperty);
            set => SetValue(CollectionFolderProperty, value);
        }

        public static readonly StyledProperty<CollectionFolder> CollectionFolderProperty =
            AvaloniaProperty.Register<Folder, CollectionFolder>(nameof(CollectionFolder));

        public Folder()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Set DataContext to provided avalonia property
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_Folder_Initialized(object? sender, System.EventArgs e)
        {
            this.DataContext = CollectionFolder;
        }

        private void On_ButtonCollapse_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (this.DataContext == null) return;
            this.CollectionFolder.Collapsed = !CollectionFolder.Collapsed;
        }
    }
}
