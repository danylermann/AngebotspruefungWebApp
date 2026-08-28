using Check23.Models;
using Check23.Services.DatabankAccess;
using Microsoft.AspNetCore.Mvc;

namespace Check23.Controllers
{
    public class CommentController : Controller
    {
        public IActionResult CreateRequirementComment(int requirementId)
        {
            ViewBag.requirementId = requirementId;            
            return View();
        }

        public IActionResult ProcessCreateRequirementComment(Comment requirementComment)
        {
            Check23DAO check23DAO = new Check23DAO();
            requirementComment.Creator = Request.Cookies["Check23Username"] ?? "No Username";
            check23DAO.comment.InsertRequirementComment(requirementComment, check23DAO.connectionString);
            return RedirectToAction("DetailsRequirement", "Requirement", new { requirementId = requirementComment.ForeignKey_id });
        }

        public IActionResult CreateSolutionComment(int solutionId)
        {
            ViewBag.solutionId = solutionId;
            return View();
        }

        public IActionResult ProcessCreateSolutionComment(Comment solutionComment)
        {
            Check23DAO check23DAO = new Check23DAO();
            solutionComment.Creator = Request.Cookies["Check23Username"] ?? "No Username";
            check23DAO.comment.InsertSolutionComment(solutionComment, check23DAO.connectionString);
            return RedirectToAction("DetailsSolution", "Solution", new { solutionId = solutionComment.ForeignKey_id });
        }
    }
}
