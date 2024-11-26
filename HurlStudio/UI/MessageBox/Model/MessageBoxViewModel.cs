using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Avalonia.Media.Imaging;
using HurlStudio.Model.Enums;

namespace HurlStudio.UI.MessageBox.Model
{
    public class MessageBoxViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void Notify([CallerMemberName] string propertyName = "") => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        
        private string _title;
        private string _content;
        private Icon _icon;
        private Bitmap _windowIcon;
        private ObservableCollection<MessageBoxButtonDefinition> _buttons;
        private MessageBoxLayout _layout;
        private string? _value;

        public MessageBoxViewModel(string title, string content, MessageBoxLayout layout, string? value, Icon icon, Bitmap windowIcon)
        {
            _title = title;
            _content = content;
            _layout = layout;
            _value = value;
            _icon = icon;
            _windowIcon = windowIcon;
            _buttons = new ObservableCollection<MessageBoxButtonDefinition>();
        }

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                this.Notify();
            }
        }

        public string Content
        {
            get => _content;
            set
            {
                _content = value;
                this.Notify();
            }
        }

        public MessageBoxLayout Layout
        {
            get => _layout;
            set
            {
                _layout = value;
                this.Notify();
                this.Notify(nameof(this.ShowInputBox));
            }
        }

        public string? Value
        {
            get => _value;
            set
            {
                _value = value;
                this.Notify();
            }
        }

        public Icon Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                this.Notify();
            }
        }

        public Bitmap WindowIcon
        {
            get => _windowIcon;
            set
            {
                _windowIcon = value;
                this.Notify();
            }
        }

        public ObservableCollection<MessageBoxButtonDefinition> Buttons
        {
            get => _buttons;
            set
            {
                _buttons = value;
                this.Notify();
            }
        }

        public bool ShowInputBox => _layout == MessageBoxLayout.Input;
        
    }
}