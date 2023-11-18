using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Model.Enums
{
    public enum Icon
    {
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
        [Icon("collection.png")]
        Collection,
        [Icon("open.png")]
        Open,
        [Icon("save.png")]
        Save,
        [Icon("save_all.png")]
        SaveAll
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class IconAttribute : Attribute
    {
        private string _fileName;

        public IconAttribute(string fileName)
        {
            _fileName = fileName;
        }

        public string Path { get => _fileName; }
    }

    public static class IconExtensions
    {
        public static string? GetFileName(this Icon icon)
        {
            MemberInfo? iconField = typeof(Icon).GetMember(icon.ToString()).FirstOrDefault(x => x.DeclaringType == typeof(Icon));
            if(iconField != null)
            {
                IconAttribute? attribute = iconField.GetCustomAttributes<IconAttribute>(false).FirstOrDefault();
                if (attribute != null)
                {
                    return attribute.Path;
                }
            }

            return string.Empty;
        }
    }
}
