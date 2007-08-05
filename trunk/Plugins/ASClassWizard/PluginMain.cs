using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections;

using PluginCore.Localization;
using PluginCore.Utilities;
using PluginCore.Managers;
using PluginCore.Helpers;
using PluginCore;

using ProjectManager.Projects;
using ProjectManager.Projects.AS3;
using ProjectManager.Projects.AS2;

using ASCompletion.Model;
using ASCompletion.Context;

using ASClassWizard.Resources;
using ASClassWizard.Wizards;

using FlashDevelop;

namespace ASClassWizard
{
	public class PluginMain : IPlugin
	{
        private String pluginName = "ASClassWizard";
        private String pluginGuid = "a2c159c1-7d21-4483-aeb1-38d9fdc4c7f3";
        private String pluginHelp = "www.sephiroth.it";
        private String pluginDesc = "Actionscript class wizard for the new FlashDevelop 3.";
        private String pluginAuth = "Alessandro Crugnola";
        private String settingFilename;
        private Settings settingObject;

        string lastFileFromTemplate;
        AS3ClassOptions lastFileOptions;

        public static IMainForm MainForm { get { return PluginBase.MainForm; } }

	    #region Required Properties

        /// <summary>
        /// Name of the plugin
        /// </summary> 
        public String Name
		{
			get { return this.pluginName; }
		}

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public String Guid
		{
			get { return this.pluginGuid; }
		}

        /// <summary>
        /// Author of the plugin
        /// </summary> 
        public String Author
		{
			get { return this.pluginAuth; }
		}

        /// <summary>
        /// Description of the plugin
        /// </summary> 
        public String Description
		{
			get { return this.pluginDesc; }
		}

        /// <summary>
        /// Web address for help
        /// </summary> 
        public String Help
		{
			get { return this.pluginHelp; }
		}

        /// <summary>
        /// Object that contains the settings
        /// </summary>
        [Browsable(false)]
        public Object Settings
        {
            get { return null; }
        }
		
		#endregion
		
		#region Required Methods
		
		/// <summary>
		/// Initializes the plugin
		/// </summary>
		public void Initialize()
		{
            this.InitBasics();
            this.LoadSettings();
            this.AddEventHandlers();
            this.InitLocalization();
        }
		
		/// <summary>
		/// Disposes the plugin
		/// </summary>
		public void Dispose()
		{
            this.SaveSettings();
		}
		
		/// <summary>
		/// Handles the incoming events
		/// </summary>
		public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority prority)
		{
            Project project;

            switch (e.Type)
            {
                case EventType.Command:
                    if (e is DataEvent)
                    {
                        DataEvent evt = (DataEvent)e;
                        if (evt.Action == "ProjectManager.CreateNewFile")
                        {
                            Hashtable table = evt.Data as Hashtable;
                            project = PluginBase.CurrentProject as Project;
                            if ((project.Language == "as3" || project.Language == "as2") && Path.GetFileName(table["templatePath"] as String).Equals("Class.as.fdt"))
                            {
                                evt.Handled = true;
                                NewASClassWizard(table["inDirectory"] as String);
                            }
                        }
                    }
                    break;

                case EventType.ProcessArgs:
                    TextEvent te = e as TextEvent;
                    project = PluginBase.CurrentProject as Project;
                    if (project != null && (project.Language == "as3" || project.Language == "as2"))
                    {
                        te.Handled = true;
                        BuildEventVars vars = new BuildEventVars(project);
                        foreach (BuildEventInfo info in vars.GetVars())
                            te.Value = te.Value.Replace("$(" + info.Name + ")", info.Value);

                        te.Value = ProcessArgs(project, te.Value);

                    }

                    break;
            }
		}
		
		#endregion

        #region Custom Methods
       
        /// <summary>
        /// Initializes important variables
        /// </summary>
        public void InitBasics()
        {
            String dataPath = Path.Combine(PathHelper.DataDir, "ASClassWizard");
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");
        }

