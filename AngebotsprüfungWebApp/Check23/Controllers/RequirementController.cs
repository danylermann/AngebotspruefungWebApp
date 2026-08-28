using Check23.Additional_Classes;
using Check23.Models;
using Check23.Services.AccessValidation;
using Check23.Services.DatabankAccess;
using Check23.Services.EmailService;
using Check23.Services.UploadService;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Diagnostics;

namespace Check23.Controllers
{
    public class RequirementController : Controller
    {
        private readonly IFileUploadService _fileUploadService;
        private readonly IAccessValidationService _accessValidator;
        private readonly IEmailService _emailService;
        private const string localAccessor = "requirement";
        public RequirementController(IFileUploadService fileUploadService, IAccessValidationService accessValidationService, IEmailService emailService) 
        {
            _fileUploadService = fileUploadService;
            _accessValidator = accessValidationService;
            _emailService = emailService;
        }
        public IActionResult CreateRequirement(int? quoteEvalId, string? quoteEvalName, Requirement? oldData)
        {
            Check23DAO check23DAO = new Check23DAO();
            if (Request.Cookies["Check23UserAccess"] == null)
            {
                string returnRoute = Request.GetEncodedUrl();
                return RedirectToAction("SaveUserAccessCookie", "Home", new { nextRoute = returnRoute });
            }
            else if (_accessValidator.Requirement(check23DAO.accessGroup.GetAccessGroupById(Convert.ToInt32(Request.Cookies["Check23UserAccess"]), check23DAO.connectionString)))
            {
                if (!String.IsNullOrEmpty(oldData.ErrorMessage))
                {                    
                    ViewBag.quoteEvalName = oldData.Name;
                    ViewBag.quoteEvalId = oldData.QuoteEvaluation_id;
                    return View(oldData);
                }
                ViewBag.quoteEvalName = quoteEvalName;
                ViewBag.quoteEvalId = quoteEvalId;
                //ViewBag.errorMessage = errorMessage;
                Requirement newReq = new Requirement();
                newReq.Name = quoteEvalName;
                return View(newReq);
            }
            else { return View("AccessDenied"); }
        }
        public IActionResult ProcessCreate(Requirement requirement)
        {
            int quoteEvalId = requirement.QuoteEvaluation_id;
            if (!ModelState.IsValid)
            {
                requirement.ErrorMessage = "Erstellung fehlgeschlagen. Mindestens ein Zuständigkeitsbereich muss ausgewählt werden.";
                return RedirectToAction("CreateRequirement", requirement);
            }            
            Check23DAO check23DAO = new Check23DAO();
            if(check23DAO.requirement.RequirementNameAlreadyExists(requirement.Name, quoteEvalId, check23DAO.connectionString))
            {
                return RedirectToAction("CreateRequirement", new { quoteEvalId, errorMessage = "Name bereits vorhanden" });
            }
            int requirementId = check23DAO.requirement.InsertRequirement(requirement, quoteEvalId, check23DAO.connectionString);
            int activityLogId = check23DAO.logging.GetActivityLogIdByQuouteEvaluationId(quoteEvalId, check23DAO.connectionString);
            LogEntry logEntry = new LogEntry("Created new requirement", requirementId, activityLogId);
            logEntry.Person = Request.Cookies["Check23Username"] ?? "No Username";
            check23DAO.logging.InsertLogEntry(logEntry, "requirement", check23DAO.connectionString);
            Solution standardSolution = new Solution("Wie beschrieben", "Wie beschrieben", false, requirementId);
            int tempId = check23DAO.solution.InsertSolution(standardSolution, requirementId, check23DAO.connectionString);
            check23DAO.estimation.InsertEstimation(tempId, check23DAO.connectionString);
            LogEntry autoLog = new LogEntry("Standard solution by System", tempId, activityLogId);
            autoLog.Person = "System";
            check23DAO.logging.InsertLogEntry(autoLog, "solution", check23DAO.connectionString);
            LogEntry logEntryEstimation = new LogEntry("Estimation created by System", tempId, activityLogId);
            logEntryEstimation.Person = "System";
            check23DAO.logging.InsertLogEntry(logEntryEstimation, "estimation", check23DAO.connectionString);
            List<int> areaOfResposibilityIds = _emailService.GetAreaOfResponsibilityIdsByRequirement(requirement);
            HashSet<string> emailAdresses = new HashSet<string>();
            foreach(int id in areaOfResposibilityIds)
            {
                emailAdresses.UnionWith(check23DAO.user.GetEmailadressesByAreaOfResponsibilityId(id, check23DAO.connectionString));
            }
            _emailService.SendEmail("Neue Anforderung bei Angebotsprüfung", "Eine neue Anforderung in ihrem Zuständigkeitsbereich wurde erstellt. Angebotsprüfungs-Id: " + quoteEvalId + " Link: " + "http://angebotspr-ew/QuoteEvaluation/DetailsQuoteEvaluation?quoteEvalId=" + quoteEvalId + " Name der Anforderung: " + requirement.Name + ". Bitte die Lösung abschätzen oder falls notwendig neue Lösungsvorschläge erstellen.", emailAdresses);
            return RedirectToAction("DetailsQuoteEvaluation", "QuoteEvaluation", new { quoteEvalId } ); //new { quoteEvalId } sind die routeValues als Object übertragen, da der Zielwert in der Controller Funktion eine quoteEvalId erwartet und der Wert den ich übertragen will schon quoteEvalId heißt muss ich nicht quoteEvalId = quoteEvalId schreiben sondern es reicht einfach QuoteEvalId
        }

