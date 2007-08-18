using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI;
using PluginCore;
using System.IO;
using System.Diagnostics;
using System.Timers;

namespace BookmarkPanel
{
	public class PluginUI : UserControl
    {
        private ListView listView1;
        private ColumnHeader columnLine;
        private ColumnHeader columnText;
        private ImageList imageList1;
        private System.ComponentModel.IContainer components;
		private PluginMain pluginMain;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pluginMain"></param>
		public PluginUI(PluginMain pluginMain)
		{
			this.InitializeComponent();
			this.pluginMain = pluginMain;
        }

        #region Private Methods

        /// <summary>
        /// Ok, let's update the bookmarks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="position"></param>
        private void SciControl_DwellStart( ScintillaNet.ScintillaControl sender, int position )
        {
            UpdateMarkers(sender);
        }

        private void timer_Elapsed( Object sender, ElapsedEventArgs e )
        {
            TagTimer timer = ((TagTimer)sender);
            timer.Enabled = false;
            timer.Stop();

            if (((bool)((Hashtable)((ListViewGroup)timer.Tag).Tag)["Parsed"]) == false)
            {
                UpdateMarkers( ((ITabbedDocument)((Hashtable)((ListViewGroup)timer.Tag).Tag)["Document"]).SciControl );
                ((Hashtable)((ListViewGroup)timer.Tag).Tag)["Parsed"] = true;
                ((ITabbedDocument)((Hashtable)((ListViewGroup)timer.Tag).Tag)["Document"]).SciControl.DwellStart += new ScintillaNet.DwellStartHandler(SciControl_DwellStart);
            }

        }

        /// <summary>
        /// Update document bookmarks
        /// </summary>
        /// <param name="sender"></param>
        private void UpdateMarkers( ScintillaNet.ScintillaControl sender )
        {
            ITabbedDocument document = (ITabbedDocument)sender.Parent;
            List<int> markers = GetMarkers(sender);
            ListViewGroup group = FindGroup(document);
            if (group != null)
            {
                if (true)
                {
                    this.RemoveItemsFromGroup(group);
                    ListViewItem[] items = new ListViewItem[markers.Count];
                    ListViewItem item;
                    int index = 0;
                    foreach (int marker in markers)
                    {
                         item = new ListViewItem(new string[]{ 
                            (marker+1).ToString(), sender.GetLine(marker).TrimStart() },
                            -1);
                        item.Group = group;
                        item.Tag = marker;
                        item.Name = (string)((Hashtable)group.Tag)["FileName"];
                        items[index] = item;
                        index++;
                    }

                    this.listView1.BeginUpdate();
                    this.listView1.Items.AddRange(items);
                    ((Hashtable)group.Tag)["Markers"] = markers;
                    this.listView1.EndUpdate();

                    Debug.WriteLine(group.Items.Count);
                }
                else
                {
                    Debug.WriteLine("No need to update");
                }
            }
            
        }

        /// <summary>
        /// Return all the markers from a scintilla document
        /// </summary>
        /// <param name="sci"></param>
        /// <returns></returns>
        private List<int> GetMarkers( ScintillaNet.ScintillaControl sci )
        {
            List<int> markerLines = new List<int>();
            int line    = 0;
            int maxLine = 0;

            while (true)
            {
                if ((sci.MarkerNext(line, sci.GetMarginMaskN(0)) == -1) || (line > sci.LineCount)) break;
                line = sci.MarkerNext(line, sci.GetMarginMaskN(0));
                markerLines.Add(line);
                maxLine = Math.Max(maxLine, line);
                line++;
            }

            return markerLines;
        }

        /// <summary>
        /// Remove from the ListView all the items contained in a ListViewGroup
        /// </summary>
        /// <param name="group"></param>
        private void RemoveItemsFromGroup( ListViewGroup group )
        {
            ListViewItem[] items = new ListViewItem[group.Items.Count];
            group.Items.CopyTo(items, 0);

            foreach (ListViewItem item in items) item.Remove();
        }

        /// <summary>
        /// Double click on an item in the list view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView1_DoubleClick(Object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count > 0)
            {
                ListViewItem item = this.listView1.SelectedItems[0];
                Debug.WriteLine(item.Group);
                String filename = (String)((Hashtable)item.Group.Tag)["FileName"];
                int line = (int)item.Tag;
                if (PluginMain.ActivateDocument(filename))
                {
                    PluginMain.MainForm.CurrentDocument.SciControl.GotoLine(line);
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Create a new ListViewGroup and assign to the current listview
        /// </summary>
        /// <param name="doc"></param>
        public void CreateDocument( ITabbedDocument doc )
        {
            Debug.WriteLine("CreateDocument: " + doc);
            ListViewGroup group = new ListViewGroup();
            Hashtable table = new Hashtable();
            table["FileName"] = doc.FileName;
            table["Document"] = doc;
            table["Markers"]  = new List<int>();
            table["Parsed"]   = false;

            group.Header = Path.GetFileName(doc.FileName);
            group.Tag    = table;
            group.Name = doc.FileName;
            
            this.listView1.BeginUpdate();
            this.listView1.Groups.Add(group);
            this.listView1.EndUpdate();

            TagTimer timer = new TagTimer();
            timer.AutoReset = true;
            timer.SynchronizingObject = this;
            timer.Interval = 200;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Tag = group;
            timer.Start();
        }

        /// <summary>
        /// Remove the group and all associated subitems
        /// </summary>
        /// <param name="doc"></param>
        public void CloseDocument( ITabbedDocument doc )
        {
            doc.SciControl.DwellStart -= new ScintillaNet.DwellStartHandler(SciControl_DwellStart);
            this.listView1.BeginUpdate();
            foreach (ListViewGroup group in this.listView1.Groups)
            {
                if (((Hashtable)group.Tag)["Document"] == doc)
                {
                    this.listView1.Groups.Remove(group);
                    foreach (ListViewItem item in group.Items)
                    {
                        item.Remove();
                    }
                    break;
                }
            }
            this.listView1.EndUpdate();
        }

        /// <summary>
        /// Find a group from a given ITabbledDicument
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public ListViewGroup FindGroup( ITabbedDocument doc )
        {
            foreach (ListViewGroup group in this.listView1.Groups)
            {
                if (((Hashtable)group.Tag)["Document"] == doc)
                {
                    return group;
                }
            }
            return null;
        }

        /// <summary>
        /// Close All active documents/groups
        /// </summary>
        public void CloseAll()
        {
            this.listView1.BeginUpdate();
            this.listView1.Groups.Clear();
            this.listView1.Items.Clear();
            this.listView1.EndUpdate();
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
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnLine = new System.Windows.Forms.ColumnHeader();
            this.columnText = new System.Windows.Forms.ColumnHeader();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnLine,
            this.columnText});
            this.listView1.FullRowSelect = true;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(3, 3);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(274, 349);
            this.listView1.SmallImageList = this.imageList1;
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.DoubleClick += new EventHandler(listView1_DoubleClick);
            // 
            // columnLine
            // 
            this.columnLine.Text = "Line";
            // 
            // columnText
            // 
            this.columnText.Text = "Text";
            this.columnText.Width = 214;
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // PluginUI
            // 
            this.Controls.Add(this.listView1);
            this.Name = "PluginUI";
            this.Size = new System.Drawing.Size(280, 352);
            this.ResumeLayout(false);

		}

		#endregion
				
 	}

    public class TagTimer : System.Timers.Timer
    {
        public Object Tag;
    }

}
