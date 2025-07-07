using System.ComponentModel.DataAnnotations;

namespace Attendance.Data;

public class ActivityLog
{
    internal string DurationText;
    public string? ActivityName { get; set; } 

    [Key]
    public int Id { get; set; }

    public int LogId { get; set; }

    public string ReportText { get; set; }


    public int UserId { get; set; }
    public User User { get; set; }

    public int ActivityId { get; set; }
    public Activity Activity { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; internal set; }
}