        public IActionResult DetailsRequirement(int requirementId, string? fileUploadMessage)
        {
            Check23DAO check23DAO = new Check23DAO();
            Requirement foundRequirement = check23DAO.requirement.GetRequirementById(requirementId, check23DAO.connectionString);
            int folderId = check23DAO.fileUpload.GetSpecifiedFolderIdByAccessorId(_fileUploadService.GetUploadFolderName(), requirementId, localAccessor, check23DAO.connectionString);
            foundRequirement.files = check23DAO.fileUpload.GetAllUploadedFiles(folderId, check23DAO.connectionString);
            foundRequirement.solutions = check23DAO.solution.GetSolutionsByRequirementId(requirementId, check23DAO.connectionString);
            foundRequirement.comments = check23DAO.comment.GetRequirementCommentsByRequirementId(requirementId, check23DAO.connectionString);
            foreach (var solution in foundRequirement.solutions)
            {
                solution.estimation = check23DAO.estimation.GetEstimationBySolutionId(solution.Id, check23DAO.connectionString);
            }
            ViewBag.quoteEvalName = check23DAO.quoteEvaluation.GetQuoteEvaluationById(foundRequirement.QuoteEvaluation_id, check23DAO.connectionString).Name;
            if (!string.IsNullOrEmpty(fileUploadMessage))
            {
                ViewBag.fileUploadMessage = fileUploadMessage;
            }
            return View(foundRequirement);
        }

        public IActionResult EditRequirement(int requirementId, string? errorMessage)
        {
            Check23DAO check23DAO = new Check23DAO();
            if (Request.Cookies["Check23UserAccess"] == null)
            {
                string returnRoute = Request.GetEncodedUrl();
                return RedirectToAction("SaveUserAccessCookie", "Home", new { nextRoute = returnRoute });
            }
            else if (_accessValidator.Requirement(check23DAO.accessGroup.GetAccessGroupById(Convert.ToInt32(Request.Cookies["Check23UserAccess"]), check23DAO.connectionString)))
            {
                Requirement foundRequirement = check23DAO.requirement.GetRequirementById(requirementId, check23DAO.connectionString);
                ViewBag.validationError = errorMessage;
                return View(foundRequirement);
            }
            else { return View("AccessDenied"); }
        }

