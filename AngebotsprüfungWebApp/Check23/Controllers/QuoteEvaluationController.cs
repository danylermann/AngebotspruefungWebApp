using Check23.Additional_Classes;
using Check23.Models;
using Check23.Services.AccessValidation;
using Check23.Services.DatabankAccess;
using Check23.Services.EmailService;
using Check23.Services.UploadService;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.StaticFiles;
using System.Diagnostics;

namespace Check23.Controllers
{
    public class QuoteEvaluationController : Controller
    {
        private const string localAccessor = "quoteEval"; //Equivalent zum im FileUpload switch-case verwendeten case für die Angebotsprüfung
        private readonly IFileUploadService _fileUploadService;
        private readonly IAccessValidationService _accessValidator;
        private readonly IEmailService _emailService;
        public QuoteEvaluationController(IFileUploadService fileUploadService, IAccessValidationService accessValidationService, IEmailService emailService) 
        {
            _fileUploadService = fileUploadService;
            _accessValidator = accessValidationService;
            _emailService = emailService;
        }        
        public IActionResult Index()
        {
            Check23DAO check23DAO = new Check23DAO();
            List<QuoteEvaluation> list = check23DAO.quoteEvaluation.GetAllQuouteEvaluations(check23DAO.connectionString);
            foreach (QuoteEvaluation quoteEvaluation in list) 
            {
                if (quoteEvaluation.Client_id >= 0)
                {
                    quoteEvaluation.client = check23DAO.client.GetClientById(quoteEvaluation.Client_id, check23DAO.connectionString);
                }
            }
            return View(list);
        }

        public IActionResult SelectOrderCategory()
        {
            return View();
        }

        public IActionResult CreateQuoteEvaluation(int? clientId, string? errorMessage)
        {
            Check23DAO check23DAO = new Check23DAO();
            if (Request.Cookies["Check23UserAccess"] == null)
            {
                string returnRoute = Request.GetEncodedUrl(); //needs "get" method type to function properly otherwise route values are missing
                return RedirectToAction("SaveUserAccessCookie", "Home", new { nextRoute = returnRoute });
            }
            else if (_accessValidator.QuoteEvaluation(check23DAO.accessGroup.GetAccessGroupById(Convert.ToInt32(Request.Cookies["Check23UserAccess"]), check23DAO.connectionString)))
            {
                List<Client> clients = check23DAO.client.GetAllClients(check23DAO.connectionString);
                SelectList clientsSelectList = new SelectList(clients, nameof(Client.Id), nameof(Client.Name));
                int i = 0;
                foreach (SelectListItem item in clientsSelectList)
                {
                    if (!string.IsNullOrEmpty(clients[i].Location))
                    {
                        item.Text += " aus " + clients[i].Location;
                    }
                    if (item.Value == clientId.ToString())
                    {
                        item.Selected = true;
                    }
                    i++;
                }
                ViewBag.clientsSelectList = clientsSelectList;
                ViewBag.errorMessage = errorMessage;
                return View();
            }
            else { return View("AccessDenied"); }
        }

