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
    public class CartItemController : ControllerBase
    {
        private readonly DoAnDataContext _context;

        public CartItemController(DoAnDataContext context)
        {
            _context = context;
        }

        // POST: api/CartItem/{cartId}
        //[Authorize(Roles = "customer")]
        [HttpPost("{cartId}")]
        public async Task<ActionResult<CartItem>> AddCartItem(int cartId, CartItem cartItem)
        {
            var cart = await _context.Cart.FindAsync(cartId);
            if (cart == null)
            {
                return NotFound();
            }

            var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (cart.CustomerId != customerId)
            {
                return Forbid();
            }

            cartItem.CartId = cartId;
            _context.CartItem.Add(cartItem);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(AddCartItem), new { cartId = cartItem.CartId }, cartItem);
        }

        // DELETE: api/CartItem/{cartId}/{productId}
        //[Authorize(Roles = "customer")]
        [HttpDelete("{cartId}/{productId}")]
        public async Task<IActionResult> DeleteCartItem(int cartId, int productId)
        {
            var cartItem = await _context.CartItem
                .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);
            if (cartItem == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart.FindAsync(cartId);
            var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (cart.CustomerId != customerId)
            {
                return Forbid();
            }

            _context.CartItem.Remove(cartItem);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}