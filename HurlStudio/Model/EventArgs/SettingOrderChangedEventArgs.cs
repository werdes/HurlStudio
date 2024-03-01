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
        private readonly HurlSettingContainer _hurlSetting;
        private readonly MoveDirection _moveDirection;

        public SettingOrderChangedEventArgs(HurlSettingContainer setting, MoveDirection moveDirection)
        {
            _hurlSetting = setting;
            _moveDirection = moveDirection;
        }

        public HurlSettingContainer Setting { get => _hurlSetting; }
        public MoveDirection MoveDirection { get => _moveDirection; }
    }
}
