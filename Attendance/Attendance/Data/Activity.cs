using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Attendance.Data; 

[Table("Activities")]
public class Activity
{
    [Key]
    public int Id { get; set; } 



    [MaxLength(50), Display(Name = "Činnost")]
    [Required]
    public string Name { get; set; }
}
