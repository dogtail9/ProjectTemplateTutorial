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

            Project mandatoryPproject = AddProject(destination, projectName, templateName);
            mandatoryPproject.SetResponsibility(ProjectResponsibilities.Mandatory);


            if (_addOptionalProject)
            {
                projectName = $"{_replacementsDictionary["$safeprojectname$"]}.Optional";
                templateName = "ProjectTemplateTutorial.Optional";

                Project optionalProject = AddProject(destination, projectName, templateName);
                optionalProject.SetResponsibility(ProjectResponsibilities.Optional);

                InstallNuGetPackage(projectName, "Newtonsoft.Json");
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

        private Project AddProject(string destination, string projectName, string templateName)
        {
            string projectPath = Path.Combine(destination, projectName);
            string templatePath = ((Solution4)_dte.Solution).GetProjectTemplate(templateName, "CSharp");

            _dte.Solution.AddFromTemplate(templatePath, projectPath, projectName, false);

            Project project = (from Project p in _dte.Solution.Projects
                               where p.Name.Equals(projectName)
                               select p).FirstOrDefault();

            return project;
        }

        private bool InstallNuGetPackage(string projectName, string package)
        {
            bool installedPkg = true;

            Project project = (from Project p in (Array)_dte.ActiveSolutionProjects
                               where p.Name.Equals(projectName)
                               select p).First();
            try
            {
                var componentModel = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));
                IVsPackageInstallerServices installerServices = componentModel.GetService<IVsPackageInstallerServices>();
                if (!installerServices.IsPackageInstalled(project, package))
                {
                    _dte.StatusBar.Text = @"Installing " + package + " NuGet package, this may take a minute...";
                    IVsPackageInstaller installer = componentModel.GetService<IVsPackageInstaller>();
                    installer.InstallPackage(null, project, package, (System.Version)null, false);
                    _dte.StatusBar.Text = @"Finished installing the " + package + " NuGet package";
                }
            }
            catch (Exception ex)
            {
                string t = ex.Message;
                installedPkg = false;
                _dte.StatusBar.Text = @"Unable to install the  " + package + " NuGet package";
            }

            return installedPkg;
        }
    }
}
