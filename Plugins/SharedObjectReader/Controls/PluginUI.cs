using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SOReader;
using SOReader.Sol;
using SOReader.Sol.AMF;
using System.Diagnostics;
using System.Collections;
using SOReader.Sol.AMF.DataType;
using SharedObjectReader.Controls;

namespace SharedObjectReader.Controls
{
    public partial class PluginUI : Form
    {
        private SharedObject current_sol;
        private string current_filename;

        public PluginUI()
        {
            InitializeComponent();
        }


        private void OpenSharedObject(string filename)
        {
            try
            {
                current_filename = filename;
                current_sol = Reader.Parse(filename);
                tabControl1.Visible = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error parsing shared object", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            reloadToolStripMenuItem.Enabled = true;
            PopulateTree(current_sol);
        }

        private void CloseSharedObject()
        {
            if (current_sol != null)
            {
                EmptyTree();
                current_sol = null;
            }
            current_filename = null;
            reloadToolStripMenuItem.Enabled = false;
            tabControl1.Visible = false;
            toolStripStatusLabel1.Text = "Ready";
        }

        private void EmptyTree()
        {
            this.treeView1.Nodes.Clear();
        }

        private void PopulateTree(SharedObject obj)
        {
            this.treeView1.Nodes.Clear();
            this.treeView1.BeginUpdate();

            TreeNode rootnode = new TreeNode(current_sol.Name);
            rootnode.Tag = current_sol;
            rootnode.ImageKey = "shared_object";
            rootnode.SelectedImageKey = "shared_object";
            treeView1.Nodes.Add(rootnode);

            TreeNode datanode = new TreeNode("data");
            AssignImage(datanode, (IAMFBase)obj.Data);
            datanode.Tag = current_sol.Data;


            foreach (string key in ((Dictionary<string,object>)obj.Data.Source).Keys)
            {
                Dictionary<string, object> data = (Dictionary<string, object>)obj.Data.Source;
                TreeNode node = new TreeNode(key);
                AssignImage(node, (AMFBase)data[key]);
                node.Tag = ((Dictionary<string, object>)obj.Data.Source)[key];
                PopulateTree(node, ((Dictionary<string, object>)obj.Data.Source)[key]);
                datanode.Nodes.Add(node);
            }
            rootnode.Nodes.Add(datanode);
            this.treeView1.EndUpdate();
            toolStripStatusLabel1.Text = "Name: " + current_sol.Name + " | FileSize: " + current_sol.FileSize + " | AMF encoding: " + current_sol.AMFEncoding;

            this.treeView1.SelectedNode = rootnode;

        }

        private void PopulateTree(TreeNode node, object p)
        {
            IAMFBase realObj = (IAMFBase)p;
            if (realObj.AmfType == AMFType.AMF0_ARRAY)
            {
                PopulateNode(node, (Dictionary<string, object>)((AMF0Array)p).Source);
            }
            else if (realObj.AmfType == AMFType.AMF3_ARRAYLIST)
            {
                PopulateNode(node, (ArrayList)((AMF3ArrayList)p).Source);
            }
            else if (realObj.AmfType == AMFType.AMF0_OBJECT || realObj.AmfType == AMFType.AMF3_OBJECT)
            {
                PopulateNode(node, (Dictionary<string, object>)((IAMFBase)p).Source);
            }
        }

        private void PopulateNode(TreeNode parent_node, Dictionary<string, object> item)
        {
            foreach (string key in item.Keys)
            {
                TreeNode node = new TreeNode(key);
                node.Tag = item[key];
                AssignImage(node, (IAMFBase)item[key]);
                parent_node.Nodes.Add(node);
                // populate sub-nodes
                PopulateTree(node, item[key]);
            }
        }

        private void PopulateNode(TreeNode parent_node, ArrayList item)
        {
            for(int i = 0; i < item.Count; i++)
            {
                TreeNode node = new TreeNode("" + i);
                node.Tag = item[i];
                AssignImage(node, (IAMFBase)item[i]);
                parent_node.Nodes.Add(node);
                // populate sub-nodes
                PopulateTree(node, item[i]);
            }
        }

        private void AssignImage(TreeNode node, IAMFBase element)
        {
            if (imageList1.Images.ContainsKey(element.Name))
            {
                node.ImageKey = element.Name;
                node.SelectedImageKey = element.Name;
            }
            else
            {
                node.ImageKey = "None";
                node.SelectedImageKey = "None";
            }
        }

        #region Events

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                CloseSharedObject();
                OpenSharedObject(openFileDialog1.FileName);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseSharedObject();
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenSharedObject(current_filename);
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (((TreeView)sender).SelectedNode.Tag != null)
            {
                if (((TreeView)sender).SelectedNode.Tag is IAMFBase)
                {
                    tab_Select(((TreeView)sender).SelectedNode.ImageKey, ((TreeView)sender).SelectedNode.Text, (IAMFBase)((TreeView)sender).SelectedNode.Tag);
                }
                else if (((TreeView)sender).SelectedNode.Tag is SharedObject)
                {
                    tab_Select(((TreeView)sender).SelectedNode.ImageKey, ((TreeView)sender).SelectedNode.Text, (SharedObject)((TreeView)sender).SelectedNode.Tag);
                }
            }
        }

