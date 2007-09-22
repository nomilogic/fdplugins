using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;

namespace SharedObjectReader
{
    [Serializable]
    public class Settings
    {
        private Keys menuShortcut = Keys.Control | Keys.F1;
        
        /// <summary> 
        /// Get and sets the menu Shortcut
        /// </summary>
        [Description("shortcut menu setting."), DefaultValue(Keys.Control | Keys.F1)]
        public Keys MenuShortcut
        {
            get { return this.menuShortcut; }
            set { this.menuShortcut = value; }
        }

    }

}
