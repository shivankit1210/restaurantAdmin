using Microsoft.AspNetCore.Mvc;
using RestaurantAdmin.Models;
using RestaurantAdmin.Data;
using Microsoft.EntityFrameworkCore;


namespace RestaurantAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        // Get all orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders.ToListAsync();
        }

        // Get order by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound(new { message = "Order not found" });
            return order;
        }

        // Create a new order
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] Order order)
        {
            Console.WriteLine($"Received order: {order.CustomerName}, {order.FoodItem}, Price: {order.Price}");

            if (order == null || string.IsNullOrEmpty(order.CustomerName) || string.IsNullOrEmpty(order.FoodItem))
            {
                return BadRequest(new { message = "Invalid order data. Please provide CustomerName and FoodItem." });
            }

            if (order.Price <= 0)
            {
                return BadRequest(new { message = "Invalid price. Please provide a valid amount." });
            }

            try
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Order placed successfully!", orderId = order.Id });
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new { message = "Database error", error = dbEx.InnerException?.Message ?? dbEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Server error", error = ex.Message });
            }
        }

        // Class to accept order status update
        public class OrderStatusUpdate
        {
            public string?  Status { get; set; }
        }

        // Update Order Status
        [HttpPut("update-status/{id}")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] OrderStatusUpdate request)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }

            order.Status = request.Status!;  // Update status field
            await _context.SaveChangesAsync();

            return Ok(new { message = "Order status updated successfully!" });
        }

        // Delete an order
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound(new { message = "Order not found" });

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Order deleted successfully!" });
        }
    }
}
































