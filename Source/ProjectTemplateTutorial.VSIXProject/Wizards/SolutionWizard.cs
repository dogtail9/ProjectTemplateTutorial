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
            {
                sourceSolutionFolder = _dte.Solution.AddSolutionFolderEx("Source");
                destination = Path.Combine(destination, "Source");
            }

            var projectName = $"{_replacementsDictionary["$safeprojectname$"]}.Mandatory";
            var templateName = "ProjectTemplateTutorial.Mandatory";


            Project mandatoryPproject;
            if (sourceSolutionFolder == null)
            {
                if (_mandatoryFolder)
                {
                    SolutionFolder mandatoryFolder = _dte.Solution.AddSolutionFolderEx("Mandatory");
                    mandatoryPproject = mandatoryFolder.AddProject(destination, projectName, templateName);
                }
                else
                {
                    mandatoryPproject = _dte.Solution.AddProject(destination, projectName, templateName);
                }
            }
            else
            {
                if (_mandatoryFolder)
                {
                    SolutionFolder mandatoryFolder = (SolutionFolder)sourceSolutionFolder.AddSolutionFolder("Mandatory").Object;
                    mandatoryPproject = mandatoryFolder.AddProject(destination, projectName, templateName);
                }
                else
                {
                    mandatoryPproject = sourceSolutionFolder.AddProject(destination, projectName, templateName);
                }

            }
            mandatoryPproject.SetResponsibility(ProjectResponsibilities.Mandatory);

            Project optionalProject;
            if (_addOptionalProject)
            {
                projectName = $"{_replacementsDictionary["$safeprojectname$"]}.Optional";
                templateName = "ProjectTemplateTutorial.Optional";

                if (sourceSolutionFolder == null)
                {
                    if (_optionalFolder)
                    {
                        SolutionFolder optionalFolder = _dte.Solution.AddSolutionFolderEx("Optional");
                        optionalProject = optionalFolder.AddProject(destination, projectName, templateName);
                    }
                    else
                    {
                        optionalProject = _dte.Solution.AddProject(destination, projectName, templateName);
                    }
                }
                else
                {
                    if (_optionalFolder)
                    {
                        SolutionFolder optionalFolder = (SolutionFolder)sourceSolutionFolder.AddSolutionFolder("Optional").Object;
                        optionalProject = optionalFolder.AddProject(destination, projectName, templateName);
                    }
                    else
                    {
                        optionalProject = sourceSolutionFolder.AddProject(destination, projectName, templateName);
                    }
                }

                optionalProject.SetResponsibility(ProjectResponsibilities.Optional);
                optionalProject.InstallNuGetPackage("Newtonsoft.Json");
                optionalProject.AddItem("ProjectTemplateTutorial.ItemTemplate", "Json1.jc");
            }
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
