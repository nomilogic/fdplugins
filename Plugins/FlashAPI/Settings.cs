using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;

using PluginCore.Localization;

namespace FlashAPI
{

    public delegate void HelpFolderChangedEvent();

    [Serializable]
    public class Settings
    {
        public event HelpFolderChangedEvent OnHelpFolderChanged;
        protected string[] helpFolder = null;

        [DisplayName("Help Files")]
        [Category("Common"), LocalizedDescription("FlashAPI.Description.HelpFolder")]
        public string[] UserClasspath
        {
            get { return helpFolder; }
            set
            {
                helpFolder = value;
                FireChanged();
            }
        }

        [Browsable(false)]
        private void FireChanged()
        {
            if (OnHelpFolderChanged != null) OnHelpFolderChanged();
        }
    }

}
