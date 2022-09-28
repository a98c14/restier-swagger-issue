using System.ComponentModel.DataAnnotations;

namespace restier_swagger_bug_report.Models
{
    public class Person
    {
        [Key]
        public int Id { get; set;}
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
    }
}
