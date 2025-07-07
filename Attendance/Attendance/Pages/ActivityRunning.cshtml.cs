using Attendance.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;

namespace Attendance.Pages;

public class ActivityRunningModel : PageModel
{

    [BindProperty(SupportsGet = true)]
    public int LogId { get;  set; } 
    public string ActivityName   { get;  set; }
    public DateTime StartTime { get;  set; }
    public object EndTime { get; private set; }
    public object Nick { get; private set; }

    public void OnGet(int logId)
    {
        LogId = logId;

        using var conn = DBConnect.Connect();

        using var cmd = new SqliteCommand(@"
        SELECT a.Name AS ActivityName, l.StartTime
        FROM ActivityLogs l
        JOIN Activities a ON l.ActivityId = a.Id
        WHERE l.Id = @id", conn);
        cmd.Parameters.AddWithValue("@id", logId);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            ActivityName = reader["ActivityName"].ToString();
            StartTime = DateTime.Parse(reader["StartTime"].ToString());
        }
    }


    public IActionResult OnPostFinish()
    {
        var end = DateTime.Now;

        using var conn = DBConnect.Connect();

        DateTime start;
        using (var cmd = new SqliteCommand("SELECT StartTime FROM ActivityLogs WHERE Id = @id", conn))
        {
            cmd.Parameters.AddWithValue("@id", LogId);
            var result = cmd.ExecuteScalar();
            if (result == null)
            {
                // napø. pøesmìrování zpìt nebo chyba
                TempData["Error"] = "Záznam nebyl nalezen nebo nemá StartTime.";
                return RedirectToPage("Error");
            }
            start = DateTime.Parse(result.ToString());
        }

        var duration = end - start;
        var text = FormatDuration(duration);

        string report = $"Uživatel: {Nick}\n" +
                $"Èinnost: {ActivityName}\n" +
                $"Zaèátek: {StartTime}\n" +
                $"Konec: {EndTime}\n" +
                $"Doba trvání: {text}";

        using var update = new SqliteCommand("UPDATE ActivityLogs SET EndTime = @end, DurationText = @text, ReportText = @report WHERE Id = @id", conn);
        update.Parameters.AddWithValue("@end", end);
        update.Parameters.AddWithValue("@text", text);
        update.Parameters.AddWithValue("@report", report);
        update.Parameters.AddWithValue("@id", LogId);
        update.ExecuteNonQuery();

        int userId = GetUserId(LogId, conn); // naète userId z DB
        return RedirectToPage("ActiveUsers", new { userId = userId });

    }

    private string FormatDuration(TimeSpan d)
    {
        if (d.TotalSeconds < 60)
            return $"{(int)d.TotalSeconds} sekund";
        if (d.TotalMinutes < 60)
            return $"{(int)d.TotalMinutes} minut";
        return $"{(int)d.TotalHours} hodin {(d.Minutes)} minut";
    }

    private int GetUserId(int logId, SqliteConnection conn)
    {
        using var cmd = new SqliteCommand("SELECT UserId FROM ActivityLogs WHERE Id = @id", conn);
        cmd.Parameters.AddWithValue("@id", logId);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

}
