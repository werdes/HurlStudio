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

namespace HurlStudio.Utility
{
    /// <summary>
    /// Provides a System.Windows.Forms-like Wrapper for the MessageBox.Avalonia kit
    /// </summary>
    public static class MessageBox
    {
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
                ImageIcon = icon.GetBitmap(ThemeVariant ?? ThemeVariant.Default),
                WindowStartupLocation = WindowStartupLocation.CenterOwner
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
                    IsCancel = true,
                    Name = _typeLocalization[ButtonType.Cancel]
                },
                new ButtonDefinition()
                {
                    IsCancel = false,
                    Name = _typeLocalization[ButtonType.OK]
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
                ImageIcon = icon.GetBitmap(ThemeVariant ?? ThemeVariant.Default),
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            });

            return box;
        }

        public static async Task<ButtonType> ShowInfo(string message, string title) => await Show(message, title, [ButtonType.OK], Icon.MessageBoxInfo);
        public static async Task<ButtonType> ShowWarning(string message, string title) => await Show(message, title, [ButtonType.OK], Icon.MessageBoxWarning);
        public static async Task<ButtonType> ShowError(string message, string title) => await Show(message, title, [ButtonType.OK], Icon.MessageBoxError);
        public static async Task<ButtonType> ShowQuestionYesNo(string message, string title) => await Show(message, title, [ButtonType.Yes, ButtonType.No], Icon.MessageBoxQuestion);
        public static async Task<ButtonType> ShowInfoDialog(Window owner, string message, string title) => await ShowDialog(owner, message, title, [ButtonType.OK], Icon.MessageBoxInfo);
        public static async Task<ButtonType> ShowWarningDialog(Window owner, string message, string title) => await ShowDialog(owner, message, title, [ButtonType.OK], Icon.MessageBoxWarning);
        public static async Task<ButtonType> ShowErrorDialog(Window owner, string message, string title) => await ShowDialog(owner, message, title, [ButtonType.OK], Icon.MessageBoxError);
        public static async Task<ButtonType> ShowQuestionYesNoDialog(Window owner, string message, string title) => await ShowDialog(owner, message, title, [ButtonType.Yes, ButtonType.No], Icon.MessageBoxQuestion);
    }
}
