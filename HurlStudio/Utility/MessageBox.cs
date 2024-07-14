using Avalonia.Controls;
using HurlStudio.Model.Enums;
using HurlStudio.UI.Localization;
using HurlStudio.UI.Windows;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.Models;
using MsBox.Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Styling;
using MsBox.Avalonia.Controls;
using MsBox.Avalonia.ViewModels;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System.Reflection;

namespace HurlStudio.Utility
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

        public enum ButtonType
        {
            Undefined,
            OK,
            Cancel,
            Save,
            Discard,
            Rename,
            Yes,
            No
        }

        private static Dictionary<ButtonType, string> _typeLocalization = new Dictionary<ButtonType, string>()
        {
            { ButtonType.OK , Localization.MessageBox_Button_OK },
            { ButtonType.Discard , Localization.MessageBox_Button_Discard },
            { ButtonType.Save , Localization.MessageBox_Button_Save },
            { ButtonType.Cancel, Localization.MessageBox_Button_Cancel },
            { ButtonType.Rename, Localization.MessageBox_Button_Rename },
            { ButtonType.Yes, Localization.MessageBox_Button_Yes },
            { ButtonType.No, Localization.MessageBox_Button_No }
        };

        /// <summary>
        /// Shows a message box
        /// </summary>
        /// <param name="message"></param>
        public static async Task<ButtonType> Show(string message, string title, ButtonType[] buttons, Icon icon)
        {
            IMsBox<string> box = SetupMessageBoxInternal(message, title, buttons, icon);
            string messageBoxResult = await box.ShowAsync();
            return EvaluateMessageBoxResult(buttons, messageBoxResult);
        }

        /// <summary>
        /// Shows a message box
        /// </summary>
        /// <param name="message"></param>
        public static async Task<string?> AskInput(string message, string title, string value, Icon icon)
        {
            IMsBox<string> box = SetupInputMessageBoxInternal(message, title, value, icon);
            string messageBoxResult = await box.ShowAsync();

            if (EvaluateMessageBoxResult([ButtonType.Cancel, ButtonType.OK], messageBoxResult) == ButtonType.OK)
            {
                return box.InputValue;
            }
            return null;
        }

        /// <summary>
        /// Shows a message box as a dialog
        /// </summary>
        /// <param name="message"></param>
        public static async Task<ButtonType> ShowDialog(Window owner, string message, string title, ButtonType[] buttons, Icon icon)
        {
            IMsBox<string> box = SetupMessageBoxInternal(message, title, buttons, icon);
            string messageBoxResult = await box.ShowWindowDialogAsync(owner);
            return EvaluateMessageBoxResult(buttons, messageBoxResult);
        }

        /// <summary>
        /// Shows a message box
        /// </summary>
        /// <param name="message"></param>
        public static async Task<string?> AskInputDialog(Window owner, string message, string title, string value, Icon icon)
        {
            IMsBox<string> box = SetupInputMessageBoxInternal(message, title, value, icon);
            string messageBoxResult = await box.ShowWindowDialogAsync(owner);

            if (EvaluateMessageBoxResult([ButtonType.Cancel, ButtonType.OK], messageBoxResult) == ButtonType.OK)
            {
                return box.InputValue;
            }
            return null;
        }

        /// <summary>
        /// Returns a message box result based on the button definitions
        /// </summary>
        /// <param name="buttons"></param>
        /// <param name="messageBoxResult"></param>
        /// <returns></returns>
        private static ButtonType EvaluateMessageBoxResult(ButtonType[] buttons, string messageBoxResult)
        {
            foreach (ButtonType buttonType in buttons)
            {
                if (messageBoxResult == _typeLocalization[buttonType])
                {
                    return buttonType;
                }
            }

            return ButtonType.Undefined;
        }

        /// <summary>
        /// Builds a message box
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <param name="buttons"></param>
        /// <param name="icon"></param>
        /// <returns></returns>
        private static IMsBox<string> SetupMessageBoxInternal(string message, string title, ButtonType[] buttons, Icon icon)
        {
            List<ButtonDefinition> buttonDefinitions = buttons.Select(x => new ButtonDefinition()
            {
                IsDefault = (x == ButtonType.OK),
                IsCancel = (x == ButtonType.Cancel),
                Name = _typeLocalization[x]
            }).ToList();

            IMsBox<string> box = MessageBoxManager.GetMessageBoxCustom(new MsBox.Avalonia.Dto.MessageBoxCustomParams()
            {
                ButtonDefinitions = buttonDefinitions,
                ContentTitle = title,
                ContentMessage = message,
                CanResize = false,
                Icon = MsBox.Avalonia.Enums.Icon.Warning,
                ImageIcon = icon.GetBitmap(IconSize.M, ThemeVariant ?? ThemeVariant.Default),
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                WindowIcon = new WindowIcon(_windowIcon)
            });

            return box;
        }

        /// <summary>
        /// Builds a message box
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <param name="buttons"></param>
        /// <param name="icon"></param>
        /// <returns></returns>
        private static IMsBox<string> SetupInputMessageBoxInternal(string message, string title, string value, Icon icon)
        {
            List<ButtonDefinition> buttonDefinitions = new List<ButtonDefinition>() {
                new ButtonDefinition()
                {
                    IsCancel = false,
                    IsDefault = true,
                    Name = _typeLocalization[ButtonType.OK]
                },
                new ButtonDefinition()
                {
                    IsCancel = true,
                    Name = _typeLocalization[ButtonType.Cancel]
                }
            };

            IMsBox<string> box = MessageBoxManager.GetMessageBoxCustom(new MsBox.Avalonia.Dto.MessageBoxCustomParams()
            {
                ButtonDefinitions = buttonDefinitions,
                ContentTitle = title,
                ContentMessage = message,
                CanResize = false,
                InputParams = new MsBox.Avalonia.Dto.InputParams()
                {
                    DefaultValue = value,
                    Multiline = false
                },
                ImageIcon = icon.GetBitmap(IconSize.M, ThemeVariant ?? ThemeVariant.Default),
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                WindowIcon = new WindowIcon(_windowIcon)
            });

            return box;
        }

        public static async Task<ButtonType> ShowInfo(string message, string title) => await Show(message, title, [ButtonType.OK], Icon.InfoColor);
        public static async Task<ButtonType> ShowWarning(string message, string title) => await Show(message, title, [ButtonType.OK], Icon.WarningColor);
        public static async Task<ButtonType> ShowError(string message, string title) => await Show(message, title, [ButtonType.OK], Icon.ErrorColor);
        public static async Task<ButtonType> ShowQuestionYesNo(string message, string title) => await Show(message, title, [ButtonType.Yes, ButtonType.No], Icon.Question);
        public static async Task<ButtonType> ShowInfoDialog(Window owner, string message, string title) => await ShowDialog(owner, message, title, [ButtonType.OK], Icon.InfoColor);
        public static async Task<ButtonType> ShowWarningDialog(Window owner, string message, string title) => await ShowDialog(owner, message, title, [ButtonType.OK], Icon.WarningColor);
        public static async Task<ButtonType> ShowErrorDialog(Window owner, string message, string title) => await ShowDialog(owner, message, title, [ButtonType.OK], Icon.ErrorColor);
        public static async Task<ButtonType> ShowQuestionYesNoDialog(Window owner, string message, string title) => await ShowDialog(owner, message, title, [ButtonType.Yes, ButtonType.No], Icon.Question);
    }
}
