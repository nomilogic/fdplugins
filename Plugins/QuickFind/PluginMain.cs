using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using QuickFindPlugin.Resources;
using PluginCore.Localization;
using PluginCore.Utilities;
using PluginCore.Managers;
using PluginCore.Helpers;
using PluginCore;
using System.Reflection;
using ScintillaNet;
using PluginCore.FRService;
using System.Collections.Generic;
using System.Collections;
using System.Media;
using System.Diagnostics;

namespace QuickFindPlugin
{
	public class PluginMain : IPlugin
	{
        private String pluginName = "QuickFindPlugin";
        private String pluginGuid = "6ac00449-3a46-473d-87fe-ea417a9a77ca";
        private String pluginHelp = "www.sephiroth.it";
        private String pluginDesc = "Quick find plugin for the new FlashDevelop 3.";
        private String pluginAuth = "Alessandro Crugnola";
        private String settingFilename;
        private Settings settingObject;
        private Timer highlightTimer;

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
            this.CreateElements();
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
            String dataPath = Path.Combine(PathHelper.DataDir, "QuickFindPlugin");
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");

            highlightTimer = new Timer();
            highlightTimer.Interval = 500;
            highlightTimer.Enabled  = false;
            highlightTimer.Tick += delegate { this.highlightTimer_Tick(); };
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


        private void highlightTimer_Tick()
        {
            this.highlightTimer.Stop();

            if (this.highlightTimer.Tag != null)
            {
                if (this.highlightTimer.Tag is Hashtable)
                {
                    try
                    {
                        this.AddHighlights(((Hashtable)this.highlightTimer.Tag)["Sci"] as ScintillaControl, ((Hashtable)this.highlightTimer.Tag)["Matches"] as List<SearchMatch>);
                    }
                    catch (System.Exception error)
                    {
                        System.Diagnostics.Debug.WriteLine(error);
                    }
                }
            }
        }

        /// <summary>
        /// Highlight all results
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hilightAllButton_Click( Object sender, EventArgs e )
        {
            if(!MainForm.CurrentDocument.IsEditable) return;

            if (this.hilightAllButton.Checked)
            {
                if (this.textBox1 != null)
                {
                    String text = this.textBox1.Text;
                    if (text == "") return;

                    ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;
                    List<SearchMatch> matches = this.GetResults(sci, text);
                    if (matches != null && matches.Count != 0)
                    {
                        RemoveHighlights();

                        if (this.highlightTimer.Enabled)
                            this.highlightTimer.Stop();

                        if (this.hilightAllButton.Checked)
                            this.AddHighlights(sci, matches);
                    }
                }
            }
            else
            {
                this.RemoveHighlights();
            }
        }


        private void AddHighlights( ScintillaControl sci,  List<SearchMatch> matches )
        {
            foreach (SearchMatch match in matches)
            {
                Int32 start = sci.MBSafePosition(match.Index); // wchar to byte position
                Int32 end   = start + sci.MBSafeTextLength(match.Value); // wchar to byte text length
                Int32 line = sci.LineFromPosition(start);
                Int32 position = start;
                Int32 es = sci.EndStyled;
                Int32 mask = 1 << sci.StyleBits;
                sci.SetIndicStyle(0, (Int32)ScintillaNet.Enums.IndicatorStyle.Max);
                sci.SetIndicFore(0, settingObject.HighlightColor);
                sci.StartStyling(position, mask);
                sci.SetStyling(end - start, mask);
                sci.StartStyling(es, mask - 1);
            }
        }

        private void RemoveHighlights()
        {
            ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;
            Int32 es = sci.EndStyled;
            Int32 mask = (1 << sci.StyleBits);
            sci.StartStyling(0, mask);
            sci.SetStyling(sci.TextLength, 0);
            sci.StartStyling(es, mask - 1);
        }

        private void findNextButton_Click( Object sender, EventArgs e )
        {
            if (this.textBox1.Text != "")
                this.FindNext(this.textBox1.Text, false);
            else
                SystemSounds.Beep.Play();
        }

        private void findPrevButton_Click( Object sender, EventArgs e )
        {
            if (this.textBox1.Text != "")
                this.FindPrev(this.textBox1.Text);
            else
                SystemSounds.Beep.Play();
        }

