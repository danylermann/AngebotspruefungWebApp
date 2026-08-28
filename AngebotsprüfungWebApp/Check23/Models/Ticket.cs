using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Check23.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        [DisplayName("Nummer")]
        [Required(ErrorMessage = "Nummer wird benötigt")]
        public string Number { get; set; }
        [DisplayName("Art des Tickets")]
        [Required(ErrorMessage = "Typ wird benötigt")]
        public string Type { get; set; }
        public int QuoteEvaluation_id { get; set; }

        public Ticket(){}
        //Konstruktor zum Erstellen
        public Ticket(string number, string type, int quoteEvaluation_id)
        {
            Number = number;
            Type = type;
            QuoteEvaluation_id = quoteEvaluation_id;
        }
        //Konstruktor für Datenbankauslesung
        public Ticket(int id, string number, string type, int quoteEvaluation_id)
        {
            Id = id;
            Number = number;
            Type = type;
            QuoteEvaluation_id = quoteEvaluation_id;
        }
    }
}
