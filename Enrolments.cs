using System.ComponentModel.DataAnnotations;

namespace RaceDayApi1.Models
{
    public class Enrolments
    {
        [Key]
        public int EnrolmentID { get; set; }

        public int EventID { get; set; }

        public int CategoryID { get; set; }

        public int UserID { get; set; }

        public string Status { get; set; } =string.Empty;
    }
}
