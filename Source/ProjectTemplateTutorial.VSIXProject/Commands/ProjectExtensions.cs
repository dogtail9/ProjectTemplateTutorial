using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTemplateTutorial.VSIXProject.Commands
{
    public enum ProjectResponsibilities
    {
        Mandatory,
        Optional
    }

    public static class ProjectExtensions
    {
        public static void SetResponsibility(this Project project, params ProjectResponsibilities[] responsibilities)
        {
            foreach (var res in Enum.GetValues(typeof(ProjectResponsibilities)))
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
