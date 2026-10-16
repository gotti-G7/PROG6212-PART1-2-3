
namespace RaceDayApi1.Models
{
    public class Category
    {
        public int CategoryID { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal DistanceKM { get; set; }

        public string? Description { get; set; } = String.Empty;
    }
}
