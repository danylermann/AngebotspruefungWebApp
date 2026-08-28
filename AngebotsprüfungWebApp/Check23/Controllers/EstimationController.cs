using Check23.Additional_Classes;
using Check23.Models;
using Check23.Services.AccessValidation;
using Check23.Services.DatabankAccess;
using Check23.Services.EmailService;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Org.BouncyCastle.Asn1.Ocsp;

namespace Check23.Controllers
{
    public class EstimationController : Controller
    {
        private List<SelectListItem> TimeFrameList = new List<SelectListItem>
        {
            new SelectListItem("Stunde/n", "h"),
            new SelectListItem("Tag/e", "d"),
            new SelectListItem("Woche/n", "w"),
            new SelectListItem("Monat/e", "m")
        };

        private readonly IAccessValidationService _accessValidator;
        private readonly IEmailService _emailService;

        public EstimationController(IAccessValidationService accessValidationService, IEmailService emailService) 
        {
            _accessValidator = accessValidationService;
            _emailService = emailService;
        }

        public IActionResult ChooseEstimation(int solutionId)
        {
            //check if user is allowed to estimate if yes continue if no redirect to a page with a corresponding statement telling the user that they are not allowed to do that
            //also just save the access modifier in a cookie or session storage after checking it for the first time so it is not needed to re-grab the the user everytime he enters a page for this session                       #
            Check23DAO check23DAO = new Check23DAO();
            if (Request.Cookies["Check23UserAccess"] == null)
            {
                string returnRoute = Request.GetEncodedUrl();
                return RedirectToAction("SaveUserAccessCookie", "Home", new { nextRoute = returnRoute });
            }
            else if (_accessValidator.Estimation(check23DAO.accessGroup.GetAccessGroupById(Convert.ToInt32(Request.Cookies["Check23UserAccess"]), check23DAO.connectionString)))
            {
                Requirement requirement = check23DAO.requirement.GetRequirementById(check23DAO.solution.GetSolutionById(solutionId, check23DAO.connectionString).Requirement_id, check23DAO.connectionString);
                StatusHelper statusHelper = new StatusHelper();
                if (statusHelper.ESWRequired(requirement))
                {
                    ViewBag.esw = true;
                }
                else { ViewBag.esw = false; }
                if (statusHelper.EHWRequired(requirement))
                {
                    ViewBag.ehw = true;
                }
                else { ViewBag.ehw = false; }
                if (statusHelper.DocumentationRequired(requirement))
                {
                    ViewBag.docu = true;
                }
                else { ViewBag.docu = false; }
                if (statusHelper.ServiceRequired(requirement))
                {
                    ViewBag.service = true;
                }
                else { ViewBag.service = false; }
                //check if the user only has one department if yes and if the department requires estimation send the user there if the department they have does not require estimation send them to a page with a corresponding message telling them they are not required to estimate
                //if they have multiple departments check if only one is required if so send them there otherwise if mutliple require estimation let them choose, if none do send them to the page telling the user they are not reuired to estimate
                ViewBag.solutionId = solutionId;
                return View();
            }
            else { return View("AccessDenied"); }
        }

        public IActionResult EditEstimationESW(int solutionId)
        {
            Check23DAO check23DAO = new Check23DAO();
            Estimation foundEstimation = check23DAO.estimation.GetEstimationBySolutionId(solutionId, check23DAO.connectionString);
            ViewBag.timeFrameList = TimeFrameList;
            return View(foundEstimation);
        }

        public IActionResult ProcessEditEstimationESW(Estimation esw)
        {
            esw.ESW_time = esw.TimeFrame == null ? esw.ESW_time : esw.ESW_time + esw.TimeFrame;
            Check23DAO check23DAO = new Check23DAO();
            check23DAO.estimation.UpdateEstimationByDepartment(esw, "ESW", check23DAO.connectionString);
            LogEntry estimationLogEntry = new LogEntry("Estimated for ESW", esw.Id, check23DAO.logging.GetActivityLogIdBySolutionId(esw.Solution_id, check23DAO.connectionString));
            estimationLogEntry.Person = Request.Cookies["Check23Username"] ?? "No Username";
            check23DAO.logging.InsertLogEntry(estimationLogEntry, "estimation", check23DAO.connectionString);
            UpdateStatus(esw.Id);
            return RedirectToAction("DetailsSolution", "Solution", new { solutionId = esw.Solution_id });
        }

        public IActionResult EditEstimationEHW(int solutionId)
        {
            Check23DAO check23DAO = new Check23DAO();
            Estimation foundEstimation = check23DAO.estimation.GetEstimationBySolutionId(solutionId, check23DAO.connectionString);
            ViewBag.timeFrameList = TimeFrameList;
            return View(foundEstimation);
        }

        public IActionResult ProcessEditEstimationEHW(Estimation ehw)
        {
            ehw.EHW_time = ehw.EHW_time == null ? ehw.EHW_time : ehw.EHW_time + ehw.TimeFrame;
            Check23DAO check23DAO = new Check23DAO();
            check23DAO.estimation.UpdateEstimationByDepartment(ehw, "EHW", check23DAO.connectionString);
            LogEntry estimationLogEntry = new LogEntry("Estimated for EHW", ehw.Id, check23DAO.logging.GetActivityLogIdBySolutionId(ehw.Solution_id, check23DAO.connectionString));
            estimationLogEntry.Person = Request.Cookies["Check23Username"] ?? "No Username";
            check23DAO.logging.InsertLogEntry(estimationLogEntry, "estimation", check23DAO.connectionString);
            UpdateStatus(ehw.Id);
            return RedirectToAction("DetailsSolution", "Solution", new { solutionId = ehw.Solution_id });
        }

