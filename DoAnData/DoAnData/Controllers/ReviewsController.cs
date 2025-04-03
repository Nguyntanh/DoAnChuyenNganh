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
    public class ReviewsController : ControllerBase
    {
        private readonly DoAnDataContext _context;

        public ReviewsController(DoAnDataContext context)
        {
            _context = context;
        }

        // GET: api/Reviews/product/{productId}
        [HttpGet("product/{productId}")]
        public async Task<ActionResult<IEnumerable<Review>>> GetReviewsByProduct(int productId)
        {
            // Không yêu cầu đăng nhập để xem đánh giá
            return await _context.Reviews
                .Include(r => r.Customer)
                .Where(r => r.ProductId == productId)
                .ToListAsync();
        }

        // POST: api/Reviews
        //[Authorize(Roles = "customer")]
        [HttpPost]
        public async Task<ActionResult<Review>> CreateReview(Review review)
        {
            var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            review.CustomerId = customerId;
            review.ReviewDate = DateTime.Now;

            if (review.Rating < 1 || review.Rating > 5)
            {
                return BadRequest(new { message = "Rating phải từ 1 đến 5." });
            }

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetReviewsByProduct), new { productId = review.ProductId }, review);
        }
    }
}