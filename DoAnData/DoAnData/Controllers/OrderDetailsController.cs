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
    public class OrderDetailsController : ControllerBase
    {
        private readonly DoAnDataContext _context;

        public OrderDetailsController(DoAnDataContext context)
        {
            _context = context;
        }

        // GET: api/OrderDetails/{orderId}
        //[Authorize(Roles = "customer,seller,admin")]
        [HttpGet("{orderId}")]
        public async Task<ActionResult<IEnumerable<OrderDetail>>> GetOrderDetails(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

            // Kiểm tra quyền truy cập
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (User.IsInRole("customer") && order.CustomerId != userId)
            {
                return Forbid();
            }
            if (User.IsInRole("seller") && order.SellerId != userId)
            {
                return Forbid();
            }

            // Admin có thể xem tất cả
            return await _context.OrderDetails
                .Include(od => od.Product)
                .Where(od => od.OrderId == orderId)
                .ToListAsync();
        }
    }
}