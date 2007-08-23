using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

using ASCompletion.Model;

using ASClassWizard.Wizards;
using ASClassWizard.Resources;


namespace ASClassWizard.Wizards
{
    public partial class ClassBrowser : Form
    {

        private MemberList all;
        private List<GListBox.GListBoxItem> dataProvider;
        private FlagType invalidFlag;
        private FlagType validFlag;

        public MemberList ClassList
        {
            get { return this.all; }
            set { this.all = value; }
        }

        public List<GListBox.GListBoxItem> DataProvider
        {
            get { return this.dataProvider; }
            set { this.dataProvider = value; }
        }

        public FlagType ExcludeFlag
        {
            get { return this.invalidFlag; }
            set { this.invalidFlag = value; }
        }

        public FlagType IncludeFlag
        {
            get { return this.validFlag; }
            set { this.validFlag = value; }
        }

        public string SelectedClass
        {
            get { return this.listView1.SelectedItem != null ? this.listView1.SelectedItem.ToString() : null; }
        }

        public ClassBrowser()
        {
            this.DataProvider = new List<GListBox.GListBoxItem>();
            InitializeComponent();
            InitializeLocalization();
            this.listView1.ImageList =  ASCompletion.Context.ASContext.Panel.TreeIcons;
            CenterToParent();
        }

        private void InitializeLocalization()
        {
            this.button1.Text = LocaleHelper.GetString("Wizard.Button.SelectNone");
            this.button2.Text = LocaleHelper.GetString("Wizard.Button.Ok");
            this.Text = LocaleHelper.GetString("Wizard.Label.OpenType");
        }

        private void ClassBrowser_Load(object sender, EventArgs e)
        {
            ASClassWizard.Wizards.GListBox.GListBoxItem node;

            this.listView1.BeginUpdate();
            this.listView1.Items.Clear();
            if (this.ClassList != null)
            {
                foreach (MemberModel item in this.ClassList)
                {
                    if (ExcludeFlag > 0) if ((item.Flags & ExcludeFlag) > 0) continue;
                    if (IncludeFlag > 0)
                    {
                        if (!((item.Flags & IncludeFlag) > 0))
                        {
                            continue;
                        }
                    }

                    if (this.listView1.Items.Count > 0 && item.Name == this.listView1.Items[this.listView1.Items.Count - 1].ToString()) continue;

                    node = new ASClassWizard.Wizards.GListBox.GListBoxItem(item.Name, (item.Flags & FlagType.Interface) > 0 ? 6 : 8);
                    this.listView1.Items.Add(node);
                    this.DataProvider.Add(node);
                }
            }
            if (this.listView1.Items.Count > 0)
            {
                this.listView1.SelectedIndex = 0;
            }
            this.listView1.EndUpdate();
        }

        /// <summary>
        /// Filder the list
        /// </summary>
        private void textBox1_TextChanged( Object sender, EventArgs e)
        {
            string text = this.textBox1.Text;

            this.listView1.BeginUpdate();
            this.listView1.Items.Clear();

            List<GListBox.GListBoxItem> result = this.DataProvider.FindAll( FindAllItems );
            this.listView1.Items.AddRange(result.ToArray());

            this.listView1.EndUpdate();
        }


        /// <summary>
        /// Filder the results
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool FindAllItems( GListBox.GListBoxItem item )
        {
            return item.Text.ToLower().IndexOf(this.textBox1.Text.ToLower()) > -1;
        }


        /// <summary>
        /// Select None button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            this.listView1.SelectedItem = null;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }


        /// <summary>
        /// Ok button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void listView1_DoubleClick( object sender, EventArgs e )
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
