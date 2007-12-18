using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI;
using PluginCore;
using PluginCore.Controls;

using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;

using FlashAPI.Resources;
using FlashAPI.Controls.TreeView;

using FlashDevelop.Dialogs;

using System.Reflection;
using System.Drawing;

namespace FlashAPI
{
	public class PluginUI : UserControl
    {
        private TreeView treeView1;
		private PluginMain pluginMain;
        private Settings settings;
        private ImageList imagelist;
        private bool IsLoaded = false;
        public bool IsCreated = false;
        private Panel panel1;
        private TextBox textBox1;
        List<XPathDocument> documents;
        XPathExpression xpathExpression_2;
        private ToolStrip toolStrip1;
        private ToolStripComboBox comboBox1;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton toolStripButton1;
        XPathExpression xpathExpression_1;


        public List<XPathDocument> Documents
        {
            get { return this.documents; }
            set { this.documents = value; }
        }

        public Settings Settings
        {
            set { 
                this.settings = value;
                IsCreated = false;
                OnPluginLoaded();
            }
            get { return this.settings;  }
        }

		public PluginUI(PluginMain pluginMain)
		{
			this.InitializeComponent();
            this.imagelist = new ImageList();
            this.imagelist.Images.Add(GetResource("Icons.ActionClosed.png"));
            this.imagelist.Images.Add(GetResource("Icons.ActionOpened.png"));
            this.imagelist.Images.Add(GetResource("Icons.ActionFunction.png"));

            this.Documents = null;
            this.treeView1.ImageList = this.imagelist;
			this.pluginMain = pluginMain;

            xpathExpression_1 = XPathExpression.Compile("folder|ifdef/folder|ifmode/folder");
            xpathExpression_2 = XPathExpression.Compile("string|ifdef/string|ifmode/string|action|ifmode/action|ifdef/action");

		}

        private void PluginUI_Load( object sender, EventArgs e )
        {
            this.IsLoaded = true;
            OnPluginLoaded();
        }


        public void OnPluginLoaded()
        {
            if (this.IsLoaded && FlashAPI.PluginMain.UIStarted && IsCreated == false)
            {
                IsCreated = true;
                RefreshMenu();
            }
        }


        public void RefreshMenu()
        {
            this.comboBox1.BeginUpdate();
            this.comboBox1.Items.Clear();
            if (Settings.UserClasspath != null)
            {
                foreach (string path in Settings.UserClasspath)
                {
                    if(File.Exists(path))
                        this.comboBox1.Items.Add(Directory.GetParent(path).Name + "\\" + Path.GetFileName(path));
                }
            }
            this.comboBox1.EndUpdate();

            if (this.comboBox1.Items.Count > 0) this.comboBox1.SelectedIndex = 0;
        }

        public void RefreshTree(int index)
        {
            this.treeView1.BeginUpdate();
            this.treeView1.Nodes.Clear();

            if (Settings.UserClasspath != null)
            {
                String invalid = this.isValid(Settings.UserClasspath[index]);

                if (invalid == null)
                {
                    this.panel1.Visible = false;
                    ParseHelpFile(Settings.UserClasspath[index]);
                }
                else
                {
                    this.panel1.Visible = true;
                    this.textBox1.Text = invalid;
                }
            }
            this.treeView1.EndUpdate(); 
        }

        private String isValid( String path )
        {
            if (File.Exists(path) && path != "")
            {
                try
                {
                    XPathDocument xp = new XPathDocument(path);
                    return null;
                }
                catch (System.Exception err)
                {
                    return err.Message;
                }
            }
            else
            {
                return LocaleHelper.GetString("File.NotExists") + " " + path;
            }
        }

