using System.Net.Mail;
using System.Net;

namespace WebShopping.Areas.Admin.Repository
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true, //bật bảo mật
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("thontanhanh@gmail.com", "hrjdbkvganlhmwzf")
            };

            return client.SendMailAsync(
                new MailMessage(from: "thontanhanh@gmail.com",
                                to: email,
                                subject,
                                message
                                ));
        }
    }
}
