using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using WeifenLuo.WinFormsUI.Docking;
using RegExpPanel.Resources;
using PluginCore.Localization;
using PluginCore.Utilities;
using PluginCore.Managers;
using PluginCore.Helpers;
using PluginCore;
using ScintillaNet;
using System.Text;
using FlashDevelop;
using FlashDevelop.Managers;
using System.Diagnostics;

namespace RegExpPanel
{
	public class PluginMain : IPlugin
	{
        private String pluginName = "RegExpPanel";
        private String pluginGuid = "e854eb31-9327-4fe3-9b43-539fd07df7f1";
        private String pluginHelp = "www.sephiroth.it";
        private String pluginDesc = "RegExpPanel plugin for the new FlashDevelop 3.";
        private String pluginAuth = "Alessandro Crugnola";
        private String settingFilename;
        private Settings settingObject;
        private Form pluginPanel;
        private Image pluginImage;

        public const Int32 MARKER_MASK = 1 | (1 << 1) | (1 << 2) | (1 << 3);

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

        public static IMainForm MainForm
        {
            get { return PluginBase.MainForm; }
        }

        /// <summary>
        /// Initializes important variables
        /// </summary>
        public void InitBasics()
        {
            String dataPath = Path.Combine(PathHelper.DataDir, "RegExpPanel");
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");
            this.pluginImage = PluginBase.MainForm.FindImage("100");
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
        }

