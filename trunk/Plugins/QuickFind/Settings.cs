using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.Drawing;

namespace QuickFindPlugin
{
    [Serializable]
    public class Settings
    {
        public static Keys DEFAULT_SHORTCUT = Keys.Control | Keys.Alt | Keys.F;

        private Keys sampleShortcut = DEFAULT_SHORTCUT;
        private int highlightColor = 0xff0000;
        

        /// <summary> 
        /// Get and sets the sampleShortcut
        /// </summary>
        [Description("Shortcut setting"), DefaultValue( Keys.Control | Keys.Alt | Keys.F )]
        public Keys Shortcut
        {
            get { return this.sampleShortcut; }
            set { this.sampleShortcut = value; }
        }


        /// <summary> 
        /// Get and sets the sampleShortcut
        /// </summary>
        [Description("Color highlight"), DefaultValue(0x00ccff)]
        public int HighlightColor
        {
            get { return this.highlightColor; }
            set { this.highlightColor = value; }
        }

    }

}
