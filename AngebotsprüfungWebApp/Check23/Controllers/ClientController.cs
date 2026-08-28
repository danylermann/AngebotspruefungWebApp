using Check23.Models;
using Check23.Services.AccessValidation;
using Check23.Services.DatabankAccess;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Check23.Controllers
{
    public class ClientController : Controller
    {
        IAccessValidationService _accessValidator;
        public ClientController(IAccessValidationService accessValidationService) 
        {
            _accessValidator = accessValidationService;
        }
        public IActionResult ClientList()
        {
            Check23DAO check23DAO = new Check23DAO();
            return View(check23DAO.client.GetAllClients(check23DAO.connectionString));
        }

        public IActionResult CreateClient()
        {
            Check23DAO check23DAO = new Check23DAO();
            AccessGroup accessGroup = check23DAO.accessGroup.GetAccessGroupById(Convert.ToInt32(Request.Cookies["Check23UserAccess"]), check23DAO.connectionString);
            if (Request.Cookies["Check23UserAccess"] == null)
            {
                string returnRoute = Request.GetEncodedUrl();
                return RedirectToAction("SaveUserAccessCookie", "Home", new { nextRoute = returnRoute });
            }
            else if (_accessValidator.Client(accessGroup) || _accessValidator.QuoteEvaluation(accessGroup))
            {
                return View();
            }
            else { return View("AccessDenied"); }
        }

        public IActionResult ProcessCreate(Client client, string? errorMessage)
        {
            Check23DAO check23DAO = new Check23DAO();
            if (check23DAO.client.ClientAlreadyExists(client, check23DAO.connectionString))
            {
                return RedirectToAction("CreateClient", new { errorMessage = "Client already exists" });
            }
            check23DAO.client.InsertClient(client, check23DAO.connectionString);
            return RedirectToAction("ClientList");
        }

        public IActionResult EditClient(Client client, string? errorMessage)
        {
            Check23DAO check23DAO = new Check23DAO();
            if (Request.Cookies["Check23UserAccess"] == null)
            {
                string returnRoute = Request.GetEncodedUrl();
                return RedirectToAction("SaveUserAccessCookie", "Home", new { nextRoute = returnRoute });
            }
            else if (_accessValidator.Client(check23DAO.accessGroup.GetAccessGroupById(Convert.ToInt32(Request.Cookies["Check23UserAccess"]), check23DAO.connectionString)))
            {
                ViewBag.errorMessage = errorMessage;
                return View(client);
            }
            else { return View("AccessDenied"); }
        }

        public IActionResult ProcessEdit(Client client) 
        {
            Check23DAO check23DAO = new Check23DAO();
            if (check23DAO.client.ClientAlreadyExists(client, check23DAO.connectionString))
            {
                return RedirectToAction("EditClient", new { errorMessage = "Client already exists" });
            }
            check23DAO.client.UpdateClient(client, check23DAO.connectionString);
            return RedirectToAction("ClientList");
        }

        public IActionResult ClientSelectionForQuoteEvaluation(string? errorMessage)
        {
            Check23DAO check23DAO = new Check23DAO();
            AccessGroup accessGroup = check23DAO.accessGroup.GetAccessGroupById(Convert.ToInt32(Request.Cookies["Check23UserAccess"]), check23DAO.connectionString);
            if (Request.Cookies["Check23UserAccess"] == null)
            {
                string returnRoute = Request.GetEncodedUrl(); //needs "get" method type to function properly otherwise route values are missing
                return RedirectToAction("SaveUserAccessCookie", "Home", new { nextRoute = returnRoute });
            }
            else if (_accessValidator.QuoteEvaluation(accessGroup))
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
                    i++;
                }
                ViewBag.clientsSelectList = clientsSelectList;
                ViewBag.errorMessage = errorMessage;
                return View();
            }
            else { return View("AccessDenied"); }
        }

        public IActionResult ProcessCreateInBetween(Client client)
        {
            Check23DAO check23DAO = new Check23DAO();
            if (check23DAO.client.ClientAlreadyExists(client, check23DAO.connectionString))
            {
                return RedirectToAction("ClientSelectionForQuoteEvaluation", new { errorMessage = "Client already exists" });
            }
            int clientId = check23DAO.client.InsertClient(client, check23DAO.connectionString);
            return RedirectToAction("CreateQuoteEvaluation", "QuoteEvaluation", new { clientId });
        }
    }
}
