
#region Imports
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI;
using PluginCore;
using PluginCore.Controls;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms.Layout;

using ProjectManager.Projects;
using System.ComponentModel;

using System.Text.RegularExpressions;
using PluginCore.Helpers;
using PluginCore.Managers;

using TaskListPanel.Utilities;

using ScintillaNet;
using FlashDevelop.Dialogs;
#endregion

namespace TaskListPanel
{
    public class PluginUI : UserControl, IEventHandler
    {
        private ListView   listView1;
		private PluginMain pluginMain;
        private ColumnHeader columnType;
        private ColumnHeader columnPos;
        private ColumnHeader columnIcon;
        private ImageList imageList1;
        private IContainer components;
        private ColumnHeader columnName;
        
        private Project project;
        private Timer parseTimer;

        private List<string> extensions;
        private ColumnHeader columnPath;
        private Regex todoParser = null;
        private ColumnHeader columnText;
        private ToolStrip toolStrip1;
        private ToolStripButton toolStripButton1;
        private ToolStripSeparator toolStripSeparator1;
        private bool is_enabled = false;
        private bool refreshEnabled = false;
        private ToolStripLabel toolStripLabel1;
        private int totalFiles;
        private int processedFiles;
        private bool firstExecutionCompleted = false;
        private ListViewColumnSorter lvwColumnSorter;
        private List<string> Groups;

        private string currentFileName;
        private int currentPos;
        private ToolStripButton toolStripButton2;
        private ToolStripSeparator toolStripSeparator2;

        // let's save a files cache
        private Dictionary<string, DateTime> filesCache = new Dictionary<string, DateTime>();

		public PluginUI(PluginMain pluginMain)
		{
			this.InitializeComponent();
            this.toolStripButton1.Image = PluginBase.MainForm.FindImage("24");
            this.toolStripButton2.Image = PluginBase.MainForm.FindImage("54");
			this.pluginMain = pluginMain;

            this.lvwColumnSorter = new ListViewColumnSorter();
            this.listView1.ListViewItemSorter = lvwColumnSorter;
            this.Groups = new List<string>();

            Settings settings = ((Settings)pluginMain.Settings);

            try
            {
                extensions = new List<string>();
                extensions.AddRange(settings.FileExtenions);
                if (settings.Groups.Length > 0)
                {
                    this.Groups.AddRange(settings.Groups);
                    string pattern = string.Join("|", settings.Groups);
                    this.todoParser = new Regex(@"(//|/\*\*?|\*)[\t ]*(" + pattern + ")[:|\t ]*([^\r|\n]*)", RegexOptions.Multiline);
                    is_enabled = true;
                    this.InitGraphics();
                }
            } catch(System.Exception err)
            {
                Debug.WriteLine("Error: " + err);
                is_enabled = false;
            }

            this.parseTimer = new Timer();
            this.parseTimer.Interval = 2000;
            this.parseTimer.Tick += delegate { this.parseNextFile(); };
            this.parseTimer.Tag   = null;
            this.parseTimer.Enabled = false;
		}


        /// <summary>
        /// Update extensions and search pattern
        /// </summary>
        public void UpdateSettings()
        {
            Settings settings = ((Settings)pluginMain.Settings);
            this.Groups.Clear();
            
            is_enabled = false;

            try
            {
                extensions = new List<string>();
                extensions.AddRange(settings.FileExtenions);
                if (settings.Groups.Length > 0)
                {
                    this.Groups.AddRange(settings.Groups);
                    string pattern = string.Join("|", settings.Groups);
                    this.todoParser = new Regex(@"(//|/\*\*?|\*)[\t ]*(" + pattern + ")[:|\t ]*([^\r|\n]*)", RegexOptions.Multiline);
                    is_enabled = true;
                    this.InitGraphics();
                }
                else
                {
                    is_enabled = false;
                }
            }
            catch(System.Exception err)
            {
                Debug.WriteLine("Error: " + err);
                is_enabled = false;
            }
        }

        public ProjectManager.Projects.Project Project
        {
            get { return this.project; }
            set { 
                this.project = value;
                InitProject();
            }
        }

        /// <summary>
        /// While parsing project files we need to diable the
        /// refresh button
        /// </summary>
        public bool RefreshEnabled
        {
            get { return this.refreshEnabled; }
            set
            {
                this.refreshEnabled = value;
                this.toolStripButton1.Enabled = value;
            }
        }

        public void Terminate()
        {
            if (this.parseTimer.Enabled)
                this.parseTimer.Stop();
        }

        /// <summary>
        /// Get all available files with extension match
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private List<string> GetFiles( string path )
        {
            List<string> files = new List<string>();
            foreach (string ext in extensions)
            {
                files.AddRange(Directory.GetFiles(path, "*" + ext, SearchOption.AllDirectories));
            }
            return files;
        }


