using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using FlashDevelop;
using ScintillaNet.Configuration;
using ScintillaNet;
using PluginCore.Helpers;
using PluginCore.Utilities;
using System.Diagnostics;
using Aga.Controls.Tree;
using PluginCore.FRService;
using RegExpPanel.Resources;
using FlashDevelop.Controls;
using PluginCore;

namespace RegExpPanel
{
    public partial class PluginUI : Form
    {
        private PluginMain pluginMain;
        private ScintillaControl matchTextSci;
        private ScintillaControl replaceTextSci;
        private ScintillaControl replaceReplaceSci;
        private Settings settings;
        private TreeModel matchResultModel;

        #region Constructor

        public PluginUI( PluginMain parent )
        {
            this.pluginMain = parent;
            settings = ((Settings)pluginMain.Settings);

            InitializeComponent();

            this.matchResultModel = new TreeModel();
            this.matchResultTree.Model = this.matchResultModel;

            // match controls
            this.matchPatternTextBox.Font = settings.PatternFont;
            this.matchTextSci = PluginMain.CreateControl();
            this.matchTextSci.Size = new System.Drawing.Size(450, 100);
            this.matchTextSci.WrapMode = 0;
            this.matchTextSci.StyleSetFont((Int32)ScintillaNet.Enums.StylesCommon.Default, settings.PatternFont.FontFamily.Name);
            this.panel2.Controls.Add(this.matchTextSci);
            this.matchOpenButton.Image = PluginMain.MainForm.FindImage("214");
            this.matchTestButton.Text = LocaleHelper.GetString("Button.Match");
            this.searchTestButton.Text = LocaleHelper.GetString("Button.Search");

            // replace controls
            this.replaceTextSci = PluginMain.CreateControl();
            this.replaceTextSci.Size = new Size(450, 100);
            this.replaceTextSci.WrapMode = 0;
            this.replaceTextSci.StyleSetFont((Int32)ScintillaNet.Enums.StylesCommon.Default, settings.PatternFont.FontFamily.Name);
            this.replacePanel1.Controls.Add(this.replaceTextSci);

            this.replaceReplaceSci = PluginMain.CreateControl();
            this.replaceReplaceSci.Size = new Size(450, 100);
            this.replaceReplaceSci.WrapMode = 0;
            this.replaceReplaceSci.StyleSetFont((Int32)ScintillaNet.Enums.StylesCommon.Default, settings.PatternFont.FontFamily.Name);
            this.replacePanel2.Controls.Add(this.replaceReplaceSci);
            this.replaceReplaceSci.IsReadOnly = true;

            this.replaceOpenButton.Image = PluginMain.MainForm.FindImage("214");
            this.replaceTestButton.Text = LocaleHelper.GetString("Button.Replace");
            this.replacePatternTextBox.Font = settings.PatternFont;
            this.replaceReplaceTextBox.Font = settings.PatternFont;

            this.label1.Text = LocaleHelper.GetString("Label.Pattern");
            this.label3.Text = LocaleHelper.GetString("Label.Pattern");
            this.label4.Text = LocaleHelper.GetString("Label.Replacement");
            this.toolStripStatusLabel1.Text = LocaleHelper.GetString("Status.Ready");

            this.tabPageMatch.Text = LocaleHelper.GetString("Tab.Match");
            this.tabPageReplace.Text = LocaleHelper.GetString("Tab.Replace");
            this.tabPage3.Text = LocaleHelper.GetString("Tab.Result");
            this.tabPage1.Text = LocaleHelper.GetString("Tab.Result");

            this.groupBox1.Text = LocaleHelper.GetString("Label.Modifiers");
            this.groupBox3.Text = LocaleHelper.GetString("Label.Modifiers");

            this.treeColumn1.Header = LocaleHelper.GetString("Match.Group");
            this.treeColumn2.Header = LocaleHelper.GetString("Match.Span");
            this.treeColumn3.Header = LocaleHelper.GetString("Match.Text");

            this.libraryToolStripMenuItem.Text = LocaleHelper.GetString("Menu.Library");
            this.helpItemToolStripMenuItem.Text = LocaleHelper.GetString("Menu.Help");
            this.insertToolStripMenuItem.Text = LocaleHelper.GetString("Menu.Insert");

            this.actionscriptRegExpClassToolStripMenuItem.Text = LocaleHelper.GetString("Menu.ASRegexpHelp");
            this.regExpTutorialsToolStripMenuItem.Text = LocaleHelper.GetString("Menu.RegexpHelp");

            PopulateLibrary(settings.Library);
        }

