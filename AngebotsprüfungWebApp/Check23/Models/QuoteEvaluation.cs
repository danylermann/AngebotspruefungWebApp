using Microsoft.CodeAnalysis;
using Org.BouncyCastle.Asn1.Ocsp;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using System.Web;

namespace Check23.Models
{
    public class QuoteEvaluation
    {
        public int Id { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "Name wird benötigt")]
        public string Name { get; set; }

        [DisplayName("Ersteller")]
        public string Creator { get; set; } = "No Username";

        [DisplayName("Datum")]
        public DateTime Date { get; set; } = DateTime.Now;

        [DisplayName("Gesetzliche Vorgaben")]
        public string? LegalGuidelines { get; set; }


        [DisplayName("Externer Ansprechpartner")]
        public string? ExternalContact { get; set; }

        [DisplayName("Kategorie")]
        [Required(ErrorMessage = "Auftragstyp wird benötigt")]
        public int OrderCategory { get; set; }
        [DisplayName("Status")]
        public int Status { get; set; } = 1;

        [DisplayName("Kunde")]
        public int? Client_id { get; set; }

        public Client? client;

        public ActivityLog? activityLog;
        
        public List<Requirement> requirements = new List<Requirement>();

        public List<DatabaseFile> files = new List<DatabaseFile>();

        public List<Ticket> tickets = new List<Ticket>();

        //Konstruktor für Datenbankauslesung
        public QuoteEvaluation(
            int id, 
            string name,
            string creator,
            DateTime date,
            string? legalGuidelines,
            string? externalContact,
            int orderCategory,
            int status,
            int? client_id)
        {
            Id = id;
            Name = name;
            Creator = creator;
            Date = date;
            LegalGuidelines = legalGuidelines;
            ExternalContact = externalContact;
            OrderCategory = orderCategory;
            Status = status;
            Client_id = client_id;
        }

        //Konstruktor für das Erstellen
        public QuoteEvaluation(
            string name,
            string? legalGuidelines,
            string? externalContact,
            int orderCategory)
        {
            Name = name;
            LegalGuidelines = legalGuidelines;
            ExternalContact = externalContact;
            OrderCategory = orderCategory;
        }

        //Leerer Konstruktor für Details
        public QuoteEvaluation() { }
    }


}