        public IActionResult ProcessCreate(QuoteEvaluation quoteEval)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("CreateQuoteEvaluation", new { errorMessage = "Erstellung fehlgeschlagen, Eingaben waren nicht gültig" });
            }         
            quoteEval.Creator = Request.Cookies["Check23Username"] ?? "No Username";
            Check23DAO check23DAO = new Check23DAO();
            if (check23DAO.quoteEvaluation.QuoteEvaluationNameAlreadyExists(quoteEval.Name, check23DAO.connectionString)) 
            {
                if (quoteEval.OrderCategory == 1)
                {
                    return RedirectToAction("CreateQuoteEvaluation", new { errorMessage = "Name bereits vorhanden", clientId = quoteEval.Client_id });
                }
                else 
                {
                    return RedirectToAction("CreateDevelopmentOrder", new { errorMessage = "Name bereits vorhanden" });
                }
            }
            int createdQuoteEvalId = check23DAO.quoteEvaluation.InsertQuoteEvaluation(quoteEval, check23DAO.connectionString);
            if (createdQuoteEvalId != -1)
            {
                List<Folder> createdFolders = _fileUploadService.SetupDirectory(createdQuoteEvalId, quoteEval.Name);
                Folder firstFolder = createdFolders[0];
                check23DAO.fileUpload.InsertFolder(firstFolder, check23DAO.connectionString);
                Folder secondFolder = createdFolders[1];
                check23DAO.fileUpload.InsertFolder(secondFolder, check23DAO.connectionString);
                int createdActivityLogId = check23DAO.logging.InsertActivityLog(createdQuoteEvalId, check23DAO.connectionString);
                LogEntry newLogEntry = new LogEntry("Created new quote evaluation", createdQuoteEvalId, createdActivityLogId);
                newLogEntry.Date = quoteEval.Date;
                newLogEntry.Person = quoteEval.Creator;
                check23DAO.logging.InsertLogEntry(newLogEntry, "quote_evaluation", check23DAO.connectionString);
                return RedirectToAction("CreateRequirement", "Requirement", new { quoteEvalId = createdQuoteEvalId, quoteEvalName = quoteEval.Name });
            }
            return RedirectToAction("Index");
        }

        public IActionResult CreateDevelopmentOrder(string? errorMessage)
        {
            Check23DAO check23DAO = new Check23DAO();
            if (Request.Cookies["Check23UserAccess"] == null)
            {
                string returnRoute = Request.GetEncodedUrl(); //needs "get" method type to function properly otherwise route values are missing
                return RedirectToAction("SaveUserAccessCookie", "Home", new { nextRoute = returnRoute });
            }
            else if (_accessValidator.QuoteEvaluation(check23DAO.accessGroup.GetAccessGroupById(Convert.ToInt32(Request.Cookies["Check23UserAccess"]), check23DAO.connectionString)))
            {
                ViewBag.errorMessage = errorMessage;
                return View();
            }
            else { return View("AccessDenied"); }
        }

        public IActionResult DetailsQuoteEvaluation(int quoteEvalId, string? fileUploadMessage, string? approvalMessage)
        {
            Check23DAO check23DAO = new Check23DAO();
            QuoteEvaluation quoteEvaluation = check23DAO.quoteEvaluation.GetQuoteEvaluationById(quoteEvalId, check23DAO.connectionString);
            quoteEvaluation.requirements = check23DAO.requirement.GetRequirementsByQuoteEvaluationId(quoteEvalId, check23DAO.connectionString);
            foreach (var requirement in quoteEvaluation.requirements)
            {
                requirement.solutions = check23DAO.solution.GetSolutionsByRequirementId(requirement.Id, check23DAO.connectionString);
                requirement.comments = check23DAO.comment.GetRequirementCommentsByRequirementId(requirement.Id, check23DAO.connectionString);
            }
            foreach (var requirement in quoteEvaluation.requirements)
            {
                foreach (var solution in requirement.solutions)
                {
                    solution.estimation = check23DAO.estimation.GetEstimationBySolutionId(solution.Id, check23DAO.connectionString);
                    solution.comments = check23DAO.comment.GetSolutionCommentsBySolutionId(solution.Id, check23DAO.connectionString);
                }
            }
            quoteEvaluation.files = check23DAO.fileUpload.GetAllUploadedFiles(check23DAO.fileUpload.GetSpecifiedFolderIdByAccessorId(_fileUploadService.GetUploadFolderName(), quoteEvalId, localAccessor, check23DAO.connectionString), check23DAO.connectionString);
            quoteEvaluation.activityLog = new ActivityLog();
            quoteEvaluation.activityLog.Id = check23DAO.logging.GetActivityLogIdByQuouteEvaluationId(quoteEvalId, check23DAO.connectionString);
            quoteEvaluation.activityLog.QuoteEvaluation_id = quoteEvalId;
            if (!string.IsNullOrEmpty(fileUploadMessage))
            {
                ViewBag.fileUploadMessage = fileUploadMessage;
            }
            if (quoteEvaluation.Client_id != null)
            {
                quoteEvaluation.client = check23DAO.client.GetClientById(quoteEvaluation.Client_id, check23DAO.connectionString);
            }
            StatusHelper statusHelper = new StatusHelper();
            if (quoteEvaluation.Status == statusHelper.ApprovalGranted)
            {
                quoteEvaluation.tickets = check23DAO.ticket.GetAllTicketsByQuoteEvalId(quoteEvalId, check23DAO.connectionString);
            }            
            if (!string.IsNullOrEmpty(approvalMessage))
            {
                ViewBag.approvalMessage = approvalMessage;
            }
            return View(quoteEvaluation);
        }


        public IActionResult EditQuoteEvaluation(int quoteEvalId, string? errorMessage)
        {
            Check23DAO check23DAO = new Check23DAO();
            if (Request.Cookies["Check23UserAccess"] == null)
            {
                string returnRoute = Request.GetEncodedUrl();
                return RedirectToAction("SaveUserAccessCookie", "Home", new { nextRoute = returnRoute });
            }
            else if (_accessValidator.QuoteEvaluation(check23DAO.accessGroup.GetAccessGroupById(Convert.ToInt32(Request.Cookies["Check23UserAccess"]), check23DAO.connectionString)))
            {
                QuoteEvaluation foundQuoteEvaluation = check23DAO.quoteEvaluation.GetQuoteEvaluationById(quoteEvalId, check23DAO.connectionString);
                List<Client> clients = check23DAO.client.GetAllClients(check23DAO.connectionString);
                SelectList clientsSelectList = new SelectList(clients, nameof(Client.Id), nameof(Client.Name));
                int i = 0;
                foreach (SelectListItem item in clientsSelectList)
                {
                    if (!string.IsNullOrEmpty(clients[i].Location))
                    {
                        item.Text += " aus " + clients[i].Location;

                    }
                    i++;
                }
                string clientId = foundQuoteEvaluation.Client_id.ToString();
                if (clientId != "")
                {
                    foreach (SelectListItem client in clientsSelectList)
                    {
                        if (client.Value == clientId)
                        {
                            client.Selected = true;
                        }
                    }
                }
                ViewBag.clientsSelectList = clientsSelectList;
                ViewBag.validationError = errorMessage;

                return View(foundQuoteEvaluation);
            }
            else { return View("AccessDenied"); }
        }

        public IActionResult ProcessEdit(QuoteEvaluation quoteEval, string oldName)
        {     
            if(!ModelState.IsValid) 
            {
                return RedirectToAction("EditQuoteEvaluation", new { errorMessage = "Bearbeitung fehlgeschlagen, Eingaben waren nicht gültig", quoteEvalId = quoteEval.Id });
            }
            Check23DAO check23DAO = new Check23DAO();
            if(oldName != quoteEval.Name)
            {
                if (check23DAO.quoteEvaluation.QuoteEvaluationNameAlreadyExists(quoteEval.Name, check23DAO.connectionString)) 
                {
                    return RedirectToAction("EditQuoteEvaluation", new { errorMessage = "Name bereits vorhanden", quoteEvalId = quoteEval.Id });
                }
                string oldFolderName = oldName + "_" + quoteEval.Id.ToString();
                string newFolderName = quoteEval.Name + "_" + quoteEval.Id;
                string newFolderPath = Path.Combine(_fileUploadService.ChangeDirectoryName(oldFolderName , newFolderName), _fileUploadService.GetUploadFolderName());
                //Update Folder which is accessed by the website
                check23DAO.fileUpload.UpdateFolderPath(newFolderPath, check23DAO.fileUpload.GetSpecifiedFolderIdByAccessorId(_fileUploadService.GetUploadFolderName(), quoteEval.Id, localAccessor, check23DAO.connectionString) ,check23DAO.connectionString);
                //Update folder that is driectly accessed
                check23DAO.fileUpload.UpdateFolderPath(newFolderPath, check23DAO.fileUpload.GetSpecifiedFolderIdByAccessorId(_fileUploadService.GetDirectAccessFilesFolderName(), quoteEval.Id, localAccessor, check23DAO.connectionString), check23DAO.connectionString);
            }
            check23DAO.quoteEvaluation.UpdateQuoteEvaluation(quoteEval, check23DAO.connectionString);
            int activityLogId = check23DAO.logging.GetActivityLogIdByQuouteEvaluationId(quoteEval.Id, check23DAO.connectionString);
            LogEntry logEntry = new LogEntry("Edited quote evaluation", quoteEval.Id, activityLogId);
            logEntry.Person = Request.Cookies["Check23Username"] ?? "No Username";
            check23DAO.logging.InsertLogEntry(logEntry, "quote_evaluation", check23DAO.connectionString);
            return RedirectToAction("DetailsQuoteEvaluation", new { quoteEvalId = quoteEval.Id });
        }

        public IActionResult ActivityLog(ActivityLog activityLog)
        {
            Check23DAO check23DAO = new Check23DAO();            
            ViewBag.quoteEvalId = activityLog.QuoteEvaluation_id;
            return View(check23DAO.logging.GetAllLogEntriesByActivityLogId(activityLog.Id, check23DAO.connectionString));
        }

        public async Task<IActionResult> UploadFile(int quoteEvalId, [FromForm]IFormFile file)
        {
            Check23DAO check23DAO = new Check23DAO();
            QuoteEvaluation temp = check23DAO.quoteEvaluation.GetQuoteEvaluationById(quoteEvalId, check23DAO.connectionString);
            string quoteEvalFolder = temp.Name + "_" + quoteEvalId.ToString();
            DatabaseFile newFile = await _fileUploadService.UploadFile(quoteEvalFolder, file);
            string? fileUploadMessage = null;
            if((newFile.Name == "error" || string.IsNullOrEmpty(newFile.Name)) || (newFile.DataPath == "error" || string.IsNullOrEmpty(newFile.DataPath)))
            {
                fileUploadMessage = "Beim Hochladen der Datei ist ein Fehler aufgetreten";
            }
            else
            {
                fileUploadMessage = "Datei erfolgreich hochgeladen";
                newFile.Folder_id = check23DAO.fileUpload.GetSpecifiedFolderIdByAccessorId(_fileUploadService.GetUploadFolderName(), quoteEvalId, localAccessor, check23DAO.connectionString);
                check23DAO.fileUpload.InsertFile(newFile, check23DAO.connectionString);
            }                        
            return RedirectToAction("DetailsQuoteEvaluation", new { quoteEvalId, fileUploadMessage }) ;
        }

        public IActionResult DeleteFile(int fileId, int quoteEvalId)
        {
            Check23DAO check23DAO = new Check23DAO();
            DatabaseFile file = check23DAO.fileUpload.GetFileById(fileId, check23DAO.connectionString);
            if (string.IsNullOrEmpty(file.DataPath))
            {
                return RedirectToAction("DetailsQuoteEvaluation", new { quoteEvalId });
            }
            System.IO.File.Delete(file.DataPath);
            check23DAO.fileUpload.DeleteFile(fileId, check23DAO.connectionString);
            return RedirectToAction("DetailsQuoteEvaluation", new { quoteEvalId });
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

        public IActionResult OpenExplorer(int quoteEvalId)
        {
            Check23DAO check23DAO = new Check23DAO();
            int directAccessFilesFolderId = check23DAO.fileUpload.GetSpecifiedFolderIdByAccessorId(_fileUploadService.GetDirectAccessFilesFolderName(), quoteEvalId, localAccessor, check23DAO.connectionString);
            Folder directAccessFilesFolder = check23DAO.fileUpload.GetFolderById(directAccessFilesFolderId, check23DAO.connectionString);
            string directAccessFilesFolderPath = directAccessFilesFolder.FolderPath;
            Process.Start("explorer", directAccessFilesFolderPath);
            return RedirectToAction("DetailsQuoteEvaluation", new { quoteEvalId });
        }

        //Wenn ich eine Liste mitbringen kann muss ich beim Sortieren nicht extra überprüfen ob bereits gesucht wurde. Nachteil ist die Seite kann nicht direkt gefunden werden und nicht mit F5 neu geldaden werden.
        public IActionResult SortBy(string sortBy, bool desc, List<int> quoteEvaluationIds)
        {
            if (quoteEvaluationIds.Count == 0)
            {
                return RedirectToAction("Index");
            }
            else
            {
                List<QuoteEvaluation> quoteEvaluationList;
                Check23DAO check23DAO = new Check23DAO();
                quoteEvaluationList = check23DAO.quoteEvaluation.GetQuoteEvaluationsByListOrdered(quoteEvaluationIds.ToHashSet(), sortBy, desc, check23DAO.connectionString);
                foreach (QuoteEvaluation quoteEvaluation in quoteEvaluationList)
                {
                    if (quoteEvaluation.Client_id >= 0)
                    {
                        quoteEvaluation.client = check23DAO.client.GetClientById(quoteEvaluation.Client_id, check23DAO.connectionString);
                    }
                }
                return View("Index", quoteEvaluationList);
            }

        }

        public IActionResult Search(string searchTerm, bool mustIncludeAll)
        {
            if (!string.IsNullOrEmpty(searchTerm))
            {
                List<QuoteEvaluation> quoteEvaluationList;
                Check23DAO check23DAO = new Check23DAO();
                HashSet<int> quoteEvaluationIds = check23DAO.SearchDatabaseFor(searchTerm, mustIncludeAll);
                quoteEvaluationList = check23DAO.quoteEvaluation.GetQuoteEvaluationsByList(quoteEvaluationIds, check23DAO.connectionString);
                foreach (QuoteEvaluation quoteEvaluation in quoteEvaluationList)
                {
                    if (quoteEvaluation.Client_id >= 0)
                    {
                        quoteEvaluation.client = check23DAO.client.GetClientById(quoteEvaluation.Client_id, check23DAO.connectionString);
                    }
                }
                return View("Index", quoteEvaluationList);
            }
            else
            {
                return RedirectToAction("Index");
            }
            
        }

        public IActionResult GrantApproval(int quoteEvalId, int solutionId)
        {

            Check23DAO check23DAO = new Check23DAO();
            if (Request.Cookies["Check23UserAccess"] == null)
            {
                string returnRoute = Request.GetEncodedUrl();
                return RedirectToAction("SaveUserAccessCookie", "Home", new { nextRoute = returnRoute });
            }
            else if (_accessValidator.QuoteEvaluation(check23DAO.accessGroup.GetAccessGroupById(Convert.ToInt32(Request.Cookies["Check23UserAccess"]), check23DAO.connectionString)))
            {
                check23DAO.quoteEvaluation.UpdateStatus(quoteEvalId, 3, check23DAO.connectionString);
                int activityLogId = check23DAO.logging.GetActivityLogIdByQuouteEvaluationId(quoteEvalId, check23DAO.connectionString);
                LogEntry logEntry = new LogEntry("Selected a solution and granted approval", quoteEvalId, activityLogId);
                logEntry.Person = Request.Cookies["Check23Username"] ?? "No Username";
                check23DAO.logging.InsertLogEntry(logEntry, "quote_evaluation", check23DAO.connectionString);
                check23DAO.solution.SetSelected(solutionId, check23DAO.connectionString);
                Solution solution = check23DAO.solution.GetSolutionById(solutionId, check23DAO.connectionString);
                Estimation estimation = check23DAO.estimation.GetEstimationBySolutionId(solutionId, check23DAO.connectionString);
                QuoteEvaluation quoteEvaluation = check23DAO.quoteEvaluation.GetQuoteEvaluationById(quoteEvalId, check23DAO.connectionString);
                List<string> estimators = check23DAO.logging.GetPersonsInEstimationLogsByEstimationId(estimation.Id, check23DAO.connectionString);
                HashSet<string> EmailAddresses = new HashSet<string>();
                foreach(string estimator in estimators)
                {
                    EmailAddresses.Add(check23DAO.user.GetEmailAddressByUsername(estimator, check23DAO.connectionString));
                }
                _emailService.SendEmail("Angebotsprüfung freigegeben", "Die Lösung " + solution.Name + ", für die sie abgeschätzt haben, wurde ausgewählt und die Angebotsprüfung " + quoteEvaluation.Name + " Id: " + quoteEvalId  + " wurde zur Bearbeitung freigegeben.", EmailAddresses);
                return RedirectToAction("DetailsQuoteEvaluation", new { quoteEvalId, approvalMessage = "Freigabe erfolgreich" });
            }
            else { return View("AccessDenied"); }
        }

        public IActionResult TestDetails(int quoteEvalId, string? fileUploadMessage, string? approvalMessage)
        {
            Check23DAO check23DAO = new Check23DAO();
            QuoteEvaluation quoteEvaluation = check23DAO.quoteEvaluation.GetQuoteEvaluationById(quoteEvalId, check23DAO.connectionString);
            quoteEvaluation.requirements = check23DAO.requirement.GetRequirementsByQuoteEvaluationId(quoteEvalId, check23DAO.connectionString);
            foreach (var requirement in quoteEvaluation.requirements)
            {
                requirement.solutions = check23DAO.solution.GetSolutionsByRequirementId(requirement.Id, check23DAO.connectionString);
            }
            quoteEvaluation.files = check23DAO.fileUpload.GetAllUploadedFiles(check23DAO.fileUpload.GetSpecifiedFolderIdByAccessorId(_fileUploadService.GetUploadFolderName(), quoteEvalId, localAccessor, check23DAO.connectionString), check23DAO.connectionString);
            quoteEvaluation.activityLog = new ActivityLog();
            quoteEvaluation.activityLog.Id = check23DAO.logging.GetActivityLogIdByQuouteEvaluationId(quoteEvalId, check23DAO.connectionString);
            quoteEvaluation.activityLog.QuoteEvaluation_id = quoteEvalId;
            if (!string.IsNullOrEmpty(fileUploadMessage))
            {
                ViewBag.fileUploadMessage = fileUploadMessage;
            }
            if (quoteEvaluation.Client_id != null)
            {
                quoteEvaluation.client = check23DAO.client.GetClientById(quoteEvaluation.Client_id, check23DAO.connectionString);
            }
            StatusHelper statusHelper = new StatusHelper();
            if (quoteEvaluation.Status == statusHelper.ApprovalGranted)
            {
                quoteEvaluation.tickets = check23DAO.ticket.GetAllTicketsByQuoteEvalId(quoteEvalId, check23DAO.connectionString);
            }
            else if (quoteEvaluation.Status == statusHelper.FullyEstimated)
            {
                ViewBag.fullyEstimated = 1;
            }
            if (!string.IsNullOrEmpty(approvalMessage))
            {
                ViewBag.approvalMessage = approvalMessage;
            }
            return View(quoteEvaluation);
        }

        public IActionResult TestDetails2(int quoteEvalId, string? fileUploadMessage, string? approvalMessage)
        {
            Check23DAO check23DAO = new Check23DAO();
            QuoteEvaluation quoteEvaluation = check23DAO.quoteEvaluation.GetQuoteEvaluationById(quoteEvalId, check23DAO.connectionString);
            quoteEvaluation.requirements = check23DAO.requirement.GetRequirementsByQuoteEvaluationId(quoteEvalId, check23DAO.connectionString);
            foreach (var requirement in quoteEvaluation.requirements)
            {
                requirement.solutions = check23DAO.solution.GetSolutionsByRequirementId(requirement.Id, check23DAO.connectionString);
                requirement.comments = check23DAO.comment.GetRequirementCommentsByRequirementId(requirement.Id, check23DAO.connectionString);
            }
            foreach(var requirement in quoteEvaluation.requirements)
            {
                foreach (var solution in requirement.solutions)
                {
                    solution.estimation = check23DAO.estimation.GetEstimationBySolutionId(solution.Id, check23DAO.connectionString);
                    solution.comments = check23DAO.comment.GetSolutionCommentsBySolutionId(solution.Id, check23DAO.connectionString);
                }
            }
            quoteEvaluation.files = check23DAO.fileUpload.GetAllUploadedFiles(check23DAO.fileUpload.GetSpecifiedFolderIdByAccessorId(_fileUploadService.GetUploadFolderName(), quoteEvalId, localAccessor, check23DAO.connectionString), check23DAO.connectionString);
            quoteEvaluation.activityLog = new ActivityLog();
            quoteEvaluation.activityLog.Id = check23DAO.logging.GetActivityLogIdByQuouteEvaluationId(quoteEvalId, check23DAO.connectionString);
            quoteEvaluation.activityLog.QuoteEvaluation_id = quoteEvalId;
            if (!string.IsNullOrEmpty(fileUploadMessage))
            {
                ViewBag.fileUploadMessage = fileUploadMessage;
            }
            if (quoteEvaluation.Client_id != null)
            {
                quoteEvaluation.client = check23DAO.client.GetClientById(quoteEvaluation.Client_id, check23DAO.connectionString);
            }
            StatusHelper statusHelper = new StatusHelper();
            if (quoteEvaluation.Status == statusHelper.ApprovalGranted)
            {
                quoteEvaluation.tickets = check23DAO.ticket.GetAllTicketsByQuoteEvalId(quoteEvalId, check23DAO.connectionString);
            }
            if (!string.IsNullOrEmpty(approvalMessage))
            {
                ViewBag.approvalMessage = approvalMessage;
            }
            return View(quoteEvaluation);
        }
        //Falls ich keine Liste von Ids mitgeben kann muss ich jedes mal auch beim sortieren die Daten neu aus der Datenbank holen
        //public IActionResult SearchAndSort(string orderBy, bool desc, string searchTerm)
        //{
        //    List<QuoteEvaluation> quoteEvaluationList = new List<QuoteEvaluation>();
        //    Check23DAO check23DAO = new Check23DAO();
        //    ViewBag.SearchTerm = searchTerm;
        //    if(string.IsNullOrEmpty(searchTerm))
        //    {
        //        //Get List of Quote Evaluations orderd by orderBy and ascOrDesc
        //        quoteEvaluationList = check23DAO.quoteEvaluation.GetAllQuouteEvaluationsOrdered(orderBy, desc, check23DAO.connectionString);
        //    }
        //    else if (string.IsNullOrEmpty(orderBy))
        //    {
        //        //Search Database for searchTerm and return all distinct Quote Evaluation Ids
        //        HashSet<int> quoteEvaluationIds = check23DAO.SearchDatabaseFor(searchTerm, check23DAO.connectionString);
        //        quoteEvaluationList = check23DAO.quoteEvaluation.GetQuoteEvaluationsByList(quoteEvaluationIds, check23DAO.connectionString);

        //    }
        //    else
        //    {
        //        //Get List by searching the database and than ordering it
        //        HashSet<int> quoteEvaluationIds = check23DAO.SearchDatabaseFor(searchTerm, check23DAO.connectionString);
        //        quoteEvaluationList = check23DAO.quoteEvaluation.GetQuoteEvaluationsByListOrdered(quoteEvaluationIds, orderBy, desc, check23DAO.connectionString);                
        //    }

        //    return View("Index", quoteEvaluationList);
        //}
    }
}
