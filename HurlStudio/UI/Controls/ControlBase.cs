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

namespace HurlStudio.UI.Controls
{

    public abstract class ControlBase : UserControl
    {

        /// <summary>
        /// Displays an error type message box
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected async Task ShowErrorMessage(Exception exception, string? title = null, string? message = null)
        {
            string messageText = message ?? exception.Message;
            string titleText = title ?? Localization.Localization.MessageBox_ErrorTitle;

            IMsBox<ButtonResult> box = MessageBoxManager.GetMessageBoxStandard(titleText, messageText, ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
            await box.ShowWindowAsync();
        }

        /// <summary>
        /// Opens a file selection dialog for a single file
        /// </summary>
        /// <param name="storageProvider"></param>
        /// <param name="title"></param>
        /// <param name="allowedTypes"></param>
        /// <returns>A file path or null</returns>
        protected async Task<string?> OpenFileSingle(IStorageProvider storageProvider, string title, FilePickerFileType[] allowedTypes)
        {
            if (!storageProvider.CanOpen) return null;

            FilePickerOpenOptions filePickerOpenOptions = new FilePickerOpenOptions
            {
                AllowMultiple = false,
                Title = title,
                FileTypeFilter = allowedTypes
            };
            IReadOnlyList<IStorageFile> files =
                await storageProvider.OpenFilePickerAsync(filePickerOpenOptions);

            if (files.Count == 1)
            {
                return files.First().Path.AbsolutePath;
            }

            return null;
        }

        /// <summary>
        /// Opens a file selection dialog for a multiple files
        /// </summary>
        /// <param name="storageProvider"></param>
        /// <param name="title"></param>
        /// <param name="allowedTypes"></param>
        /// <returns></returns>
        protected async Task<IReadOnlyList<IStorageFile>?> OpenFileMulti(IStorageProvider storageProvider, string title, FilePickerFileType[] allowedTypes)
        {
            if (!storageProvider.CanOpen) return null;

            FilePickerOpenOptions filePickerOpenOptions = new FilePickerOpenOptions
            {
                AllowMultiple = false,
                Title = title,
                FileTypeFilter = allowedTypes
            };
            IReadOnlyList<IStorageFile> files =
                await storageProvider.OpenFilePickerAsync(filePickerOpenOptions);

            return files;
        }
    }
}
