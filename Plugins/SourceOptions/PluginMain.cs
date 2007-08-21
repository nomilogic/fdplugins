using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections;
using SourceOptions.Resources;
using PluginCore.Localization;
using PluginCore.Utilities;
using PluginCore.Managers;
using PluginCore.Helpers;
using PluginCore;
using ASCompletion.Model;
using ASCompletion.Context;
using ASCompletion.Completion;
using ScintillaNet;


namespace SourceOptions
{
	public class PluginMain : IPlugin
	{
        private String pluginName = "SourceOptions";
        private String pluginGuid = "e8a412a3-702c-4b83-b218-4209ecaebff9";
        private String pluginHelp = "www.sephiroth.it";
        private String pluginDesc = "SourceOptions for actionScript source code";
        private String pluginAuth = "Alessandro Crugnola";
        private String settingFilename;
        private Settings settingObject;
        private PluginUI pluginDialog;

        ToolStripMenuItem sourceMenu;
        ToolStripMenuItem organizeImportsMenu;
        ToolStripMenuItem getterMenu;
        ToolStripMenuItem accessorMenu;
        ToolStripSeparator separator;
        ToolStripMenuItem copyMenu;

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
            get { return null; }
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
            this.CreateContextMenu();
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
            String dataPath = Path.Combine(PathHelper.DataDir, "SourceOptions");
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
        /// Organize current Document import statements
        /// </summary>
        private void OrganizeImports(object sender, EventArgs e)
        {
            IASContext context = ASContext.Context;
            ScintillaControl sci = MainForm.CurrentDocument.SciControl;
            string eol = ASComplete.GetNewLineMarker(sci.EOLMode);
            int index;

            List<MemberModel> imports = new List<MemberModel>(context.CurrentModel.Imports.Count);
            imports.AddRange(context.CurrentModel.Imports.Items);

            if (imports.Count > 1)
            {
                ImportsComparerLine comparerLine = new ImportsComparerLine();
                ImportsComparerType comparerType = new ImportsComparerType();

                imports.Sort(comparerLine);

                foreach (MemberModel item in imports)
                {
                    sci.GotoLine(item.LineFrom);
                    sci.LineDelete();
                }
                imports.Sort(comparerType);

                sci.GotoLine(0);
                string lastNamespace = "";
                MemberModel lastItem = null;
                index = 0;

                for (index = 0; index < imports.Count; index++)
                {
                    if (lastItem != null)
                    {
                        if (lastItem.Type == imports[index].Type) continue;
                    }
                    lastItem = imports[index];
                    if (lastNamespace != lastItem.Type.Split('.')[0] && index > 0)
                    {
                        sci.InsertText(sci.CurrentPos, eol);
                    }
                    lastNamespace = lastItem.Type.Split('.')[0];
                    sci.InsertText(sci.CurrentPos, "import " + lastItem.Type.ToString() + ";" + (index == 0 && sci.GetLine(0).Trim() == "" ? "" : eol));
                }
            }
        }


        private void CopyFileName(Object sender, EventArgs e)
        {
            if (MainForm.CurrentDocument.IsEditable && !MainForm.CurrentDocument.IsUntitled)
            {
                String filename = MainForm.CurrentDocument.FileName;
                Clipboard.SetText(filename, TextDataFormat.UnicodeText);
            }
        }

