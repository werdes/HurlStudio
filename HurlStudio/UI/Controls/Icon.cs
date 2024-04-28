﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Utilities;
using HurlStudio.Model.CollectionContainer;
using HurlStudio.Model.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.UI.Controls
{
    public class Icon : Image
    {
        public Model.Enums.Icon Type
        {
            get => (Model.Enums.Icon)this.GetValue(TypeProperty);
            set => this.SetValue(TypeProperty, value);
        }

        public static readonly StyledProperty<Model.Enums.Icon> TypeProperty =
            AvaloniaProperty.Register<Icon, Model.Enums.Icon>(nameof(Type));

        public Icon()
        {
            this.Initialized += this.On_Icon_Initialized;
            this.Height = 16;
            this.Width = 16;
        }

        /// <summary>
        /// Set the image source according to the icon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_Icon_Initialized(object? sender, EventArgs e)
        {
            ThemeVariant currentThemeVariant = this.ActualThemeVariant;
            
            this.Source = this.Type.GetBitmap(currentThemeVariant);
        }
    }
}
