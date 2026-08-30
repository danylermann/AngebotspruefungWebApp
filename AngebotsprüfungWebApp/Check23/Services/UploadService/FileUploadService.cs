using Check23.Models;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Quic;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using Microsoft.VisualBasic.FileIO;

namespace Check23.Services.UploadService
{
    public class FileUploadService : IFileUploadService
    {
        private const string fileServerPath = "C:\\Users\\User\\Desktop\\TestFiles";
        private const string uploadFolderName = "UploadedFiles";        
        private const string directAccessFilesFolderName = "DirectAccessFiles";
        public string GetUploadFolderName()
        {
            return uploadFolderName;
        }
        public string GetDirectAccessFilesFolderName()
        {
            return directAccessFilesFolderName;
        }
        

        public List<Folder> SetupDirectory(int quoteEvalId, string quoteEvalName)
        {
            List<Folder> createdFolders = new List<Folder>();
            string cleanedQuoteEvalName = quoteEvalName;
            foreach(char c in Path.GetInvalidFileNameChars())
            {
                cleanedQuoteEvalName = cleanedQuoteEvalName.Replace(c, '_');
            }
            string quoteEvalFolderPath = Path.Combine(fileServerPath, cleanedQuoteEvalName + "_" + quoteEvalId.ToString());
            Directory.CreateDirectory(quoteEvalFolderPath);
            string uploadFileFolderPath = Path.Combine(quoteEvalFolderPath, uploadFolderName);
            Directory.CreateDirectory(uploadFileFolderPath);
            createdFolders.Add(new Folder(uploadFolderName, uploadFileFolderPath, quoteEvalId));
            string directAccessFilesFolderPath = Path.Combine(quoteEvalFolderPath, directAccessFilesFolderName);
            Directory.CreateDirectory(directAccessFilesFolderPath);
            createdFolders.Add(new Folder(directAccessFilesFolderName, directAccessFilesFolderPath, quoteEvalId));
            return createdFolders;
        }
        public async Task<DatabaseFile> UploadFileOld(MultipartReader reader, MultipartSection section)
        {
            DatabaseFile file = null;        
            while(section != null)
            {
                var hasContentDispostionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition);

                if (hasContentDispostionHeader)
                {
                    if(contentDisposition.DispositionType.Equals("form-data") &&
                        (!string.IsNullOrEmpty(contentDisposition.FileName.Value)) ||
                        !string.IsNullOrEmpty(contentDisposition.FileNameStar.Value))
                    {                        
                        byte[] fileArray;
                        string filePath = Path.Combine(fileServerPath, "id+name of quoteEvalution as folder", uploadFolderName);
                        string resultingFileName = contentDisposition.FileName.Value;                        
                        for (int i = 1; File.Exists(Path.Combine(filePath, resultingFileName)); i++)
                        {
                            resultingFileName = Path.GetFileNameWithoutExtension(contentDisposition.FileName.Value) + i.ToString() + Path.GetExtension(contentDisposition.FileName.Value);                            
                        }
                        string resultingFilePath = Path.Combine(filePath, resultingFileName);
                        using (var memoryStream = new MemoryStream())
                        {
                            await section.Body.CopyToAsync(memoryStream);
                            fileArray = memoryStream.ToArray();
                        }
                        using (var fileStream = File.Create(resultingFilePath))
                        {
                            await fileStream.WriteAsync(fileArray);
                        }                        
                        file = new DatabaseFile(-1, resultingFileName, resultingFilePath, -1, null);
                    }
                }
            }           
            return file;
        }
        public async Task<DatabaseFile> UploadFile(string quoteEvalFolderName, IFormFile file)
        {
            string filePath = Path.Combine(fileServerPath, quoteEvalFolderName, uploadFolderName);
            string resultingFileName = "";
            string resultingPath = "";
            try
            {
                if(file != null && file.Length > 0)
                {
                    resultingFileName = file.FileName;
                    for(int i = 1; File.Exists(Path.Combine(filePath, resultingFileName)); i++)
                    {
                        resultingFileName = Path.GetFileNameWithoutExtension(file.FileName) + i.ToString() + Path.GetExtension(file.FileName);
                    }
                    resultingPath = Path.Combine(filePath, resultingFileName);
                    using (var fileStream = new FileStream(resultingPath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.ToString());
                resultingPath = "error";
                resultingFileName = "error";
            }
            return new DatabaseFile(-1, resultingFileName, resultingPath, -1);
        }

        public string ChangeDirectoryName(string oldName, string newName)
        {
            try
            {
                FileSystem.RenameDirectory(Path.Combine(fileServerPath, oldName), newName);
            }
            catch (DirectoryNotFoundException dirEx)
            {
                string id = newName.Substring(newName.LastIndexOf('_') + 1);
                string[] folderWithIdMatch = Directory.GetDirectories(fileServerPath, "*_" + id, System.IO.SearchOption.TopDirectoryOnly);
                if(folderWithIdMatch.Count() != 1)
                {
                    throw new Exception("Multiple matching ids or no match found, fileserver directory in incorrect state. Service currently unavailable");
                }                
                FileSystem.RenameDirectory(folderWithIdMatch[0], newName);
            }
            string newPath = Path.Combine(fileServerPath, newName);
            return newPath;
        }
    }
}
