using System.ComponentModel.DataAnnotations;

namespace RestaurantAdmin.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public string? CustomerName { get; set; }
        public string? FoodItem { get; set; }
        public decimal? Price { get; set; }
        public string Status { get; set; } = "Pending";
    }
}
