using NotificationServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationServices.Implementation
{
    public class EmailNotification: IEmailNotification
    {
        public void Send(string sender, List<string> receiver, string subject, string content)
        {
            File.WriteAllText(ConfigurationManager.AppSettings["EmailFolder"] + subject + "-" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + ".txt", content);
        }
    }
}