        public IActionResult ProcessEdit(Requirement requirement, string oldName)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("EditRequirement", new { requirementId = requirement.Id, errorMessage = "Bearbeitung fehlgeschlagen, Eingaben waren nicht gültig" });
            }
            Check23DAO check23DAO = new Check23DAO();
            if (oldName != requirement.Name)
            {
                if(check23DAO.requirement.RequirementNameAlreadyExists(requirement.Name, requirement.QuoteEvaluation_id, check23DAO.connectionString))
                {
                    return RedirectToAction("EditRequirement", new { requirementId = requirement.Id, errorMessage = "Name bereits vorhanden" });
                }
            }            
            check23DAO.requirement.UpdateRequirement(requirement, check23DAO.connectionString);
            LogEntry logEntry = new LogEntry("Edited requirement", requirement.Id, check23DAO.logging.GetActivityLogIdByQuouteEvaluationId(requirement.QuoteEvaluation_id, check23DAO.connectionString));
            logEntry.Person = Request.Cookies["Check23Username"] ?? "No Username";
            check23DAO.logging.InsertLogEntry(logEntry, "requirement", check23DAO.connectionString);
            return RedirectToAction("DetailsRequirement", new { requirementId = requirement.Id });
        }
        public async Task<IActionResult> UploadFile(int requirementId, [FromForm] IFormFile file)
        {
            Check23DAO check23DAO = new Check23DAO();
            Requirement currentRequirement = check23DAO.requirement.GetRequirementById(requirementId, check23DAO.connectionString);
            QuoteEvaluation quoteEval = check23DAO.quoteEvaluation.GetQuoteEvaluationById(currentRequirement.QuoteEvaluation_id, check23DAO.connectionString);
            string quoteEvalFolder = quoteEval.Name + "_" + quoteEval.Id.ToString();
            DatabaseFile newFile = await _fileUploadService.UploadFile(quoteEvalFolder, file);
            string? fileUploadMessage;
            if ((newFile.Name == "error" || string.IsNullOrEmpty(newFile.Name)) || (newFile.DataPath == "error" || string.IsNullOrEmpty(newFile.DataPath)))
            {
                fileUploadMessage = "Beim Hochladen der Datei ist ein Fehler aufgetreten";
            }
            else
            {
                fileUploadMessage = "Datei erfolgreich hochgeladen";
                newFile.Folder_id = check23DAO.fileUpload.GetSpecifiedFolderIdByAccessorId(_fileUploadService.GetUploadFolderName(), currentRequirement.Id, localAccessor, check23DAO.connectionString);
                check23DAO.fileUpload.InsertFile(newFile, check23DAO.connectionString);
            }
            return RedirectToAction("DetailsRequirement", new { requirementId, fileUploadMessage });
        }

        public IActionResult DeleteFile(int fileId, int requirementId)
        {
            Check23DAO check23DAO = new Check23DAO();
            DatabaseFile file = check23DAO.fileUpload.GetFileById(fileId, check23DAO.connectionString);
            if (string.IsNullOrEmpty(file.DataPath))
            {
                return RedirectToAction("DetailsRequirement", new { requirementId });
            }
            System.IO.File.Delete(file.DataPath);
            check23DAO.fileUpload.DeleteFile(fileId, check23DAO.connectionString);
            return RedirectToAction("DetailsRequirement", new { requirementId });
        }

        public PhysicalFileResult DownloadFile(DatabaseFile file)
        {
            bool hasContentType = new FileExtensionContentTypeProvider().TryGetContentType(file.Name, out var contentType);
            if (hasContentType)
            {
                return PhysicalFile(file.DataPath, contentType, file.Name);
            }
            else
            {
                throw new Exception("File not found");
            }
        }

        public IActionResult OpenExplorer(int requirementId)
        {
            Check23DAO check23DAO = new Check23DAO();
            Requirement temp = check23DAO.requirement.GetRequirementById(requirementId, check23DAO.connectionString);
            int directAccessFilesFolderId = check23DAO.fileUpload.GetSpecifiedFolderIdByAccessorId(_fileUploadService.GetDirectAccessFilesFolderName(), temp.Id, localAccessor, check23DAO.connectionString);
            Folder directAccessFilesFolder = check23DAO.fileUpload.GetFolderById(directAccessFilesFolderId, check23DAO.connectionString);
            string directAccessFilesFolderPath = directAccessFilesFolder.FolderPath;
            Process.Start("explorer", directAccessFilesFolderPath);
            return RedirectToAction("DetailsRequirement", new { requirementId });
        }
    }
}
