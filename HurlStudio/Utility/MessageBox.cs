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
            Discard
        }

        private static Dictionary<ButtonType, string> _typeLocalization = new Dictionary<ButtonType, string>()
        {
            { ButtonType.OK , Localization.MessageBox_Button_OK },
            { ButtonType.Discard , Localization.MessageBox_Button_Discard },
            { ButtonType.Save , Localization.MessageBox_Button_Save },
            { ButtonType.Cancel, Localization.MessageBox_Button_Cancel }
        };

        public static async Task<ButtonType> ShowInfo(string message, string title) => await Show(message, title, [ButtonType.OK], Icon.MessageBoxInfo);
        public static async Task<ButtonType> ShowWarning(string message, string title) => await Show(message, title, [ButtonType.OK], Icon.MessageBoxWarning);
        public static async Task<ButtonType> ShowError(string message, string title) => await Show(message, title, [ButtonType.OK], Icon.MessageBoxError);

        /// <summary>
        /// Shows a message box
        /// </summary>
        /// <param name="message"></param>
        public static async Task<ButtonType> Show(string message, string title, ButtonType[] buttons, Icon icon)
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

            string messageBoxResult = await box.ShowAsync();

            foreach(ButtonType buttonType in buttons)
            {
                if(messageBoxResult == _typeLocalization[buttonType])
                {
                    return buttonType;
                }
            }

            return ButtonType.Undefined;
        }
    }
}
