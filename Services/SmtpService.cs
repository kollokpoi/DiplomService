using DiplomService.ViewModels.Email;
using System.Net;
using System.Net.Mail;

namespace DiplomService.Services
{
    public class SmtpService
    {
        private static readonly string senderEmail = "kollokpoi@yandex.ru";

        public static async void SendMessage(MailMessage m)
        {
            var smtp = new SmtpClient("smtp.yandex.ru", 587)
            {
                Credentials = new NetworkCredential(senderEmail, "dtxijdqjdjqcipif"),
                EnableSsl = true
            };
            await smtp.SendMailAsync(m);
        }

        public static void SendApplicationResponse(string messageBody, ApplicationEmailViewModel messageVM)
        {
            var from = new MailAddress(senderEmail, "PrograMatch");
            var to = new MailAddress(messageVM.UserEmail);
            var m = new MailMessage(from, to)
            {
                Subject = "PrograMatch. Ваша заявка была рассмотрена.",
                Body = messageBody,
                IsBodyHtml = true
            };
            SendMessage(m);
        }
        public static void SendUserRegistration(string messageBody, UserRegistratedEmailViewModel messageVM)
        {
            var from = new MailAddress(senderEmail, "PrograMatch");
            var to = new MailAddress(messageVM.Email);
            var m = new MailMessage(from, to)
            {
                Subject = "PrograMatch. Вы зарегистрированы в системе.",
                Body = messageBody,
                IsBodyHtml = true
            };
            SendMessage(m);
        }
        public static void SendEventApplicationResponce(string messageBody, EventApplicationViewModel messageVM)
        {
            var from = new MailAddress(senderEmail, "PrograMatch");
            var to = new MailAddress(messageVM.Email);
            var m = new MailMessage(from, to)
            {
                Subject = "PrograMatch. Ваша заявка была рассмотрена.",
                Body = messageBody,
                IsBodyHtml = true
            };
            SendMessage(m);
        }
    }
}