        /// <summary>
        /// Text into the main search textbox has changed, then process with
        /// find next occurrence of the word
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_TextChanged( Object sender, EventArgs e )
        {
            String text = this.textBox1.Text;
            this.FindNext(text, this.hilightAllButton.Checked);
        }

        /// <summary>
        /// Escape key has been pressed into the toolstriptextbox
        /// then assign the current focus to the current scintilla control
        /// </summary>
        private void textBox1_OnKeyEscape()
        {
            if(MainForm.CurrentDocument.IsEditable)
                MainForm.CurrentDocument.SciControl.Focus();

            // Should the quick find panel hide itself?
            //this.HidePluginPanel();
        }

        /// <summary>
        /// Pressed key on the main textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_KeyPress( Object sender, KeyPressEventArgs e )
        {
            if (e.KeyChar == (char)Keys.Return && this.textBox1.Text != "")
            {
                e.Handled = true;
                this.FindNext(this.textBox1.Text, false);
            }
        }

        /// <summary>
        /// Find next match
        /// </summary>
        /// <param name="text"></param>
        public void FindNext( String text, bool refreshHighlights )
        {
            this.textBox1.BackColor = SystemColors.Window;

            if (text == "" || !MainForm.CurrentDocument.IsEditable) return;

            ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;
            List<SearchMatch> matches = this.GetResults(sci, text);
            if (matches != null && matches.Count != 0)
            {
                SearchMatch match = GetNextDocumentMatch(sci, matches, true);
                if (match != null)
                {
                    SelectMatch(sci, match);
                }

                if(refreshHighlights)
                {
                    RemoveHighlights();

                    if (this.highlightTimer.Enabled)
                        this.highlightTimer.Stop();

                    Hashtable table = new Hashtable();
                    table["Sci"]     = sci;
                    table["Matches"] = matches;

                    this.highlightTimer.Tag = table;
                    this.highlightTimer.Start();
                }
            }
            else
            {
                this.textBox1.BackColor = Color.Salmon;
                System.Media.SystemSounds.Beep.Play();
            }
        }

        /// <summary>
        /// Find previous match
        /// </summary>
        /// <param name="text"></param>
        public void FindPrev( String text )
        {
            this.textBox1.BackColor = SystemColors.Window;

            if (text == "" || !MainForm.CurrentDocument.IsEditable) return;

            ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;
            List<SearchMatch> matches = this.GetResults(sci, text);
            if (matches != null && matches.Count != 0)
            {
                SearchMatch match = GetNextDocumentMatch(sci, matches, false);
                if (match != null)
                {
                    SelectMatch(sci, match);
                }
            }
            else
            {
                this.textBox1.BackColor = Color.Salmon;
                System.Media.SystemSounds.Beep.Play();
            }
        }

        /// <summary>
        /// Gets search results for a document
        /// </summary>
        private List<SearchMatch> GetResults( ScintillaControl sci, String text )
        {
            String pattern = text;
            FRSearch search = new FRSearch(pattern);
            search.Filter = SearchFilter.None;
            search.NoCase = true;
            return search.Matches(sci.Text);
        }

        /// <summary>
        /// Gets an index of the search match
        /// </summary>
        public static Int32 GetMatchIndex( SearchMatch match, List<SearchMatch> matches )
        {
            for (Int32 i = 0; i < matches.Count; i++)
            {
                if (match == matches[i]) return i + 1;
            }
            return -1;
        }

        /// <summary>
        /// Selects a search match
        /// </summary>
        public static void SelectMatch( ScintillaControl sci, SearchMatch match )
        {
            Int32 start = sci.MBSafePosition(match.Index); // wchar to byte position
            Int32 end = start + sci.MBSafeTextLength(match.Value); // wchar to byte text length
            Int32 line = sci.LineFromPosition(start);
            sci.EnsureVisible(line);
            sci.SetSel(start, end);
        }

