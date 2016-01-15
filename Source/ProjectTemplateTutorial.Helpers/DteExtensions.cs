using EnvDTE;
using EnvDTE100;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using NuGet.VisualStudio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTemplateTutorial.Helpers
{
    public static class DteExtensions
    {
        public static Project AddProject(this Solution solution, string destination, string projectName, string templateName)
        {
            string projectPath = Path.Combine(destination, projectName);
            string templatePath = ((Solution4)solution).GetProjectTemplate(templateName, "CSharp");

            solution.AddFromTemplate(templatePath, projectPath, projectName, false);

            Project project = (from Project p in solution.Projects
                               where p.Name.Equals(projectName)
                               select p).FirstOrDefault();

            return project;
        }

        public static void AddItem(this Project project, string itemTemplateName, string itemName)
        {
            string templatePath = ((Solution4)project.DTE.Solution).GetProjectItemTemplate(itemTemplateName, "CSharp");
            project.ProjectItems.AddFromTemplate(templatePath, itemName);
        }

        public static bool InstallNuGetPackage(this Project project, string packageName)
        {
            bool installedPkg = true;
            
            try
            {
                var componentModel = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));
                IVsPackageInstallerServices installerServices = componentModel.GetService<IVsPackageInstallerServices>();
                if (!installerServices.IsPackageInstalled(project, packageName))
                {
                    IVsPackageInstaller installer = componentModel.GetService<IVsPackageInstaller>();
                    installer.InstallPackage(null, project, packageName, (System.Version)null, false);
                }
            }
            catch (Exception ex)
            {
                installedPkg = false;
            }

            return installedPkg;
        }

        public static void SetResponsibility<T>(this Project project, params T[] responsibilities)
        {
            foreach (var res in Enum.GetValues(typeof(T)))
            {
                string name = res.ToString();
                project.Globals[name] = Boolean.FalseString;
                project.Globals.set_VariablePersists(name, true);
            }

            foreach (var res in responsibilities)
            {
                string name = res.ToString();
                project.Globals[name] = Boolean.TrueString;
                project.Globals.set_VariablePersists(name, true);
            }
        }

        public static bool IsProjectResponsible(this Project project, Enum responsibility)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            if (project.Globals.get_VariableExists(responsibility.ToString()))
            {
                string propertyValue = (string)project.Globals[responsibility.ToString()];
                bool propertyValueBoolean;

                if (Boolean.TryParse(propertyValue, out propertyValueBoolean))
                {
                    if (propertyValueBoolean)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
