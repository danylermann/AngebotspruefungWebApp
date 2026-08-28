using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Check23.Models
{
    public class Solution
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name wird benötigt")]
        public string Name { get; set; }

        [DisplayName("Beschreibung der Lösung")]
        [Required(ErrorMessage = "Beschreibung wird benötigt")]
        public string Description { get; set; }

        [DisplayName("Ausgewählt")]
        public bool IsSelected { get; set; }
        public int Requirement_id { get; set; }

        public Estimation estimation = new Estimation();

        public List<DatabaseFile> files = new List<DatabaseFile> { };

        public List<Comment> comments = new List<Comment>();

        //Konstruktor für Details
        public Solution() { }

        //Konstruktor für das Erstellen
        public Solution(string name, string description, bool isSelected, int requirement_id)
        {
            Name = name;
            Description = description;
            IsSelected = isSelected;
            Requirement_id = requirement_id;
        }
        //Konstruktor für Datenbankauslesung
        public Solution(int id, string name, string description, bool isSelected, int requirement_id)
        {
            Id = id;
            Name = name;
            Description = description;
            IsSelected = isSelected;
            Requirement_id = requirement_id;
        }
    }
}