        public static SearchMatch GetNextDocumentMatch( ScintillaControl sci, List<SearchMatch> matches, Boolean forward )
        {
            SearchMatch nearestMatch = matches[0];
            Int32 currentPosition = sci.MBSafeCharPosition(sci.CurrentPos); // byte to wchar position
            for (Int32 i = 0; i < matches.Count; i++)
            {
                if (forward)
                {
                    if (currentPosition > matches[matches.Count - 1].Index)
                    {
                        return matches[0];
                    }
                    if (matches[i].Index >= currentPosition)
                    {
                        return matches[i];
                    }
                }
                else
                {
                    if (sci.SelText.Length > 0 && currentPosition <= matches[0].Index + matches[0].Value.Length)
                    {
                        return matches[matches.Count - 1];
                    }
                    if (currentPosition < matches[0].Index + matches[0].Value.Length)
                    {
                        return matches[matches.Count - 1];
                    }
                    if (sci.SelText.Length == 0 && currentPosition == matches[i].Index + matches[i].Value.Length)
                    {
                        return matches[i];
                    }
                    if (matches[i].Index > nearestMatch.Index && matches[i].Index + matches[i].Value.Length < currentPosition)
                    {
                        nearestMatch = matches[i];
                    }
                }
            }
            return nearestMatch;
        }


        /// <summary>
        /// Remove the status strip elements
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClosePanel( Object sender, EventArgs e )
        {
            /*
            if (this.closeButton != null)
            {
                MainForm.StatusStrip.Items.Remove(this.findLabel);
                MainForm.StatusStrip.Items.Remove(this.closeButton);
                MainForm.StatusStrip.Items.Remove(this.textBox1);
                MainForm.StatusStrip.Items.Remove(this.separator1);
                MainForm.StatusStrip.Items.Remove(this.findNextButton);
                MainForm.StatusStrip.Items.Remove(this.findPrevButton);
                MainForm.StatusStrip.Items.Remove(this.hilightAllButton);
                MainForm.StatusStrip.Items.Remove(this.separator2);
            }
            */

            this.HidePluginPanel();
        }

        /// <summary>
        /// Hide the panel and remove the controls from the main status strip container
        /// </summary>
        public void HidePluginPanel()
        {
            if (MainForm.Controls.Contains(this.strip))
                MainForm.Controls.Remove(this.strip);
        }

        /// <summary>
        /// Creates a plugin panel for the plugin
        /// </summary>
        public void ShowPluginPanel( object sender, EventArgs e)
        {
            /*
            if (this.closeButton != null)
            {
                if (MainForm.StatusStrip.Items.Count > 0)
                {
                    if (MainForm.StatusStrip.Items.IndexOf(this.closeButton) == -1)
                    {
                        MainForm.StatusStrip.Items.Insert(0, this.closeButton);
                        MainForm.StatusStrip.Items.Insert(1, this.findLabel);
                        MainForm.StatusStrip.Items.Insert(2, this.textBox1);
                        MainForm.StatusStrip.Items.Insert(3, this.separator1);
                        MainForm.StatusStrip.Items.Insert(4, this.findNextButton);
                        MainForm.StatusStrip.Items.Insert(5, this.findPrevButton);
                        MainForm.StatusStrip.Items.Insert(6, this.hilightAllButton);
                        MainForm.StatusStrip.Items.Insert(7, this.separator2);
                    }
                }
                this.textBox1.Focus();
                this.textBox1.SelectAll();
            }*/

            if (!MainForm.Controls.Contains(this.strip))
            {
                int index = MainForm.Controls.GetChildIndex(MainForm.StatusStrip, false);
                MainForm.Controls.Add(this.strip);

                if(index > -1)
                    MainForm.Controls.SetChildIndex(this.strip, index);
            }
            this.textBox1.Focus();
            this.textBox1.SelectAll();
        }

        /// <summary>
        /// Creates a menu item for the plugin and adds a ignored key
        /// </summary>
        public void CreateMenuItem()
        {
            ToolStripMenuItem sMenu = (ToolStripMenuItem)PluginBase.MainForm.FindMenuItem("SearchMenu");
            sMenu.DropDownItems.Add(new ToolStripSeparator());

            // try with the user defined shortcut combination
            try
            {
                sMenu.DropDownItems.Add(new ToolStripMenuItem(LocaleHelper.GetString("Menu.Label"), null, new EventHandler(this.ShowPluginPanel), this.settingObject.Shortcut));
                PluginBase.MainForm.IgnoredKeys.Add(this.settingObject.Shortcut);
            }
            catch
            {
                sMenu.DropDownItems.Add(new ToolStripMenuItem(LocaleHelper.GetString("Menu.Label"), null, new EventHandler(this.ShowPluginPanel), QuickFindPlugin.Settings.DEFAULT_SHORTCUT));
                PluginBase.MainForm.IgnoredKeys.Add( QuickFindPlugin.Settings.DEFAULT_SHORTCUT );
                this.settingObject.Shortcut = QuickFindPlugin.Settings.DEFAULT_SHORTCUT;
            }
        }

