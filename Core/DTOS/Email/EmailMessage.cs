using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOS.Email
{
    public class EmailMessage
    {
        public EmailMessage() { }
        public string ToAddresses { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
