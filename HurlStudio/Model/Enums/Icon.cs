using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using HurlStudio.UI.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Model.Enums
{
    /// <summary>
    /// Icon colors:
    ///  Light: #333333
    ///  Dark:  #cccccc
    /// </summary>
    public enum Icon
    {
        [Icon("blank.png")]
        Blank,
        [Icon("back.png")]
        Back,
        [Icon("forward.png")]
        Forward,
        [Icon("collapsed_square.png")]
        Collapsed,
        [Icon("expanded_square.png")]
        Expanded,
        [Icon("file.png")]
        File,
        [Icon("folder.png")]
        Folder,
        [Icon("folder_clear.png")]
        FolderClear,
        [Icon("collection.png")]
        Collection,
        [Icon("collection_32.png")]
        Collection32,
        [Icon("open.png")]
        Open,
        [Icon("save.png")]
        Save,
        [Icon("save_all.png")]
        SaveAll,
        [Icon("expand_all.png")]
        ExpandAll,
        [Icon("collapse_all.png")]
        CollapseAll,
        [Icon("environment.png")]
        Environment,
        [Icon("new.png")]
        New,
        [Icon("properties.png")]
        Properties,
        [Icon("rename.png")]
        Rename,
        [Icon("plus.png")]
        Plus,
        [Icon("more.png")]
        More,
        [Icon("open_neutral.png")]
        OpenNeutral,
        [Icon("add_folder.png")]
        AddFolder,
        [Icon("add_file.png")]
        AddFile,
        [Icon("import.png")]
        Import,
        [Icon("notifications_list.png")]
        NotificationsList,
        [Icon("close.png")]
        Close,
        [Icon("notification_none.png")]
        NotificationNone,
        [Icon("notification_info.png")]
        NotificationInfo,
        [Icon("notification_warning.png")]
        NotificationWarning,
        [Icon("notification_error.png")]
        NotificationError,
        [Icon("trash.png")]
        Trash,
        [Icon("reveal_explorer.png")]
        RevealExplorer,
        [Icon("word_wrap.png")]
        WordWrap,
        [Icon("end_of_line.png")]
        EndOfLine,
        [Icon("whitespace.png")]
        Whitespace,
        [Icon("close_tabs.png")]
        CloseTabs,
        [Icon("statusbar_idle.png")]
        StatusBarIdle,
        [Icon("redo.png")]
        Redo,
        [Icon("undo.png")]
        Undo,
        [Icon("overwritten.png")]
        Overwritten,
        [Icon("checked_box.png")]
        CheckedBox,
        [Icon("unchecked_box.png")]
        UncheckedBox,
        [Icon("move_up.png")]
        MoveUp,
        [Icon("move_down.png")]
        MoveDown,
        [Icon("question_32.png")]
        Question32,
        [Icon("add_setting.png")]
        AddSetting,
        [Icon("home.png")]
        Home,
        [Icon("messagebox_info_50.png")]
        MessageBoxInfo,
        [Icon("messagebox_warning_50.png")]
        MessageBoxWarning,
        [Icon("messagebox_error_50.png")]
        MessageBoxError,
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class IconAttribute : Attribute
    {
        private string _fileName;

        public IconAttribute(string fileName)
        {
            _fileName = fileName;
        }

        public string FileName { get => _fileName; }
    }

    public static class IconExtensions
    {
        public static string? GetFileName(this Icon icon)
        {
            MemberInfo? iconField = typeof(Icon).GetMember(icon.ToString()).FirstOrDefault(x => x.DeclaringType == typeof(Icon));
            if (iconField != null)
            {
                IconAttribute? attribute = iconField.GetCustomAttributes<IconAttribute>(false).FirstOrDefault();
                if (attribute != null)
                {
                    return attribute.FileName;
                }
            }

            return string.Empty;
        }

        public static Bitmap? GetBitmap(this Icon icon, ThemeVariant themeVariant)
        {
            string? fileName = icon.GetFileName();
            string? assemblyName = Assembly.GetExecutingAssembly()?.GetName()?.Name;

            if (!string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(assemblyName))
            {
                Uri path = new Uri($"avares://{assemblyName}/Assets/Icons/{themeVariant}/{fileName}");

                Bitmap bitmap = new Bitmap(AssetLoader.Open(path));
                return bitmap;
            }
            return null;
        }
    }
}
