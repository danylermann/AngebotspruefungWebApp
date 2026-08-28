using Check23.Additional_Classes;
using Check23.Models;
using Check23.Services.DatabankAccess;
using Check23.Services.EmailService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Check23.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _contextAccessor;

        public HomeController(ILogger<HomeController> logger, IEmailService emailService, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _emailService = emailService;
            _contextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult RedirectToQuoteEvaluationIndex()
        {
            return RedirectToAction("Index", "QuoteEvaluation");
        }

        public IActionResult CookieUsername()
        {
            return View();
        }

        public IActionResult SaveUsernameCookie(string username)
        {
            if (!string.IsNullOrEmpty(username) && Request.Cookies["Check23Username"] == null ) 
            {
                CookieOptions options = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(360),
                    Path = "/"
                };                
                Response.Cookies.Append("Check23Username", username, options);
            }
            Check23DAO check23DAO = new Check23DAO();
            if(!check23DAO.user.UsernameExists(username, check23DAO.connectionString))
            {
                return RedirectToAction("AutomaticUserCreation", "User", new { username });
            }
            return View("Index");
        }

        public IActionResult SaveUserAccessCookie(string nextRoute)
        {
            Check23DAO check23DAO = new Check23DAO();
            int userAccessGroupId = check23DAO.user.GetUserByName(Request.Cookies["Check23Username"], check23DAO.connectionString).AccessGroup_Id;
            CookieOptions options = new CookieOptions
            {               
                Path = "/"
            };
            Response.Cookies.Append("Check23UserAccess", userAccessGroupId.ToString(), options);
            return Redirect(nextRoute);
        }

        public IActionResult Test() 
        {
            //_emailService.SendEmail("Test", "E-Mail an Michael und mich", new HashSet<string> { "dlermann@weetech.de", "mhartrich@weetech.de" });

            return View("AccessDenied");
        }
        




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //Modify Search term so every word has a *
        //public IActionResult Search(string searchTerm)
        //{
        //    string modifiedSearchTerm = searchTerm;
        //    int wordCount = 0;
        //    for(int i = 0; i < searchTerm.Length; i++)
        //    {
        //        if (char.IsWhiteSpace(searchTerm[i]))
        //        {
        //            modifiedSearchTerm = modifiedSearchTerm.Insert(i+wordCount, "*");
        //            wordCount++;
        //        }
        //    }
        //    modifiedSearchTerm += "*";
        //    Console.WriteLine(modifiedSearchTerm);
        //    return View("Index");
        //}
    }
}