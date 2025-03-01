using System;
using HurlStudio.Model.Enums;

namespace HurlStudio.UI.MessageBox
{
    public enum MessageBoxResult
    {
        Undefined,

        // ReSharper disable once InconsistentNaming
        OK,
        Cancel,
        Save,
        Discard,
        Rename,
        Yes,
        No
    }

    public static class MessageBoxResultExtensions
    {
        /// <summary>
        /// Returns a localized string corresponding to a messageBoxResult
        /// </summary>
        /// <param name="messageBoxResult"></param>
        /// <returns></returns>
        public static string GetLocalizedString(this MessageBoxResult messageBoxResult)
        {
            switch (messageBoxResult)
            {
                case MessageBoxResult.OK:
                    return Localization.Localization.MessageBox_Button_OK;
                case MessageBoxResult.Cancel:
                    return Localization.Localization.MessageBox_Button_Cancel;
                case MessageBoxResult.Save:
                    return Localization.Localization.MessageBox_Button_Save;
                case MessageBoxResult.Discard:
                    return Localization.Localization.MessageBox_Button_Discard;
                case MessageBoxResult.Rename:
                    return Localization.Localization.MessageBox_Button_Rename;
                case MessageBoxResult.Yes:
                    return Localization.Localization.MessageBox_Button_Yes;
                case MessageBoxResult.No:
                    return Localization.Localization.MessageBox_Button_No;
                case MessageBoxResult.Undefined:
                default:
                    return messageBoxResult.ToString();
            }
        }
        
        /// <summary>
        /// Returns the corresponding icon to a messageBoxResult
        /// </summary>
        /// <param name="messageBoxResult"></param>
        /// <returns></returns>
        public static Icon GetIcon(this MessageBoxResult messageBoxResult)
        {
            switch (messageBoxResult)
            {
                case MessageBoxResult.OK:
                    return Icon.Ok;
                case MessageBoxResult.Cancel:
                    return Icon.Cancel;
                case MessageBoxResult.Save:
                    return Icon.Save;
                case MessageBoxResult.Discard:
                    return Icon.Trash;
                case MessageBoxResult.Rename:
                    return Icon.Rename;
                case MessageBoxResult.Yes:
                    return Icon.Ok;
                case MessageBoxResult.No:
                    return Icon.Cancel;
                case MessageBoxResult.Undefined:
                default:
                    return Icon.Blank;
            }
        }
    }
}