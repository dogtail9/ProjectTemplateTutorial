using Microsoft.VisualStudio.TemplateWizard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System.IO;
using EnvDTE100;
using ProjectTemplateTutorial.VSIXProject.Dialogs;
using Microsoft.VisualStudio.ComponentModelHost;
using NuGet.VisualStudio;
using ProjectTemplateTutorial.VSIXProject.Commands;
using ProjectTemplateTutorial.Helpers;
using EnvDTE80;

namespace ProjectTemplateTutorial.VSIXProject.Wizards
{
    public class SolutionWizard : IWizard
    {
        private Dictionary<string, string> _replacementsDictionary = new Dictionary<string, string>();
        private DTE _dte;
        private bool _addOptionalProject;
        private bool _sourceFolder;
        private bool _mandatoryFolder;
        private bool _optionalFolder;

        public SolutionWizard()
        {
            _dte = ServiceProvider.GlobalProvider.GetService(typeof(DTE)) as DTE;
        }

        public void BeforeOpeningFile(ProjectItem projectItem)
        {
        }

        public void ProjectFinishedGenerating(Project project)
        {
        }

        public void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {
        }

        public void RunFinished()
        {
            string destination = _replacementsDictionary["$destinationdirectory$"];
            string fileName = _replacementsDictionary["$safeprojectname$"] + ".sln";
            _dte.Solution.SaveAs(Path.Combine(destination, fileName));

            SolutionFolder sourceSolutionFolder = null;
            if (_sourceFolder)
                sourceSolutionFolder = _dte.Solution.AddSolutionFolderEx("Source");

            string mandatoryFolderName = null;
            if (_mandatoryFolder)
                mandatoryFolderName = "Mandatory";

            string optionalFolderName = null;
            if (_optionalFolder)
                optionalFolderName = "Optional";


            Project mandatoryPproject = AddProject("Mandatory", "ProjectTemplateTutorial.Mandatory", sourceSolutionFolder, mandatoryFolderName);
            mandatoryPproject.SetResponsibility(ProjectResponsibilities.Mandatory);

            Project mandatoryPproject2 = AddProject("Mandatory2", "ProjectTemplateTutorial.Mandatory", sourceSolutionFolder, mandatoryFolderName);
            mandatoryPproject2.SetResponsibility(ProjectResponsibilities.Mandatory);

            if (_addOptionalProject)
            {
                Project optionalProject = AddProject("Optional", "ProjectTemplateTutorial.Optional", sourceSolutionFolder, optionalFolderName);
                optionalProject.SetResponsibility(ProjectResponsibilities.Optional);
                optionalProject.InstallNuGetPackage("Newtonsoft.Json");
                optionalProject.AddItem("ProjectTemplateTutorial.ItemTemplate", "Json1.jc");
            }
        }

        private Project AddProject(string projectSufix, string templateName, SolutionFolder sourceSolutionFolder = null, string folderName = null)
        {
            string destination = _replacementsDictionary["$destinationdirectory$"];

            if (_sourceFolder)
            {
                destination = Path.Combine(destination, "Source");
            }

            var projectName = $"{_replacementsDictionary["$safeprojectname$"]}.{projectSufix}";

            Project project;
            if (sourceSolutionFolder == null)
            {
                if (folderName != null)
                {
                    SolutionFolder optionalFolder = _dte.Solution.AddSolutionFolderEx(folderName);
                    project = optionalFolder.AddProject(destination, projectName, templateName);
                }
                else
                {
                    project = _dte.Solution.AddProject(destination, projectName, templateName);
                }
            }
            else
            {
                if (folderName != null)
                {
                    SolutionFolder folder = (SolutionFolder)sourceSolutionFolder.AddSolutionFolderEx(folderName);
                    project = folder.AddProject(destination, projectName, templateName);
                }
                else
                {
                    project = sourceSolutionFolder.AddProject(destination, projectName, templateName);
                }
            }

            return project;
        }

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            _replacementsDictionary = replacementsDictionary;

            SolutionWizardDialog dialog = new SolutionWizardDialog(_replacementsDictionary["$safeprojectname$"]);
            var result = dialog.ShowModal();

            if (result == null || !result.Value)
            {
                throw new WizardCancelledException();
            }
            else
            {
                _addOptionalProject = (bool)dialog.OptionalProjectNameCbx.IsChecked;
                _sourceFolder = (bool)dialog.SourceFolderCbx.IsChecked;
                _mandatoryFolder = (bool)dialog.MandatoryProjectSolutionFolderCbx.IsChecked;
                _optionalFolder = (bool)dialog.OptionalProjectSolutionFolderCbx.IsChecked;
            }
        }

        public bool ShouldAddProjectItem(string filePath) => true;
    }
}
