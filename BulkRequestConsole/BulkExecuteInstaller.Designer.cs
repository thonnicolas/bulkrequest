namespace BulkRequestConsole
{
    partial class BulkExecuteInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.BulkRequestServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.BulkRequestServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // BulkRequestServiceProcessInstaller
            // 
            this.BulkRequestServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.BulkRequestServiceProcessInstaller.Password = null;
            this.BulkRequestServiceProcessInstaller.Username = null;
            this.BulkRequestServiceProcessInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.BulkRequestServiceProcessInstaller_AfterInstall);
            // 
            // BulkRequestServiceInstaller
            // 
            this.BulkRequestServiceInstaller.Description = "Execute Bulk Request Service for Web Admin";
            this.BulkRequestServiceInstaller.DisplayName = "IT-Bulk-Request";
            this.BulkRequestServiceInstaller.ServiceName = "ITBulkRequestService";
            this.BulkRequestServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // BulkExecuteInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.BulkRequestServiceProcessInstaller,
            this.BulkRequestServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller BulkRequestServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller BulkRequestServiceInstaller;
    }
}