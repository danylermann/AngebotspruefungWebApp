namespace Check23.Models
{
    public class ActivityLog
    {
        public int Id { get; set; }
        public int QuoteEvaluation_id { get; set; }

        public List<LogEntry> logEntries = new List<LogEntry>();
        //Konstruktor für Datenbankauslesung
        public ActivityLog(int id, int quoteEvaluation_id)
        {
            Id = id;
            QuoteEvaluation_id = quoteEvaluation_id;
        }
        //Konstruktor für das Erstellen
        public ActivityLog(int angebotsprüfung_id)
        {
            QuoteEvaluation_id = angebotsprüfung_id;
        }

        public ActivityLog() { }
    }
}
