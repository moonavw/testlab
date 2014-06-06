using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace TestLab.AgentService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();

            this.serviceProcessInstaller1.Username = ConfigurationManager.AppSettings["ServiceInstaller_Username"];
            this.serviceProcessInstaller1.Password = ConfigurationManager.AppSettings["ServiceInstaller_Password"];
        }
    }
}
