using System.ComponentModel;

namespace Check23.Models
{
    public class Estimation
    {
        

        public int Id { get; set; }
        [DisplayName("ESW Zeitaufwand")]
        public string? ESW_time { get; set; }
		[DisplayName("ESW Zeitaufwand Beschreibung")]
		public string? ESW_time_description { get; set; }
		[DisplayName("ESW Kosten")]
        public string? ESW_cost { get; set; }
		[DisplayName("ESW Kosten Beschreibung")]
		public string? ESW_cost_description { get; set; }
		[DisplayName("EHW Zeitaufwand")]
        public string? EHW_time { get; set; }
		[DisplayName("EHW Zeitaufwand Beschreibung")]
		public string? EHW_time_description { get; set; }
		[DisplayName("EHW Kosten")]
        public string? EHW_cost { get; set; }
		[DisplayName("EHW Kosten Beschreibung")]
		public string? EHW_cost_description { get; set; }
		[DisplayName("CDE Zeitaufwand")]
        public string? CDE_time { get; set; }
        [DisplayName("CDE Kosten")]
        public string? CDE_cost { get; set; }
        [DisplayName("Dokumentation Zeitaufwand")]
        public string? Documentation_time { get; set; }
		[DisplayName("Dokumentation Zeitaufwand Beschreibung")]
		public string? Documentation_time_description { get; set; }
		[DisplayName("Dokumentation Kosten")]
        public string? Documentation_cost { get; set; }
		[DisplayName("Dokumentation Kosten Beschreibung")]
		public string? Documentation_cost_description { get; set; }
		[DisplayName("Service Zeitaufwand")]
        public string? Service_time { get; set; }
		[DisplayName("Service Zeitaufwand Beschreibung")]
		public string? Service_time_description { get; set; }
		[DisplayName("Service Kosten")]
        public string? Service_cost { get; set; }
		[DisplayName("Service Kosten Beschreibung")]
		public string? Service_cost_description { get; set; }
		public int Solution_id { get; set; }
        public string TimeFrame {  get; set; }

        //Konstruktor für das Erstellen
        public Estimation(int id,
            string? eSW_time,
            string? eSW_time_description,
            string? eSW_cost,
            string? eSW_cost_description,
            string? eHW_time,
            string? eHW_time_description,
            string? eHW_cost,
            string? eHW_cost_description,
            string? cDE_time,
            string? cDE_cost,
            string? documentation_time,
            string? documentation_time_description,
            string? documentation_cost,
            string? documentation_cost_description,
            string? service_time,
            string? service_time_description,
            string? service_cost,
            string? service_cost_description,
            int solution_id)
        {
            Id = id;
            ESW_time = eSW_time;
            ESW_time_description = eSW_time_description;
            ESW_cost = eSW_cost;
            ESW_cost_description = eSW_cost_description;
            EHW_time = eHW_time;
            EHW_time_description = eHW_time_description;
            EHW_cost = eHW_cost;
            EHW_cost_description = eHW_cost_description;
            CDE_time = cDE_time;
            CDE_cost = cDE_cost;
            Documentation_time = documentation_time;
            Documentation_time_description = documentation_time_description;
            Documentation_cost = documentation_cost;
            Documentation_cost_description = documentation_cost_description;
            Service_time = service_time;
            Service_time_description = service_time_description;
            Service_cost = service_cost;
            Service_cost_description = service_cost_description;
            Solution_id = solution_id;
        }

        public Estimation() { }
    }
}
