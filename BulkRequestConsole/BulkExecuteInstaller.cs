using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;


namespace BulkRequestConsole
{
    [RunInstaller(true)]
    public partial class BulkExecuteInstaller : System.Configuration.Install.Installer
    {
        public BulkExecuteInstaller()
        {
            InitializeComponent();
        }

        private void BulkRequestServiceProcessInstaller_AfterInstall(object sender, InstallEventArgs e)
        {

        }
    }
}