        #endregion

        #region Other Members

        /// <summary>
        /// Populate the "insert" menuitem adding all the string defined in
        /// the Library setting option
        /// </summary>
        /// <param name="list"></param>
        private void PopulateLibrary(string[] list)
        {
            this.insertToolStripMenuItem.DropDownItems.Clear();

            if (list != null && list.Length > 0)
            {
                ToolStripMenuItem item;
                Int32 k = 0;
                foreach (String line in list)
                {
                    string[] menu_text = line.Split(new char[] { ':' }, 2);
                    if (menu_text.Length > 1)
                    {
                        item = new ToolStripMenuItem();
                        item.Name = "insertToolStripMenuItem_" + k;
                        item.Size = new System.Drawing.Size(152, 22);
                        item.ToolTipText = menu_text[1];
                        item.Text = menu_text[0];
                        item.Tag = menu_text[1];
                        item.Click += new EventHandler(libraryItem_Click);
                        this.insertToolStripMenuItem.DropDownItems.Add(item);
                    }
                    k++;
                }
            }
            else
            {
                this.insertToolStripMenuItem.Enabled = false;
            }
        }

        /// <summary>
        /// Open a document and display its text into the passed
        /// scintilla control
        /// </summary>
        /// <param name="file"></param>
        /// <param name="targetSci"></param>
        private void OpenDocument(string file, ScintillaControl targetSci)
        {
            String text = String.Empty;
            Int32 codepage = FileHelper.GetFileCodepage(file);
            if (codepage == -1) return;
            text = FileHelper.ReadFile(file, Encoding.GetEncoding(codepage));
            targetSci.Encoding = Encoding.GetEncoding(codepage);
            targetSci.Text = DataConverter.ChangeEncoding(text, codepage, targetSci.CodePage);
        }

        #endregion

        #region Events

        /// <summary>
        /// Selected library menu item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void libraryItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            if (this.tabControl1.SelectedIndex == 0)
            {
                // match & search
                this.matchPatternTextBox.Text = this.matchPatternTextBox.Text.Substring(0, this.matchPatternTextBox.SelectionStart) + (String)item.Tag + this.matchPatternTextBox.Text.Substring(this.matchPatternTextBox.SelectionStart + this.matchPatternTextBox.SelectionLength);
            }
            else
            {
                // replace
                this.replacePatternTextBox.Text = this.replacePatternTextBox.Text.Substring(0, this.replacePatternTextBox.SelectionStart) + (String)item.Tag + this.replacePatternTextBox.Text.Substring(this.replacePatternTextBox.SelectionStart + this.replacePatternTextBox.SelectionLength);
            }
        }

        /// <summary>
        /// Open and display a document into the current scintilla control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void openButton_Click(object sender, EventArgs e)
        {
            ScintillaControl sci = null;
            if (sender == this.replaceOpenButton)
            {
                sci = this.replaceTextSci;
            }
            else if (sender == this.matchOpenButton)
            {
                sci = this.matchTextSci;
            }

            if (sci != null)
            {
                if (this.openFileDialog1.ShowDialog(this) == DialogResult.OK && this.openFileDialog1.FileName.Length != 0)
                {
                    this.OpenDocument(this.openFileDialog1.FileName, sci);
                }
            }
        }

