using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Model.EventArgs
{
    public class ControlSelectionChangedEventArgs : System.EventArgs
    {
		private bool _controlSelected;

        public ControlSelectionChangedEventArgs(bool controlSelected)
        {
            _controlSelected = controlSelected;
        }

        public bool ControlSelected
		{
			get => _controlSelected; 
			set => _controlSelected = value;
		}
	}
}
