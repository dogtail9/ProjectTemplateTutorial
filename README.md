# ProjectTemplateTutorial
This is a tutorial on how to create a project template for Visual Studio with multiple projects, commands, dialogs and external tools.
The project template will have a mandatory and an optional project that you can choose from an dialog when the project is created.
There will also be an item template that uses Text Template Transformation Toolkit (T4) to generate code from an DSL.

## Prerequisites
I have the following softwares installed on my machine.

* Visual Studio 2015 Update 1
* Visual Studio SDK
* [Extensibility Tools for Visual Studio](https://github.com/madskristensen/ExtensibilityTools)
* [ILSyp](http://ilspy.net/)

If you want to skip some parts of the tutorial, you can download the code and start where you want.
* [Add mandatory project](https://github.com/dogtail9/ProjectTemplateTutorial#mandatory-project-template)
* [Add optional project](https://github.com/dogtail9/ProjectTemplateTutorial#optional-project-template)

## Create a custom project template
This is a tutorial on how to create a project template with multiple projects, custom commands and dialogs. We will also add an external tool that generate code from a domain specific language.

### Solution
First we need a solution to add our project to. Open Visual Studio and follow the steps bellow.

![Create blank solution](Images/0010_Solution/0010.PNG)

*Choose the blank solution project template*

Now we have our solution that we can start adding project to.

### VSIX Project
The VSIX project where we will put all logic such as wizards, commands and dialogs. 

![Create blank solution](Images/0020_VSIX/0010.PNG)

*Add a VSIX Project to the solution*

![Create blank solution](Images/0020_VSIX/0020.PNG)

*Delete the unnecessary files*

![Create blank solution](Images/0020_VSIX/0030.PNG)

*Add folders for Commands, Dialogs and Wizards*

## Solution Project Template
Now we will add to a project template whose sole purpose is to run a wizard, where we can add the code to create our project.

![Create blank solution](Images/0030_SolutionProjectTemplate/0010.PNG)

*Add the solution project template*

![Create blank solution](Images/0030_SolutionProjectTemplate/0020.PNG)

*Delete the unnecessary files*

```xml
<Project File="ProjectTemplate.csproj" ReplaceParameters="true">
  <ProjectItem ReplaceParameters="true" TargetFileName="Properties\AssemblyInfo.cs">AssemblyInfo.cs</ProjectItem>
  <ProjectItem ReplaceParameters="true" OpenInEditor="true">Class1.cs</ProjectItem>
</Project>
```

*Delete the content of the TemplateContent element in the ProjectTemplateTutorial.Solution.vstemplate file*

![Create blank solution](Images/0030_SolutionProjectTemplate/0040.PNG)

*Change the type attribute of the VSTemplate element in the vstemplate file to ProjectGroup*

![Create blank solution](Images/0030_SolutionProjectTemplate/0100.PNG)

*Add a category for the project template in the new project dialog*


## Solution wizard
Now we need to add a wizard class where the logic for creating our project template.
Add a class to the Wizard folder in the VSIXProject, name it SolutionWizard. The SolutionWizard class should implement the IWizard interface.
Sign all projects in the solution.

![Create blank solution](Images/0030_SolutionProjectTemplate/0050.PNG)

*Add the references to envdte and Microsoft.VisualStudio.TemplateWizardInterface in the VSIXProject*

```CSharp
using Microsoft.VisualStudio.TemplateWizard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;

namespace ProjectTemplateTutorial.VSIXProject.Wizards
{
    public class SolutionWizard : IWizard
    {
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
            int i = 0;
        }

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
        }

        public bool ShouldAddProjectItem(string filePath) => true;
    }
}
```

*The SolutionWizard class*

Set a breakpoint in the int i = 0; line in the RunFinished method.

## Assets
The only thing left to do is to add assets to the VSIXProject. Open the source.extension.vsixmanifest file in the VSIXProject. Add the assets below.

![Create blank solution](Images/0030_SolutionProjectTemplate/0070.PNG)

*Add assembly*

![Create blank solution](Images/0030_SolutionProjectTemplate/0080.PNG)

*Add project template*

## Add the wizard to the solution project template
![Create blank solution](Images/0030_SolutionProjectTemplate/0060.PNG)

*I use ILSpy to get the strongname of the ProjectTemplate.VSIXProject.dll*

```xml
<WizardExtension>
  <Assembly>ProjectTemplateTutorial.VSIXProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=b490d4518b7bc751</Assembly>
  <FullClassName> ProjectTemplateTutorial.VSIXProject.Wizards.SolutionWizard</FullClassName>
</WizardExtension>
```

*Add the WizardExtension element to VSTemplate element in the vstemplate file*

Let´s try to create a project with our project template.

![Create blank solution](Images/0030_SolutionProjectTemplate/0110.PNG)

*The project template is located in the tutorial category in the New Project dialog*

![Create blank solution](Images/0030_SolutionProjectTemplate/0120.PNG)

*The break point in the RunFinished method should be hit*

![Create blank solution](Images/0030_SolutionProjectTemplate/0130.PNG)

*The empty solution created is created*

We are done with the first step out of this tutorial.

## Mandatory project template
Let´s add the project template for the mandatory project in our project template.
If you skiped the first step in this tutorial you can download the code from the [Solution](https://github.com/dogtail9/ProjectTemplateTutorial/releases) release and start the tutorial here.  

![Create blank solution](Images/0040_MandatoryProjectTemplate/0010.PNG)

*Add the mandatory project template*

We want to hide this project template in the New Project dialog and add the project to the solution from the SolutionWizard class.

```xml
<Hidden>true</Hidden>
```

*Add the Hidden element to the TemplateData element in the ProjectTemplateTutorial.Mandatory.vstemplate file*

![Create blank solution](Images/0040_MandatoryProjectTemplate/0020.PNG)

*Add the mandatory project template as an asset in the VSIXproject* 

![Create blank solution](Images/0040_MandatoryProjectTemplate/0030.PNG)

*Add the Microsoft.VisualStudio.Shell.14.0 NuGet package to the VSIXProject*

Let's write the code to add the mandatory project to the solution. Visual Studio passes a dictionary with data from the New Project dialog to the wizard. We need to save this data and use it to create the mandatory project from our mandatory project template. 

```CSharp
private Dictionary<string, string> _replacementsDictionary = new Dictionary<string, string>();
DTE _dte;

public SolutionWizard()
{
    _dte = ServiceProvider.GlobalProvider.GetService(typeof(DTE)) as DTE;
}
```

*Add constructor and fields to the SolutionWizard class*

```CSharp
public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
{
    _replacementsDictionary = replacementsDictionary;
}
```

*Store the replacementsDictionary in the field in the RunStarted method*

```CSharp
public void RunFinished()
{
    string destination = _replacementsDictionary["$destinationdirectory$"];
    string fileName = _replacementsDictionary["$safeprojectname$"] + ".sln";
    _dte.Solution.SaveAs(Path.Combine(destination, fileName));
    
    var projectName = $"{_replacementsDictionary["$safeprojectname$"]}.Mandatory";
    var templateName = "ProjectTemplateTutorial.Mandatory";

    AddProject(destination, projectName, templateName);
}
```

*Get the data for our project from the replacementsDictionary and pass it to the AddProject method*

We will use the DTE object to add the project to our solution. Later we will refactor this method to a helper library so we can reuse it in other project templates but for now just put the AddProject method in the SolusionWizard class. 

```CSharp
private void AddProject(string destination, string projectName, string templateName)
{
    string projectPath = Path.Combine(destination, projectName);
    string templatePath = ((Solution4)_dte.Solution).GetProjectTemplate(templateName, "CSharp");

    _dte.Solution.AddFromTemplate(templatePath, projectPath, projectName, false);
}
```

*Code to add the a project to the solution*

Let's try the project template to se that the mandatory project is created.

![Create blank solution](Images/0040_MandatoryProjectTemplate/0040.PNG)

*The mandatory project is created*

You can also add projects and use the build in project template if you like. You find the build in project templates in the *C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\ProjectTemplate*s folder.

```CSharp
projectName = $"{_replacementsDictionary["$safeprojectname$"]}.WCFServiceLibrary";
AddProject(destination, projectName, "WcfServiceLibrary");
```

*Add a WCFServiceLibrary to the solution*

![Create blank solution](Images/0040_MandatoryProjectTemplate/0050.PNG)

*The built-in WCFServiceLibrary template is now added to the solution*

We done with step two of this tutorial. 

## Optional project template
Let´s add the project template and a dialog for the optional project in our project template.
If you skiped the second step in this tutorial you can download the code from the [Mandatory](https://github.com/dogtail9/ProjectTemplateTutorial/releases) release and start the tutorial here.  

*Add the optional project template*

## Project creation dialog
We need a dialog to select the projects to be created.

*Add a CustomControl*

*Change the root element to Window*

*Change the base class in the code behind file to Window*

```xml
<Window>
</Window>
```
*The XAML for the solution dialog*

```CSharp
public void AddProject()
{
}
```
*Code to add the optional project to the solution*

![Dogtail](http://www.dogtail.se/dogtail.gif2)

*Project created*


## Add NuGet packages as part of the project creation process

### NuGet helper code

### Add NuGet packages

## Helper library

## Add a command

### VSPackage

### RelayCommand

### Implement some usefull feature with a dialog and NuGet packages

## Create a custom item template

### Item template

### T4 (Text Template Transformation Toolkit) Code Generation 
