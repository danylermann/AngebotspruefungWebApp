using ExpressiveAnnotations.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Check23.Models
{
    public class Requirement
    {
        public int Id { get; set; }
     
        [Required(ErrorMessage = "Name wird benötigt")]
        public string Name { get; set; }

        [DisplayName("Beschreibung der Anforderung")]
        public string? Description { get; set; }

        [DisplayName("Angebotsnummer")]
        public string? OrderNumber { get; set; }
        [AssertThat("ESW_CEETIS == true || ESW_IVISionStudio == true || ESW_Netstar == true || ESW_InterneTools == true || ESW_Other == true || EHW_HV_Tester == true || EHW_Construction == true || EHW_TPMs == true || EHW_LV_Tester == true || EHW_InterneTools == true || EHW_Other == true || CDE == true || Documentation == true || Service == true", ErrorMessage = "Bitte mindestens eine der Abteilungen auswählen")]
        [DisplayName("ESW CEETIS")]
        public bool ESW_CEETIS { get; set; }
        [DisplayName("ESW IVISionStudio")]
        public bool ESW_IVISionStudio { get; set; }
        [DisplayName("ESW Netstar")]
        public bool ESW_Netstar { get; set; }
        [DisplayName("ESW Interne Tools")]
        public bool ESW_InterneTools { get; set; }
        [DisplayName("ESW Other")]
        public bool ESW_Other { get; set; }
        [DisplayName("EHW HV Tester")]
        public bool EHW_HV_Tester { get; set; }
        [DisplayName("EHW Konstruktion")]
        public bool EHW_Construction { get; set; }
        [DisplayName("EHW TPMs")]
        public bool EHW_TPMs { get; set; }
        [DisplayName("EHW LV Tester")]
        public bool EHW_LV_Tester { get; set; }
        [DisplayName("EHW Interne Tools")]
        public bool EHW_InterneTools { get; set; }
        [DisplayName("EHW Other")]
        public bool EHW_Other { get; set; }
        public bool CDE { get; set; }
        [DisplayName("Dokumentation")]
        public bool Documentation { get; set; }
        public bool Service { get; set; }

        [DisplayName("Nicht zu bearbeiten")]
        public bool NotToBeCarriedOut { get; set; }
        public int QuoteEvaluation_id { get; set; }

        public List<Solution> solutions = new List<Solution> { };

        public List<DatabaseFile> files = new List<DatabaseFile> { };
        
        public List<Comment> comments = new List<Comment> { };

        public string? ErrorMessage {  get; set; }

        //Konstruktor für Datenbankauslesung
        public Requirement(int id, 
            string name, 
            string? description, 
            string? orderNumber, 
            bool eSW_CEETIS, 
            bool eSW_IVISionStudio, 
            bool eSW_Netstar, 
            bool eSW_InternelTools, 
            bool eSW_Other, 
            bool eHW_HV_Tester, 
            bool eHW_Construction, 
            bool eHW_TPMs, 
            bool eHW_LV_Tester, 
            bool eHW_InternelTools, 
            bool eHW_Other, 
            bool cDE, 
            bool documentation,
            bool service,
            bool notToBeCarriedOut, 
            int quouteEvaluation_id)
        {
            Id = id;
            Name = name;
            Description = description;
            OrderNumber = orderNumber;
            ESW_CEETIS = eSW_CEETIS;
            ESW_IVISionStudio = eSW_IVISionStudio;
            ESW_Netstar = eSW_Netstar;
            ESW_InterneTools = eSW_InternelTools;
            ESW_Other = eSW_Other;
            EHW_HV_Tester = eHW_HV_Tester;
            EHW_Construction = eHW_Construction;
            EHW_TPMs = eHW_TPMs;
            EHW_LV_Tester = eHW_LV_Tester;
            EHW_InterneTools = eHW_InternelTools;
            EHW_Other = eHW_Other;
            CDE = cDE;
            Documentation = documentation;
            Service = service;
            NotToBeCarriedOut = notToBeCarriedOut;
            QuoteEvaluation_id = quouteEvaluation_id;
        }
        //Konstruktor für das Erstelen
        public Requirement(string name, 
            string? description, 
            string? orderNumber, 
            bool eSW_CEETIS, 
            bool eSW_IVISionStudio, 
            bool eSW_Netstar, 
            bool eSW_InternelTools, 
            bool eSW_Other, 
            bool eHW_HV_Tester, 
            bool eHW_Construction, 
            bool eHW_TPMs, 
            bool eHW_LV_Tester, 
            bool eHW_InternelTools, 
            bool eHW_Other, 
            bool cDE, 
            bool documentation, 
            bool service,
            bool notToBeCarriedOut, 
            int quouteEvaluation_id)
        {
            Name = name;
            Description = description;
            OrderNumber = orderNumber;
            ESW_CEETIS = eSW_CEETIS;
            ESW_IVISionStudio = eSW_IVISionStudio;
            ESW_Netstar = eSW_Netstar;
            ESW_InterneTools = eSW_InternelTools;
            ESW_Other = eSW_Other;
            EHW_HV_Tester = eHW_HV_Tester;
            EHW_Construction = eHW_Construction;
            EHW_TPMs = eHW_TPMs;
            EHW_LV_Tester = eHW_LV_Tester;
            EHW_InterneTools = eHW_InternelTools;
            EHW_Other = eHW_Other;
            CDE = cDE;
            Documentation = documentation;
            Service = service;
            NotToBeCarriedOut = notToBeCarriedOut;
            QuoteEvaluation_id = quouteEvaluation_id;
        }
        //Konstruktor für Details
        public Requirement()
        {

        }
    }
}
