using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Check23.Models
{
    public class User
    {
        public int Id { get; set; }

        [DisplayName("Windows Benutzername")]
        [Required(ErrorMessage = "Name wird benötigt und muss mit dem Windwos-Username übereinstimmen")]
        public string Name { get; set; }
        [DisplayName("Zugriffsgruppe")]
        [Required(ErrorMessage = "Zugriffsgruppe wird benötigt")]
        public int AccessGroup_Id { get; set; }

        public string Email { get; set; }
        [DisplayName("Zuständigkeitsbereiche (mehrere auswählbar)")]
        public IEnumerable<int> AreasOfResponsibility { get; set; }
        public IEnumerable<int> oldAreasOfResponsibility {  set; get; }

        public User()
        {
        }

        public User(int id, string name, string email, int accessGroup_Id)
        {
            Id = id;
            Name = name;
            Email = email;
            AccessGroup_Id = accessGroup_Id;
        }
        public User(string name, string email, int accessGroup_Id)
        {
            Name = name;
            Email = email;
            AccessGroup_Id = accessGroup_Id;
        }
    }
}
