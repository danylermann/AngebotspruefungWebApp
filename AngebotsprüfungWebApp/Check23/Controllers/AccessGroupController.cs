using Check23.Models;
using Check23.Services.AccessValidation;
using Check23.Services.DatabankAccess;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Check23.Controllers
{
    public class AccessGroupController : Controller
    {
        IAccessValidationService _accessValidator;
        public AccessGroupController(IAccessValidationService accessValidationService) 
        { 
            _accessValidator = accessValidationService;
        }
        public IActionResult AccessGroupList()
        {
            Check23DAO check23DAO = new Check23DAO();
            if (Request.Cookies["Check23UserAccess"] == null)
            {
                string returnRoute = Request.GetEncodedUrl();
                return RedirectToAction("SaveUserAccessCookie", "Home", new { nextRoute = returnRoute });
            }
            else if (_accessValidator.AccessGroup(check23DAO.accessGroup.GetAccessGroupById(Convert.ToInt32(Request.Cookies["Check23UserAccess"]), check23DAO.connectionString)))
            {
                return View(check23DAO.accessGroup.GetAllAccessGroupsExceptDefault(check23DAO.connectionString));
            }
            else { return View("AccessDenied"); }
        }

        public IActionResult CreateAccessGroup()
        {
            Check23DAO check23DAO = new Check23DAO();
            if (Request.Cookies["Check23UserAccess"] == null)
            {
                string returnRoute = Request.GetEncodedUrl();
                return RedirectToAction("SaveUserAccessCookie", "Home", new { nextRoute = returnRoute });
            }
            else if (_accessValidator.AccessGroup(check23DAO.accessGroup.GetAccessGroupById(Convert.ToInt32(Request.Cookies["Check23UserAccess"]), check23DAO.connectionString)))
            {
                return View();
            }
            else { return View("AccessDenied"); }
        }

        public IActionResult ProcessCreate(AccessGroup accessGroup)
        {
            Check23DAO check23DAO = new Check23DAO();
            check23DAO.accessGroup.InsertAccessGroup(accessGroup, check23DAO.connectionString);
            return RedirectToAction("AccessGroupList");
        }

        public IActionResult EditAccessGroup(int accessGroupId) 
        {
            Check23DAO check23DAO = new Check23DAO();
            if (Request.Cookies["Check23UserAccess"] == null)
            {
                string returnRoute = Request.GetEncodedUrl();
                return RedirectToAction("SaveUserAccessCookie", "Home", new { nextRoute = returnRoute });
            }
            else if (_accessValidator.AccessGroup(check23DAO.accessGroup.GetAccessGroupById(Convert.ToInt32(Request.Cookies["Check23UserAccess"]), check23DAO.connectionString)))
            {
                return View(check23DAO.accessGroup.GetAccessGroupById(accessGroupId, check23DAO.connectionString));
            }
            else { return View("AccessDenied"); }
        }

        public IActionResult ProcessEdit(AccessGroup accessGroup)
        {
            Check23DAO check23DAO = new Check23DAO();
            check23DAO.accessGroup.UpdateAccessGroup(accessGroup, check23DAO.connectionString);
            return RedirectToAction("AccessGroupList");
        }

        public IActionResult DetailsAccessGroup(int accessGroupId)
        {
            Check23DAO check23DAO = new Check23DAO();
            if (Request.Cookies["Check23UserAccess"] == null)
            {
                string returnRoute = Request.GetEncodedUrl();
                return RedirectToAction("SaveUserAccessCookie", "Home", new { nextRoute = returnRoute });
            }
            else if (_accessValidator.AccessGroup(check23DAO.accessGroup.GetAccessGroupById(Convert.ToInt32(Request.Cookies["Check23UserAccess"]), check23DAO.connectionString)))
            {
                return View(check23DAO.accessGroup.GetAccessGroupById(accessGroupId, check23DAO.connectionString));
            }
            else { return View("AccessDenied"); }
        }
    }
}
