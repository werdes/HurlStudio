using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using AvaloniaEdit.Utils;
using HurlStudio.Model.Enums;
using HurlStudio.UI.MessageBox.Controls;
using HurlStudio.UI.MessageBox.Model;
using MsBox.Avalonia;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.Models;

namespace HurlStudio.UI.MessageBox
{
    /// <summary>
    /// Provides a System.Windows.Forms-like Wrapper for the MessageBox.Avalonia kit
    /// </summary>
    public static class MessageBox
    {
        private static Bitmap _windowIcon;

        static MessageBox()
        {
#pragma warning disable CS8601 // Mögliche Nullverweiszuweisung.
            _windowIcon = GetWindowIcon() ?? Icon.Blank.GetBitmap(IconSize.S, ThemeVariant.Default);
#pragma warning restore CS8601 // Mögliche Nullverweiszuweisung.
        }

        /// <summary>
        /// Returns a bitmap with the icon
        /// </summary>
        /// <returns></returns>
        private static Bitmap? GetWindowIcon()
        {
            string? assemblyName = Assembly.GetExecutingAssembly()?.GetName()?.Name;
            if (assemblyName == null) return null;

            Uri path = new Uri($"avares://{assemblyName}/Assets/Icons/icon.ico");
            if (!AssetLoader.Exists(path)) return null;

            Bitmap bitmap = new Bitmap(AssetLoader.Open(path));
            return bitmap;
        }

        public static ThemeVariant? ThemeVariant { get; set; }


        /// <summary>
        /// Shows a message box
        /// </summary>
        /// <param name="message">The message text displayed inside the message box</param>
        /// <param name="title">The message box's window title</param>
        /// <param name="buttons">The buttons to be displayed</param>
        /// <param name="icon">The icon displayed inside the message box</param>
        public static void Show(string message, string title, MessageBoxResult[] buttons, Icon icon)
        {
            MessageBoxWindow messageBox = SetupMessageBoxWindow(message, title, buttons, icon);
            messageBox.Show();
        }

        /// <summary>
        /// Shows a message box as a dialog
        /// </summary>
        /// <param name="owner">The owner window of the message box</param>
        /// <param name="message">The message text displayed inside the message box</param>
        /// <param name="title">The message box's window title</param>
        /// <param name="buttons">The buttons to be displayed</param>
        /// <param name="icon">The icon displayed inside the message box</param>
        public static async Task<MessageBoxResult> ShowDialog(Window owner, string message, string title,
            MessageBoxResult[] buttons, Icon icon)
        {
            MessageBoxWindow messageBox = SetupMessageBoxWindow(message, title, buttons, icon);

            MessageBoxResult messageBoxResult = await messageBox.ShowDialog<MessageBoxResult>(owner);
            return messageBoxResult;
        }

        /// <summary>
        /// Shows a message box as a dialog asking for a user input
        /// </summary>
        /// <param name="owner">The owner window of the message box</param>
        /// <param name="message">The message text displayed inside the message box</param>
        /// <param name="title">The message box's window title</param>
        /// <param name="value">The initial value of the box's input field</param>
        /// <param name="icon">The icon displayed inside the message box</param>
        public static async Task<string?> AskInputDialog(Window owner, string message, string title, string value,
            Icon icon)
        {
            MessageBoxWindow messageBox = SetupInputMessageBoxWindow(message, title, value, icon);
            string? messageBoxResult = await messageBox.ShowDialog<string?>(owner);

            return messageBoxResult;
        }

        /// <summary>
        /// Builds a message box
        /// </summary>
        /// <param name="message">The message text displayed inside the message box</param>
        /// <param name="title">The message box's window title</param>
        /// <param name="buttons">The buttons to be displayed</param>
        /// <param name="icon">The icon displayed inside the message box</param>
        /// <returns></returns>
        private static MessageBoxWindow SetupMessageBoxWindow(string message, string title, MessageBoxResult[] buttons,
            Icon icon)
        {
            List<MessageBoxButtonDefinition> buttonDefinitions = buttons.Select(x =>
                new MessageBoxButtonDefinition(
                    x.GetIcon(),
                    x.GetLocalizedString(),
                    x,
                    (x == MessageBoxResult.OK),
                    (x == MessageBoxResult.Cancel))).ToList();

            MessageBoxViewModel viewModel = new MessageBoxViewModel(
                title,
                message,
                MessageBoxLayout.Default,
                string.Empty,
                icon,
                _windowIcon);
            viewModel.Buttons.AddRange(buttonDefinitions);

            MessageBoxWindow messageBoxWindow = new MessageBoxWindow(viewModel);

            return messageBoxWindow;
        }

        /// <summary>
        /// Builds a message box for value input
        /// </summary>
        /// <param name="message">The message text displayed inside the message box</param>
        /// <param name="title">The message box's window title</param>
        /// <param name="value">The initial value of the box's input field</param>
        /// <param name="icon">The icon displayed inside the message box</param>
        /// <returns>The message box window</returns>
        private static MessageBoxWindow SetupInputMessageBoxWindow(string message, string title, string? value,
            Icon icon)
        {
            List<MessageBoxButtonDefinition> buttonDefinitions = new List<MessageBoxButtonDefinition>()
            {
                new MessageBoxButtonDefinition(MessageBoxResult.OK, true, false),
                new MessageBoxButtonDefinition(MessageBoxResult.Cancel, false, true)
            };

            MessageBoxViewModel viewModel = new MessageBoxViewModel(
                title,
                message,
                MessageBoxLayout.Input,
                value,
                icon,
                _windowIcon);
            viewModel.Buttons.AddRange(buttonDefinitions);

            MessageBoxWindow messageBoxWindow = new MessageBoxWindow(viewModel);
            return messageBoxWindow;
        }

        public static void ShowError(string message, string title) =>
            Show(message, title, [MessageBoxResult.OK], Icon.ErrorColor);

        public static async Task<MessageBoxResult> ShowInfoDialog(Window owner, string message, string title) =>
            await ShowDialog(owner, message, title, [MessageBoxResult.OK], Icon.InfoColor);

        public static async Task<MessageBoxResult> ShowWarningDialog(Window owner, string message, string title) =>
            await ShowDialog(owner, message, title, [MessageBoxResult.OK], Icon.WarningColor);

        public static async Task<MessageBoxResult> ShowErrorDialog(Window owner, string message, string title) =>
            await ShowDialog(owner, message, title, [MessageBoxResult.OK], Icon.ErrorColor);

        public static async Task<MessageBoxResult>
            ShowQuestionYesNoDialog(Window owner, string message, string title) =>
            await ShowDialog(owner, message, title, [MessageBoxResult.Yes, MessageBoxResult.No], Icon.Question);
    }
}