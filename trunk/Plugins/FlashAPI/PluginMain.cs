using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;
using System.Reflection;

using WeifenLuo.WinFormsUI.Docking;

using PluginCore.Localization;
using PluginCore.Utilities;
using PluginCore.Managers;
using PluginCore.Helpers;
using PluginCore;

using FlashAPI.Resources;

namespace FlashAPI
{
	public class PluginMain : IPlugin
	{
        private String pluginName = "FlashAPI";
        private String pluginGuid = "1091dc85-1ac1-49c1-9880-f519f32d33da";
        private String pluginHelp = "www.sephiroth.it";
        private String pluginDesc = "FlashAPI panel for the new FlashDevelop 3.";
        private String pluginAuth = "Alessandro Crugnola";
        private String settingFilename;
        private Settings settingObject;
        private PluginUI pluginUI;
        private DockContent pluginPanel;
        private Image pluginImage;
        public static bool UIStarted = false;


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
            this.SaveSettings( false );
		}
		
		/// <summary>
		/// Handles the incoming events
		/// </summary>
		public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority prority)
		{
            if (e.Type == EventType.UIStarted)
            {
                UIStarted = true;

                if (pluginUI != null)
                    pluginUI.OnPluginLoaded();
            }
		}
		
		#endregion

        #region Custom Methods
       
        /// <summary>
        /// Initializes important variables
        /// </summary>
        public void InitBasics()
        {
            String dataPath = Path.Combine(PathHelper.DataDir, "FlashAPI");
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");

            this.pluginImage = GetResource("Icons.ActionFunction.png");
        }

        /// <summary>
        /// Adds the required event handlers
        /// </summary> 
        public void AddEventHandlers()
        {
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
        /// Loads the plugin settings
        /// </summary>
        public void LoadSettings()
        {
            this.settingObject = new Settings();
            if (!File.Exists(this.settingFilename)) this.SaveSettings( true );
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
        public void SaveSettings( bool restoreListener )
        {
            if (this.settingObject != null)
                RemoveSettingsListeners();

            ObjectSerializer.Serialize(this.settingFilename, this.settingObject);

            AddSettingsListeners();
        }


        /// <summary>
        /// Creates a plugin panel for the plugin
        /// </summary>
        public void CreatePluginPanel()
        {
            this.pluginUI = new PluginUI(this);
            this.pluginUI.Settings = this.settingObject;
            this.pluginUI.Text = LocaleHelper.GetString("Title.PluginPanel");
            this.pluginPanel = PluginBase.MainForm.CreateDockablePanel(this.pluginUI, this.pluginGuid, this.pluginImage, DockState.DockRight);
        }

        /// <summary>
        /// Opens the plugin panel if closed
        /// </summary>
        public void OpenPanel( Object sender, System.EventArgs e )
        {
            this.pluginPanel.Show();
        }

        public static Image GetResource( string resourceID )
        {
            resourceID = "FlashAPI." + resourceID;
            Assembly assembly = Assembly.GetExecutingAssembly();
            Image image = new Bitmap(assembly.GetManifestResourceStream(resourceID));
            return image;
        }

        public void CreateMenuItem()
        {
            ToolStripMenuItem viewMenu = (ToolStripMenuItem)MainForm.FindMenuItem("ViewMenu");
            if (viewMenu != null)
            {
                viewMenu.DropDownItems.Add(new ToolStripMenuItem(LocaleHelper.GetString("Title.PluginPanel"), this.pluginImage, new EventHandler(this.OpenPanel), null));
            }
        }

        void AddSettingsListeners()
        {
            this.settingObject.OnHelpFolderChanged += settingObject_OnHelpFolderChanged;
        }

        void RemoveSettingsListeners()
        {
            this.settingObject.OnHelpFolderChanged -= settingObject_OnHelpFolderChanged;
        }


        void settingObject_OnHelpFolderChanged()
        {
            this.pluginUI.Settings = this.settingObject;
        }

		#endregion

	}
	
}
