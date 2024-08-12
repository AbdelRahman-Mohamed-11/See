using Application.Interfaces;
using Core.DTOS.Email;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        private readonly UserManager<ApplicationUser> _userManager;

        public EmailService(IConfiguration config, UserManager<ApplicationUser> userManager)
        {
            _config = config;
            _userManager = userManager;
        }

        public async Task SendEmail(EmailMessage emailMessage)
        {
            var client = new SendGridClient(_config["SendGrid:sendGridKey"]);

            var msg = new SendGridMessage();
            
            msg.SetFrom(
                new SendGrid.Helpers.Mail.EmailAddress("belalmohamed5350@gmail.com", "Glasses Application"));

            var user = await _userManager.FindByEmailAsync(emailMessage.ToAddresses);
            
            if(user is null)
            {
                return;
            }
            
            msg.AddTo(new SendGrid.Helpers.Mail.EmailAddress(emailMessage.ToAddresses,user.FirstName + " " + user.LastName));
            

            msg.SetSubject(emailMessage.Subject);

            // Use HTML to style your email content
            msg.AddContent(MimeType.Html, "<html><body style='font-family: Arial, sans-serif;'>" +
                "<h2 style='color: #0078d4;'>Glasses Application Welcoming You </h2>" +
                "<p style='font-size: 16px;'>Hello, " + emailMessage.ToAddresses + "!</p>" +  // here send only to the first
                $"<p style='color: #333;'>{emailMessage.Body}</p>" +

                "</body></html>");

            await client.SendEmailAsync(msg);
        }
    }
}
