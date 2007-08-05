using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;

using ProjectManager.Controls;
using ProjectManager.Projects;

using PluginCore;

using ASClassWizard.Controls.TreeView;

namespace ASClassWizard.Wizards
{
    public partial class PackageBrowser : Form
    {

        List<string> classpathList;
        Project project;

        public PackageBrowser()
        {
            Initialize();
            InitializeComponent();
            CenterToParent();

            this.treeView1.ImageList = Icons.ImageList;
            this.treeView1.BeforeExpand += onBeforeExpandNode;
        }

        public Project Project
        {
            get { return project; }
            set { this.project = value; }
        }

        public String Package
        {
            get { 
                return this.treeView1.SelectedNode != null ? ((SimpleDirectoryNode)this.treeView1.SelectedNode).directoryPath : null;
            }
        }

        private void Initialize()
        {
            classpathList = new List<string>();
        }


        public void AddClassPath(string value)
        {
            classpathList.Add(value);
        }

        private void RefreshTree()
        {
            SimpleDirectoryNode node;

            this.treeView1.BeginUpdate();
            this.treeView1.Nodes.Clear();

            if (classpathList.Count > 0)
            {
                foreach(string cp in classpathList)
                {
                    foreach (string item in Directory.GetDirectories(cp))
                    {
                        if (IsDirectoryExcluded(item) == false)
                        {
                            node = new SimpleDirectoryNode(item, Path.Combine(cp, item));
                            node.ImageIndex = Icons.Folder.Index;
                            node.SelectedImageIndex = Icons.Folder.Index;
                            this.treeView1.Nodes.Add(node);
                        }
                    }
                }
            }
            this.treeView1.EndUpdate();

        }

        private void PackageBrowser_Load(object sender, EventArgs e)
        {
            RefreshTree();
        }

        private void onBeforeExpandNode(Object sender, TreeViewCancelEventArgs e)
        {
            SimpleDirectoryNode newNode;

            if (e.Node is SimpleDirectoryNode)
            {
                SimpleDirectoryNode node = e.Node as SimpleDirectoryNode;
                if (node.dirty)
                {
                    node.dirty = false;
                    e.Node.Nodes.Clear();

                    foreach (string item in Directory.GetDirectories(node.directoryPath))
                    {
                        if (IsDirectoryExcluded(item) == false)
                        {
                            newNode = new SimpleDirectoryNode(item, Path.Combine(node.directoryPath, item));
                            newNode.ImageIndex = Icons.Folder.Index;
                            newNode.SelectedImageIndex = Icons.Folder.Index;
                            node.Nodes.Add(newNode);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Verify if a given directory is hidden
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected bool IsDirectoryExcluded(string path)
        {
            string dirName = Path.GetFileName(path);
            foreach (string excludedDir in ProjectManager.PluginMain.Settings.ExcludedDirectories)
            {
                if (dirName.ToLower() == excludedDir)
                {
                    return true;
                }
            }
            return Project.IsPathHidden(path) && !Project.ShowHiddenPaths;
        }



        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.treeView1.SelectedNode = null;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

    }
}
