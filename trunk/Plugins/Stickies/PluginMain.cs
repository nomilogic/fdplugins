// The Sticky note engine derives from http://code.google.com/p/stickies-windows/source
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may
// not use this file except in compliance with the License. You may obtain
// a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations
// under the License.



#region Imports

using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using WeifenLuo.WinFormsUI.Docking;
using Stickies.Resources;
using PluginCore.Localization;
using PluginCore.Utilities;
using PluginCore.Managers;
using PluginCore.Helpers;
using PluginCore;
using Stickies.Notes;
using System.Collections.Generic;

#endregion

namespace Stickies
{
	public class PluginMain : IPlugin
	{
        private String pluginName = "Stickies";
        private String pluginGuid = "5afc7ace-f3d6-4152-a6ef-fed8fb12413e";
        private String pluginHelp = "www.sephiroth.it/";
        private String pluginDesc = "Stickies for the new FlashDevelop 3.";
        private String pluginAuth = "Alessandro Crugnola";
        private String settingFilename;
        private Settings settingObject;
        private Image pluginImage;

        private Preferences preferences;
        private List<NoteForm> noteForms;
        private ToolStripMenuItem deleteAllNotesMenuItem;
        private ToolStripMenuItem stickiesMenu;

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
        }
		
		/// <summary>
		/// Disposes the plugin
		/// </summary>
		public void Dispose()
		{
            this.SaveSettings();
		}
		
		/// <summary>
		/// Handles the incoming events
		/// </summary>
		public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority prority)
		{
            switch (e.Type)
            {
                case EventType.UIStarted:
                    this.stickiesMenu.Enabled = true;
                    LoadStickies();
                    break;
            }
		}
		
		#endregion

        #region Custom Methods

        /// <summary>
        /// Stickies storage directory
        /// </summary>
        public String StickiesDirectory
        {
            get { return Path.Combine(PathHelper.DataDir, this.pluginName); }
        }


        /// <summary>
        /// Initializes important variables
        /// </summary>
        public void InitBasics()
        {
            String dataPath = Path.Combine( PathHelper.DataDir, this.pluginName );
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");
            this.pluginImage = PluginBase.MainForm.FindImage("273");

            Stickies.Notes.Settings.defaultDirectory = this.StickiesDirectory;

            this.noteForms = new List<NoteForm>();
            this.preferences = new Preferences();

        }


        /// <summary>
        /// Load Stickies on start-up
        /// </summary>
        private void LoadStickies()
        {
            List<Note> notes = LoadNotes();
            foreach (Note note in notes)
            {
                ShowNote(new NoteForm(this, note, false));
            }
            UpdateMenus();
        }

        /// <summary>
        /// Loads all the notes from disk, returning an array of note structs.
        /// </summary>
        private List<Note> LoadNotes()
        {
            List<Note> notes = new List<Note>();

            //if( !Directory.Exists( Stickies.Notes.Settings.SettingsDirectory() ) )
            //{
            //    Directory.CreateDirectory( Stickies.Notes.Settings.SettingsDirectory() );
            //}

            try
            {
                foreach (string path in Directory.GetFiles( this.StickiesDirectory , "*" + Note.PathSuffix))
                {
                    try
                    {
                        notes.Add(Note.Load(path));
                    }
                    catch (Exception e)
                    {
                        PluginCore.Managers.ErrorManager.ShowError(e);
                    }
                }
            }
            catch (Exception e)
            {
                PluginCore.Managers.ErrorManager.ShowError(e);
            }
            return notes;
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
                    // Plugins should default to English...
                    LocaleHelper.Initialize(LocaleVersion.en_US);
                    break;
            }
        }

        /// <summary>
        /// Creates a menu item for the plugin and adds a ignored key
        /// </summary>
        public void CreateMenuItem()
        {
            ToolStripMenuItem viewMenu = (ToolStripMenuItem)PluginBase.MainForm.FindMenuItem("ViewMenu");
            stickiesMenu = new ToolStripMenuItem(LocaleHelper.GetString("Label.ViewMenuItem"), this.pluginImage);
            deleteAllNotesMenuItem = new ToolStripMenuItem(LocaleHelper.GetString("Label.RemoveAll"), null, new EventHandler(this.deleteAllNotesMenuItemClick));
            deleteAllNotesMenuItem.Enabled = false;

            stickiesMenu.DropDownItems.Add(new ToolStripMenuItem(LocaleHelper.GetString("Label.AddNewNote"), null, new EventHandler(this.CreateNote)));
            stickiesMenu.DropDownItems.Add(deleteAllNotesMenuItem);
            viewMenu.DropDownItems.Add(stickiesMenu);
            stickiesMenu.Enabled = false;
        }

        /// <summary>
        /// Deletes all the visible sticky notes.
        /// </summary>
        private void DeleteAllNotes()
        {
            NoteForm[] notes = new NoteForm[noteForms.Count];
            noteForms.CopyTo(notes);
            foreach (NoteForm noteForm in notes)
            {
                noteForm.Delete(false);
            }
        }

        /// <summary>
        /// Create a new empty sticky note
        /// </summary>
        private void CreateNote(Object sender, EventArgs e)
        {
            Note note = (Note)preferences.Note.Copy();
            note.Guid = System.Guid.NewGuid().ToString();
            note.Left = Control.MousePosition.X - note.Width / 2;
            note.Top = Control.MousePosition.Y - note.Height / 2;
            NoteForm noteForm = new NoteForm(this, note, true);
            ShowNote(noteForm);
            noteForm.Activate();
        }

        /// <summary>
        /// Registers and displays the given NoteForm.
        /// </summary>
        private void ShowNote(NoteForm noteForm)
        {
            noteForms.Add(noteForm);
            noteForm.HandleDestroyed += new EventHandler(NoteFormHandleDestroyed);
            noteForm.Show(PluginBase.MainForm);
            UpdateMenus();
        }

        /// <summary>
        /// Enables or disables menus based on whether the user has any visible
        /// sticky notes.
        /// </summary>
        private void UpdateMenus()
        {
            deleteAllNotesMenuItem.Enabled = noteForms.Count > 0;
        }

        /// <summary>
        /// Called when a NoteForm closes. We removed the NoteForm from our
        /// collection when it is closed.
        /// </summary>
        private void NoteFormHandleDestroyed(object sender, EventArgs e)
        {
            noteForms.Remove((NoteForm)sender);
            UpdateMenus();
        }

        /// <summary>
        /// Deletes all the visible notes if the user clicks Yes to a prompt.
        /// </summary>
        private void deleteAllNotesMenuItemClick(object sender, EventArgs e)
        {
            DeleteAllNotes();
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
            }
        }

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        public void SaveSettings()
        {
            ObjectSerializer.Serialize(this.settingFilename, this.settingObject);
        }

		#endregion

	}
	
}