        //public IActionResult EditEstimationCDE(int solutionId)
        //{
        //    Check23DAO check23DAO = new Check23DAO();
        //    Estimation foundEstimation = check23DAO.estimation.GetEstimationBySolutionId(solutionId, check23DAO.connectionString);
        //    return View(foundEstimation);
        //}

        //public IActionResult ProcessEditEstimationCDE(Estimation cde)
        //{
        //    Check23DAO check23DAO = new Check23DAO();
        //    check23DAO.estimation.UpdateEstimationByDepartment(cde, "CDE", check23DAO.connectionString);
        //    LogEntry estimationLogEntry = new LogEntry("Estimated for CDE", cde.Id, check23DAO.logging.GetActivityLogIdBySolutionId(cde.Solution_id, check23DAO.connectionString));
        //    estimationLogEntry.Person = Request.Cookies["Check23Username"] ?? "No Username";
        //    check23DAO.logging.InsertLogEntry(estimationLogEntry, "estimation", check23DAO.connectionString);
        //    return RedirectToAction("DetailsSolution", "Solution", new { solutionId = cde.Solution_id });
        //}

        public IActionResult EditEstimationDocumentation(int solutionId)
        {
            Check23DAO check23DAO = new Check23DAO();
            Estimation foundEstimation = check23DAO.estimation.GetEstimationBySolutionId(solutionId, check23DAO.connectionString);
            ViewBag.timeFrameList = TimeFrameList;
            return View(foundEstimation);
        }

        public IActionResult ProcessEditEstimationDocumentation(Estimation documentation)
        {
            documentation.Documentation_time = documentation.Documentation_time == null ? documentation.Documentation_time : documentation.Documentation_time + documentation.TimeFrame;
            Check23DAO check23DAO = new Check23DAO();
            check23DAO.estimation.UpdateEstimationByDepartment(documentation, "Documentation", check23DAO.connectionString);
            LogEntry estimationLogEntry = new LogEntry("Estimated for Documentation", documentation.Id, check23DAO.logging.GetActivityLogIdBySolutionId(documentation.Solution_id, check23DAO.connectionString));
            estimationLogEntry.Person = Request.Cookies["Check23Username"] ?? "No Username";
            check23DAO.logging.InsertLogEntry(estimationLogEntry, "estimation", check23DAO.connectionString);
            UpdateStatus(documentation.Id);
            return RedirectToAction("DetailsSolution", "Solution", new { solutionId = documentation.Solution_id });
        }

        public IActionResult EditEstimationService(int solutionId)
        {
            Check23DAO check23DAO = new Check23DAO();
            Estimation foundEstimation = check23DAO.estimation.GetEstimationBySolutionId(solutionId, check23DAO.connectionString);
            ViewBag.timeFrameList = TimeFrameList;
            return View(foundEstimation);
        }

        public IActionResult ProcessEditEstimationService(Estimation service)
        {
            service.Service_time = service.TimeFrame == null ? service.Service_time : service.Service_time + service.TimeFrame;
            Check23DAO check23DAO = new Check23DAO();
            check23DAO.estimation.UpdateEstimationByDepartment(service, "Service", check23DAO.connectionString);
            LogEntry estimationLogEntry = new LogEntry("Estimated for Service", service.Id, check23DAO.logging.GetActivityLogIdBySolutionId(service.Solution_id, check23DAO.connectionString));
            estimationLogEntry.Person = Request.Cookies["Check23Username"] ?? "No Username";
            check23DAO.logging.InsertLogEntry(estimationLogEntry, "estimation", check23DAO.connectionString);
            UpdateStatus(service.Id);
            return RedirectToAction("DetailsSolution", "Solution", new { solutionId = service.Solution_id });
        }

        private void UpdateStatus(int estimationId)
        {
            Check23DAO check23DAO = new Check23DAO();
            Estimation estimation = check23DAO.estimation.GetEstimationById(estimationId, check23DAO.connectionString);
            Requirement requirement = check23DAO.requirement.GetRequirementById(check23DAO.solution.GetSolutionById(estimation.Solution_id, check23DAO.connectionString).Requirement_id, check23DAO.connectionString);
            QuoteEvaluation quoteEvaluation = check23DAO.quoteEvaluation.GetQuoteEvaluationById(requirement.QuoteEvaluation_id, check23DAO.connectionString);
            if (quoteEvaluation.Status == 1)
            {
                StatusHelper statusUpdater = new StatusHelper();
                if (statusUpdater.CheckFullyEstimated(requirement, estimation))
                {
                    check23DAO.quoteEvaluation.UpdateStatus(requirement.QuoteEvaluation_id, 2, check23DAO.connectionString);
                    _emailService.SendEmail("Angebotsprüfung vollständig abgeschätzt", "Die Angebotsprüfung " + quoteEvaluation.Name + " mit Id: " + quoteEvaluation.Id + " ist vollständig abgeschätzt.", new HashSet<string> { check23DAO.user.GetEmailAddressByUsername(quoteEvaluation.Creator, check23DAO.connectionString) });

                }
            }
        }
    }
}