        private void ParseHelpFile( String path )
        {
            if (File.Exists(path))
            {
                try
                {
                    XPathDocument xp = new XPathDocument(path);
                    XPathNavigator nav = xp.CreateNavigator();
                    XPathNavigator actions = nav.SelectSingleNode("/toolbox/actionspanel");

                    ParseNode(actions, this.treeView1);
                }
                catch (System.Exception err)
                {
                    MessageBox.Show(err.Message, Path.GetFileName(path), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void ParseNode( XPathNavigator node, TreeView tree ) 
        {
            TreeAPINode newNode;
            XPathNavigator currNode;

            XPathNodeIterator folder = node.Select( xpathExpression_1 );
            if (folder.Count > 0)
            {
                while (folder.MoveNext())
                {
                    currNode = folder.Current as XPathNavigator;
                    newNode = new TreeAPINode(currNode.GetAttribute("name", ""));
                    newNode.ToolTipText = currNode.GetAttribute("tiptext", "");
                    tree.Nodes.Add(newNode);
                    ParseNode(currNode, newNode);
                }
            }
            else
            {
                XPathNodeIterator items = node.Select( xpathExpression_2 );
                while (items.MoveNext())
                {
                    currNode = items.Current as XPathNavigator;
                    if (currNode.GetAttribute("text", "") != "")
                    {
                        newNode = new TreeAPINode(currNode.GetAttribute("name", ""), currNode.GetAttribute("text", ""));
                        newNode.ImageIndex = 2;
                        newNode.SelectedImageIndex = 2;
                        newNode.ToolTipText = currNode.GetAttribute("tiptext", "");
                        tree.Nodes.Add(newNode);
                    }
                }
            }
        }


        /// <summary>
        /// Recursively parse the current node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="treeNode"></param>
        private void ParseNode( XPathNavigator node, TreeAPINode treeNode )
        {
            TreeAPINode newNode;
            XPathNavigator currNode;

            XPathNodeIterator folder = node.Select(  xpathExpression_1 );
            if (folder.Count > 0)
            {
                while (folder.MoveNext())
                {
                    currNode = folder.Current as XPathNavigator;
                    newNode = new TreeAPINode(currNode.GetAttribute("name", ""));
                    newNode.ToolTipText = currNode.GetAttribute("tiptext", "");
                    treeNode.Nodes.Add(newNode);
                    ParseNode(currNode, newNode);
                }
            }
            else
            {
                XPathNodeIterator items = node.Select( xpathExpression_2 );
                while (items.MoveNext())
                {
                    currNode = items.Current as XPathNavigator;
                    if (currNode.GetAttribute("text", "") != "")
                    {
                        newNode = new TreeAPINode(currNode.GetAttribute("name", ""), currNode.GetAttribute("text", ""));
                        newNode.ImageIndex = 2;
                        newNode.SelectedImageIndex = 2;
                        newNode.ToolTipText = currNode.GetAttribute("tiptext", "");
                        treeNode.Nodes.Add(newNode);
                    }
                }
            }
        }

        private void treeView1_DoubleClick( object sender, EventArgs e )
        {
            TreeAPINode node = this.treeView1.SelectedNode as TreeAPINode;

            if (node.IsActionNode)
            {
                String text = node.Action;
                if (text.LastIndexOf(")") > -1)
                {
                    if (text.LastIndexOf(":") > text.LastIndexOf(")"))
                        text = text.Substring(0, text.LastIndexOf(":"));
                }
                if (text.IndexOf("(") > -1)
                    text = text.Substring(0, text.IndexOf("(")) + "(";

                ScintillaNet.ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;

                sci.ReplaceSel(text);
                sci.Focus();

                // show calltip
                if (UITools.CallTip.CallTipActive)
                    UITools.CallTip.Hide();

                if (!UITools.CallTip.CallTipActive || UITools.Manager.ShowDetails != UITools.Manager.ShowDetails)
                {
                    if (node.ToolTipText != "")
                    {
                        UITools.CallTip.CallTipShow(sci, sci.CurrentPos, node.Action + "\n" + node.ToolTipText);
                    }
                }

            }
        }

        private void treeView1_afterExpand( object sender, TreeViewEventArgs e )
        {
            e.Node.ImageIndex = 1;
            e.Node.SelectedImageIndex = 1;
        }

        private void treeView1_afterCollapse( object sender, TreeViewEventArgs e )
        {
            e.Node.ImageIndex = 0;
            e.Node.SelectedImageIndex = 0;
        }

        public static System.Drawing.Image GetResource( string resourceID )
        {
            resourceID = "FlashAPI." + resourceID;
            Assembly assembly = Assembly.GetExecutingAssembly();
            Image image = new Bitmap(assembly.GetManifestResourceStream(resourceID));
            return image;
        }
		
		#region Windows Forms Designer Generated Code

		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() 
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PluginUI));
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.comboBox1 = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.panel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.Location = new System.Drawing.Point(0, 27);
            this.treeView1.Name = "treeView1";
            this.treeView1.ShowRootLines = false;
            this.treeView1.Size = new System.Drawing.Size(280, 325);
            this.treeView1.TabIndex = 1;
            this.treeView1.DoubleClick += new System.EventHandler(this.treeView1_DoubleClick);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Location = new System.Drawing.Point(0, 27);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(280, 325);
            this.panel1.TabIndex = 2;
            this.panel1.Visible = false;
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.Control;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Cursor = System.Windows.Forms.Cursors.Default;
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Enabled = false;
            this.textBox1.Location = new System.Drawing.Point(0, 0);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(280, 325);
            this.textBox1.TabIndex = 0;
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.comboBox1,
            this.toolStripSeparator1,
            this.toolStripButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(280, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "Properties";
            // 
            // comboBox1
            // 
            this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(235, 21);
            this.comboBox1.SelectedIndexChanged +=new EventHandler(comboBox1_SelectedIndexChanged);

            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            //this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.Image = PluginBase.MainForm.FindImage("54");
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = LocaleHelper.GetString("Button.Settings");
            this.toolStripButton1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.toolStripButton1.Click +=new EventHandler(toolStripButton1_Click);
            // 
            // PluginUI
            // 
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.treeView1);
            this.Name = "PluginUI";
            this.Size = new System.Drawing.Size(280, 352);
            this.Load += new System.EventHandler(this.PluginUI_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private void comboBox1_SelectedIndexChanged( object sender, EventArgs e )
        {
            this.RefreshTree(((ToolStripComboBox)sender).SelectedIndex);
        }

        private void toolStripButton1_Click( object sender, EventArgs e )
        {
            SettingDialog.Show(pluginMain.Name);
        }
				
 	}

}
