using Avalonia;
using Avalonia.Controls;
using HurlStudio.Model.CollectionContainer;
using System;
using System.ComponentModel;

namespace HurlStudio.UI.Controls.CollectionExplorer
{
    public partial class File : UserControl
    {
        private CollectionFile CollectionFile
        {
            get => (CollectionFile)GetValue(CollectionFileProperty);
            set => SetValue(CollectionFileProperty, value);
        }

        public static readonly StyledProperty<CollectionFile> CollectionFileProperty =
            AvaloniaProperty.Register<File, CollectionFile>(nameof(CollectionFile));

        public File()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Set DataContext to provided avalonia property
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_File_Initialized(object? sender, System.EventArgs e)
        {
            this.DataContext = CollectionFile;
        }
    }
}