        private void CreateElements()
        {
            this.findLabel = new ToolStripLabel(LocaleHelper.GetString("Find.Label"));

            this.closeButton = new ToolStripButton();
            this.closeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.closeButton.Image = GetResource("Icons.close_up.png");
            this.closeButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.closeButton.Name = "toolStripButton1";
            this.closeButton.Size = new System.Drawing.Size(23, -1);
            this.closeButton.Text = "Close";
            this.closeButton.Click += new EventHandler(ClosePanel);

            this.textBox1 = new myTextBox();
            this.textBox1.Size = new Size(200, -1);
            this.textBox1.Font = new Font("Tahoma", 8.25F, FontStyle.Regular);
            this.textBox1.TextChanged += new EventHandler(textBox1_TextChanged);
            this.textBox1.OnKeyEscape += new KeyEscapeEvent(textBox1_OnKeyEscape);

            this.findNextButton = new ToolStripButton(LocaleHelper.GetString("FindNext.Label"));
            this.findNextButton.Name = "findNextButton";
            this.findNextButton.Click += new EventHandler(findNextButton_Click);
            this.findNextButton.Image = MainForm.FindImage("22");
            this.findNextButton.Margin = new Padding(5, 1, 5, 1);
            
            this.findPrevButton = new ToolStripButton(LocaleHelper.GetString("FindPrev.Label"));
            this.findPrevButton.Name = "findPrevButton";
            this.findPrevButton.Click += new EventHandler(findPrevButton_Click);
            this.findPrevButton.Image = MainForm.FindImage("8");
            this.findPrevButton.Margin = new Padding(5, 1, 5, 1);

            this.hilightAllButton = new ToolStripButton(LocaleHelper.GetString("HighlightAll.Label"));
            this.hilightAllButton.Name = "hilightAllButton";
            this.hilightAllButton.Click += new EventHandler(hilightAllButton_Click);
            this.hilightAllButton.CheckOnClick = true;
            this.hilightAllButton.Image = MainForm.FindImage("368");
            this.hilightAllButton.Margin = new Padding(10, 1, 5, 1);

            this.textBox1.KeyPress += new KeyPressEventHandler(textBox1_KeyPress);

            this.strip = new StatusStrip();
            this.strip.Stretch = true;
            this.strip.SizingGrip = false;
            this.strip.RenderMode = ToolStripRenderMode.Professional;
            //this.strip.Visible = false;

            this.strip.Items.AddRange( new ToolStripItem[]{
                this.closeButton,
                this.findLabel,
                this.textBox1,
                new ToolStripSeparator(),
                this.findNextButton,
                this.findPrevButton,
                this.hilightAllButton,
            });
        }

        public static Image GetResource( string resourceID )
        {
            resourceID = "QuickFindPlugin." + resourceID;
            Assembly assembly = Assembly.GetExecutingAssembly();
            Image image = new Bitmap(assembly.GetManifestResourceStream(resourceID));
            return image;
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

        private ToolStripButton closeButton;
        private myTextBox textBox1;
        private ToolStripButton findNextButton;
        private ToolStripButton findPrevButton;
        private ToolStripButton hilightAllButton;
        private ToolStripLabel findLabel;
        private StatusStrip strip;


		#endregion

	}


    public delegate void KeyEscapeEvent();

    public class myTextBox : ToolStripTextBox
    {
        public event KeyEscapeEvent OnKeyEscape;

        public myTextBox()
            : base()
        {
        }

        protected override bool ProcessCmdKey( ref Message m, Keys keyData )
        {
            if (keyData == Keys.Escape)
            {
                OnPressEscapeKey();
            }
            return false;
        }

        protected void OnPressEscapeKey()
        {
            if (OnKeyEscape != null)
                OnKeyEscape();
        }
    }
	
}
