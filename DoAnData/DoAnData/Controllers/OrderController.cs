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
    public class OrderController : ControllerBase
    {
        private readonly DoAnDataContext _context;

        public OrderController(DoAnDataContext context)
        {
            _context = context;
        }

        // GET: api/Order/customer
        //[Authorize(Roles = "customer")]
        [HttpGet("customer")]
        public async Task<ActionResult<IEnumerable<Order>>> GetCustomerOrders()
        {
            var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Seller)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Where(o => o.CustomerId == customerId)
                .ToListAsync();
        }

        // GET: api/Order/seller
        //[Authorize(Roles = "seller")]
        [HttpGet("seller")]
        public async Task<ActionResult<IEnumerable<Order>>> GetSellerOrders()
        {
            var sellerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Seller)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Where(o => o.SellerId == sellerId)
                .ToListAsync();
        }

        // GET: api/Order/all
        //[Authorize(Roles = "admin")]
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Seller)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .ToListAsync();
        }

        // POST: api/Order
        //[Authorize(Roles = "customer")]
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(Order order)
        {
            var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            order.CustomerId = customerId;
            order.OrderDate = DateTime.Now;
            order.Status = "pending"; // Mặc định trạng thái đơn hàng

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCustomerOrders), new { id = order.Id }, order);
        }

        // PUT: api/Order/seller/process/{id}
        //[Authorize(Roles = "seller")]
        [HttpPut("seller/process/{id}")]
        public async Task<IActionResult> ProcessOrder(int id, [FromBody] string status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            var sellerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (order.SellerId != sellerId)
            {
                return Forbid();
            }

            order.Status = status; // Ví dụ: "shipped", "delivered"
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // GET: api/Order/seller/revenue
        //[Authorize(Roles = "seller")]
        [HttpGet("seller/revenue")]
        public async Task<ActionResult> GetSellerRevenue()
        {
            var sellerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var revenue = await _context.Orders
                .Where(o => o.SellerId == sellerId && o.Status == "delivered")
                .SumAsync(o => o.TotalPrice);

            return Ok(new { SellerId = sellerId, TotalRevenue = revenue });
        }
    }
}