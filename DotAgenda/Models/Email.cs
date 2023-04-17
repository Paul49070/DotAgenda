using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using DotAgenda.MethodClass;
using Microsoft.SqlServer.Management.Smo;
using System.Net.Mime;

namespace DotAgenda.Models
{
    public class Email
    {
        private string GetServer(string mailAdr)
        {
            GlobalDict._dict.InitSmtpList();

            int Debutsmtp = mailAdr.IndexOf("@") + 1;
            string server_temp = mailAdr.Substring(Debutsmtp, mailAdr.Length - Debutsmtp);
            int FinSmtp = server_temp.IndexOf(".");
            server_temp = server_temp.Substring(0, FinSmtp);
            server_temp = server_temp.ToLower();

            return GlobalDict._dict.SmtpList[server_temp];
        }

        public Email(string prenom, string mailAdr, string generatedCode, string subject = "Création de compte .agenda !")
        {
            //GetServer(mailAdr);

            MailAddress from = new MailAddress("DotAgendaContact1@gmail.com");
            //MailAddress to = new MailAddress(mailAdr);
            MailAddress to = new MailAddress("paul.lemonnier70@gmail.fr");

            MailMessage message = new MailMessage(from, to);
            message.Subject = subject;

            string relativePath = @"../../MailTemplate/WelcomeMail/";
            string absolutePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath));

            string Body = string.Empty;
            string templatePath = Path.Combine(absolutePath, "WelcomeMail.html");

            if (File.Exists(templatePath))
            {
                Body = File.ReadAllText(templatePath);
            }

            Body = Body.Replace("{PRENOM}", prenom);
            Body = Body.Replace("{VERIFICATIONCODE}", generatedCode);

            message.IsBodyHtml = true;

            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(Body, null, MediaTypeNames.Text.Html);
            
            message.AlternateViews.Add(htmlView);

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                EnableSsl = true,
                Credentials = new NetworkCredential("DotAgendaContact1@gmail.com", "lantpoajyrhsgone")
            };            // Include credentials if the server requires them.

            try
            {
                smtpClient.Send(message);
            }
            catch
            {
                MessageBox.Show("Impossible d'envoyer le mail de confirmation !", "Error", MessageBoxButton.OK);
            }
        }

        public static void Send()
        {

        }
    }
}
