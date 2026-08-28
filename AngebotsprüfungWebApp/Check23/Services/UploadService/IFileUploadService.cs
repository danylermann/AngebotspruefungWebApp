using Check23.Models;
using Microsoft.AspNetCore.WebUtilities;
using MimeKit;

namespace Check23.Services.UploadService
{
    public interface IFileUploadService
    {
        string GetUploadFolderName();
        string GetDirectAccessFilesFolderName();
        Task<DatabaseFile> UploadFileOld(MultipartReader reader, MultipartSection section);
        Task<DatabaseFile> UploadFile(string quoteEvalFolderName, IFormFile file);
        List<Folder> SetupDirectory(int quoteEvalId, string quoteEvalName);
        string ChangeDirectoryName(string oldName, string newName);
    }
}
