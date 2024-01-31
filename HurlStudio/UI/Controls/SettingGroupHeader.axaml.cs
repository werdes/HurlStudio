using Avalonia;
using Avalonia.Controls;
using HurlStudio.Model.Enums;

namespace HurlStudio.UI.Controls
{
    public partial class SettingGroupHeader : UserControl
    {
        public static readonly StyledProperty<string> TextProperty =
            AvaloniaProperty.Register<SettingGroupHeader, string>(nameof(Text));
        public static readonly StyledProperty<Model.Enums.Icon> IconProperty =
            AvaloniaProperty.Register<SettingGroupHeader, Model.Enums.Icon>(nameof(Icon));
        public static readonly StyledProperty<string> SubTextProperty =
            AvaloniaProperty.Register<SettingGroupHeader, string>(nameof(SubText));


        public string Text
        {
            get => this.GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public Model.Enums.Icon Icon
        {
            get => this.GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public string SubText
        {
            get => this.GetValue(SubTextProperty);
            set => SetValue(SubTextProperty, value);
        }

        public SettingGroupHeader()
        {
            InitializeComponent();
        }
    }
}
