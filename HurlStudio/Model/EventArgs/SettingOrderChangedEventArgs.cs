using HurlStudio.Model.Enums;
using HurlStudio.Model.HurlSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Model.EventArgs
{
    public class SettingOrderChangedEventArgs : System.EventArgs
    {
        private readonly HurlSettingContainer _hurlSettingContainer;
        private readonly MoveDirection _moveDirection;

        public SettingOrderChangedEventArgs(HurlSettingContainer settingContainer, MoveDirection moveDirection)
        {
            _hurlSettingContainer = settingContainer;
            _moveDirection = moveDirection;
        }

        public HurlSettingContainer SettingContainer { get => _hurlSettingContainer; }
        public MoveDirection MoveDirection { get => _moveDirection; }
    }
}
