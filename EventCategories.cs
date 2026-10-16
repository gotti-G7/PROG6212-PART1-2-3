using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayApi1.Models
{
    [Table("Event_Categories")]
    [PrimaryKey(nameof(EventID), nameof(CategoryID))]
    public class EventCategory
    {
        public int EventID { get; set; }

        public int CategoryID { get; set; }
    }
}