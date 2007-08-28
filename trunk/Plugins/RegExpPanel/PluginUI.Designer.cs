namespace RegExpPanel
{
    partial class PluginUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageMatch = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.searchTestButton = new System.Windows.Forms.Button();
            this.matchTestButton = new System.Windows.Forms.Button();
            this.matchPatternTextBox = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cb_findAll = new System.Windows.Forms.CheckBox();
            this.cb_dotAll = new System.Windows.Forms.CheckBox();
            this.cb_verbose = new System.Windows.Forms.CheckBox();
            this.cb_multiline = new System.Windows.Forms.CheckBox();
            this.cb_ignoreCase = new System.Windows.Forms.CheckBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.matchOpenButton = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.matchTabResult = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.matchResultTree = new Aga.Controls.Tree.TreeViewAdv();
            this.treeColumn1 = new Aga.Controls.Tree.TreeColumn();
            this.treeColumn2 = new Aga.Controls.Tree.TreeColumn();
            this.treeColumn3 = new Aga.Controls.Tree.TreeColumn();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.matchAsTextBox = new System.Windows.Forms.TextBox();
            this.tabPageReplace = new System.Windows.Forms.TabPage();
            this.splitContainer5 = new System.Windows.Forms.SplitContainer();
            this.label4 = new System.Windows.Forms.Label();
            this.replaceReplaceTextBox = new System.Windows.Forms.RichTextBox();
            this.replaceTestButton = new System.Windows.Forms.Button();
            this.replacePatternTextBox = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rcb_dotAll = new System.Windows.Forms.CheckBox();
            this.rcb_verbose = new System.Windows.Forms.CheckBox();
            this.rcb_multiline = new System.Windows.Forms.CheckBox();
            this.rcb_ignoreCase = new System.Windows.Forms.CheckBox();
            this.rcb_findAll = new System.Windows.Forms.CheckBox();
            this.splitContainer6 = new System.Windows.Forms.SplitContainer();
            this.replaceOpenButton = new System.Windows.Forms.Button();
            this.replacePanel1 = new System.Windows.Forms.Panel();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.replacePanel2 = new System.Windows.Forms.Panel();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.replaceAsTextBox = new System.Windows.Forms.TextBox();
            this.nodeTextBox1 = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeTextBox2 = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeTextBox3 = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this._match_group = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this._match_span = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this._match_text = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.nodeTextBox4 = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeTextBox5 = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeTextBox6 = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.libraryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actionscriptRegExpClassToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.regExpTutorialsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.libraryItem1ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1.SuspendLayout();
            this.tabPageMatch.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.matchTabResult.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPageReplace.SuspendLayout();
            this.splitContainer5.Panel1.SuspendLayout();
            this.splitContainer5.Panel2.SuspendLayout();
            this.splitContainer5.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.splitContainer6.Panel1.SuspendLayout();
            this.splitContainer6.Panel2.SuspendLayout();
            this.splitContainer6.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPageMatch);
            this.tabControl1.Controls.Add(this.tabPageReplace);
            this.tabControl1.Location = new System.Drawing.Point(0, 27);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(902, 627);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPageMatch
            // 
            this.tabPageMatch.Controls.Add(this.splitContainer1);
            this.tabPageMatch.Location = new System.Drawing.Point(4, 22);
            this.tabPageMatch.Name = "tabPageMatch";
            this.tabPageMatch.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMatch.Size = new System.Drawing.Size(894, 601);
            this.tabPageMatch.TabIndex = 0;
            this.tabPageMatch.Text = "Match";
            this.tabPageMatch.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.searchTestButton);
            this.splitContainer1.Panel1.Controls.Add(this.matchTestButton);
            this.splitContainer1.Panel1.Controls.Add(this.matchPatternTextBox);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(888, 595);
            this.splitContainer1.SplitterDistance = 155;
            this.splitContainer1.TabIndex = 0;
            // 
            // searchTestButton
            // 
            this.searchTestButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.searchTestButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.searchTestButton.Location = new System.Drawing.Point(663, 130);
            this.searchTestButton.Margin = new System.Windows.Forms.Padding(1);
            this.searchTestButton.Name = "searchTestButton";
            this.searchTestButton.Size = new System.Drawing.Size(75, 23);
            this.searchTestButton.TabIndex = 7;
            this.searchTestButton.Text = "&Search";
            this.searchTestButton.UseVisualStyleBackColor = true;
            this.searchTestButton.Click += new System.EventHandler(this.searchTestButton_Click);
            // 
            // matchTestButton
            // 
            this.matchTestButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.matchTestButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.matchTestButton.Location = new System.Drawing.Point(588, 130);
            this.matchTestButton.Margin = new System.Windows.Forms.Padding(1);
            this.matchTestButton.Name = "matchTestButton";
            this.matchTestButton.Size = new System.Drawing.Size(73, 23);
            this.matchTestButton.TabIndex = 6;
            this.matchTestButton.Text = "&Match";
            this.matchTestButton.UseVisualStyleBackColor = true;
            this.matchTestButton.Click += new System.EventHandler(this.matchTestButton_Click);
            // 
            // matchPatternTextBox
            // 
            this.matchPatternTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.matchPatternTextBox.Location = new System.Drawing.Point(6, 22);
            this.matchPatternTextBox.Name = "matchPatternTextBox";
            this.matchPatternTextBox.Size = new System.Drawing.Size(732, 104);
            this.matchPatternTextBox.TabIndex = 5;
            this.matchPatternTextBox.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(2, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(135, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Regular Expression Pattern";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.cb_findAll);
            this.groupBox1.Controls.Add(this.cb_dotAll);
            this.groupBox1.Controls.Add(this.cb_verbose);
            this.groupBox1.Controls.Add(this.cb_multiline);
            this.groupBox1.Controls.Add(this.cb_ignoreCase);
            this.groupBox1.Location = new System.Drawing.Point(744, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(139, 150);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Modifiers";
            // 
            // cb_findAll
            // 
            this.cb_findAll.AutoSize = true;
            this.cb_findAll.Location = new System.Drawing.Point(6, 95);
            this.cb_findAll.Margin = new System.Windows.Forms.Padding(1);
            this.cb_findAll.Name = "cb_findAll";
            this.cb_findAll.Size = new System.Drawing.Size(75, 17);
            this.cb_findAll.TabIndex = 4;
            this.cb_findAll.Text = "&Find All (g)";
            this.cb_findAll.UseVisualStyleBackColor = true;
            // 
            // cb_dotAll
            // 
            this.cb_dotAll.AutoSize = true;
            this.cb_dotAll.Location = new System.Drawing.Point(6, 76);
            this.cb_dotAll.Margin = new System.Windows.Forms.Padding(1);
            this.cb_dotAll.Name = "cb_dotAll";
            this.cb_dotAll.Size = new System.Drawing.Size(71, 17);
            this.cb_dotAll.TabIndex = 3;
            this.cb_dotAll.Text = "&Dot All (s)";
            this.cb_dotAll.UseVisualStyleBackColor = true;
            // 
            // cb_verbose
            // 
            this.cb_verbose.AutoSize = true;
            this.cb_verbose.Location = new System.Drawing.Point(6, 57);
            this.cb_verbose.Margin = new System.Windows.Forms.Padding(1);
            this.cb_verbose.Name = "cb_verbose";
            this.cb_verbose.Size = new System.Drawing.Size(79, 17);
            this.cb_verbose.TabIndex = 2;
            this.cb_verbose.Text = "&Verbose (x)";
            this.cb_verbose.UseVisualStyleBackColor = true;
            // 
            // cb_multiline
            // 
            this.cb_multiline.AutoSize = true;
            this.cb_multiline.Location = new System.Drawing.Point(6, 38);
            this.cb_multiline.Margin = new System.Windows.Forms.Padding(1);
            this.cb_multiline.Name = "cb_multiline";
            this.cb_multiline.Size = new System.Drawing.Size(81, 17);
            this.cb_multiline.TabIndex = 1;
            this.cb_multiline.Text = "&Multiline (m)";
            this.cb_multiline.UseVisualStyleBackColor = true;
            // 
            // cb_ignoreCase
            // 
            this.cb_ignoreCase.AutoSize = true;
            this.cb_ignoreCase.Location = new System.Drawing.Point(6, 19);
            this.cb_ignoreCase.Margin = new System.Windows.Forms.Padding(1);
            this.cb_ignoreCase.Name = "cb_ignoreCase";
            this.cb_ignoreCase.Size = new System.Drawing.Size(94, 17);
            this.cb_ignoreCase.TabIndex = 0;
            this.cb_ignoreCase.Text = "&Ignore Case (i)";
            this.cb_ignoreCase.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.matchOpenButton);
            this.splitContainer2.Panel1.Controls.Add(this.panel2);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.matchTabResult);
            this.splitContainer2.Size = new System.Drawing.Size(888, 436);
            this.splitContainer2.SplitterDistance = 233;
            this.splitContainer2.TabIndex = 0;
            // 
            // matchOpenButton
            // 
            this.matchOpenButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.matchOpenButton.Location = new System.Drawing.Point(858, 4);
            this.matchOpenButton.Margin = new System.Windows.Forms.Padding(1);
            this.matchOpenButton.Name = "matchOpenButton";
            this.matchOpenButton.Size = new System.Drawing.Size(26, 26);
            this.matchOpenButton.TabIndex = 1;
            this.matchOpenButton.UseVisualStyleBackColor = true;
            this.matchOpenButton.Click += new System.EventHandler(this.openButton_Click);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Location = new System.Drawing.Point(5, 5);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(849, 235);
            this.panel2.TabIndex = 0;
            // 
            // matchTabResult
            // 
            this.matchTabResult.Controls.Add(this.tabPage3);
            this.matchTabResult.Controls.Add(this.tabPage4);
            this.matchTabResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.matchTabResult.Location = new System.Drawing.Point(0, 0);
            this.matchTabResult.Name = "matchTabResult";
            this.matchTabResult.SelectedIndex = 0;
            this.matchTabResult.Size = new System.Drawing.Size(888, 199);
            this.matchTabResult.TabIndex = 1;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.matchResultTree);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(880, 173);
            this.tabPage3.TabIndex = 0;
            this.tabPage3.Text = "Result";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // matchResultTree
            // 
            this.matchResultTree.BackColor = System.Drawing.SystemColors.Window;
            this.matchResultTree.Columns.Add(this.treeColumn1);
            this.matchResultTree.Columns.Add(this.treeColumn2);
            this.matchResultTree.Columns.Add(this.treeColumn3);
            this.matchResultTree.DefaultToolTipProvider = null;
            this.matchResultTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.matchResultTree.DragDropMarkColor = System.Drawing.Color.Black;
            this.matchResultTree.FullRowSelect = true;
            this.matchResultTree.GridLineStyle = ((Aga.Controls.Tree.GridLineStyle)((Aga.Controls.Tree.GridLineStyle.Horizontal | Aga.Controls.Tree.GridLineStyle.Vertical)));
            this.matchResultTree.LineColor = System.Drawing.SystemColors.ControlDark;
            this.matchResultTree.Location = new System.Drawing.Point(3, 3);
            this.matchResultTree.Model = null;
            this.matchResultTree.Name = "matchResultTree";
            this.matchResultTree.NodeControls.Add(this.nodeTextBox4);
            this.matchResultTree.NodeControls.Add(this.nodeTextBox5);
            this.matchResultTree.NodeControls.Add(this.nodeTextBox6);
            this.matchResultTree.SelectedNode = null;
            this.matchResultTree.Size = new System.Drawing.Size(874, 167);
            this.matchResultTree.TabIndex = 0;
            this.matchResultTree.UseColumns = true;
            this.matchResultTree.SelectionChanged += new System.EventHandler(this.matchResultTree_SelectionChanged);
            // 
            // treeColumn1
            // 
            this.treeColumn1.Header = "Group";
            this.treeColumn1.SortOrder = System.Windows.Forms.SortOrder.None;
            this.treeColumn1.TooltipText = null;
            this.treeColumn1.Width = 150;
            // 
            // treeColumn2
            // 
            this.treeColumn2.Header = "Span";
            this.treeColumn2.SortOrder = System.Windows.Forms.SortOrder.None;
            this.treeColumn2.TooltipText = null;
            this.treeColumn2.Width = 150;
            // 
            // treeColumn3
            // 
            this.treeColumn3.Header = "Text";
            this.treeColumn3.SortOrder = System.Windows.Forms.SortOrder.None;
            this.treeColumn3.TooltipText = null;
            this.treeColumn3.Width = 250;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.matchAsTextBox);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(870, 187);
            this.tabPage4.TabIndex = 1;
            this.tabPage4.Text = "Actionscript";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // matchAsTextBox
            // 
            this.matchAsTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.matchAsTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.matchAsTextBox.Location = new System.Drawing.Point(3, 3);
            this.matchAsTextBox.Multiline = true;
            this.matchAsTextBox.Name = "matchAsTextBox";
            this.matchAsTextBox.Size = new System.Drawing.Size(864, 181);
            this.matchAsTextBox.TabIndex = 0;
            // 
            // tabPageReplace
            // 
            this.tabPageReplace.Controls.Add(this.splitContainer5);
            this.tabPageReplace.Location = new System.Drawing.Point(4, 22);
            this.tabPageReplace.Name = "tabPageReplace";
            this.tabPageReplace.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageReplace.Size = new System.Drawing.Size(884, 618);
            this.tabPageReplace.TabIndex = 3;
            this.tabPageReplace.Text = "Replace";
            this.tabPageReplace.UseVisualStyleBackColor = true;
            // 
            // splitContainer5
            // 
            this.splitContainer5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer5.Location = new System.Drawing.Point(3, 3);
            this.splitContainer5.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer5.Name = "splitContainer5";
            this.splitContainer5.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer5.Panel1
            // 
            this.splitContainer5.Panel1.Controls.Add(this.label4);
            this.splitContainer5.Panel1.Controls.Add(this.replaceReplaceTextBox);
            this.splitContainer5.Panel1.Controls.Add(this.replaceTestButton);
            this.splitContainer5.Panel1.Controls.Add(this.replacePatternTextBox);
            this.splitContainer5.Panel1.Controls.Add(this.label3);
            this.splitContainer5.Panel1.Controls.Add(this.groupBox3);
            this.splitContainer5.Panel1.Padding = new System.Windows.Forms.Padding(2);
            // 
            // splitContainer5.Panel2
            // 
            this.splitContainer5.Panel2.Controls.Add(this.splitContainer6);
            this.splitContainer5.Size = new System.Drawing.Size(878, 612);
            this.splitContainer5.SplitterDistance = 159;
            this.splitContainer5.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(370, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(107, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Replacement Pattern";
            // 
            // replaceReplaceTextBox
            // 
            this.replaceReplaceTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.replaceReplaceTextBox.Location = new System.Drawing.Point(373, 22);
            this.replaceReplaceTextBox.Name = "replaceReplaceTextBox";
            this.replaceReplaceTextBox.Size = new System.Drawing.Size(355, 102);
            this.replaceReplaceTextBox.TabIndex = 7;
            this.replaceReplaceTextBox.Text = "";
            // 
            // replaceTestButton
            // 
            this.replaceTestButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.replaceTestButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.replaceTestButton.Location = new System.Drawing.Point(655, 128);
            this.replaceTestButton.Margin = new System.Windows.Forms.Padding(1);
            this.replaceTestButton.Name = "replaceTestButton";
            this.replaceTestButton.Size = new System.Drawing.Size(73, 23);
            this.replaceTestButton.TabIndex = 6;
            this.replaceTestButton.Text = "&Replace";
            this.replaceTestButton.UseVisualStyleBackColor = true;
            this.replaceTestButton.Click += new System.EventHandler(this.replaceTestButton_Click);
            // 
            // replacePatternTextBox
            // 
            this.replacePatternTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.replacePatternTextBox.Location = new System.Drawing.Point(6, 22);
            this.replacePatternTextBox.Name = "replacePatternTextBox";
            this.replacePatternTextBox.Size = new System.Drawing.Size(361, 102);
            this.replacePatternTextBox.TabIndex = 5;
            this.replacePatternTextBox.Text = "";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(2, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(135, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Regular Expression Pattern";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.rcb_dotAll);
            this.groupBox3.Controls.Add(this.rcb_verbose);
            this.groupBox3.Controls.Add(this.rcb_multiline);
            this.groupBox3.Controls.Add(this.rcb_ignoreCase);
            this.groupBox3.Controls.Add(this.rcb_findAll);
            this.groupBox3.Location = new System.Drawing.Point(734, 5);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(139, 149);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Modifiers";
            // 
            // rcb_dotAll
            // 
            this.rcb_dotAll.AutoSize = true;
            this.rcb_dotAll.Location = new System.Drawing.Point(6, 76);
            this.rcb_dotAll.Margin = new System.Windows.Forms.Padding(1);
            this.rcb_dotAll.Name = "rcb_dotAll";
            this.rcb_dotAll.Size = new System.Drawing.Size(71, 17);
            this.rcb_dotAll.TabIndex = 8;
            this.rcb_dotAll.Text = "&Dot All (s)";
            this.rcb_dotAll.UseVisualStyleBackColor = true;
            // 
            // rcb_verbose
            // 
            this.rcb_verbose.AutoSize = true;
            this.rcb_verbose.Location = new System.Drawing.Point(6, 57);
            this.rcb_verbose.Margin = new System.Windows.Forms.Padding(1);
            this.rcb_verbose.Name = "rcb_verbose";
            this.rcb_verbose.Size = new System.Drawing.Size(79, 17);
            this.rcb_verbose.TabIndex = 7;
            this.rcb_verbose.Text = "&Verbose (x)";
            this.rcb_verbose.UseVisualStyleBackColor = true;
            // 
            // rcb_multiline
            // 
            this.rcb_multiline.AutoSize = true;
            this.rcb_multiline.Location = new System.Drawing.Point(6, 38);
            this.rcb_multiline.Margin = new System.Windows.Forms.Padding(1);
            this.rcb_multiline.Name = "rcb_multiline";
            this.rcb_multiline.Size = new System.Drawing.Size(81, 17);
            this.rcb_multiline.TabIndex = 6;
            this.rcb_multiline.Text = "&Multiline (m)";
            this.rcb_multiline.UseVisualStyleBackColor = true;
            // 
            // rcb_ignoreCase
            // 
            this.rcb_ignoreCase.AutoSize = true;
            this.rcb_ignoreCase.Location = new System.Drawing.Point(6, 19);
            this.rcb_ignoreCase.Margin = new System.Windows.Forms.Padding(1);
            this.rcb_ignoreCase.Name = "rcb_ignoreCase";
            this.rcb_ignoreCase.Size = new System.Drawing.Size(94, 17);
            this.rcb_ignoreCase.TabIndex = 5;
            this.rcb_ignoreCase.Text = "&Ignore Case (i)";
            this.rcb_ignoreCase.UseVisualStyleBackColor = true;
            // 
            // rcb_findAll
            // 
            this.rcb_findAll.AutoSize = true;
            this.rcb_findAll.Location = new System.Drawing.Point(6, 95);
            this.rcb_findAll.Margin = new System.Windows.Forms.Padding(1);
            this.rcb_findAll.Name = "rcb_findAll";
            this.rcb_findAll.Size = new System.Drawing.Size(75, 17);
            this.rcb_findAll.TabIndex = 9;
            this.rcb_findAll.Text = "&Find All (g)";
            this.rcb_findAll.UseVisualStyleBackColor = true;
            // 
            // splitContainer6
            // 
            this.splitContainer6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer6.Location = new System.Drawing.Point(0, 0);
            this.splitContainer6.Name = "splitContainer6";
            this.splitContainer6.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer6.Panel1
            // 
            this.splitContainer6.Panel1.Controls.Add(this.replaceOpenButton);
            this.splitContainer6.Panel1.Controls.Add(this.replacePanel1);
            // 
            // splitContainer6.Panel2
            // 
            this.splitContainer6.Panel2.Controls.Add(this.tabControl2);
            this.splitContainer6.Size = new System.Drawing.Size(878, 449);
            this.splitContainer6.SplitterDistance = 205;
            this.splitContainer6.TabIndex = 0;
            // 
            // replaceOpenButton
            // 
            this.replaceOpenButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.replaceOpenButton.Location = new System.Drawing.Point(848, 4);
            this.replaceOpenButton.Margin = new System.Windows.Forms.Padding(1);
            this.replaceOpenButton.Name = "replaceOpenButton";
            this.replaceOpenButton.Size = new System.Drawing.Size(26, 26);
            this.replaceOpenButton.TabIndex = 3;
            this.replaceOpenButton.UseVisualStyleBackColor = true;
            this.replaceOpenButton.Click += new System.EventHandler(this.openButton_Click);
            // 
            // replacePanel1
            // 
            this.replacePanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.replacePanel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.replacePanel1.Location = new System.Drawing.Point(5, 5);
            this.replacePanel1.Name = "replacePanel1";
            this.replacePanel1.Size = new System.Drawing.Size(839, 198);
            this.replacePanel1.TabIndex = 2;
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage1);
            this.tabControl2.Controls.Add(this.tabPage2);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Location = new System.Drawing.Point(0, 0);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(878, 240);
            this.tabControl2.TabIndex = 4;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.replacePanel2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(870, 214);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Result";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // replacePanel2
            // 
            this.replacePanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.replacePanel2.Location = new System.Drawing.Point(3, 3);
            this.replacePanel2.Name = "replacePanel2";
            this.replacePanel2.Size = new System.Drawing.Size(864, 208);
            this.replacePanel2.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.replaceAsTextBox);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(876, 226);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Actionscript";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // replaceAsTextBox
            // 
            this.replaceAsTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.replaceAsTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.replaceAsTextBox.Location = new System.Drawing.Point(3, 3);
            this.replaceAsTextBox.Multiline = true;
            this.replaceAsTextBox.Name = "replaceAsTextBox";
            this.replaceAsTextBox.Size = new System.Drawing.Size(870, 220);
            this.replaceAsTextBox.TabIndex = 2;
            // 
            // nodeTextBox1
            // 
            this.nodeTextBox1.DataPropertyName = "Group";
            this.nodeTextBox1.IncrementalSearchEnabled = true;
            this.nodeTextBox1.LeftMargin = 3;
            this.nodeTextBox1.ParentColumn = this.treeColumn1;
            // 
            // nodeTextBox2
            // 
            this.nodeTextBox2.DataPropertyName = "Span";
            this.nodeTextBox2.IncrementalSearchEnabled = true;
            this.nodeTextBox2.LeftMargin = 3;
            this.nodeTextBox2.ParentColumn = this.treeColumn2;
            // 
            // nodeTextBox3
            // 
            this.nodeTextBox3.DataPropertyName = "Text";
            this.nodeTextBox3.IncrementalSearchEnabled = true;
            this.nodeTextBox3.LeftMargin = 3;
            this.nodeTextBox3.ParentColumn = this.treeColumn3;
            // 
            // _match_group
            // 
            this._match_group.DataPropertyName = "Group";
            this._match_group.IncrementalSearchEnabled = true;
            this._match_group.LeftMargin = 3;
            this._match_group.ParentColumn = this.treeColumn1;
            // 
            // _match_span
            // 
            this._match_span.DataPropertyName = "Span";
            this._match_span.IncrementalSearchEnabled = true;
            this._match_span.LeftMargin = 3;
            this._match_span.ParentColumn = this.treeColumn2;
            // 
            // _match_text
            // 
            this._match_text.DataPropertyName = "Text";
            this._match_text.IncrementalSearchEnabled = true;
            this._match_text.LeftMargin = 3;
            this._match_text.ParentColumn = this.treeColumn3;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.RestoreDirectory = true;
            // 
            // nodeTextBox4
            // 
            this.nodeTextBox4.DataPropertyName = "Group";
            this.nodeTextBox4.IncrementalSearchEnabled = true;
            this.nodeTextBox4.LeftMargin = 3;
            this.nodeTextBox4.ParentColumn = this.treeColumn1;
            // 
            // nodeTextBox5
            // 
            this.nodeTextBox5.DataPropertyName = "Span";
            this.nodeTextBox5.IncrementalSearchEnabled = true;
            this.nodeTextBox5.LeftMargin = 3;
            this.nodeTextBox5.ParentColumn = this.treeColumn2;
            // 
            // nodeTextBox6
            // 
            this.nodeTextBox6.DataPropertyName = "Text";
            this.nodeTextBox6.IncrementalSearchEnabled = true;
            this.nodeTextBox6.LeftMargin = 3;
            this.nodeTextBox6.ParentColumn = this.treeColumn3;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 657);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(902, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(42, 17);
            this.toolStripStatusLabel1.Text = "Ready.";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.libraryToolStripMenuItem,
            this.helpItemToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(902, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // libraryToolStripMenuItem
            // 
            this.libraryToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.insertToolStripMenuItem});
            this.libraryToolStripMenuItem.Name = "libraryToolStripMenuItem";
            this.libraryToolStripMenuItem.Size = new System.Drawing.Size(55, 20);
            this.libraryToolStripMenuItem.Text = "&Library";
            // 
            // helpItemToolStripMenuItem
            // 
            this.helpItemToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.actionscriptRegExpClassToolStripMenuItem,
            this.regExpTutorialsToolStripMenuItem});
            this.helpItemToolStripMenuItem.Name = "helpItemToolStripMenuItem";
            this.helpItemToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpItemToolStripMenuItem.Text = "&Help";
            // 
            // actionscriptRegExpClassToolStripMenuItem
            // 
            this.actionscriptRegExpClassToolStripMenuItem.Name = "actionscriptRegExpClassToolStripMenuItem";
            this.actionscriptRegExpClassToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.actionscriptRegExpClassToolStripMenuItem.Text = "Actionscript RegExp class";
            this.actionscriptRegExpClassToolStripMenuItem.Click += new System.EventHandler(this.actionscriptRegExpClassToolStripMenuItem_Click);
            // 
            // regExpTutorialsToolStripMenuItem
            // 
            this.regExpTutorialsToolStripMenuItem.Name = "regExpTutorialsToolStripMenuItem";
            this.regExpTutorialsToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.regExpTutorialsToolStripMenuItem.Text = "RegExp Tutorials";
            this.regExpTutorialsToolStripMenuItem.Click += new System.EventHandler(this.regExpTutorialsToolStripMenuItem_Click);
            // 
            // insertToolStripMenuItem
            // 
            this.insertToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.libraryItem1ToolStripMenuItem1});
            this.insertToolStripMenuItem.Name = "insertToolStripMenuItem";
            this.insertToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.insertToolStripMenuItem.Text = "Insert";
            // 
            // libraryItem1ToolStripMenuItem1
            // 
            this.libraryItem1ToolStripMenuItem1.Name = "libraryItem1ToolStripMenuItem1";
            this.libraryItem1ToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.libraryItem1ToolStripMenuItem1.Text = "Library Item 1";
            // 
            // PluginUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(902, 679);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tabControl1);
            this.Name = "PluginUI";
            this.Text = "PluginUI";
            this.Load += new System.EventHandler(this.PluginUI_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PluginUI_FormClosing);
            this.tabControl1.ResumeLayout(false);
            this.tabPageMatch.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.matchTabResult.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.tabPageReplace.ResumeLayout(false);
            this.splitContainer5.Panel1.ResumeLayout(false);
            this.splitContainer5.Panel1.PerformLayout();
            this.splitContainer5.Panel2.ResumeLayout(false);
            this.splitContainer5.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.splitContainer6.Panel1.ResumeLayout(false);
            this.splitContainer6.Panel2.ResumeLayout(false);
            this.splitContainer6.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageMatch;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox cb_findAll;
        private System.Windows.Forms.CheckBox cb_dotAll;
        private System.Windows.Forms.CheckBox cb_verbose;
        private System.Windows.Forms.CheckBox cb_multiline;
        private System.Windows.Forms.CheckBox cb_ignoreCase;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button matchOpenButton;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RichTextBox matchPatternTextBox;
        private Aga.Controls.Tree.TreeViewAdv matchResultTree;
        private Aga.Controls.Tree.TreeColumn treeColumn1;
        private Aga.Controls.Tree.TreeColumn treeColumn2;
        private Aga.Controls.Tree.TreeColumn treeColumn3;
        private Aga.Controls.Tree.NodeControls.NodeTextBox _match_group;
        private Aga.Controls.Tree.NodeControls.NodeTextBox _match_span;
        private Aga.Controls.Tree.NodeControls.NodeTextBox _match_text;
        private System.Windows.Forms.Button matchTestButton;
        private System.Windows.Forms.TabControl matchTabResult;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TextBox matchAsTextBox;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeTextBox1;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeTextBox2;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeTextBox3;
        private System.Windows.Forms.TabPage tabPageReplace;
        private System.Windows.Forms.SplitContainer splitContainer5;
        private System.Windows.Forms.Button replaceTestButton;
        private System.Windows.Forms.RichTextBox replacePatternTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.SplitContainer splitContainer6;
        private System.Windows.Forms.CheckBox rcb_dotAll;
        private System.Windows.Forms.CheckBox rcb_verbose;
        private System.Windows.Forms.CheckBox rcb_multiline;
        private System.Windows.Forms.CheckBox rcb_ignoreCase;
        private System.Windows.Forms.CheckBox rcb_findAll;
        private System.Windows.Forms.Button replaceOpenButton;
        private System.Windows.Forms.Panel replacePanel1;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Panel replacePanel2;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox replaceAsTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RichTextBox replaceReplaceTextBox;
        private System.Windows.Forms.Button searchTestButton;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeTextBox4;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeTextBox5;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeTextBox6;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem libraryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem libraryItem1ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem helpItemToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem actionscriptRegExpClassToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem regExpTutorialsToolStripMenuItem;
    }
}