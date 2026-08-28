using Check23.Models;
using Check23.Services.DatabankAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Check23.Controllers
{
    public class TicketController : Controller
    {
        private List<string> types = new List<string> { "ESW", "EHW"/*, "Service" */};
        public IActionResult CreateTicket(int quoteEvalId)
        {
            ViewBag.quoteEvalId = quoteEvalId;
            SelectList typeSelectList = new SelectList(types);
            ViewBag.typeSelectList = typeSelectList;
            return View();
        }

        public IActionResult ProcessCreate(Ticket ticket)
        {
            Check23DAO check23DAO = new Check23DAO();
            check23DAO.ticket.InsertTicket(ticket, check23DAO.connectionString);
            return RedirectToAction("DetailsQuoteEvaluation", "QuoteEvaluation", new { quoteEvalId = ticket.QuoteEvaluation_id });
        }

        public IActionResult DetailsTicket(int ticketId) 
        {
            Check23DAO check23DAO = new Check23DAO();
            return View(check23DAO.ticket.GetTicketById(ticketId, check23DAO.connectionString));
        }

        public IActionResult EditTicket(Ticket ticket)
        {
            SelectList typeSelectList = new SelectList(types);
            foreach (var type in typeSelectList) 
            {
                if(type.Value == ticket.Type)
                {
                    type.Selected = true;
                }
            }
            ViewBag.typeSelectList = typeSelectList;
            return View(ticket);
        }

        public IActionResult ProcessEdit(Ticket ticket)
        {
            Check23DAO check23DAO = new Check23DAO();
            check23DAO.ticket.UpdateTicket(ticket, check23DAO.connectionString);
            return RedirectToAction("DetailsQuoteEvaluation", "QuoteEvaluation", new { quoteEvalId = ticket.QuoteEvaluation_id });
        }
    }
}
