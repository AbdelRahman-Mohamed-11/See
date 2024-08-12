using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Notifications
{
    public class ApnNotification
    {
        public string DeviceToken { get; set; }
        public Dictionary<string, object> Payload { get; set; }
    }

}
