using System.Security.Principal;

namespace Check23.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Creator { get; set; } = "No Username";
        public DateTime Date { get; set; } = DateTime.Now;
        public string Message { get; set; }
        public int ForeignKey_id { get; set; }

        public Comment(int id, string creator, DateTime date, string message, int foreignKey_id)
        {
            Id = id;
            Creator = creator;
            Date = date;
            Message = message;
            ForeignKey_id = foreignKey_id;
        }

        public Comment() { }
    }
}