        /// <summary>
        /// Adds the required event handlers
        /// </summary> 
        public void AddEventHandlers()
        {
            EventManager.AddEventHandler(this, EventType.Command | EventType.ProcessArgs);
        }

        /// <summary>
        /// Initializes the localization of the plugin
        /// </summary>
        public void InitLocalization()
        {
            LocaleVersion locale = PluginBase.MainForm.Settings.LocaleVersion;
            switch (locale)
            {
                default : 
                    LocaleHelper.Initialize(LocaleVersion.en_US);
                    break;
            }
        }


        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        public void LoadSettings()
        {
            this.settingObject = new Settings();
            if (!File.Exists(this.settingFilename)) this.SaveSettings();
            else
            {
                Object obj = ObjectSerializer.Deserialize(this.settingFilename, this.settingObject);
                this.settingObject = (Settings)obj;
            }
        }

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        public void SaveSettings()
        {
            ObjectSerializer.Serialize(this.settingFilename, this.settingObject);
        }

        /// <summary>
        /// Display the ActionScript class wizard dialog
        /// </summary>
        /// <param name="inDirectory"></param>
        private void NewASClassWizard(String inDirectory)
        {
            Project project = PluginBase.CurrentProject as Project;

            String classpath = project.AbsoluteClasspaths.GetClosestParent(inDirectory);
            String package;

            try
            {
                package = Path.GetDirectoryName(ProjectPaths.GetRelativePath(classpath, Path.Combine(inDirectory, "foo")));
            }
            catch (System.NullReferenceException)
            {
                package = "";
            }

            AS3ClassWizard dialog = new AS3ClassWizard();
            dialog.Project        = project;
            dialog.Directory      = inDirectory;

            if (package != null)
            {
                package = package.Replace(Path.DirectorySeparatorChar, '.');
                dialog.StartupPackage = package;
            }

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string cPackage     = dialog.getPackage();
                string newFilePath  = Path.ChangeExtension(Path.Combine(dialog.Directory.Replace('.', Path.DirectorySeparatorChar), dialog.getClassName()), ".as");
                string templatePath = Path.Combine(ProjectPaths.FileTemplatesDirectory, Path.Combine(project.GetType().Name, "Class.as.fdt"));
                lastFileFromTemplate = newFilePath;

                lastFileOptions = new AS3ClassOptions(
                                                    project.Language,
                                                    dialog.getSuperClass(),
                                                    dialog.hasInterfaces() ? dialog.getInterfaces() : null,
                                                    dialog.isPublic(),
                                                    dialog.isDynamic(),
                                                    dialog.isFinal(),
                                                    dialog.getGenerateInheritedMethods(),
                                                    dialog.getGenerateConstructor()
                                                  );

                MainForm.FileFromTemplate(templatePath, newFilePath);
            }
        }



        public string ProcessArgs(Project project, string args)
        {
            if (lastFileFromTemplate != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(lastFileFromTemplate);

                args = args.Replace("$(FileName)", fileName);

                if (args.Contains("$(FileNameWithPackage)") || args.Contains("$(Package)"))
                {
                    string package = "";
                    string path = lastFileFromTemplate;

                    // Find closest parent
                    string classpath = project.AbsoluteClasspaths.GetClosestParent(path);

                    // Can't find parent, look in global classpaths
                    if (classpath == null)
                    {
                        PathCollection globalPaths = new PathCollection();
                        foreach (string cp in ProjectManager.PluginMain.Settings.GlobalClasspaths)
                            globalPaths.Add(cp);
                        classpath = globalPaths.GetClosestParent(path);
                    }
                    if (classpath != null)
                    {
                        // Parse package name from path
                        package = Path.GetDirectoryName(ProjectPaths.GetRelativePath(classpath, path));
                        package = package.Replace(Path.DirectorySeparatorChar, '.');
                    }

                    args = args.Replace("$(Package)", package);

                    if (package != "")
                        args = args.Replace("$(FileNameWithPackage)", package + "." + fileName);
                    else
                        args = args.Replace("$(FileNameWithPackage)", fileName);


                    // lastClassFileOptions

                    if (lastFileOptions != null)
                    {
                        Int32 eolMode = (Int32)MainForm.Settings.EOLMode;
                        String lineBreak = LineEndDetector.GetNewLineMarker(eolMode);
                        ClassModel cmodel;
                        IASContext context;

                        string imports = "";
                        string extends = "";
                        string implements = "";
                        string access = "";
                        string inheritedMethods = "";
                        string paramString = "";
                        string superConstructor = "";
                        int index;

                        context = ASContext.GetLanguageContext( lastFileOptions.Language );

                        // resolve imports
                        if (lastFileOptions.interfaces != null && lastFileOptions.interfaces.Count > 0)
                        {
                            implements = " implements ";
                            string[] _implements;
                            index = 0;
                            foreach (string item in lastFileOptions.interfaces)
                            {
                                if(item.Split('.').Length > 1)
                                    imports += "import " + item + ";" + lineBreak + ( lastFileOptions.Language == "as3" ? "\t" : "");
                                _implements = item.Split('.');
                                implements += (index > 0 ? ", " : "") + _implements[_implements.Length - 1];

                                if (lastFileOptions.createInheritedMethods)
                                {
                                    cmodel = context.GetModel(item.Substring(0, item.LastIndexOf('.') == -1 ? item.Length : item.LastIndexOf('.')), _implements[_implements.Length - 1], "");
                                    if (!cmodel.IsVoid())
                                    {
                                        foreach (MemberModel member in cmodel.Members)
                                        {
                                            inheritedMethods += "public function " + member.ToString() + "$(CSLB){" + lineBreak + (lastFileOptions.Language == "as3" ? "\t\t" : "\t") + "}" + lineBreak + lineBreak + (lastFileOptions.Language == "as3" ? "\t\t" : "\t");
                                        }
                                    }
                                }

                                index++;
                            }
                        }

                        if (lastFileOptions.superClass != "")
                        {
                            if(lastFileOptions.superClass.Split('.').Length > 1)
                                imports += "import " + lastFileOptions.superClass  + ";" + lineBreak + "\t";
                            string[] _extends = lastFileOptions.superClass.Split('.');
                            extends = " extends " + _extends[_extends.Length - 1];

                            if (lastFileOptions.createConstructor)
                            {
                                cmodel = context.GetModel(lastFileOptions.superClass.Substring(0, lastFileOptions.superClass.LastIndexOf('.') == -1 ? lastFileOptions.superClass.Length : lastFileOptions.superClass.LastIndexOf('.')), _extends[_extends.Length - 1], "");
                                if (!cmodel.IsVoid())
                                {
                                    foreach (MemberModel member in cmodel.Members)
                                    {
                                        if (member.Name == cmodel.Constructor)
                                        {
                                            paramString = member.ParametersString();
                                            superConstructor = "super(";

                                            index = 0;
                                            foreach (MemberModel param in member.Parameters)
                                            {
                                                superConstructor += (index > 0 ? ", " : "") + param.Name;
                                                index++;
                                            }
                                            superConstructor += ");";
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (lastFileOptions.Language == "as3")
                        {
                            access = lastFileOptions.isPublic ? "public " : "internal ";
                            access += lastFileOptions.isDynamic ? "dynamic " : "";
                            access += lastFileOptions.isFinal ? "final " : "";
                        }
                        else
                        {
                            access = lastFileOptions.isDynamic ? "dynamic " : "";
                        }

                        args = args.Replace("$(Import)", imports);
                        args = args.Replace("$(Extends)", extends);
                        args = args.Replace("$(Implements)", implements);
                        args = args.Replace("$(Access)", access);
                        args = args.Replace("$(InheritedMethods)", inheritedMethods);
                        args = args.Replace("$(ConstructorArguments)", paramString);
                        args = args.Replace("$(Super)", superConstructor);

                        args = FlashDevelop.Utilities.ArgsProcessor.ProcessCodeStyleLineBreaks(args);

                    }

                }
            }
            return args;
        }

		#endregion

	}
	
}
