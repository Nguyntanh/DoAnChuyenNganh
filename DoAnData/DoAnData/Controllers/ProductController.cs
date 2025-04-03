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
    public class ProductController : ControllerBase
    {
        private readonly DoAnDataContext _context;

        public ProductController(DoAnDataContext context)
        {
            _context = context;
        }

        // GET: api/Product
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            // Customer và Admin có thể xem danh sách sản phẩm đã duyệt
            // Không yêu cầu đăng nhập, nhưng chỉ hiển thị sản phẩm "approved"
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .Where(p => p.Status == "approved")
                .ToListAsync();
        }

        // GET: api/Product/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            // Customer và Admin có thể xem chi tiết sản phẩm đã duyệt
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            // Chỉ hiển thị sản phẩm đã duyệt cho khách hàng không đăng nhập
            if (product.Status != "approved" && !User.Identity.IsAuthenticated)
            {
                return NotFound();
            }

            // Seller chỉ xem được sản phẩm của mình, Admin xem tất cả
            if (User.Identity.IsAuthenticated && User.IsInRole("seller"))
            {
                var sellerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if (product.SellerId != sellerId)
                {
                    return Forbid();
                }
            }

            return product;
        }

        // POST: api/Product
        //[Authorize(Roles = "seller")] // Chỉ Seller được tạo sản phẩm
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            var sellerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            product.SellerId = sellerId; // Gán SellerId từ token
            product.Status = "pending"; // Mặc định trạng thái là "pending" để Admin duyệt
            product.CreatedAt = DateTime.Now;

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId }, product);
        }

        // PUT: api/Product/5
        //[Authorize(Roles = "seller,admin")] // Seller và Admin được cập nhật
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product product)
        {
            if (id != product.ProductId)
            {
                return BadRequest();
            }

            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            // Seller chỉ được cập nhật sản phẩm của mình
            if (User.IsInRole("seller"))
            {
                var sellerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if (existingProduct.SellerId != sellerId)
                {
                    return Forbid();
                }
            }

            // Admin có thể cập nhật bất kỳ sản phẩm nào
            _context.Entry(existingProduct).CurrentValues.SetValues(product);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            return NoContent();
        }

        [Authorize]
        // DELETE: api/Product/5
        //[Authorize(Roles = "seller,admin")] // Seller và Admin được xóa
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Seller chỉ được xóa sản phẩm của mình
            if (User.IsInRole("seller"))
            {
                var sellerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if (product.SellerId != sellerId)
                {
                    return Forbid();
                }
            }

            // Admin có thể xóa bất kỳ sản phẩm nào
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}