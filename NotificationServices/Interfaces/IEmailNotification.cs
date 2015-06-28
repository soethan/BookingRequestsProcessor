using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationServices.Interfaces
{
    public interface IEmailNotification
    {
        void Send(string sender, List<string> receiver, string subject, string content);
    }
}