        /// <summary>
        /// Create Getter/Setter methods from a class variable
        /// </summary>
        private void AddGetterSetterMethods(object sender, EventArgs e)
        {
            IASContext context = ASContext.Context.CurrentModel.Context;
            ScintillaControl sci = MainForm.CurrentDocument.SciControl;
            ASResult result = context.GetDeclarationAtLine(context.CurrentLine);
            string eol = ASComplete.GetNewLineMarker(sci.EOLMode);

            if (result.IsNull() == false)
            {
                if ((result.Member.Flags & FlagType.Variable) > 0)
                {
                    sci = MainForm.CurrentDocument.SciControl;
                    int lastLine = context.CurrentClass.LineTo;

                    pluginDialog = new PluginUI();
                    pluginDialog.Member = result.Member;
                    pluginDialog.ContextClass = context.CurrentClass;


                    if (pluginDialog.ShowDialog() == DialogResult.OK)
                    {
                        String memberName = pluginDialog.MemberName;
                        String text;
                        String indent = GetLineIndentation(sci.GetLine(context.CurrentClass.LineFrom), 0);
                        String type = result.Member.Type;
                        indent += indent;
                        if (indent == "") indent = "\t";

                        if (type == "") type = "Object";
                        sci.GotoLine(lastLine - 1);


                        if (context.GetType().ToString().Equals("AS2Context.Context"))
                        {
                            text = eol + "$(NLTAB)public function get " + memberName + "():" + type + " $(CSLB)$(NLTAB){" + eol + "$(NLTAB)\treturn " + result.Member.Name + ";" + eol + "$(NLTAB)}$(NLTAB)" + eol;
                            text += eol + "$(NLTAB)public function set " + memberName + "( value:" + type + " ):Void " + "$(CSLB)$(NLTAB){" + eol + "$(NLTAB)\t" + result.Member.Name + " = value;" + eol + "$(NLTAB)}$(NLTAB)" + eol;
                            text = FlashDevelop.Utilities.ArgsProcessor.ProcessCodeStyleLineBreaks(text);
                            text = ProcessLineBreaks(text, indent);
                            sci.InsertText(sci.PositionFromLine(lastLine), text);
                        }
                        else if (context.GetType().ToString().Equals("AS3Context.Context"))
                        {
                            text = eol + "$(NLTAB)public function get " + memberName + "():" + type + " $(CSLB)$(NLTAB){" + eol + "$(NLTAB)\treturn " + result.Member.Name + ";" + eol + "$(NLTAB)}$(NLTAB)" + eol;
                            text += eol + "$(NLTAB)public function set " + memberName + "( value:" + type + " ):void " + "$(CSLB)$(NLTAB){" + eol + "$(NLTAB)\t" + result.Member.Name + " = value;" + eol + "$(NLTAB)}$(NLTAB)" + eol;
                            text = FlashDevelop.Utilities.ArgsProcessor.ProcessCodeStyleLineBreaks(text);
                            text = ProcessLineBreaks(text, indent);
                            sci.InsertText(sci.PositionFromLine(lastLine), text);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Create Accessor methods from the selected member
        /// </summary>
        private void AddAccessorMethods(object sender, EventArgs e)
        {
            IASContext context = ASContext.Context.CurrentModel.Context;
            ScintillaControl sci = MainForm.CurrentDocument.SciControl;
            ASResult result = context.GetDeclarationAtLine(context.CurrentLine);
            string eol = ASComplete.GetNewLineMarker(sci.EOLMode);
            string accessorGet;
            string accessorSet;
            string memberName;

            if (result.IsNull() == false)
            {
                if ((result.Member.Flags & FlagType.Variable) > 0)
                {
                    sci = MainForm.CurrentDocument.SciControl;
                    int lastLine = context.CurrentClass.LineTo;
                    memberName = result.Member.Name;
                    memberName = memberName.Substring(0, 1).ToUpper() + memberName.Substring(1);

                    accessorGet = "get" + memberName;
                    accessorSet = "set" + memberName;

                    if (memberExistsInClass(accessorGet, context.CurrentClass) == false && memberExistsInClass(accessorSet, context.CurrentClass) == false)
                    {
                        String text;
                        String indent = GetLineIndentation( sci.GetLine(context.CurrentClass.LineFrom), 0 );
                        String type = result.Member.Type;
                        indent += indent;
                        if (indent == "") indent = "\t";
                        if (type == "") type = "Object";
                        sci.GotoLine(lastLine - 1);

                        if (context.GetType().ToString().Equals("AS2Context.Context"))
                        {
                            text = eol + "$(NLTAB)public function " + accessorGet + "():" + type + " $(CSLB)$(NLTAB){" + eol + "$(NLTAB)\treturn " + result.Member.Name + ";" + eol + "$(NLTAB)}$(NLTAB)" + eol;
                            text += eol + "$(NLTAB)public function " + accessorSet + "( value:" + type + " ):Void " + "$(CSLB)$(NLTAB){" + eol + "$(NLTAB)\t" + result.Member.Name + " = value;" + eol + "$(NLTAB)}$(NLTAB)" + eol;
                            text = FlashDevelop.Utilities.ArgsProcessor.ProcessCodeStyleLineBreaks(text);
                            text = ProcessLineBreaks(text, indent);
                            sci.InsertText(sci.PositionFromLine(lastLine), text);                            
                        }
                        else if (context.GetType().ToString().Equals("AS3Context.Context"))
                        {
                            text = eol + "$(NLTAB)public function " + accessorGet + "():" + type + " $(CSLB)$(NLTAB){" + eol + "$(NLTAB)\treturn " + result.Member.Name + ";" + eol + "$(NLTAB)}$(NLTAB)" + eol;
                            text += eol + "$(NLTAB)public function " + accessorSet + "( value:" + type + " ):void " + "$(CSLB)$(NLTAB){" + eol + "$(NLTAB)\t" + result.Member.Name + " = value;" + eol + "$(NLTAB)}$(NLTAB)" + eol;
                            text = FlashDevelop.Utilities.ArgsProcessor.ProcessCodeStyleLineBreaks(text);
                            text = ProcessLineBreaks(text, indent);
                            sci.InsertText(sci.PositionFromLine(lastLine), text);
                        }
                    }
                    else
                    {
                        MessageBox.Show("One of the accessor methods already exists in the current class", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        public static String ProcessLineBreaks( String text, String currentIdentation )
        {
            const String TAB = "$(NLTAB)";
            Int32 nextIndex = text.IndexOf(TAB);
            if (nextIndex < 0) return text;
            String result = "";
            Int32 currentIndex = 0;
            while (nextIndex >= 0)
            {
                result += text.Substring(currentIndex, nextIndex - currentIndex) + currentIdentation;
                currentIndex = nextIndex + TAB.Length;
                nextIndex = text.IndexOf(TAB, currentIndex);
            }
            return result + text.Substring(currentIndex);
        }

        private static String GetLineIndentation( String text, Int32 position )
        {
            Char c;
            Int32 endPos = position;
            while (endPos < text.Length)
            {
                c = text[endPos];
                if (c != '\t' && c != ' ') break;
                endPos++;
            }
            return text.Substring(0, endPos);
        }

        private void CreateContextMenu()
        {
            sourceMenu = new ToolStripMenuItem(LocaleHelper.GetString("Menu.Source"));
            copyMenu = new ToolStripMenuItem(LocaleHelper.GetString("Menu.CopyFileName"), null, new EventHandler(this.CopyFileName));
            organizeImportsMenu = new ToolStripMenuItem(LocaleHelper.GetString("Menu.OrganizeImports"), null, new EventHandler(this.OrganizeImports));
            getterMenu = new ToolStripMenuItem(LocaleHelper.GetString("Menu.Getter"), null, new EventHandler(this.AddGetterSetterMethods));
            accessorMenu = new ToolStripMenuItem(LocaleHelper.GetString("Menu.Accessor"), null, new EventHandler(this.AddAccessorMethods));
            separator = new ToolStripSeparator();

            sourceMenu.DropDownItems.Add( copyMenu );
            sourceMenu.DropDownItems.Add( organizeImportsMenu );
            sourceMenu.DropDownItems.Add( new ToolStripSeparator() );
            sourceMenu.DropDownItems.Add( getterMenu );
            sourceMenu.DropDownItems.Add( accessorMenu );

            MainForm.EditorMenu.Opening += this.EditorMenu_opening;
            MainForm.EditorMenu.Closed  += this.EditorMenu_closed;
        }

        /// <summary>
        /// Before the scintilla menu opens, check which menu item to display into
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditorMenu_opening( object sender, EventArgs e)
        {
            IASContext context   = ASContext.Context.CurrentModel.Context;
            ScintillaControl sci;
            ASResult result;

            if (context != null && (context.GetType().ToString().Equals("AS2Context.Context") || context.GetType().ToString().Equals("AS3Context.Context")))
            {
                if (!MainForm.EditorMenu.Items.Contains(sourceMenu))
                {
                    MainForm.EditorMenu.Items.Add(separator);
                    MainForm.EditorMenu.Items.Add(sourceMenu);
                }

                sci    = MainForm.CurrentDocument.SciControl;
                result = context.GetDeclarationAtLine(context.CurrentLine);

                getterMenu.Enabled = !result.IsNull() && (result.Member.Flags & FlagType.Variable) > 0;
                accessorMenu.Enabled = !result.IsNull() && (result.Member.Flags & FlagType.Variable) > 0;
                organizeImportsMenu.Enabled = context.GetType().ToString().Equals("AS2Context.Context");
            }
        }

        private void EditorMenu_closed( object sender, EventArgs e )
        {
            if(MainForm.EditorMenu.Items.Contains(separator))
                MainForm.EditorMenu.Items.Remove(separator);

            if(MainForm.EditorMenu.Items.Contains(sourceMenu))
                MainForm.EditorMenu.Items.Remove(sourceMenu);
        }

        public static bool memberExistsInClass(string name, ClassModel classContext)
        {
            foreach (MemberModel item in classContext.Members)
            {
                if (item.Name == name) return true;
            }
            return false;
        }

		#endregion

	}
	
}
