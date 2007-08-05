using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;

namespace TaskListPanel
{

    public delegate void GroupsChangedEvent();
    public delegate void ExtensionChangedEvent();
    public delegate void ImagesIndexChangedEvent();

    [Serializable]
    public class Settings
    {

        public event GroupsChangedEvent OnGroupsChanged;
        public event ImagesIndexChangedEvent OnImagesIndexChanged;
        public event ExtensionChangedEvent OnExtensionChanged;

        private string[] extensions = new string[]{ ".as", ".txt" };
        private string[] groups     = new string[]{ "TODO", "BUG" };
        private int[] images        = new int[] { 229, 197 };

        /// <summary> 
        /// file extension to listen for
        /// </summary>
        /// 
        [Description("Files extensions."), DefaultValue(new string[] {".as",".txt"})]
        public string[] FileExtenions
        {
            get { return this.extensions; }
            set { 
                this.extensions = value;
                if (OnExtensionChanged != null) OnExtensionChanged();
            }
        }

        /// <summary> 
        /// Grouped results
        /// </summary>
        [Description("Groups."), DefaultValue(new string[] { "TODO", "FIXME", "BUG" })]
        public string[] Groups
        {
            get { return this.groups; }
            set { 
                this.groups = value;
                if (OnGroupsChanged != null) OnGroupsChanged();
            }
        }

        /// <summary> 
        /// Images results
        /// </summary>
        [Description("Images index"), DefaultValue(new object[] { 229, 197 })]
        public int[] ImagesIndex
        {
            get { return this.images; }
            set
            {
                this.images = value;
                if (OnImagesIndexChanged != null) OnImagesIndexChanged();
            }
        }
    }

}
