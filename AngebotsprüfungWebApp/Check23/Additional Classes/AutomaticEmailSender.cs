using Check23.Models;
using Check23.Services.DatabankAccess;
using Check23.Services.EmailService;
using System.Runtime.CompilerServices;
using System.Text;

namespace Check23.Additional_Classes
{
    public class AutomaticEmailSender
    {
        private HashSet<string> EmailAddresses = new HashSet<string>();
        private bool ESWCEETIStemp = false;
        private bool ESWIVIsiontemp = false;
        private bool ESWNettemp = false;
        private bool ESWITtemp = false;
        private bool ESWSonsttemp = false;
        private bool EHWHVtemp = false;
        private bool EHWLVtemp = false;
        private bool EHWKonsttemp = false;
        private bool EHWTPMstemp = false;
        private bool EHWITtemp = false;
        private bool EHWSonsttemp = false;
        private bool CDEtemp = false;
        private bool Dokutemp = false;

        private HashSet<string> FetchEmailAddressEntwicklung(QuoteEvaluation Ap)
        {
            HashSet<string> result = new HashSet<string>();

            foreach(Requirement anforderung in Ap.requirements) 
            {
                if (anforderung.ESW_CEETIS)
                {
                    ESWCEETIStemp = true;
                }
                if (anforderung.ESW_IVISionStudio)
                {
                    ESWIVIsiontemp = true;
                }
                if (anforderung.ESW_Netstar)
                {
                    ESWNettemp = true;
                }
                if(anforderung.ESW_InterneTools)
                {
                    ESWITtemp = true;
                }
                if (anforderung.ESW_Other)
                {
                    ESWSonsttemp = true;
                }
                if(anforderung.EHW_HV_Tester)
                {
                    EHWHVtemp = true;
                }
                if (anforderung.EHW_LV_Tester)
                {
                    EHWLVtemp = true;
                }
                if (anforderung.EHW_Construction)
                {
                    EHWKonsttemp = true;
                }
                if (anforderung.EHW_TPMs)
                {
                    EHWTPMstemp = true;
                }
                if (anforderung.EHW_InterneTools)
                {
                    EHWITtemp = true;
                }
                if (anforderung.EHW_Other)
                {
                    EHWSonsttemp = true;
                }
                if (anforderung.CDE)
                {
                    CDEtemp = true;
                }
                if (anforderung.Documentation)
                {
                    Dokutemp = true;
                }

            }

            StringBuilder sqlStatement = new StringBuilder();
            sqlStatement.Append("SELECT Adresse " +
                "FROM e_mail_address mail " +
                "JOIN specification_has_e_mail_address speci_has_mail ON mail.id = speci_has_mail.e_mail_address_id " +
                "JOIN Abteil specification on speci_has_mail.specification_id = specification.id" +
                "WHERE ");

            //Namen sind Abhängig von dem tatsächlichen Inhalt in der Datenbank. Name von specification muss mit den Werten hier übereinstimmen.
            if (ESWCEETIStemp)
            {
                sqlStatement.Append("abteil.Name = 'ESW:CEETIS' OR ");
            }
            if (ESWIVIsiontemp)
            {
                sqlStatement.Append("abteil.Name = 'ESW:IVISionStudio' OR ");
            }
            if (ESWNettemp)
            {
                sqlStatement.Append("abteil.Name = 'ESW:Netstar' OR ");
            }
            if (ESWITtemp)
            {
                sqlStatement.Append("abteil.Name = 'ESW:InterneTools' OR ");
            }
            if (ESWSonsttemp)
            {
                sqlStatement.Append("abteil.Name = 'ESW:Sonstiges' OR ");
            }
            if (EHWHVtemp)
            {
                sqlStatement.Append("abteil.Name = 'EHW:HVTester' OR ");
            }
            if (EHWLVtemp)
            {
                sqlStatement.Append("abteil.Name = 'EHW:LVTester' OR ");
            }
            if (EHWKonsttemp)
            {
                sqlStatement.Append("abteil.Name = 'EHW:Konstruktion' OR ");
            }
            if (EHWTPMstemp)
            {
                sqlStatement.Append("abteil.Name = 'EHW:TPMs' OR ");
            }
            if (EHWITtemp)
            {
                sqlStatement.Append("abteil.Name = 'EHW:InterneTools' OR ");
            }
            if (EHWSonsttemp)
            {
                sqlStatement.Append("abteil.Name = 'EHW:Sonstiges' OR ");
            }
            if (CDEtemp)
            {
                sqlStatement.Append("abteil.Name = 'CDE' OR ");
            }
            if (Dokutemp)
            {
                sqlStatement.Append("abteil.Name = 'Dokumentation' OR ");
            }
            sqlStatement.Append("abteil.Name = IS Null");

            //Datenbank verbinden und Mailadressen mit sqlCommand auslesen
            Check23DAO check23DAO = new Check23DAO();
            List<string> DatabankMailaddresses = check23DAO.GetEmailaddresses(sqlStatement.ToString());
            //Hinzufügen der Adressen in Hashset
            foreach (string address in DatabankMailaddresses)
            {
                result.Add(address);
            }

            return result;
        }

        private HashSet<string> FetchEmailAddressVertrieb()
        {
            HashSet<string> result = new HashSet<string>();
            string sqlCommand = "SELECT Adresse " +
                "FROM check23.EMailadresse mail " +
                "JOIN check23.Abteil_has_EMailadresse AhE ON mail.id = AhE.EMailadresse_id " +
                "JOIN check23.Abteil abteil on AhE.Abteil_id = abteil.id" +
                "WHERE abteil.Name = Vertrieb";

            //Datenbank verbinden und Mailadressen mit sqlCommand auslesen
            //Hinzufügen der Adressen in Hashset
             return result;
        }
        public void SendAutomatedEmail(bool entwicklung, bool vertrieb, QuoteEvaluation Ap, IEmailService emailService, string subject, string body)
        {
            if (entwicklung)
            {
                EmailAddresses = FetchEmailAddressEntwicklung(Ap);
            }
            if (vertrieb)
            {
                EmailAddresses = FetchEmailAddressVertrieb();
            }
            emailService.SendEmail(subject, body, EmailAddresses);
        }
        
    }
}