        /// <summary>
        /// Creates a menu item for the plugin and adds a ignored key
        /// </summary>
        public void CreateMenuItem()
        {
            ToolStripMenuItem viewMenu = (ToolStripMenuItem)PluginBase.MainForm.FindMenuItem("ViewMenu");
            viewMenu.DropDownItems.Add(new ToolStripMenuItem(LocaleHelper.GetString("Menu.Label"), this.pluginImage, new EventHandler(this.OpenPanel), this.settingObject.Shortcut));
            PluginBase.MainForm.IgnoredKeys.Add(this.settingObject.Shortcut);
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
        /// Opens the plugin panel if closed
        /// </summary>
        public void OpenPanel(Object sender, System.EventArgs e)
        {
            this.pluginPanel = new PluginUI(this);
            this.pluginPanel.Text = LocaleHelper.GetString("Panel.Title");
            this.pluginPanel.Show();
        }

        public static ScintillaControl CreateControl( )
        {
            ScintillaControl sci = new ScintillaControl();
            sci.AutoCSeparator = 32;
            sci.AutoCTypeSeparator = 63;
            sci.IsAutoCGetAutoHide = true;
            sci.IsAutoCGetCancelAtStart = false;
            sci.IsAutoCGetChooseSingle = false;
            sci.IsAutoCGetDropRestOfWord = false;
            sci.IsAutoCGetIgnoreCase = false;
            sci.ControlCharSymbol = 0;
            sci.CurrentPos = 0;
            sci.CursorType = -1;
            sci.Dock = System.Windows.Forms.DockStyle.Fill;
            sci.DocPointer = 187541976;
            sci.EndAtLastLine = 1;
            sci.EdgeColumn = 0;
            sci.EdgeMode = 0;
            sci.IsHScrollBar = true;
            sci.IsMouseDownCaptures = true;
            sci.IsBufferedDraw = true;
            sci.IsOvertype = false;
            sci.IsReadOnly = false;
            sci.IsUndoCollection = true;
            sci.IsVScrollBar = true;
            sci.IsUsePalette = true;
            sci.IsTwoPhaseDraw = true;
            sci.LayoutCache = 1;
            sci.Lexer = 3;
            sci.Location = new System.Drawing.Point(0, 0);
            sci.MarginLeft = 0;
            sci.MarginRight = 0;
            sci.ModEventMask = (Int32)ScintillaNet.Enums.ModificationFlags.InsertText | (Int32)ScintillaNet.Enums.ModificationFlags.DeleteText | (Int32)ScintillaNet.Enums.ModificationFlags.RedoPerformed | (Int32)ScintillaNet.Enums.ModificationFlags.UndoPerformed;
            sci.MouseDwellTime = ScintillaControl.MAXDWELLTIME;
            sci.Name = "sci";
            sci.PasteConvertEndings = false;
            sci.PrintColourMode = (Int32)ScintillaNet.Enums.PrintOption.Normal;
            sci.PrintWrapMode = (Int32)ScintillaNet.Enums.Wrap.Word;
            sci.PrintMagnification = 0;
            sci.SearchFlags = 0;
            sci.SelectionEnd = 0;
            sci.SelectionMode = 0;
            sci.SelectionStart = 0;
            sci.SmartIndentType = ScintillaNet.Enums.SmartIndent.CPP;
            sci.Status = 0;
            sci.StyleBits = 7;
            sci.TabIndex = 0;
            sci.TargetEnd = 0;
            sci.TargetStart = 0;
            sci.WrapStartIndent = 4;
            sci.WrapVisualFlagsLocation = (Int32)ScintillaNet.Enums.WrapVisualLocation.EndByText;
            sci.WrapVisualFlags = (Int32)ScintillaNet.Enums.WrapVisualFlag.End;
            sci.XOffset = 0;
            sci.ZoomLevel = 0;
            sci.UsePopUp(true);
            sci.SetMarginTypeN(0, 1);
            sci.SetMarginMaskN(0, 0);
            sci.SetYCaretPolicy((Int32)(ScintillaNet.Enums.CaretPolicy.Jumps | ScintillaNet.Enums.CaretPolicy.Even), 0);
            sci.Encoding = Encoding.GetEncoding((Int32)MainForm.Settings.DefaultCodePage);
            sci.EmptyUndoBuffer(); 
            ApplySciSettings(sci);
            return sci;
        }

        public static void ApplySciSettings(ScintillaControl sci)
        {
            try
            {
                sci.CaretPeriod = MainForm.Settings.CaretPeriod;
                sci.CaretWidth = MainForm.Settings.CaretWidth;
                sci.EOLMode = LineEndDetector.DetectNewLineMarker(sci.Text, (Int32)MainForm.Settings.EOLMode);
                sci.Indent = MainForm.Settings.IndentSize;
                sci.IsBackSpaceUnIndents = true;
                sci.IsCaretLineVisible = false;
                sci.IsTabIndents = true;
                sci.IsUseTabs = true;
                sci.IsViewEOL = false;
                sci.ScrollWidth = MainForm.Settings.ScrollWidth;
                sci.TabWidth = MainForm.Settings.TabWidth;
                sci.ViewWS = Convert.ToInt32(MainForm.Settings.ViewWhitespace);
                sci.WrapMode = Convert.ToInt32(MainForm.Settings.WrapText);
                sci.SetProperty("fold", Convert.ToInt32(MainForm.Settings.UseFolding).ToString());
                sci.SetProperty("fold.comment", Convert.ToInt32(MainForm.Settings.FoldComment).ToString());
                sci.SetProperty("fold.compact", Convert.ToInt32(MainForm.Settings.FoldCompact).ToString());
                sci.SetProperty("fold.preprocessor", Convert.ToInt32(MainForm.Settings.FoldPreprocessor).ToString());
                sci.SetProperty("fold.at.else", Convert.ToInt32(MainForm.Settings.FoldAtElse).ToString());
                sci.SetProperty("fold.html", Convert.ToInt32(MainForm.Settings.FoldHtml).ToString());
                sci.SetFoldFlags((Int32)MainForm.Settings.FoldFlags);

                /** 
                * Set correct line number margin width
                */
                Boolean viewLineNumbers = true;
                if (viewLineNumbers) sci.SetMarginWidthN(0, 36);
                else sci.SetMarginWidthN(0, 0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

		#endregion

	}
	
}
