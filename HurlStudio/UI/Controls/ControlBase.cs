using Avalonia.Controls;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using HurlStudio.UI.Windows;
using HurlStudio.Utility;

namespace HurlStudio.UI.Controls
{
    public abstract class ControlBase : UserControl
    {
        protected Windows.WindowBase? _window;
        public Windows.WindowBase? Window
        {
            get => _window;
        }

        protected async Task<string?> DisplayOpenFilePickerSingle(string title, FilePickerFileType[] allowedTypes) => 
            await StorageUtility.DisplayOpenFilePickerSingle(_window?.StorageProvider, title, allowedTypes);
        protected async Task<IReadOnlyList<IStorageFile>?> DisplayOpenFilePickerMulti(string title, FilePickerFileType[] allowedTypes) => 
            await StorageUtility.DisplayOpenFilePickerMulti(_window?.StorageProvider, title, allowedTypes);
        protected async Task<string?> DisplayOpenDirectoryPickerSingle(string title) => 
            await StorageUtility.DisplayOpenDirectoryPickerSingle(_window?.StorageProvider, title);
        protected async Task<string?> DisplaySaveFilePicker(string title, string defaultExtension, FilePickerFileType[] allowedTypes) =>
            await StorageUtility.DisplaySaveFilePickerSingle(_window?.StorageProvider, title, defaultExtension, allowedTypes);
    }
}
