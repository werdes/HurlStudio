﻿using Avalonia.Controls;
using HurlStudio.UI.ViewModels;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HurlStudio.UI.Controls;

namespace HurlStudio.UI.Views
{
    public abstract class ViewBase<T> : ViewModelBasedControl<T>
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
    }
}