        private List<string> GetFiles( string[] paths )
        {
            List<string> files = new List<string>();
            foreach (string path in paths)
                files.AddRange(GetFiles( project.GetAbsolutePath(path) ));
            return files;
        }

        /// <summary>
        /// When a new project is opened recreate the ui
        /// </summary>
        private void InitProject()
        {
            currentFileName = null;
            currentPos = -1;

            if (!is_enabled)
                return;

            this.listView1.Items.Clear();
            this.filesCache.Clear();

            if (this.Project != null)
            {
                RefreshProject();
            }
        }

        /// <summary>
        /// Refresh the current project parsing all files
        /// </summary>
        private void RefreshProject()
        {
            currentFileName = null;
            currentPos = -1;

            if (is_enabled && this.Project != null)
            {
                RefreshEnabled = false;
                if (this.parseTimer.Enabled)
                    this.parseTimer.Stop();

                System.Collections.Hashtable table = new Hashtable();
                table["status"] = 0;
                table["files"]  = GetFiles( project.Classpaths.ToArray() );

                this.parseTimer.Tag = table;
                this.parseTimer.Interval = 2000;
                this.parseTimer.Enabled = true;
                this.parseTimer.Start();

                totalFiles     = ((List<string>)table["files"]).Count;
                processedFiles = 0;

                this.toolStripLabel1.Text = "Refreshing...";
            }
        }

        /// <summary>
        /// At startup parse all opened files
        /// </summary>
        private void parseNextFile()
        {
            Hashtable table;
            String path;
            int status;

            if (this.parseTimer.Tag is Hashtable)
            {
                table = (Hashtable)this.parseTimer.Tag;
                status = (int)table["status"];

                if (status == 0)
                {
                    table["status"] = 1;
                    this.parseTimer.Interval = 80;
                }
                else if (status == 1)
                {
                    if (((List<string>)table["files"]).Count > 0)
                    {
                        bool parse_file = false;
                        path = ((List<string>)table["files"])[0];
                        DateTime lastWriteTime = new FileInfo(path).LastWriteTime;
                        if (!filesCache.ContainsKey(path))
                        {
                            filesCache[path] = lastWriteTime;
                            parse_file = true;
                        }
                        else
                        {
                            if (filesCache[path] != lastWriteTime)
                            {
                                parse_file = true;
                            }
                        }
                        ((List<string>)table["files"]).RemoveAt(0);

                        if(parse_file)
                            this.ParseFile(path);

                        processedFiles++;
                        this.toolStripLabel1.Text = "Processing " + processedFiles + " of " + totalFiles;
                    }
                    else
                    {
                        table["status"] = 2;
                    }
                }
                else
                {
                    this.ParseTimerCompleted();
                }
            }
            else
            {
                this.toolStripLabel1.Text = "";
            }
        }

        /// <summary>
        /// Parse timer completed parsing all files
        /// </summary>
        private void ParseTimerCompleted()
        {
            this.parseTimer.Enabled = false;
            this.parseTimer.Stop();
            this.RefreshEnabled = true;
            this.toolStripLabel1.Text = "";

            if (this.firstExecutionCompleted == false)
            {
                foreach (ITabbedDocument doc in PluginBase.MainForm.Documents)
                {
                    if(doc.IsEditable)
                    {
                        doc.SciControl.DwellStart += new DwellStartHandler(SciControl_DwellStart); ;
                    }
                }
                EventManager.AddEventHandler(this, EventType.FileSwitch | EventType.FileOpen | EventType.FileClose);
            }
            this.firstExecutionCompleted = true;
        }

