//------------------------------------------------------------------------------
// <copyright file="RelayCommandPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using EnvDTE;

namespace ProjectTemplateTutorial.VSIXProject.Commands
{
    [ProvideAutoLoad("{f1536ef8-92ec-443c-9ed7-fdadf150da82}")]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(RelayCommandPackage.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class RelayCommandPackage : Package
    {
        private RelayCommand addCopyrightCommand;

        public const string PackageGuidString = "edc30286-8947-4257-9355-8d5d25829c5d";

        public RelayCommandPackage()
        {

        }

        protected override void Initialize()
        {
            addCopyrightCommand = new RelayCommand(
                this, 
                PackageIds.AddCopyrightCommand, 
                PackageGuids.guidRelayCommandPackageCmdSet,
                AddCopyrightComment
                ,
                (sender, e) =>
                {
                    var cmd = (OleMenuCommand)sender;
                    cmd.Visible = true;
                });

            base.Initialize();
        }

        private void AddCopyrightComment(object sender, EventArgs e)
        {
            DTE dte = GetService(typeof(DTE)) as DTE;

            Array projects = (Array)dte.ActiveSolutionProjects;
            
            foreach (Project project in projects)
            {
                foreach (ProjectItem projectItem in project.ProjectItems)
                {
                    Document document;
                    try
                    {
                        projectItem.Open();
                        document = projectItem.Document;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("failed to load document");
                        continue;
                    }
                    if (document == null)
                    {
                        continue;
                    }

                    TextDocument editDoc = (TextDocument)document.Object("TextDocument");
                    if (document.Name.EndsWith(".cs"))
                    {
                        EditPoint objEditPt = editDoc.CreateEditPoint();
                        objEditPt.StartOfDocument();
                        document.ReadOnly = false;

                        objEditPt.Insert("//-----------------------------------------------------------------------------");
                        objEditPt.Insert(Environment.NewLine);
                        objEditPt.Insert("// Copyright (c) The Corporation.  All rights reserved.");
                        objEditPt.Insert(Environment.NewLine);
                        objEditPt.Insert("//-----------------------------------------------------------------------------");
                        objEditPt.Insert(Environment.NewLine);

                        document.Save(document.FullName);
                    }
                }
            }
        }
    }
}
