using System.Net;
using System.Net.Mail;
using System.Text;
using SportsStore.Domain.Entities;
using SportsStore.Domain.Abstract;

namespace SportsStore.Domain.Concrete
{
    public class EmailSettings
    {
        public string MailToAddress = "zamowienia@przyklad.pl";
        public string MailFromAddress = "sklepsportowy@przyklad.pl";
        public bool useSsl = true;
        public string Username = "UzytkownikSmtp";
        public string Password = "HasloSmtp";
        public string ServerName = "smtp.przyklad.pl";
        public int ServerPort = 559;
        public bool WriteAsFile = false;
        public string FileLocation = @"C:\Users\Admin\source\repos\SportStore\Orders";
    }
   public class EmailOrderProcessor:IOrderProcessor
    {
        private EmailSettings emailSettings;

        public EmailOrderProcessor(EmailSettings emailSettings)
        {
            this.emailSettings = emailSettings;
        }

        public void ProcessOrder(Cart cart, ShippingDetails shippingDetails)
        {
            using(var smtpClient=new SmtpClient())
            {
                smtpClient.EnableSsl = emailSettings.useSsl;
                smtpClient.Host = emailSettings.ServerName;
                smtpClient.Port = emailSettings.ServerPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password);

                if(emailSettings.WriteAsFile)
                {
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation = emailSettings.FileLocation;
                    smtpClient.EnableSsl = false;
                }
                StringBuilder body = new StringBuilder().AppendLine("Nowe zamówienie").AppendLine("________").AppendLine("Produkty:");

                foreach(var line in cart.Lines)
                {
                    var subTotal = line.Product.Price * line.Quantity;
                    body.AppendFormat("{0}x[1] (Wartość: {2:c}", line.Quantity, line.Product.Name, subTotal);
                }

                body.AppendFormat("Wartość całkowita: {0:c}", cart.ComputeTotalValue()).AppendLine("____________").AppendLine("Dane do wysyłki").AppendLine(shippingDetails.Name)
                    .AppendLine(shippingDetails.Line1).AppendLine(shippingDetails.Line2 ?? "").AppendLine(shippingDetails.Line3 ?? "").AppendLine(shippingDetails.City).AppendLine(shippingDetails.State ?? "")
                    .AppendLine(shippingDetails.Zip).AppendLine("_________").AppendFormat("Opakowanie prezentowe: {0}", shippingDetails.GiftWrap ? "Tak" : "Nie");


                MailMessage mailMessage = new MailMessage(emailSettings.MailFromAddress, emailSettings.MailToAddress, "Otrzymano nowe zamówienie!", body.ToString());
                if (emailSettings.WriteAsFile)
                {
                    mailMessage.BodyEncoding = Encoding.ASCII;
                }
                smtpClient.Send(mailMessage);
            }
        }
    }
}