        private void tab_Select(string imagekey, string p, IAMFBase element)
        {
            tabPage1.Text = element.Name;
            tabPage1.ImageKey = imagekey;

            tabPage1.Controls.Clear();
            UserControl panel = AddPropertyPanel(element);
            if (panel != null)
            {
                panel.Dock = DockStyle.Fill;
                tabPage1.Controls.Add(panel);
                ((IAMFDisplayPanel)panel).Populate(p, element);
                if (panel is panelAMF3Reference)
                {
                    ((panelAMF3Reference)panel).Reference += new EventHandler<EventArgs>(Form1_Reference);
                }
            }
        }

        private void tab_Select(string imagekey, string p, SharedObject element)
        {
            tabPage1.Text = element.Name;
            tabPage1.ImageKey = imagekey;
            tabPage1.Controls.Clear();
            UserControl panel = AddPropertyPanel(element);
            if (panel != null)
            {
                panel.Dock = DockStyle.Fill;
                tabPage1.Controls.Add(panel);
                ((panelSharedObject)panel).Populate(p, element);
            }
        }

        /// <summary>
        /// Click on the link label into a reference panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Form1_Reference(object sender, EventArgs e)
        {
            AMF0Reference element = (AMF0Reference)((panelAMF3Reference)sender).Element;
            IAMFBase result = null;
            try
            {
                result = element.Parser.FindObjectReference(element.Reference);
            } catch(ArgumentOutOfRangeException error)
            {
                MessageBox.Show(error.Message, "Invalid reference #" + element.Reference, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (result != null)
            {
                TreeNode treenode_result = FindForm(this.treeView1.Nodes, result);
                if (treenode_result != null)
                {
                    this.treeView1.SelectedNode = treenode_result;
                }
            }
            else
            {
                Debug.WriteLine("cannot find " + element.Reference);
            }
        }

        private TreeNode FindForm(TreeNodeCollection treeNodeCollection, object element)
        {
            foreach (TreeNode node in treeNodeCollection)
            {
                if (node.Tag == element)
                {
                    return node;
                }
                if (node.Nodes.Count > 0)
                {
                    TreeNode ret = FindForm(node.Nodes, element);
                    if (ret != null)
                    {
                        return ret;
                    }
                }
            }
            return null;
        }

        private UserControl AddPropertyPanel(SharedObject element)
        {
            return new panelSharedObject();
        }

        private UserControl AddPropertyPanel(IAMFBase element)
        {
            UserControl panel = null;

            switch (element.AmfType)
            {
                case AMFType.AMF0_STRING:
                    panel = new panelAMF0String();
                    break;

                case AMFType.AMF0_BOOLEAN:
                    panel = new panelAMF0Boolean();
                    break;

                case AMFType.AMF0_CLASS:
                    break;

                case AMFType.AMF0_DATE:
                    panel = new panelAMF0Date();
                    break;

                case AMFType.AMF0_NULL:
                case AMFType.AMF0_UNDEFINED:
                    panel = new panelAMF0Null();
                    break;

                case AMFType.AMF0_NUMBER:
                    panel = new panelAMF0Number();
                    break;

                case AMFType.AMF0_XML_STRING:
                case AMFType.AMF3_XML:
                    panel = new panelAMF0Xml();
                    break;

                case AMFType.AMF3_BYTEARRAY:
                    panel = new panelAMF3ByteArray();
                    break;

                case AMFType.AMF3_INT:
                    panel = new panelAMF3Int();
                    break;

                case AMFType.AMF3_REFERENCE:
                case AMFType.AMF0_REFERENCE:
                case AMFType.AMF0_LOST_REFERENCE:
                    panel = new panelAMF3Reference();
                    break;

                case AMFType.AMF3_OBJECT:
                case AMFType.AMF0_OBJECT:
                case AMFType.AMF0_ARRAY:
                case AMFType.AMF0_LIST:
                case AMFType.AMF3_ARRAYLIST:
                default:
                    panel = new panelAMFDefault();
                    Debug.WriteLine("Unknown: " + element.AmfType);
                    break;
            }
            return panel;
        }

        #endregion
    }
}