        /// <summary>
        /// Parse a file adding all the found Matches into the listView
        /// </summary>
        /// <param name="path"></param>
        private void ParseFile( string path )
        {
            String text = String.Empty;
            Int32 codepage = FileHelper.GetFileCodepage(path);
            if (codepage == -1) return; // If the file is locked, stop.
            text = FileHelper.ReadFile(path, Encoding.GetEncoding(codepage));
            MatchCollection matches = this.todoParser.Matches(text);
            ListViewItem item;
            Hashtable itemTag;

            this.RemoveItemsByPath( path );

            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    itemTag = new Hashtable();
                    itemTag["FullPath"]      = path;
                    itemTag["LastWriteTime"] = new FileInfo(path).LastWriteTime;
                    itemTag["Position"] = match.Groups[2].Index;

                    item = new ListViewItem(new string[] {
                        "",
                        match.Groups[2].Index.ToString(),
                        match.Groups[2].Value,
                        match.Groups[3].Value.Trim(), 
                        Path.GetFileName(path),
                        Path.GetDirectoryName(path)
                    }, FindImageIndex( match.Groups[2].Value ));

                    item.Tag   = itemTag;
                    item.Name  = path;
                    this.listView1.Items.Add(item);
                }
            }
        }

        /// <summary>
        /// Parse a string
        /// </summary>
        /// <param name="text"></param>
        private void ParseFile( string text, string path )
        {
            MatchCollection matches = this.todoParser.Matches(text);
            ListViewItem item;
            Hashtable itemTag;

            this.RemoveItemsByPath(path);

            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    itemTag = new Hashtable();
                    itemTag["FullPath"] = path;
                    itemTag["LastWriteTime"] = new FileInfo(path).LastWriteTime;
                    itemTag["Position"] = match.Groups[2].Index;

                    item = new ListViewItem(new string[] {
                        "",
                        match.Groups[2].Index.ToString(),
                        match.Groups[2].Value,
                        match.Groups[3].Value.Trim(), 
                        Path.GetFileName(path),
                        Path.GetDirectoryName(path)
                    }, FindImageIndex(match.Groups[2].Value));

                    item.Tag = itemTag;
                    item.Name = path;
                    this.listView1.Items.Add(item);
                }
            }
        }

        /// <summary>
        /// Find the corresponding image index
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private int FindImageIndex( string p )
        {
            if (this.Groups.Contains(p))
                return this.Groups.IndexOf(p);
            else
                return -1;
        }

        /// <summary>
        /// Initialize the imagelist for the listView1
        /// </summary>
        private void InitGraphics()
        {
            Settings settings = (Settings)this.pluginMain.Settings;
            this.imageList1.Images.Clear();

            if (settings != null && settings.ImagesIndex != null)
            {
                foreach (int index in settings.ImagesIndex)
                {
                    this.imageList1.Images.Add(PluginBase.MainForm.FindImage(index.ToString()));
                }
            }
        }

        /// <summary>
        /// Remove all items by filename
        /// </summary>
        /// <param name="path">File full path</param>
        private void RemoveItemsByPath( string path )
        {
            ListViewItem[] items = this.listView1.Items.Find(path, false);
            foreach (ListViewItem item in items)
                item.Remove();
        }

        /// <summary>
        /// Remove invalid items. Check if the item file exists
        /// </summary>
        private void RemoveInvalidItems()
        {
            this.listView1.BeginUpdate();
            foreach (ListViewItem item in this.listView1.Items)
            {
                if (!File.Exists((string)item.Name))
                    item.Remove();
            }
            this.listView1.EndUpdate();
        }


        /// <summary>
        /// Move the document position
        /// </summary>
        /// <param name="sci"></param>
        /// <param name="position"></param>
        private void MoveToPosition( ScintillaControl sci, int position )
        {
            int line = sci.LineFromPosition(position);
            sci.EnsureVisible( line );
            sci.GotoPos(position);
            sci.SetSel(position, sci.LineEndPosition(line));
            sci.Focus();
        }

        #region Events

        /// <summary>
        /// Clicked on "Refresh" project button. This will refresh all the project files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton1_Click(Object sender, EventArgs e)
        {
            if (!is_enabled)
            {
                this.toolStripLabel1.Text = "Error. Verify settings";
            }
            else
            {
                RemoveInvalidItems();
                this.RefreshProject();
            }
        }

        /// <summary>
        /// Settings button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton2_Click( object sender, EventArgs e )
        {
            SettingDialog.Show(pluginMain.Settings, pluginMain.Name);
        }

        /// <summary>
        /// When user stop mouse movement parse again this file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="position"></param>
        private void SciControl_DwellStart( Object sender, int position )
        {
            if (!is_enabled) return;
            try
            {
                ScintillaControl sci = ((ScintillaControl)sender);
                if (this.filesCache.ContainsKey(sci.FileName))
                    this.filesCache.Remove(sci.FileName);

                this.ParseFile(sci.Text, sci.FileName);
            }
            catch(System.Exception error)
            {
                Debug.WriteLine("Error: " + error.Message);
            }
        }

        /// <summary>
        /// Double click on an element, open the file and move to the correct position
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView1_DoubleClick( object sender, EventArgs e )
        {
            if (!is_enabled) return;

            System.Windows.Forms.ListView.SelectedListViewItemCollection selected = this.listView1.SelectedItems;
            currentFileName = null;
            currentPos      = -1;
            if (selected.Count > 0)
            {
                ListViewItem firstSelected = selected[0];
                string path = (string)firstSelected.Name;

                currentFileName = path;
                currentPos = (int)((Hashtable)firstSelected.Tag)["Position"];

                if (PluginBase.MainForm.CurrentDocument.IsEditable)
                {
                    if (PluginBase.MainForm.CurrentDocument.FileName.ToUpper() == path.ToUpper())
                    {
                        MoveToPosition(PluginBase.MainForm.CurrentDocument.SciControl, currentPos);
                        currentFileName = null;
                        currentPos = -1;
                    }
                    else
                    {
                        PluginBase.MainForm.OpenEditableDocument(path, false);
                    }
                }
                else
                {
                    PluginBase.MainForm.OpenEditableDocument(path, false);
                }
            }
        }


        public void HandleEvent( Object sender, NotifyEvent e, HandlingPriority prority )
        {
            if (!is_enabled) return;

            switch(e.Type)
            {
                case EventType.FileOpen:
                    if (PluginBase.MainForm.CurrentDocument.IsEditable)
                    {
                        PluginBase.MainForm.CurrentDocument.SciControl.DwellStart += new DwellStartHandler(SciControl_DwellStart);
                        if (currentFileName != null && currentPos > -1)
                        {
                            if (currentFileName.ToUpper() == PluginBase.MainForm.CurrentDocument.FileName.ToUpper())
                            {
                                MoveToPosition(PluginBase.MainForm.CurrentDocument.SciControl, currentPos);
                            }
                        }
                    }
                    currentFileName = null;
                    currentPos = -1;
                    break;

                case EventType.FileSwitch:
                    if (PluginBase.MainForm.CurrentDocument.IsEditable)
                    {
                        if (currentFileName != null && currentPos > -1)
                        {
                            if (currentFileName.ToUpper() == PluginBase.MainForm.CurrentDocument.FileName.ToUpper())
                            {
                                MoveToPosition(PluginBase.MainForm.CurrentDocument.SciControl, currentPos);
                            }
                        }
                    }
                    currentFileName = null;
                    currentPos = -1;
                    break;

                case EventType.FileClose:
                    if(PluginBase.MainForm.CurrentDocument.IsEditable)
                    {
                        PluginBase.MainForm.CurrentDocument.SciControl.DwellStart -= new DwellStartHandler(SciControl_DwellStart);
                    }
                    break;
            }
        }


        /// <summary>
        /// Click on a listview header column, then sort the view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView1_ColumnClick( Object sender, ColumnClickEventArgs e )
        {
            if (!is_enabled) return;

            if (e.Column == lvwColumnSorter.SortColumn)
            {
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                    lvwColumnSorter.Order = SortOrder.Descending;
                else
                    lvwColumnSorter.Order = SortOrder.Ascending;
            }
            else
            {
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            this.listView1.Sort();
        }

        #endregion

        #region Windows Forms Designer Generated Code

        /// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() 
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PluginUI));
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnIcon = new System.Windows.Forms.ColumnHeader();
            this.columnPos = new System.Windows.Forms.ColumnHeader();
            this.columnType = new System.Windows.Forms.ColumnHeader();
            this.columnText = new System.Windows.Forms.ColumnHeader();
            this.columnName = new System.Windows.Forms.ColumnHeader();
            this.columnPath = new System.Windows.Forms.ColumnHeader();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnIcon,
            this.columnPos,
            this.columnType,
            this.columnText,
            this.columnName,
            this.columnPath});
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.LabelWrap = false;
            this.listView1.Location = new System.Drawing.Point(0, 26);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.ShowGroups = false;
            this.listView1.ShowItemToolTips = true;
            this.listView1.Size = new System.Drawing.Size(280, 326);
            this.listView1.SmallImageList = this.imageList1;
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
            this.listView1.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView1_ColumnClick);
            // 
            // columnIcon
            // 
            this.columnIcon.Text = "!";
            this.columnIcon.Width = 25;
            // 
            // columnPos
            // 
            this.columnPos.Text = "Position";
            this.columnPos.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // columnType
            // 
            this.columnType.Text = "Type";
            this.columnType.Width = 55;
            // 
            // columnText
            // 
            this.columnText.Text = "Text Line";
            this.columnText.Width = 150;
            // 
            // columnName
            // 
            this.columnName.Text = "File Name";
            this.columnName.Width = 150;
            // 
            // columnPath
            // 
            this.columnPath.Text = "File Path";
            this.columnPath.Width = 53;
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton2,
            this.toolStripSeparator1,
            this.toolStripButton1,
            this.toolStripSeparator2,
            this.toolStripLabel1
            });
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(280, 25);
            this.toolStrip1.Stretch = true;
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton2.Text = "Settings";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "Refresh";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(0, 22);
            this.toolStripLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolStripLabel1.Text = "Task List";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // PluginUI
            // 
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.listView1);
            this.Name = "PluginUI";
            this.Size = new System.Drawing.Size(280, 352);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
 	}

}
