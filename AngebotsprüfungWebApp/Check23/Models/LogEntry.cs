using System.Security.Principal;

namespace Check23.Models
{
    public class LogEntry
    {
        public int Id { get; set; }
        public string Person { get; set; } = "No Username";
        public DateTime Date { get; set; } = DateTime.Now;
        public string Activity { get; set; }
        public int ForeignKey_id { get; set; }
        public int ActivityLog_id { get; set; }

        //Konstruktor Datenbankauslesung
        public LogEntry(int id, string person, DateTime date, string activity, int foreignKey_id, int activityLog_id)
        {
            Id = id;
            Person = person;
            Date = date;
            Activity = activity;
            ForeignKey_id = foreignKey_id;
            ActivityLog_id = activityLog_id;
        }

        //Konstruktor zum Erstellen
        public LogEntry(string activity, int foreignKey_id, int activityLog_id)
        {
            Activity = activity;
            ForeignKey_id = foreignKey_id;
            ActivityLog_id = activityLog_id;
        }

        //Konstruktor für das Erstellen des Protokolls
        public LogEntry(string person, DateTime date, string activity)
        {
            Person = person;
            Date = date;
            Activity = activity;
        }
    }
}
