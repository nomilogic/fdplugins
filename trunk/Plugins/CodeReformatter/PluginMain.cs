using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections;
using PluginCore.Localization;
using PluginCore.Utilities;
using PluginCore.Managers;
using PluginCore.Helpers;
using PluginCore;
using ASCompletion.Context;
using ASCompletion.Completion;
using ScintillaNet;
using System.Diagnostics;

using CodeReformatter.Generators;
using CodeReformatter.Resources;
using System.Windows.Forms;
using PluginCore.Controls;
using Antlr.Runtime;


namespace CodeReformatter
{
	public class PluginMain : IPlugin
	{
        private String pluginName = "CodeReformatter";
        private String pluginGuid = "b1820ee6-00f7-4bca-bd27-9822b94f77c5";
        private String pluginHelp = "www.sephiroth.it";
        private String pluginDesc = "Code Reformatter for FlashDevelop 3";
        private String pluginAuth = "Alessandro Crugnola";
        private String settingFilename;
        private Settings settingObject;
        private ToolStripMenuItem ReformatMenuItem;

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
		}
		
		#endregion

        #region Custom Methods
       
        /// <summary>
        /// Initializes important variables
        /// </summary>
        public void InitBasics()
        {
            String dataPath = Path.Combine(PathHelper.DataDir, "CodeReformatter");
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");
        }

        /// <summary>
        /// Adds the required event handlers
        /// </summary> 
        public void AddEventHandlers()
        {
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
            this.pluginDesc = LocaleHelper.GetString("Info.Description");
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


        /// <summary>
        /// Add the menu entry
        /// </summary>
        private void CreateMenuItem()
        {
            ReformatMenuItem = new ToolStripMenuItem(LocaleHelper.GetString("Menu.Label"), null, new EventHandler(this.Reformat));
            ToolStripMenuItem editMenu = (ToolStripMenuItem)PluginBase.MainForm.FindMenuItem("EditMenu");
            editMenu.DropDownItems.Add(new ToolStripSeparator());
            editMenu.DropDownItems.Add(ReformatMenuItem);
            editMenu.DropDownOpening += new EventHandler(editMenu_DropDownOpening);
        }

        void editMenu_DropDownOpening(object sender, EventArgs e)
        {
            IASContext context   = ASContext.Context.CurrentModel.Context;

            if (context != null && (context.GetType().ToString().Equals("AS2Context.Context")))
            {
                ReformatMenuItem.Enabled = true;
            }
            else
            {
                ReformatMenuItem.Enabled = false;
            }
        }

        /// <summary>
        /// Reformat AS2/AS3 Code
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Reformat(Object sender, EventArgs e)
        {
            ITabbedDocument doc = MainForm.CurrentDocument;
            IGenerator generator = null;
            GeneratorReturnScope result = null;
            CodeReformatter.Generators.ReformatOptions options = new CodeReformatter.Generators.ReformatOptions( this.settingObject );

            if (doc.IsEditable)
            {
                if (ASContext.Context.CurrentModel.Context.GetType().ToString().Equals("AS2Context.Context"))
                {
                    generator = new AS2Generator(options);
                }

                if (generator != null)
                {
                    MainForm.CallCommand("PluginCommand", "ResultsPanel.ClearResults");
                    String source = doc.SciControl.Text;
                    generator.NewLine = ASComplete.GetNewLineMarker(doc.SciControl.EOLMode);
                    
                    if (MainForm.Settings.UseTabs)
                    {
                        generator.TabString = "\t";
                    }
                    else
                    {
                        String tab = " ";
                        for (int i = 0; i < doc.SciControl.TabWidth; i++)
                        {
                            tab += " ";
                        }
                        generator.TabString = tab;
                    }

                    try
                    {
                        result = generator.Parse(source);
                    } catch (RecognitionException error)
                    {
                        MessageBar.ShowWarning(error.Line + ": " + error.Message);
                        TraceItem item = new TraceItem(doc.FileName + ":" + 
                                                        error.Line + ": characters " + error.CharPositionInLine + "-" + 
                                                        (error.CharPositionInLine + ((CommonToken)error.Token).Text.Length) + " : " + 
                                                        error.Message.TrimStart() + ". Unexpected " + 
                                                        generator.Parser.TokenNames.GetValue(error.UnexpectedType) + 
                                                        (error is MismatchedTokenException ? ", expecting: " + generator.Parser.TokenNames.GetValue(((MismatchedTokenException)error).expecting) : ""),
                                                        -3);
                        TraceManager.Add(item);
                        TraceManager.Add(error.StackTrace);
                        MainForm.CallCommand("PluginCommand", "ResultsPanel.ShowResults");
                        return;
                    } catch (Exception error)
                    {
                        TraceManager.Add(error.StackTrace);
                        MessageBar.ShowWarning(error.Message);
                        MainForm.CallCommand("PluginCommand", "ResultsPanel.ShowResults");
                        return;
                    }

                    doc.SciControl.Text = result.Buffer.ToString();

                }
            }
        }


		#endregion

	}
	
}
