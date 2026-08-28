namespace Check23.Models
{
    public class DatabaseFile
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? DataPath { get; set; }
        //public string? Category { get; set; }
        public int Folder_id { get; set; }
        public IFormFile? FileData { get; set; }

        public DatabaseFile(int id, string? name, string? dataPath, int folder_id, IFormFile? fileData)
        {
            Id = id;
            Name = name;
            DataPath = dataPath;
            //Category = category;
            Folder_id = folder_id;
            FileData = fileData;
        }

        public DatabaseFile(int id, string? name, string? dataPath, int folder_id)
        {
            Id = id;
            Name = name;
            DataPath = dataPath;
            //Category = category;
            Folder_id = folder_id;
        }

        public DatabaseFile()
        {
        }
    }
}
