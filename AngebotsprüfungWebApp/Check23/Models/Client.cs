using System.ComponentModel;

namespace Check23.Models
{
    public class Client
    {
        public int Id { get; set; }
        [DisplayName("Firmen- oder Kundenname")]
        public string Name { get; set; }
        [DisplayName("Standort (optional)")]
        public string? Location { get; set; }

        public Client (int id, string name, string? location)
        {
            Id = id;
            Name = name;
            Location = location;
        }

        //Leerer Konstruktor mit Default um Fehler beim Auslesen der Kunden zu ermitteln
        public Client() { Name = "error"; }
    }
}
