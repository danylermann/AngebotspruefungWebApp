namespace Check23.Models
{
    public class Folder
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FolderPath { get; set; }
        public int QuoteEvaluationId { get; set; }

        public Folder(int id, string name, string folderPath, int quoteEvaluationId)
        {
            Id = id;
            Name = name;
            FolderPath = folderPath;
            QuoteEvaluationId = quoteEvaluationId;
        }

        public Folder()
        {
        }

        public Folder(string name, string folderPath, int quoteEvaluationId)
        {
            Name = name;
            FolderPath = folderPath;
            QuoteEvaluationId = quoteEvaluationId;
        }
    }
}
