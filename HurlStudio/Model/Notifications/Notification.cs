using HurlStudio.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Model.Notifications
{
    public class Notification : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void Notify([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        private NotificationType _type;
        private string _title;
        private string _text;

        public Notification(NotificationType type, string title, string text)
        {
            _type = type;
            _title = title;
            _text = text;
        }

        public NotificationType Type
        {
            get => _type;
            set
            {
                _type = value;
                Notify();
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                Notify();
            }
        }

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                Notify();
            }
        }

        public string GetLogMessage()
        {
            return $"{_title}: {_text}";
        }

        public override string ToString()
        {
            return this.GetLogMessage();
        }
    }
}
