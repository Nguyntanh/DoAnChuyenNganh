using DoAnData.Data;
using DoAnData.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DoAnData.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupportTicketController : ControllerBase
    {
        private readonly DoAnDataContext _context;

        public SupportTicketController(DoAnDataContext context)
        {
            _context = context;
        }

        // GET: api/SupportTicket
        //[Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SupportTicket>>> GetSupportTickets()
        {
            return await _context.SupportTickets
                .Include(st => st.User)
                .Include(st => st.Order)
                .ToListAsync();
        }

        // POST: api/SupportTicket
        //[Authorize(Roles = "customer")]
        [HttpPost]
        public async Task<ActionResult<SupportTicket>> CreateSupportTicket(SupportTicket ticket)
        {
            var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            ticket.UserId = customerId;
            ticket.Status = "open";
            ticket.CreatedAt = DateTime.Now;

            _context.SupportTickets.Add(ticket);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSupportTickets), new { id = ticket.Id }, ticket);
        }

        // PUT: api/SupportTicket/resolve/{id}
        //[Authorize(Roles = "admin")]
        [HttpPut("resolve/{id}")]
        public async Task<IActionResult> ResolveSupportTicket(int id)
        {
            var ticket = await _context.SupportTickets.FindAsync(id);
            if (ticket == null || ticket.Status != "open")
            {
                return NotFound();
            }

            ticket.Status = "resolved";
            ticket.ResolvedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}