        /// <summary>
        /// Match a regular expression
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void matchTestButton_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("matchTestButton_Click");
            RegexOptions options = RegexOptions.None;

            if (this.cb_dotAll.Checked)
                options |= RegexOptions.Singleline;

            if (this.cb_multiline.Checked)
                options |= RegexOptions.Multiline;

            if (this.cb_verbose.Checked)
                options |= RegexOptions.IgnorePatternWhitespace;

            if (this.cb_ignoreCase.Checked)
                options |= RegexOptions.IgnoreCase;

            TestRegExp_Match(this.matchPatternTextBox.Text, this.matchTextSci.Text, options, this.cb_findAll.Checked);
        }

        private void searchTestButton_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("searchTestButton_Click");
            RegexOptions options = RegexOptions.None;

            if (this.cb_dotAll.Checked)
                options |= RegexOptions.Singleline;

            if (this.cb_multiline.Checked)
                options |= RegexOptions.Multiline;

            if (this.cb_verbose.Checked)
                options |= RegexOptions.IgnorePatternWhitespace;

            if (this.cb_ignoreCase.Checked)
                options |= RegexOptions.IgnoreCase;

            TestRegExp_Search(this.matchPatternTextBox.Text, this.matchTextSci.Text, options);
        }

        /// <summary>
        /// Replacement test
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void replaceTestButton_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("replaceTestButton_Click");
            RegexOptions options = RegexOptions.None;

            if (this.rcb_dotAll.Checked)
                options |= RegexOptions.Singleline;

            if (this.rcb_multiline.Checked)
                options |= RegexOptions.Multiline;

            if (this.rcb_verbose.Checked)
                options |= RegexOptions.IgnorePatternWhitespace;

            if (this.rcb_ignoreCase.Checked)
                options |= RegexOptions.IgnoreCase;

            TestRegExp_Replace(this.replacePatternTextBox.Text, this.replaceReplaceTextBox.Text, this.replaceTextSci.Text, options, this.rcb_findAll.Checked);
        }

        /// <summary>
        /// Selection changed into the treelistview
        /// then hilight the selection into the scintilla control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void matchResultTree_SelectionChanged(object sender, System.EventArgs e)
        {
            Debug.WriteLine("matchResultTree_SelectionChanged");
            MyNode node;
            if (this.matchResultTree.SelectedNode != null)
            {
                if (this.matchResultTree.SelectedNode.Tag != null)
                {
                    node = (MyNode)this.matchResultTree.SelectedNode.Tag;
                    SearchMatch match = new SearchMatch();
                    match.Index = node.Spans[0];
                    match.Length = node.Spans[1];
                    match.Value = node.Text;
                    SelectMatch(this.matchTextSci, match);
                }
            }
        }

        /// <summary>
        /// Restore from settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PluginUI_Load(object sender, EventArgs e)
        {
            this.matchPatternTextBox.Text = settings.LastMatchTextPattern;
            this.replacePatternTextBox.Text = settings.LastReplaceTextPattern;
            this.replaceReplaceTextBox.Text = settings.LastReplaceTextReplacement;

            this.cb_dotAll.Checked = settings.MatchDotAll;
            this.cb_findAll.Checked = settings.MatchFindAll;
            this.cb_ignoreCase.Checked = settings.MatchIgnoreCase;
            this.cb_multiline.Checked = settings.MatchMultiline;
            this.cb_verbose.Checked = settings.MatchVerbose;

            this.rcb_dotAll.Checked = settings.ReplaceDotAll;
            this.rcb_findAll.Checked = settings.ReplaceFindAll;
            this.rcb_ignoreCase.Checked = settings.ReplaceIgnoreCase;
            this.rcb_multiline.Checked = settings.ReplaceMultiline;
            this.rcb_verbose.Checked = settings.ReplaceVerbose;

            if (PluginMain.MainForm.CurrentDocument.IsEditable)
            {
                this.matchTextSci.Text   = PluginMain.MainForm.CurrentDocument.SciControl.Text;
                this.replaceTextSci.Text = PluginMain.MainForm.CurrentDocument.SciControl.Text;
            }
        }

        /// <summary>
        /// Save settings on unload
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PluginUI_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            settings.LastMatchTextPattern  = this.matchPatternTextBox.Text;
            settings.LastReplaceTextPattern = this.replacePatternTextBox.Text;
            settings.LastReplaceTextReplacement = this.replaceReplaceTextBox.Text;

            settings.MatchDotAll = this.cb_dotAll.Checked;
            settings.MatchFindAll = this.cb_findAll.Checked;
            settings.MatchIgnoreCase = this.cb_ignoreCase.Checked;
            settings.MatchMultiline = this.cb_multiline.Checked;
            settings.MatchVerbose = this.cb_verbose.Checked;

            settings.ReplaceDotAll = this.rcb_dotAll.Checked;
            settings.ReplaceFindAll = this.rcb_findAll.Checked;
            settings.ReplaceIgnoreCase = this.rcb_ignoreCase.Checked;
            settings.ReplaceMultiline = this.rcb_multiline.Checked;
            settings.ReplaceVerbose = this.rcb_verbose.Checked;
        }

        #endregion

        #region RegExp Match

        /// <summary>
        /// create the as code for regexp.match
        /// </summary>
        private void update_matchAS()
        {
            Debug.WriteLine("update_matchAS");
            String text = this.matchPatternTextBox.Text;
            String options = "";

            if (this.cb_dotAll.Checked) options += "s";
            if (this.cb_findAll.Checked) options += "g";
            if (this.cb_ignoreCase.Checked) options += "i";
            if (this.cb_multiline.Checked) options += "m";
            if (this.cb_verbose.Checked) options += "x";

            this.matchAsTextBox.Text = "var reg:RegExp = new RegExp(\"" + text + "\", \"" + options + "\");\r\n";
            this.matchAsTextBox.Text += "var testString:String = \"example text....\";\r\n";
            this.matchAsTextBox.Text += "trace(testString.match(reg));\r\n";
        }

        /// <summary>
        /// Perform the match test
        /// </summary>
        /// <param name="p"></param>
        /// <param name="p_2"></param>
        /// <param name="options"></param>
        /// <param name="match_all"></param>
        private void TestRegExp_Match(string p, string p_2, RegexOptions options, Boolean match_all)
        {
            Debug.WriteLine("TestRegExp_Match");
            Regex reg;

            this.matchResultTree.SelectedNode = null;
            this.matchResultModel.Nodes.Clear();
            RemoveHighlights(this.matchTextSci);

            try
            {
                reg = new Regex(p, options);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, LocaleHelper.GetString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.matchResultTree.BeginUpdate();
            if (match_all)
            {
                MatchCollection results = reg.Matches(p_2);
                TestRegExpMatch_DisplayResults(results);
                UpdateStatusResults(results);
            }
            else
            {
                Match result = reg.Match(p_2);
                TestRegExpMatch_DisplayResults(result, 0);
                UpdateStatusResults(result);
            }
            this.matchResultTree.EndUpdate();

            update_matchAS();
        }

        private void UpdateStatusResults(MatchCollection results)
        {
            this.toolStripStatusLabel1.Text = results.Count + " results";
        }

        private void UpdateStatusResults(Match result)
        {
            if (result.Success)
                this.toolStripStatusLabel1.Text = "1 result";
            else
                this.toolStripStatusLabel1.Text = "0 result";
        }

        /// <summary>
        /// Display all the results into the treelistview
        /// </summary>
        private void TestRegExpMatch_DisplayResults(MatchCollection result)
        {
            Debug.WriteLine("TestRegExp_DisplayResults");
            Int32 i = 0;
            foreach (Match match in result)
            {
                TestRegExpMatch_DisplayResults(match, i);
                i++;
            }
        }

        /// <summary>
        /// Display the match into the treelistview
        /// </summary>
        /// <param name="match"></param>
        /// <param name="index"></param>
        private void TestRegExpMatch_DisplayResults(Match match, Int32 index)
        {
            Debug.WriteLine("TestRegExp_DisplayResults(2)");
            MyNode item;
            MyNode node;
            Group group;
            SearchMatch s_match;

            if (match.Success)
            {
                item = new MyNode("Group " + index + " Group(0)", match.Index + "-" + (match.Index + match.Length), match.Value);
                item.Spans = new int[] { match.Index, match.Length };

                if (match.Groups.Count > 1)
                {
                    for (int k = 1; k < match.Groups.Count; k++)
                    {
                        group = match.Groups[k];
                        node = new MyNode("Group " + index + " Group(" + k + ")", group.Index + "-" + (group.Index + group.Length), group.Value);
                        node.Spans = new int[] { group.Index, group.Length };
                        item.Nodes.Add(node);
                    }

                    s_match = new SearchMatch();
                    s_match.Index = match.Index;
                    s_match.Length = match.Length;
                    s_match.Value = match.Value;

                    AddHighlight(this.matchTextSci, s_match);
                }
                this.matchResultModel.Nodes.Add(item);
            }
        }

        #endregion

        #region RegExp Search

        /// <summary>
        /// create the as code for regexp.search
        /// </summary>
        private void update_searchAS()
        {
            Debug.WriteLine("update_searchAS");
            String text = this.matchPatternTextBox.Text;
            String options = "";

            if (this.cb_dotAll.Checked) options += "s";
            if (this.cb_ignoreCase.Checked) options += "i";
            if (this.cb_multiline.Checked) options += "m";
            if (this.cb_verbose.Checked) options += "x";

            this.matchAsTextBox.Text = "var reg:RegExp = new RegExp(\"" + text + "\", \"" + options + "\");\r\n";
            this.matchAsTextBox.Text += "var testString:String = \"example text....\";\r\n";
            this.matchAsTextBox.Text += "trace(testString.search(reg));\r\n";
        }

        /// <summary>
        /// Perform the match test
        /// </summary>
        /// <param name="p"></param>
        /// <param name="p_2"></param>
        /// <param name="options"></param>
        /// <param name="match_all"></param>
        private void TestRegExp_Search(string p, string p_2, RegexOptions options)
        {
            Debug.WriteLine("TestRegExp_Search");
            Regex reg;

            this.matchResultTree.SelectedNode = null;
            this.matchResultModel.Nodes.Clear();
            RemoveHighlights(this.matchTextSci);

            try
            {
                reg = new Regex(p, options);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, LocaleHelper.GetString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.matchResultTree.BeginUpdate();
            Match result = reg.Match(p_2);
            TestRegExpSearch_DisplayResults(result);
            UpdateStatusResults(result);
            this.matchResultTree.EndUpdate();
            update_searchAS();
        }

        private void TestRegExpSearch_DisplayResults(Match match)
        {
            Debug.WriteLine("TestRegExpSearch_DisplayResults");
            MyNode item;
            SearchMatch s_match;

            if (match.Success)
            {

                item = new MyNode("Result (" + match.Index + ")", match.Index + "-" + (match.Index + match.Length), match.Value);
                item.Spans = new int[] { match.Index, match.Length };
                s_match = new SearchMatch();
                s_match.Index = match.Index;
                s_match.Length = match.Length;
                s_match.Value = match.Value;
                AddHighlight(this.matchTextSci, s_match);
                this.matchResultModel.Nodes.Add(item);
            }
        }

        #endregion

        #region Replace

        /// <summary>
        /// create the code for regexp.replace
        /// </summary>
        private void update_replaceAS()
        {
            String text = this.replacePatternTextBox.Text;
            String options = "";

            if (this.rcb_dotAll.Checked) options += "s";
            if (this.rcb_ignoreCase.Checked) options += "i";
            if (this.rcb_multiline.Checked) options += "m";
            if (this.rcb_verbose.Checked) options += "x";

            this.replaceAsTextBox.Text = "var reg:RegExp = new RegExp(\"" + text + "\", \"" + options + "\");\r\n";
            this.replaceAsTextBox.Text += "var testString:String = \"example text....\";\r\n";
            this.replaceAsTextBox.Text += "var replacementString:String = \"" + this.replaceReplaceTextBox.Text + "\";\r\n";
            this.replaceAsTextBox.Text += "trace(testString.replace(reg, replacementStr));\r\n";
        }

        /// <summary>
        /// Perform the replace test
        /// </summary>
        /// <param name="p"></param>
        /// <param name="p_2"></param>
        /// <param name="p_3"></param>
        /// <param name="options"></param>
        private void TestRegExp_Replace(string p, string p_2, string p_3, RegexOptions options, Boolean all_matches)
        {
            Debug.WriteLine("TestRegExp_Replace");
            Regex reg;
            String result;

            this.replaceReplaceSci.IsReadOnly = false;
            this.replaceReplaceSci.Text = "";

            try
            {
                reg = new Regex(p, options);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, LocaleHelper.GetString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.replaceReplaceSci.IsReadOnly = true;
                return;
            }

            if (all_matches)
                result = reg.Replace(p_3, p_2);
            else
                result = reg.Replace(p_3, p_2, 1);

            this.toolStripStatusLabel1.Text = "Replace done.";

            this.replaceReplaceSci.Text = result;
            this.replaceReplaceSci.IsReadOnly = true;
            update_replaceAS();
        }

        #endregion

        # region ScintillaControls operations

        /// <summary>
        /// Selects a search match
        /// </summary>
        public static void SelectMatch(ScintillaControl sci, SearchMatch match)
        {
            Int32 start = sci.MBSafePosition(match.Index); // wchar to byte position
            Int32 end = start + sci.MBSafeTextLength(match.Value); // wchar to byte text length
            Int32 line = sci.LineFromPosition(start);
            sci.EnsureVisible(line);
            sci.SetSel(start, end);
        }


        /// <summary>
        /// Highlight a regexp match group
        /// </summary>
        /// <param name="sci"></param>
        /// <param name="matches"></param>
        private void AddHighlight(ScintillaControl sci, SearchMatch match)
        {
            Int32 start = sci.MBSafePosition(match.Index); // wchar to byte position
            Int32 end = start + sci.MBSafeTextLength(match.Value); // wchar to byte text length
            Int32 line = sci.LineFromPosition(start);
            Int32 position = start;
            Int32 es = sci.EndStyled;
            Int32 mask = 1 << sci.StyleBits;
            sci.SetIndicStyle(0, (Int32)ScintillaNet.Enums.IndicatorStyle.Max);
            sci.SetIndicFore(0, 0xff0000);
            sci.StartStyling(position, mask);
            sci.SetStyling(end - start, mask);
            sci.StartStyling(es, mask - 1);
        }

        /// <summary>
        /// Remove all highligths from a scintilla control
        /// </summary>
        /// <param name="sci"></param>
        private void RemoveHighlights(ScintillaControl sci)
        {
            Int32 es = sci.EndStyled;
            Int32 mask = (1 << sci.StyleBits);
            sci.StartStyling(0, mask);
            sci.SetStyling(sci.TextLength, 0);
            sci.StartStyling(es, mask - 1);
        }

        #endregion

        private void actionscriptRegExpClassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PluginBase.MainForm.CallCommand("Browse", "http://livedocs.adobe.com/flex/201/langref/index.html");
        }

        private void regExpTutorialsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PluginBase.MainForm.CallCommand("Browse", "http://www.regular-expressions.info");
        }

    }
}