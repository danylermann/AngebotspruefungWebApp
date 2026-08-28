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
    public class SolutionController : Controller
    {
        private readonly IFileUploadService _fileUploadService;
        private readonly IAccessValidationService _accessValidator;
        private readonly IEmailService _emailService;
        private const string localAccessor = "solution";
        public SolutionController(IFileUploadService fileUploadService, IAccessValidationService accessValidationService, IEmailService emailService)
        {
            _fileUploadService = fileUploadService;
            _accessValidator = accessValidationService;
            _emailService = emailService;
        }
        public IActionResult CreateSolution(int requirementId, string? errorMessage)
        {            
            Check23DAO check23DAO = new Check23DAO();
            if (Request.Cookies["Check23UserAccess"] == null)
            {
                string returnRoute = Request.GetEncodedUrl();
                return RedirectToAction("SaveUserAccessCookie", "Home", new { nextRoute = returnRoute });
            }
            else if (_accessValidator.Solution(check23DAO.accessGroup.GetAccessGroupById(Convert.ToInt32(Request.Cookies["Check23UserAccess"]), check23DAO.connectionString)))
            {
                ViewBag.requirementId = requirementId;
                ViewBag.errorMessage = errorMessage;
                Requirement requirement = check23DAO.requirement.GetRequirementById(requirementId, check23DAO.connectionString);
                ViewBag.requirementName = requirement.Name;
                QuoteEvaluation quoteEvaluation = check23DAO.quoteEvaluation.GetQuoteEvaluationById(requirement.QuoteEvaluation_id, check23DAO.connectionString);
                ViewBag.quoteEvalName = quoteEvaluation.Name;
                return View();
            }
            else { return View("AccessDenied"); }
        }

        public IActionResult ProcessCreate(Solution solution)
        {
            int requirementId = solution.Requirement_id;
            if (!ModelState.IsValid)
            {
                return RedirectToAction("CreateSolution", new { requirementId, errorMessage = "Erstellung fehlgeschlagen, Eingaben waren nicht gültig" });
            }
            Check23DAO check23DAO = new Check23DAO();
            if (check23DAO.solution.SolutionNameAlreadyExists(solution.Name, requirementId, check23DAO.connectionString))
            {
                return RedirectToAction("CreateSolution", new { requirementId, errorMessage = "Name bereits vorhanden" });
            }
            int lastCreatedId = check23DAO.solution.InsertSolution(solution, requirementId, check23DAO.connectionString);
            check23DAO.estimation.InsertEstimation(lastCreatedId, check23DAO.connectionString); //Automatisches generieren der Abschätzung
            int activityLogId = check23DAO.logging.GetActivityLogIdByRequirementId(requirementId, check23DAO.connectionString);
            LogEntry logEntrySolution = new LogEntry("Created new solution", lastCreatedId, activityLogId);
            logEntrySolution.Person = Request.Cookies["Check23Username"] ?? "No Username";
            check23DAO.logging.InsertLogEntry(logEntrySolution, "solution", check23DAO.connectionString);
            LogEntry logEntryEstimation = new LogEntry("Estimation created by System", lastCreatedId, activityLogId);
            logEntryEstimation.Person = "System";
            check23DAO.logging.InsertLogEntry(logEntryEstimation, "estimation", check23DAO.connectionString);
            return RedirectToAction("ChooseEstimation", "Estimation", new { solutionId = lastCreatedId });
        }

        public IActionResult EditSolution(int solutionId, string? errorMessage)
        {
            Check23DAO check23DAO = new Check23DAO();
            if (Request.Cookies["Check23UserAccess"] == null)
            {
                string returnRoute = Request.GetEncodedUrl();
                return RedirectToAction("SaveUserAccessCookie", "Home", new { nextRoute = returnRoute });
            }
            else if (_accessValidator.Solution(check23DAO.accessGroup.GetAccessGroupById(Convert.ToInt32(Request.Cookies["Check23UserAccess"]), check23DAO.connectionString)))
            {
                Solution foundSolution = check23DAO.solution.GetSolutionById(solutionId, check23DAO.connectionString);
                ViewBag.validationError = errorMessage;
                return View(foundSolution);
            }
            else { return View("AccessDenied"); }
        }

        public IActionResult ProcessEdit(Solution solution, string oldName)
        {
            int requirementId = solution.Requirement_id;
            if (!ModelState.IsValid)
            {
                return RedirectToAction("CreateSolution", new { solutionId = solution.Id, errorMessage = "Bearbeitung fehlgeschlagen, Eingaben waren nicht gültig" });
            }
            Check23DAO check23DAO = new Check23DAO();
            if (oldName != solution.Name)
            {
                if (check23DAO.solution.SolutionNameAlreadyExists(solution.Name, requirementId, check23DAO.connectionString))
                {
                    return RedirectToAction("CreateSolution", new { solutionId = solution.Id, errorMessage = "Name bereits vorhanden" });
                }
            }
            check23DAO.solution.UpdateSolution(solution, check23DAO.connectionString);
            LogEntry logEntry = new LogEntry("Edited solution", solution.Id, check23DAO.logging.GetActivityLogIdByRequirementId(requirementId, check23DAO.connectionString));
            logEntry.Person = Request.Cookies["Check23Username"] ?? "No Username";
            check23DAO.logging.InsertLogEntry(logEntry, "solution", check23DAO.connectionString);
            if (solution.IsSelected)
            {
                check23DAO.quoteEvaluation.UpdateStatus(check23DAO.requirement.GetRequirementById(requirementId, check23DAO.connectionString).QuoteEvaluation_id, 3, check23DAO.connectionString);
            }
            else
            {
                StatusHelper statusHelper = new StatusHelper();
                if (statusHelper.CheckFullyEstimated(check23DAO.requirement.GetRequirementById(requirementId, check23DAO.connectionString), check23DAO.estimation.GetEstimationBySolutionId(solution.Id, check23DAO.connectionString)))
                {
                    check23DAO.quoteEvaluation.UpdateStatus(check23DAO.requirement.GetRequirementById(requirementId, check23DAO.connectionString).QuoteEvaluation_id, 2, check23DAO.connectionString);
                }
                else
                {
                    check23DAO.quoteEvaluation.UpdateStatus(check23DAO.requirement.GetRequirementById(requirementId, check23DAO.connectionString).QuoteEvaluation_id, 1, check23DAO.connectionString);
                }
            }
            return RedirectToAction("DetailsSolution", new { solutionId = solution.Id });
        }

        public IActionResult DetailsSolution(int solutionId, string? fileUploadMessage, string? approvalMessage)
        {
            Check23DAO check23DAO = new Check23DAO();
            Solution foundSolution = check23DAO.solution.GetSolutionById(solutionId, check23DAO.connectionString);
            foundSolution.files = check23DAO.fileUpload.GetAllUploadedFiles(check23DAO.fileUpload.GetSpecifiedFolderIdByAccessorId(_fileUploadService.GetUploadFolderName(), solutionId, localAccessor, check23DAO.connectionString), check23DAO.connectionString);
            Estimation estimation = check23DAO.estimation.GetEstimationBySolutionId(solutionId, check23DAO.connectionString);
            EstimationPreparer preparer = new EstimationPreparer();
            (estimation, ViewBag.timeFrameESW, ViewBag.timeFrameEHW, ViewBag.timeFrameDocumentation, ViewBag.timeFrameService) = preparer.SetupEstimationForView(estimation);           
            foundSolution.estimation = estimation;
            foundSolution.comments = check23DAO.comment.GetSolutionCommentsBySolutionId(solutionId, check23DAO.connectionString);
            Requirement requirement = check23DAO.requirement.GetRequirementById(foundSolution.Requirement_id, check23DAO.connectionString);
            StatusHelper statusHelper = new StatusHelper();
            if (statusHelper.CheckFullyEstimated(requirement, foundSolution.estimation))
            {
                ViewBag.fullyEstimated = 1;
            }
            else
            {
                ViewBag.fullyEstimated = 0;
            }
            if (!string.IsNullOrEmpty(fileUploadMessage))
            {
                ViewBag.fileUploadMessage = fileUploadMessage;
            }
            if (!string.IsNullOrEmpty(approvalMessage))
            {
                ViewBag.approvalMessage = approvalMessage;
            }
            ViewBag.requirement = requirement;
            ViewBag.quoteEvalName = check23DAO.quoteEvaluation.GetQuoteEvaluationById(requirement.QuoteEvaluation_id, check23DAO.connectionString).Name;
            return View(foundSolution);
        }

        public async Task<IActionResult> UploadFile(int solutionId, [FromForm] IFormFile file)
        {
            Check23DAO check23DAO = new Check23DAO();
            Solution currentSolution = check23DAO.solution.GetSolutionById(solutionId, check23DAO.connectionString);
            Requirement requirement = check23DAO.requirement.GetRequirementById(currentSolution.Requirement_id, check23DAO.connectionString);
            QuoteEvaluation quoteEval = check23DAO.quoteEvaluation.GetQuoteEvaluationById(requirement.QuoteEvaluation_id, check23DAO.connectionString);
            string quoteEvalFolder = quoteEval.Name + "_" + quoteEval.Id.ToString();
            DatabaseFile newFile = await _fileUploadService.UploadFile(quoteEvalFolder, file);
            string? fileUploadMessage = null;
            if ((newFile.Name == "error" || string.IsNullOrEmpty(newFile.Name)) || (newFile.DataPath == "error" || string.IsNullOrEmpty(newFile.DataPath)))
            {
                fileUploadMessage = "Beim Hochladen der Datei ist ein Fehler aufgetreten";
            }
            else
            {
                fileUploadMessage = "Datei erfolgreich hochgeladen";
                newFile.Folder_id = check23DAO.fileUpload.GetSpecifiedFolderIdByAccessorId(_fileUploadService.GetUploadFolderName(), solutionId, localAccessor, check23DAO.connectionString);
                check23DAO.fileUpload.InsertFile(newFile, check23DAO.connectionString);
            }
            return RedirectToAction("DetailsSolution", new { solutionId, fileUploadMessage });
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

        public IActionResult DeleteFile(int fileId, int solutionId)
        {
            Check23DAO check23DAO = new Check23DAO();
            DatabaseFile file = check23DAO.fileUpload.GetFileById(fileId, check23DAO.connectionString);
            if (string.IsNullOrEmpty(file.DataPath))
            {
                return RedirectToAction("DetailsSolution", new { solutionId });
            }
            System.IO.File.Delete(file.DataPath);
            check23DAO.fileUpload.DeleteFile(fileId, check23DAO.connectionString);
            return RedirectToAction("DetailsSolution", new { solutionId });
        }

        public IActionResult OpenExplorer(int solutionId)
        {
            Check23DAO check23DAO = new Check23DAO();
            int directAccessFilesFolderId = check23DAO.fileUpload.GetSpecifiedFolderIdByAccessorId(_fileUploadService.GetDirectAccessFilesFolderName(), solutionId, localAccessor, check23DAO.connectionString);
            Folder directAccessFilesFolder = check23DAO.fileUpload.GetFolderById(directAccessFilesFolderId, check23DAO.connectionString);
            string directAccessFilesFolderPath = directAccessFilesFolder.FolderPath;
            Process.Start("explorer", directAccessFilesFolderPath);
            return RedirectToAction("DetailsSolution", new { solutionId });
        }

        public IActionResult GrantApproval(int solutionId)
        {
            Check23DAO check23DAO = new Check23DAO();
            if (Request.Cookies["Check23UserAccess"] == null)
            {
                string returnRoute = Request.GetEncodedUrl();
                return RedirectToAction("SaveUserAccessCookie", "Home", new { nextRoute = returnRoute });
            }
            else if (_accessValidator.QuoteEvaluation(check23DAO.accessGroup.GetAccessGroupById(Convert.ToInt32(Request.Cookies["Check23UserAccess"]), check23DAO.connectionString)))
            {
                Solution solution = check23DAO.solution.GetSolutionById(solutionId, check23DAO.connectionString);
                QuoteEvaluation quoteEvaluation = check23DAO.quoteEvaluation.GetQuoteEvaluationById(check23DAO.requirement.GetRequirementById(solution.Requirement_id, check23DAO.connectionString).QuoteEvaluation_id, check23DAO.connectionString);
                check23DAO.quoteEvaluation.UpdateStatus(quoteEvaluation.Id, 3, check23DAO.connectionString);
                check23DAO.solution.SetSelected(solutionId, check23DAO.connectionString);
                LogEntry logEntry = new LogEntry("Selected a solution and granted approval", solutionId, check23DAO.logging.GetActivityLogIdByRequirementId(check23DAO.solution.GetSolutionById(solutionId, check23DAO.connectionString).Requirement_id, check23DAO.connectionString));
                logEntry.Person = Request.Cookies["Check23Username"] ?? "No Username";
                check23DAO.logging.InsertLogEntry(logEntry, "solution", check23DAO.connectionString);                
                Estimation estimation = check23DAO.estimation.GetEstimationBySolutionId(solutionId, check23DAO.connectionString);
                List<string> estimators = check23DAO.logging.GetPersonsInEstimationLogsByEstimationId(estimation.Id, check23DAO.connectionString);
                HashSet<string> EmailAddresses = new HashSet<string>();
                foreach (string estimator in estimators)
                {
                    EmailAddresses.Add(check23DAO.user.GetEmailAddressByUsername(estimator, check23DAO.connectionString));
                }
                _emailService.SendEmail("Angebotsprüfung freigegeben", "Die Lösung " + solution.Name + ", für die sie abgeschätzt haben, wurde ausgewählt und die Angebotsprüfung " + quoteEvaluation.Name + " Id: " + quoteEvaluation.Id + " wurde zur Bearbeitung freigegeben.", EmailAddresses);
                return RedirectToAction("DetailsSolution", new { solutionId, approvalMessage = "Freigabe erfolgreich" });
            }
            else { return View("AccessDenied"); }
        }
    }
}
