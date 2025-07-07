using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Attendance.Data;

[Table("Users")]
public class User
{
    public int Id { get; set; }

    [MaxLength(50), Display(Name = "Jméno")]

    public string FirstName { get; set; }

    [MaxLength(50), Display(Name = "Příjmení")]

    public string LastName { get; set; }

    [MaxLength(50), Display(Name= "Nick")]

    public string Nick { get; set; } 
    public string logId { get; set; } 

}
