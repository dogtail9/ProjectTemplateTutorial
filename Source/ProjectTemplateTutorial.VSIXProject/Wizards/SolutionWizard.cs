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

namespace ProjectTemplateTutorial.VSIXProject.Wizards
{
    public class SolutionWizard : IWizard
    {
        private Dictionary<string, string> _replacementsDictionary = new Dictionary<string, string>();
        private DTE _dte;
        private bool _addOptionalProject;

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

            var projectName = $"{_replacementsDictionary["$safeprojectname$"]}.Mandatory";
            var templateName = "ProjectTemplateTutorial.Mandatory";

            Project mandatoryPproject = _dte.Solution.AddProject(destination, projectName, templateName);
            mandatoryPproject.SetResponsibility(ProjectResponsibilities.Mandatory);

            if (_addOptionalProject)
            {
                projectName = $"{_replacementsDictionary["$safeprojectname$"]}.Optional";
                templateName = "ProjectTemplateTutorial.Optional";

                Project optionalProject = _dte.Solution.AddProject(destination, projectName, templateName);
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
            }
        }

        public bool ShouldAddProjectItem(string filePath) => true;
    }
}
