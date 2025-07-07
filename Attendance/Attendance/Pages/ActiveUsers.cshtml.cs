using Attendance.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;

namespace Attendance.Pages;

public class ActiveUsersModel : PageModel
{
    public class ActiveLog
    {
        public string UserName { get; set; } = ""; 
        public string LogId { get; set; } = ""; 
        public string ActivityName { get; set; } = "";
        public DateTime StartTime { get; set; }
        public TimeSpan Elapsed => DateTime.Now - StartTime;
    }

    public List<ActiveLog> ActiveLogs { get; set; } = new();

    public void OnGet()
    {
        using var conn = DBConnect.Connect();

        //var cmd = new SqliteCommand(@"
        //    SELECT u.FirstName || ' ' || u.LastName AS UserName,
        //           a.Name AS ActivityName,
        //           l.StartTime
        //    FROM ActivityLogs l
        //    JOIN Users u ON u.Id = l.UserId
        //    JOIN Activities a ON a.Id = l.ActivityId
        //    WHERE l.EndTime IS NULL
        //    ORDER BY l.StartTime ASC", conn); 

        var cmd = new SqliteCommand(@"
    SELECT l.Id,
           u.FirstName || ' ' || u.LastName AS UserName,
           a.Name AS ActivityName,
           l.StartTime
    FROM ActivityLogs l
    JOIN Users u ON u.Id = l.UserId
    JOIN Activities a ON a.Id = l.ActivityId
    WHERE l.EndTime IS NULL
    ORDER BY l.StartTime ASC", conn);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            ActiveLogs.Add(new ActiveLog
            {
                LogId = reader["Id"].ToString(),
                UserName = reader["UserName"].ToString(),
                ActivityName = reader["ActivityName"].ToString(),
                StartTime = DateTime.Parse(reader["StartTime"].ToString())
            });
        }
    }

    public async Task<IActionResult> OnPostEndAsync([FromForm] int logId)
    {
        var end = DateTime.Now;

        using var conn = DBConnect.Connect();

        DateTime start;
        using (var cmd = new SqliteCommand("SELECT StartTime FROM ActivityLogs WHERE Id = @id", conn))
        {
            cmd.Parameters.AddWithValue("@id", logId);
            var result = await cmd.ExecuteScalarAsync();
            if (result == null)
            {
                return NotFound(); // nebo návrat na stránku s chybou
            }
            start = DateTime.Parse(result.ToString());
        }

        var duration = end - start;
        var text = FormatDuration(duration);

        using (var update = new SqliteCommand("UPDATE ActivityLogs SET EndTime = @end, DurationText = @text WHERE Id = @id", conn))
        {
            update.Parameters.AddWithValue("@end", end);
            update.Parameters.AddWithValue("@text", text);
            update.Parameters.AddWithValue("@id", logId);
            await update.ExecuteNonQueryAsync();
        }

        return RedirectToPage(); // Refreshne aktuální stránku
    }

    private string FormatDuration(TimeSpan d)
    {
        if (d.TotalSeconds < 60)
            return $"{(int)d.TotalSeconds} s";
        if (d.TotalMinutes < 60)
            return $"{(int)d.TotalMinutes} min {(int)d.Seconds} s";
        return $"{(int)d.TotalHours} hod {(int)d.Minutes} min {(int)d.Seconds} s";
    }

    public class ActiveUserViewModel
    {
        public int LogId { get; set; }
        public string UserFullName { get; set; }
        public string ActivityName { get; set; }
        public DateTime StartTime { get; set; }
        public string DurationText { get; set; }
    }

}
