using Check23.Models;
using Check23.Services.AccessValidation;
using Check23.Services.DatabankAccess;
using Check23.Services.EmailService;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Check23.Controllers
{
    public class UserController : Controller
    {
        private readonly IAccessValidationService _accessValidator;
        private readonly IEmailService _emailService;
        
        public UserController(IAccessValidationService accessValidationService, IEmailService emailService) 
        {
            _accessValidator = accessValidationService;
            _emailService = emailService;
        }
        public IActionResult UserList()
        {
            Check23DAO check23DAO = new Check23DAO();
            if (Request.Cookies["Check23UserAccess"] == null)
            {
                string returnRoute = Request.GetEncodedUrl();
                return RedirectToAction("SaveUserAccessCookie", "Home", new { nextRoute = returnRoute });
            }
            else if (_accessValidator.User(check23DAO.accessGroup.GetAccessGroupById(Convert.ToInt32(Request.Cookies["Check23UserAccess"]), check23DAO.connectionString)))
            {
                return View(check23DAO.user.GetAllUsers(check23DAO.connectionString));
            }
            else { return View("AccessDenied"); }
        }

        public IActionResult CreateUser()
        {
            Check23DAO check23DAO = new Check23DAO();
            if (Request.Cookies["Check23UserAccess"] == null)
            {
                string returnRoute = Request.GetEncodedUrl();
                return RedirectToAction("SaveUserAccessCookie", "Home", new { nextRoute = returnRoute });
            }
            else if (_accessValidator.User(check23DAO.accessGroup.GetAccessGroupById(Convert.ToInt32(Request.Cookies["Check23UserAccess"]), check23DAO.connectionString)))
            {
                List<AccessGroup> accessGroups = check23DAO.accessGroup.GetAllAccessGroupsExceptDefault(check23DAO.connectionString);
                SelectList accessGroupsSelectList = new SelectList(accessGroups, nameof(AccessGroup.Id), nameof(AccessGroup.Name));
                ViewBag.accessGroupSelectList = accessGroupsSelectList;
                List<AreaOfResponsibility> areasOfResponsibility = check23DAO.user.GetAllAreasOfResponsibility(check23DAO.connectionString);
                SelectList areasOfResponsibilitySelectList = new SelectList(areasOfResponsibility, nameof(AreaOfResponsibility.Id), nameof(AreaOfResponsibility.Name));
                ViewBag.areasOfResponsibilitySelectList = areasOfResponsibilitySelectList;
                return View();
            }
            else { return View("AccessDenied"); }
        }

        public IActionResult ProcessCreate(User user)
        {
            user.Email = user.Name + "@weetech.de";
            Check23DAO check23DAO = new Check23DAO();
            int insertedId = check23DAO.user.InsertUser(user, check23DAO.connectionString);
            user.AreasOfResponsibility = user.AreasOfResponsibility == null ? new List<int>() : user.AreasOfResponsibility;
            foreach (int areaOfResponsibilityId in user.AreasOfResponsibility)
            {
                check23DAO.user.InsertUserHasAreaOfResponsibility(insertedId, areaOfResponsibilityId, check23DAO.connectionString);
            }
            return RedirectToAction("UserList");
        }

        public IActionResult EditUser(int userId)
        {
            Check23DAO check23DAO = new Check23DAO();
            if (Request.Cookies["Check23UserAccess"] == null)
            {
                string returnRoute = Request.GetEncodedUrl();
                return RedirectToAction("SaveUserAccessCookie", "Home", new { nextRoute = returnRoute });
            }
            else if (_accessValidator.User(check23DAO.accessGroup.GetAccessGroupById(Convert.ToInt32(Request.Cookies["Check23UserAccess"]), check23DAO.connectionString)))
            {
                User foundUser = check23DAO.user.GetUserById(userId, check23DAO.connectionString);
                List<AccessGroup> accessGroups = check23DAO.accessGroup.GetAllAccessGroupsExceptDefault(check23DAO.connectionString);
                SelectList accessGroupsSelectList = new SelectList(accessGroups, nameof(AccessGroup.Id), nameof(AccessGroup.Name));
                ViewBag.accessGroupSelectList = accessGroupsSelectList;
                List<AreaOfResponsibility> areasOfResponsibility = check23DAO.user.GetAllAreasOfResponsibility(check23DAO.connectionString);
                SelectList areasOfResponsibilitySelectList = new SelectList(areasOfResponsibility, nameof(AreaOfResponsibility.Id), nameof(AreaOfResponsibility.Name));
                List<int> areaOfResponsibilityIds = check23DAO.user.GetAreaOfResponsibilityIdsByUserId(userId, check23DAO.connectionString);
                foreach (SelectListItem areaOfResponsibility in areasOfResponsibilitySelectList)
                {
                    if (areaOfResponsibilityIds.Contains(Convert.ToInt32(areaOfResponsibility.Value)))
                    {
                        areaOfResponsibility.Selected = true;
                    }
                }
                ViewBag.areasOfResponsibilitySelectList = areasOfResponsibilitySelectList;
                return View(foundUser);
            }
            else { return View("AccessDenied"); }
        }

        public IActionResult ProcessEdit(User user)
        {
            Check23DAO check23DAO = new Check23DAO();
            check23DAO.user.UpdateUser(user, check23DAO.connectionString);
            //Check if any ids were added into the list and insert them into the database
            user.AreasOfResponsibility = user.AreasOfResponsibility == null ? new List<int>() : user.AreasOfResponsibility;
            user.oldAreasOfResponsibility = user.oldAreasOfResponsibility == null ? new List<int>() : user.oldAreasOfResponsibility;
            foreach (int areaOfResponsibilityId in user.AreasOfResponsibility)
            {
                if (!user.oldAreasOfResponsibility.Contains(areaOfResponsibilityId))
                {
                    check23DAO.user.InsertUserHasAreaOfResponsibility(user.Id, areaOfResponsibilityId, check23DAO.connectionString);
                }
            }
            //Check if any ids were removed from the list and delete them from the database
            foreach (int areaOfResponsibilityId in user.oldAreasOfResponsibility)
            {
                if (!user.AreasOfResponsibility.Contains(areaOfResponsibilityId))
                {
                    check23DAO.user.DeleteUserHasAreOfResponsibility(user.Id, areaOfResponsibilityId, check23DAO.connectionString);
                }
            }
            
            return RedirectToAction("UserList");
        }

        public IActionResult AutomaticUserCreation(string username)
        {
            User user = new User(username, username + "@weetech.com", 0);
            Check23DAO check23DAO = new Check23DAO();
            int newId = check23DAO.user.InsertUser(user, check23DAO.connectionString);
            HashSet<string> emails = check23DAO.user.GetEmailadressesByAreaOfResponsibilityId(1, check23DAO.connectionString);
            _emailService.SendEmail("Neuer Nutzer automatisch angelegt", "Der neue Nutzer mit der Id " + newId + " und dem Namen" + username + " wurde automatisch erstellt. Zugriffsrechte und Zuständigkeitsbereich müssen eingestellt werden.", emails);
            return RedirectToAction("Index", "Home");
        }
    }
}
