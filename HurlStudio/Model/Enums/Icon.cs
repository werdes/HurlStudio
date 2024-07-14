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
    public enum IconSize
    {
        S = 1,
        M = 2,
        L = 3,
    }

    /// <summary>
    /// Icon colors:
    ///  Light:   #333333
    ///  Dark:    #cccccc
    ///  
    ///  Success: #76bc03
    ///  Info:    #64b1ff
    ///  Error:   #fa5252
    ///  Warning: #f8bd09
    /// </summary>
    public enum Icon
    {
        [Icon("blank.png")] Blank,
        [Icon("back.png")] Back,
        [Icon("forward.png")] Forward,
        [Icon("collapsed_square.png")] Collapsed,
        [Icon("expanded_square.png")] Expanded,
        [Icon("file.png")] File,
        [Icon("folder.png")] Folder,
        [Icon("folder_clear.png")] FolderClear,
        [Icon("collection.png")] Collection,
        [Icon("open.png")] Open,
        [Icon("save.png")] Save,
        [Icon("save_neutral.png")] SaveNeutral,
        [Icon("save_all.png")] SaveAll,
        [Icon("expand_all.png")] ExpandAll,
        [Icon("collapse_all.png")] CollapseAll,
        [Icon("environment.png")] Environment,
        [Icon("new.png")] New,
        [Icon("properties.png")] Properties,
        [Icon("rename.png")] Rename,
        [Icon("plus.png")] Plus,
        [Icon("more.png")] More,
        [Icon("open_neutral.png")] OpenNeutral,
        [Icon("add_folder.png")] AddFolder,
        [Icon("add_file.png")] AddFile,
        [Icon("import.png")] Import,
        [Icon("notifications_list.png")] NotificationsList,
        [Icon("close.png")] Close,
        [Icon("trash.png")] Trash,
        [Icon("reveal_explorer.png")] RevealExplorer,
        [Icon("word_wrap.png")] WordWrap,
        [Icon("end_of_line.png")] EndOfLine,
        [Icon("whitespace.png")] Whitespace,
        [Icon("close_tabs.png")] CloseTabs,
        [Icon("statusbar_idle.png")] StatusBarIdle,
        [Icon("redo.png")] Redo,
        [Icon("undo.png")] Undo,
        [Icon("overwritten.png")] Overwritten,
        [Icon("checked_box.png")] CheckedBox,
        [Icon("unchecked_box.png")] UncheckedBox,
        [Icon("move_up.png")] MoveUp,
        [Icon("move_down.png")] MoveDown,
        [Icon("move_top.png")] MoveTop,
        [Icon("move_bottom.png")] MoveBottom,
        [Icon("question_32.png")] Question32,
        [Icon("setting.png")] Setting,
        [Icon("add_setting.png")] AddSetting,
        [Icon("home.png")] Home,
        [Icon("info_color.png")] InfoColor,
        [Icon("warning_color.png")] WarningColor,
        [Icon("error_color.png")] ErrorColor,
        [Icon("question.png")] Question,
        [Icon("proxy.png")] Proxy,
        [Icon("link_external.png")] LinkExternal,
        [Icon("cancel.png")] Cancel,
        [Icon("ok.png")] Ok,
        [Icon("search.png")] Search,
        [Icon("refresh.png")] Refresh,
        [Icon("redefine.png")] Redefine,
        [Icon("duplicate.png")] Duplicate,
        [Icon("unavailable.png")] Unavailable,
        [Icon("add_environment.png")] AddEnvironment,
        [Icon("active.png")] Active,
        [Icon("inactive.png")] Inactive,
        [Icon("remove_collection.png")] RemoveCollection,
        [Icon("template.png")] Template,
        [Icon("edit.png")] Edit,
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class IconAttribute : Attribute
    {
        private readonly string _fileName;

        public IconAttribute(string fileName)
        {
            _fileName = fileName;
        }

        public string FileName
        {
            get => _fileName;
        }
    }

    public static class IconExtensions
    {
        private static Dictionary<Icon, Dictionary<IconSize, Bitmap>> _bitmapCache =
            new Dictionary<Icon, Dictionary<IconSize, Bitmap>>();

        public static string? GetFileName(this Icon icon)
        {
            MemberInfo? iconField = typeof(Icon).GetMember(icon.ToString())
                .FirstOrDefault(x => x.DeclaringType == typeof(Icon));
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

        public static Bitmap? GetBitmap(this Icon icon, IconSize iconSize, ThemeVariant themeVariant)
        {
            if (_bitmapCache.ContainsKey(icon) && _bitmapCache[icon].ContainsKey((iconSize)))
                return _bitmapCache[icon][iconSize];

            string? fileName = icon.GetFileName();
            string? assemblyName = Assembly.GetExecutingAssembly()?.GetName()?.Name;

            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(assemblyName)) return null;

            Uri path = new Uri($"avares://{assemblyName}/Assets/Icons/{themeVariant}/{iconSize}/{fileName}");
            if (!AssetLoader.Exists(path)) return null;

            Bitmap bitmap = new Bitmap(AssetLoader.Open(path));
            if (!_bitmapCache.ContainsKey(icon))
            {
                _bitmapCache[icon] = new Dictionary<IconSize, Bitmap>();
            }
            _bitmapCache[icon][iconSize] = bitmap;

            return bitmap;
        }
    }
}