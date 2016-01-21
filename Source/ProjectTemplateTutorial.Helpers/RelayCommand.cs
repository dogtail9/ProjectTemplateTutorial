using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTemplateTutorial.Helpers
{
    public class RelayCommand
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
