using Check23.Models;
using MailKit.Net.Smtp;
using MimeKit;

namespace Check23.Services.EmailService
{
    public class Emailservice : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly Dictionary<string, int> areasOfResponsibilityInDatabase = new Dictionary<string, int>
        {
            { "Admin", 1 },
            { "ESW CEETIS", 2 },
            { "ESW IVISionStudio", 3 },
            { "ESW Netstar", 4 },
            { "ESW Interne Tools", 5 },
            { "ESW Other", 6 },
            { "EHW HV Tester", 7 },
            { "EHW Konstruktion", 8 },
            { "EHW TPMs", 9 },
            { "EHW LV Tester", 10 },
            { "EHW Interne Tools", 11 },
            { "EHW Other", 12 },
            { "Dokumentation", 13 },
            { "Service", 14 },
            { "Vertrieb", 15 }
        };

        public Emailservice(IConfiguration config)
        {
            _config = config; //appsettings ist aktuell leer, da ich nicht die Firmenemail mitgeben kann
        }

        private int MapAreaOfResponsibility(string areaOfResponsibility)
        {
            return areasOfResponsibilityInDatabase[areaOfResponsibility];
        }
        public void SendEmail(string subject, string body, HashSet<string> emailaddresses)
        {
            try { 
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_config.GetSection("EmailUsername").Value));
                foreach(var emailAddress in emailaddresses)
                {
                    email.To.Add(MailboxAddress.Parse(emailAddress));
                }
                email.Subject = subject;
                email.Body = new TextPart(MimeKit.Text.TextFormat.Plain) { Text = body };
                using var smtp = new SmtpClient();                      
                smtp.Connect(_config.GetSection("EmailHost").Value, /*587*/ 25, MailKit.Security.SecureSocketOptions.Auto); //MailKit.Security.SecureSocketOptions.StartTls
                //smtp.Authenticate(_config.GetSection("EmailUsername").Value, _config.GetSection("EmailPassword").Value);
                smtp.Send(email);
                smtp.Disconnect(true);
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        public List<int> GetAreaOfResponsibilityIdsByRequirement(Requirement requirement)
        {
            List<int> result = new List<int>();

            if (requirement.ESW_CEETIS)
            {
                result.Add(MapAreaOfResponsibility("ESW CEETIS"));
            }
            if (requirement.ESW_IVISionStudio)
            {
                result.Add(MapAreaOfResponsibility("ESW IVISionStudio"));
            }
            if (requirement.ESW_Netstar)
            {
                result.Add(MapAreaOfResponsibility("ESW Netstar"));
            }
            if (requirement.ESW_InterneTools)
            {
                result.Add(MapAreaOfResponsibility("ESW Interne Tools"));
            }
            if (requirement.ESW_Other)
            {
                result.Add(MapAreaOfResponsibility("ESW Other"));
            }
            if (requirement.EHW_HV_Tester)
            {
                result.Add(MapAreaOfResponsibility("EHW HV Tester"));
            }
            if (requirement.EHW_Construction)
            {
                result.Add(MapAreaOfResponsibility("EHW Konstruktion"));
            }
            if (requirement.EHW_TPMs)
            {
                result.Add(MapAreaOfResponsibility("EHW TPMs"));
            }
            if (requirement.EHW_LV_Tester)
            {
                result.Add(MapAreaOfResponsibility("EHW LV Tester"));
            }
            if (requirement.EHW_InterneTools)
            {
                result.Add(MapAreaOfResponsibility("EHW Interne Tools"));
            }
            if (requirement.EHW_Other)
            {
                result.Add(MapAreaOfResponsibility("EHW Other"));
            }
            if (requirement.Documentation)
            {
                result.Add(MapAreaOfResponsibility("Dokumentation"));
            }
            if (requirement.Service)
            {
                result.Add(MapAreaOfResponsibility("Service"));
            }


            return result;
        }
    }
}
