using System.ComponentModel;
using System.Runtime.CompilerServices;
using HurlStudio.Model.Enums;

namespace HurlStudio.UI.MessageBox.Model
{
    public class MessageBoxButtonDefinition : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void Notify([CallerMemberName] string propertyName = "") => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        
        private Icon _icon;
        private string _text;
        private object _returnValue;
        private bool _isDefault;
        private bool _isCancel;

        public MessageBoxButtonDefinition(Icon icon, string text, object returnValue, bool isDefault, bool isCancel)
        {
            _icon = icon;
            _text = text;
            _returnValue = returnValue;
            _isDefault = isDefault;
            _isCancel = isCancel;
        }

        public MessageBoxButtonDefinition(MessageBoxResult messageBoxResult, object returnValue, bool isDefault,
            bool isCancel) : this(messageBoxResult.GetIcon(), messageBoxResult.GetLocalizedString(), returnValue,
            isDefault, isCancel)
        {
            
        }
        
        public MessageBoxButtonDefinition(MessageBoxResult messageBoxResult, bool isDefault,
            bool isCancel) : this(messageBoxResult.GetIcon(), messageBoxResult.GetLocalizedString(), messageBoxResult,
            isDefault, isCancel)
        {
            
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

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                this.Notify();
            }
        }

        public object ReturnValue
        {
            get => _returnValue;
            set
            {
                _returnValue = value;
                this.Notify();
            }
        }

        public bool IsDefault
        {
            get => _isDefault;
            set
            {
                _isDefault = value;
                this.Notify();
            }
        }

        public bool IsCancel
        {
            get => _isCancel;
            set
            {
                _isCancel = value;
                this.Notify();
            }
        }
    }
}