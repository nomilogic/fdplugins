using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;

using PluginCore;
using ProjectManager.Projects;

using ASClassWizard.Wizards;
using ASClassWizard.Resources;

using ASCompletion.Context;
using ASCompletion.Model;

using AS3Context;
using AS2Context;
using System.Reflection;


namespace ASClassWizard.Wizards
{
    public partial class AS3ClassWizard : Form
    {
        private string directoryPath;
        private Project project;
        public const string REG_IDENTIFIER = "^[a-zA-Z_$][a-zA-Z0-9_$]+$";

        public AS3ClassWizard()
        {
            InitializeComponent();
            CenterToParent();
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
        }

        public String StartupPackage
        {
            set { textBox1.Text = value; }
        }

        public string Directory
        {
            get { return this.directoryPath; }
            set { this.directoryPath = value; }
        }

        public Project Project
        {
            get { return project; }
            set { 
                this.project = value;
                if (project.Language == "as2")
                {
                    this.radioButton1.Enabled = false;
                    this.radioButton2.Enabled = false;
                    this.checkBox2.Enabled = false;
                    this.label7.Text = LocaleHelper.GetString("Wizard.Label.NewAs2Class");
                }
                else
                {
                    this.label7.Text = LocaleHelper.GetString("Wizard.Label.NewAs3Class");
                }
            }
        }

        private void ValidateClass()
        {
            string errorMessage = "";

            if (getClassName() == "" || Regex.Match(getClassName(), REG_IDENTIFIER, RegexOptions.Singleline).Success == false)
                errorMessage = LocaleHelper.GetString("Wizard.Error.InvalidClassName");


            if (errorMessage != "")
            {
                okButton.Enabled = false;
                errorIcon.Visible = true;
            }
            else
            {
                okButton.Enabled = true;
                errorIcon.Visible = false;
            }

            this.errorLabel.Text = errorMessage;
        }

        #region EventHandlers

        /// <summary>
        /// Browse project packages
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {

            PackageBrowser browser = new PackageBrowser();
            browser.Project = this.Project;

            foreach (string item in Project.AbsoluteClasspaths)
                browser.AddClassPath(item);

            if (browser.ShowDialog(this) == DialogResult.OK)
            {
                if (browser.Package != null)
                {
                    string classpath = this.Project.AbsoluteClasspaths.GetClosestParent(browser.Package);
                    string package = Path.GetDirectoryName(ProjectPaths.GetRelativePath(classpath, Path.Combine(browser.Package, "foo")));
                    if (package != null)
                    {
                        directoryPath = browser.Package;
                        package = package.Replace(Path.DirectorySeparatorChar, '.');
                        this.textBox1.Text = package;
                    }
                }
                else
                {
                    this.directoryPath = browser.Project.Directory;
                    this.textBox1.Text = "";
                }
            }
        }

        private void AS3ClassWizard_Load(object sender, EventArgs e)
        {
            this.textBox2.Select();
            this.ValidateClass();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ClassBrowser browser = new ClassBrowser();
            IASContext context   = ASContext.GetLanguageContext(PluginBase.CurrentProject.Language);
            browser.ClassList    = context.GetAllProjectClasses();
            browser.ExcludeFlag  = FlagType.Interface;
            browser.IncludeFlag  = FlagType.Class;
            if (browser.ShowDialog(this) == DialogResult.OK)
            {
                this.textBox3.Text = browser.SelectedClass;
            }
        }

        /// <summary>
        /// Added interface
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            ClassBrowser browser = new ClassBrowser();
            browser.IncludeFlag = FlagType.Interface;
            IASContext context = ASContext.GetLanguageContext(PluginBase.CurrentProject.Language);
            MemberList known = context.GetAllProjectClasses();
            known.Merge(ASContext.Context.GetVisibleExternalElements(true));
            browser.ClassList = known;

            if (browser.ShowDialog(this) == DialogResult.OK)
            {
                if (browser.SelectedClass != null)
                {
                    foreach (string item in this.listBox1.Items)
                    {
                        if (item == browser.SelectedClass) return;
                    }
                    this.listBox1.Items.Add(browser.SelectedClass);
                }
            }
            this.button6.Enabled = this.listBox1.Items.Count > 0;
            this.listBox1.SelectedIndex = this.listBox1.Items.Count - 1;
            this.checkBox4.Enabled = this.listBox1.Items.Count > 0;
            ValidateClass();
        }

        /// <summary>
        /// Remove interface
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedItem != null)
            {
                this.listBox1.Items.Remove(this.listBox1.SelectedItem);
            }

            if (this.listBox1.Items.Count > 0)
            {
                this.listBox1.SelectedIndex = this.listBox1.Items.Count - 1;
            }

            this.button6.Enabled   = this.listBox1.Items.Count > 0;
            this.checkBox4.Enabled = this.listBox1.Items.Count > 0;
            ValidateClass();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            ValidateClass();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            ValidateClass();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            this.checkBox3.Enabled = this.textBox3.Text != "";
            ValidateClass();
        }

        #endregion

        public static Image GetResource( string resourceID )
        {
            resourceID = "ASClassWizard." + resourceID;
            Assembly assembly = Assembly.GetExecutingAssembly();
            Image image = new Bitmap(assembly.GetManifestResourceStream(resourceID));
            return image;
        }

        #region user_options

        public string getPackage()
        {
            return this.textBox1.Text;
        }

        public string getClassName()
        {
            return this.textBox2.Text;
        }

        public bool isDynamic()
        {
            return this.checkBox1.Checked;
        }

        public bool isFinal()
        {
            return this.checkBox2.Checked;
        }

        public bool isPublic()
        {
            return this.radioButton1.Checked;
        }

        public string getSuperClass()
        {
            return this.textBox3.Text;
        }

        public List<string> getInterfaces()
        {
            List<string> _interfaces = new List<string>(this.listBox1.Items.Count);
            foreach (string item in this.listBox1.Items)
            {
                _interfaces.Add(item);
            }
            return _interfaces;
        }

        public bool hasInterfaces()
        {
            return this.listBox1.Items.Count > 0;
        }

        public bool getGenerateConstructor()
        {
            return this.checkBox3.Checked;
        }

        public bool getGenerateInheritedMethods()
        {
            return this.checkBox4.Checked;
        }

        #endregion


    }
}
