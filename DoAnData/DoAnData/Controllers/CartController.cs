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
    public class CartController : ControllerBase
    {
        private readonly DoAnDataContext _context;

        public CartController(DoAnDataContext context)
        {
            _context = context;
        }

        // GET: api/Cart
        //[Authorize(Roles = "customer,admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cart>>> GetCarts()
        {
            if (User.IsInRole("customer"))
            {
                // Customer chỉ xem được giỏ hàng của mình
                var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return await _context.Cart
                    .Include(c => c.Customer)
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                    .Where(c => c.CustomerId == customerId)
                    .ToListAsync();
            }

            // Admin xem được tất cả giỏ hàng
            return await _context.Cart
                .Include(c => c.Customer)
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .ToListAsync();
        }

        // GET: api/Cart/5
        //[Authorize(Roles = "customer,admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Cart>> GetCart(int id)
        {
            var cart = await _context.Cart
                .Include(c => c.Customer)
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cart == null)
            {
                return NotFound();
            }

            // Customer chỉ xem được giỏ hàng của mình
            if (User.IsInRole("customer"))
            {
                var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if (cart.CustomerId != customerId)
                {
                    return Forbid();
                }
            }

            // Admin có thể xem bất kỳ giỏ hàng nào
            return cart;
        }

        // POST: api/Cart
        //[Authorize(Roles = "customer")] // Chỉ Customer tạo giỏ hàng
        [HttpPost]
        public async Task<ActionResult<Cart>> CreateCart(Cart cart)
        {
            var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            cart.CustomerId = customerId; // Gán CustomerId từ token
            cart.CreatedAt = DateTime.Now;

            _context.Cart.Add(cart);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCart), new { id = cart.Id }, cart);
        }

        // PUT: api/Cart/5
        //[Authorize(Roles = "customer")] // Chỉ Customer cập nhật giỏ hàng
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCart(int id, Cart cart)
        {
            if (id != cart.Id)
            {
                return BadRequest();
            }

            var existingCart = await _context.Cart.FindAsync(id);
            if (existingCart == null)
            {
                return NotFound();
            }

            // Customer chỉ cập nhật giỏ hàng của mình
            var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (existingCart.CustomerId != customerId)
            {
                return Forbid();
            }

            _context.Entry(existingCart).CurrentValues.SetValues(cart);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CartExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            return NoContent();
        }

        // DELETE: api/Cart/5
        //[Authorize(Roles = "customer")] // Chỉ Customer xóa giỏ hàng
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart(int id)
        {
            var cart = await _context.Cart.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }

            // Customer chỉ xóa giỏ hàng của mình
            var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (cart.CustomerId != customerId)
            {
                return Forbid();
            }

            _context.Cart.Remove(cart);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool CartExists(int id)
        {
            return _context.Cart.Any(e => e.Id == id);
        }
    }
}