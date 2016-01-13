//------------------------------------------------------------------------------
// <copyright file="RelayCommand.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace ProjectTemplateTutorial.VSIXProject.Commands
{
    internal sealed class RelayCommand
    {
        private readonly Package package;

        public RelayCommand(Package package, int commandId, Guid commandSet, Action<object, EventArgs> menuCallback, Action<object, EventArgs> beforeQueryStatus = null)
        {
            this.package = package;

            OleMenuCommandService commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var MenuCommandID = new CommandID(commandSet, commandId);
                var MenuItem = new OleMenuCommand(menuCallback.Invoke, MenuCommandID);
                if (beforeQueryStatus != null)
                {
                    MenuItem.BeforeQueryStatus += beforeQueryStatus.Invoke;
                }
                commandService.AddCommand(MenuItem);
            }
        }

        private IServiceProvider ServiceProvider => this.package;
    }
}
