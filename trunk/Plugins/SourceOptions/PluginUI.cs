using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using ASCompletion.Model;
using ASCompletion.Context;
using ASCompletion.Completion;
using SourceOptions.Resources;

namespace SourceOptions
{
    public partial class PluginUI : Form
    {

        private MemberModel contextMember;
        private ClassModel contextClass;

        public PluginUI()
        {
            InitializeComponent();
            CenterToParent();
        }

        public MemberModel Member
        {
            get { return this.contextMember; }

            set 
            {
                this.contextMember = value;
                this.textBox1.Text = contextMember.Name;
            }
        }

        public ClassModel ContextClass
        {
            set { this.contextClass = value; }
        }

        public string MemberName
        {
            get { return this.textBox1.Text; }
        }

        /// <summary>
        /// Check if the given name alreay exists in the class scope
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool exists(string name)
        {
            foreach(MemberModel item in contextClass.Members)
            {
                if (item.Name == name) return true;
            }
            return false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.okButton.Enabled = this.textBox1.Text != contextMember.Name && this.textBox1.Text.Length > 0 && Regex.Match(this.textBox1.Text, "^[a-zA-Z_$][a-zA-Z0-9_$]+$").Success && exists(this.textBox1.Text) == false;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

    }
}
