using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using WeifenLuo.WinFormsUI.Docking;
using TaskListPanel.Resources;
using PluginCore.Localization;
using PluginCore.Utilities;
using PluginCore.Managers;
using PluginCore.Helpers;
using PluginCore;

using ProjectManager;
using System.Diagnostics;
using ProjectManager.Projects;

namespace TaskListPanel
{
	public class PluginMain : IPlugin
	{
        private String pluginName = "TaskListPanel";
        private String pluginGuid = "40feac2b-a68a-498e-ad78-52a8268efa45";
        private String pluginHelp = "www.sephiroth.it";
        private String pluginDesc = "TaskListPanel for the new FlashDevelop 3.";
        private String pluginAuth = "Alessandro Crugnola";
        private String settingFilename;
        private Settings settingObject;
        private DockContent pluginPanel;
        private PluginUI pluginUI;
        private Image pluginImage;

        public static IMainForm MainForm { get { return PluginBase.MainForm; } }

	    #region Required Properties

        /// <summary>
        /// Name of the plugin
        /// </summary> 
        public String Name
		{
			get { return this.pluginName; }
		}

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public String Guid
		{
			get { return this.pluginGuid; }
		}

        /// <summary>
        /// Author of the plugin
        /// </summary> 
        public String Author
		{
			get { return this.pluginAuth; }
		}

        /// <summary>
        /// Description of the plugin
        /// </summary> 
        public String Description
		{
			get { return this.pluginDesc; }
		}

        /// <summary>
        /// Web address for help
        /// </summary> 
        public String Help
		{
			get { return this.pluginHelp; }
		}

        /// <summary>
        /// Object that contains the settings
        /// </summary>
        [Browsable(false)]
        public Object Settings
        {
            get { return this.settingObject; }
        }
		
		#endregion
		
		#region Required Methods
		
		/// <summary>
		/// Initializes the plugin
		/// </summary>
		public void Initialize()
		{
            this.InitBasics();
            this.LoadSettings();
            this.AddEventHandlers();
            this.InitLocalization();
            this.CreateMenuItem();
            this.CreatePluginPanel();
        }
		
		/// <summary>
		/// Disposes the plugin
		/// </summary>
		public void Dispose()
		{
            this.SaveSettings();

            this.pluginUI.Terminate();
		}
		
		/// <summary>
		/// Handles the incoming events
		/// </summary>
		public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority prority)
		{
            DataEvent devent;

            switch (e.Type)
            {
                case EventType.UIStarted:
                    EventManager.AddEventHandler(this, EventType.Command);
                    break;

                case EventType.Command:
                    devent = (DataEvent)e;
                    if (devent.Action == "ProjectManager.Project")
                    {
                        if (devent.Data is Project && ((Project)devent.Data) != null)
                            this.pluginUI.Project = devent.Data as Project;
                        else
                            this.pluginUI.Project = null;
                    }
                    break;
            }
		}
		
		#endregion

        #region Custom Methods
       
        /// <summary>
        /// Initializes important variables
        /// </summary>
        public void InitBasics()
        {
            String dataPath = Path.Combine(PathHelper.DataDir, "TaskListPanel");
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");
            this.pluginImage = PluginBase.MainForm.FindImage("75");
        }

        /// <summary>
        /// Adds the required event handlers
        /// </summary> 
        public void AddEventHandlers()
        {
            // PluginBase.MainForm.CurrentDocument.FileName
            EventManager.AddEventHandler(this, EventType.UIStarted);
        }

        /// <summary>
        /// Initializes the localization of the plugin
        /// </summary>
        public void InitLocalization()
        {
            LocaleVersion locale = PluginBase.MainForm.Settings.LocaleVersion;
            switch (locale)
            {
                default : 
                    LocaleHelper.Initialize(LocaleVersion.en_US);
                    break;
            }
        }

        /// <summary>
        /// Creates a plugin panel for the plugin
        /// </summary>
        public void CreatePluginPanel()
        {
            this.pluginUI = new PluginUI(this);
            this.pluginUI.Text = LocaleHelper.GetString("Title.PluginPanel");
            this.pluginPanel = PluginBase.MainForm.CreateDockablePanel(this.pluginUI, this.pluginGuid, this.pluginImage, DockState.DockRight);
        }

        /// <summary>
        /// Creates a menu item for the plugin and adds a ignored key
        /// </summary>
        public void CreateMenuItem()
        {
            ToolStripMenuItem viewMenu = (ToolStripMenuItem)PluginBase.MainForm.FindMenuItem("ViewMenu");
            viewMenu.DropDownItems.Add(new ToolStripMenuItem(LocaleHelper.GetString("Label.ViewMenuItem"), this.pluginImage, new EventHandler(this.OpenPanel), null));
        }

        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        public void LoadSettings()
        {
            this.settingObject = new Settings();
            if (!File.Exists(this.settingFilename)) this.SaveSettings();
            else
            {
                Object obj = ObjectSerializer.Deserialize(this.settingFilename, this.settingObject);
                this.settingObject = (Settings)obj;
                AddSettingsListeners();
            }
        }

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        public void SaveSettings()
        {
            RemoveSettingsListeners();
            ObjectSerializer.Serialize(this.settingFilename, this.settingObject);
            AddSettingsListeners();
        }

        /// <summary>
        /// Opens the plugin panel if closed
        /// </summary>
        public void OpenPanel(Object sender, System.EventArgs e)
        {
            this.pluginPanel.Show();
        }


        void AddSettingsListeners()
        {
            if (this.settingObject != null)
            {
                this.settingObject.OnExtensionChanged   += new ExtensionChangedEvent(settingObject_OnSettingsChanged);
                this.settingObject.OnGroupsChanged      += new GroupsChangedEvent(settingObject_OnSettingsChanged);
                this.settingObject.OnImagesIndexChanged += new ImagesIndexChangedEvent(settingObject_OnSettingsChanged);
            }
        }

        void RemoveSettingsListeners()
        {
            if (this.settingObject != null)
            {
                this.settingObject.OnExtensionChanged   -= new ExtensionChangedEvent(settingObject_OnSettingsChanged);
                this.settingObject.OnGroupsChanged      -= new GroupsChangedEvent(settingObject_OnSettingsChanged);
                this.settingObject.OnImagesIndexChanged -= new ImagesIndexChangedEvent(settingObject_OnSettingsChanged);
            }
        }

        private void settingObject_OnSettingsChanged()
        {
            this.pluginUI.UpdateSettings();
        }

		#endregion

	}
	
}
