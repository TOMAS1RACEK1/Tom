using Attendance.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Attendance.Pages
{
    public class ReportModel : PageModel
    {
        [BindProperty]
        public string Nick { get; set; }

        public int? UserId { get; set; }
        public string UserFullName { get; set; }
        public string ReportType { get; set; } = "daily";
        public DateTime ReportDate { get; set; } = DateTime.Today;

        public List<ReportLog> Logs { get; set; } = new();

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Nick))
            {
                ModelState.AddModelError(string.Empty, "Zadejte přezdívku.");
                return Page();
            }

            using var conn = DBConnect.Connect();
            using var userCmd = new SqliteCommand("SELECT Id, FirstName, LastName FROM Users WHERE Nick = @nick", conn);
            userCmd.Parameters.AddWithValue("@nick", Nick);

            using var reader = await userCmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var userId = reader.GetInt32(0);
                return RedirectToPage("Report", new
                {
                    userId = userId,
                    reportType = "daily",
                    reportDate = DateTime.Today.ToString("yyyy-MM-dd")
                });
            }

            ModelState.AddModelError(string.Empty, "Uživatel nebyl nalezen.");
            return Page();
        }

        public async Task<IActionResult> OnGetAsync(int? userId, string reportType = "daily", DateTime? reportDate = null)
        {
            if (userId == null)
            {
                return Page(); // Pokud není uživatel vybrán
            }

            UserId = userId;
            ReportType = reportType ?? "daily";
            ReportDate = reportDate ?? DateTime.Today;

            using var conn = DBConnect.Connect();

            using var userCmd = new SqliteCommand("SELECT FirstName, LastName FROM Users WHERE Id = @id", conn);
            userCmd.Parameters.AddWithValue("@id", userId);

            using var reader = await userCmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                UserFullName = $"{reader["FirstName"]} {reader["LastName"]}";
            }
            else
            {
                UserFullName = "Neznámý uživatel";
                return Page();
            }

            var cmd = conn.CreateCommand();
            DateTime start, end;

            if (ReportType == "monthly")
            {
                start = new DateTime(ReportDate.Year, ReportDate.Month, 1);
                end = start.AddMonths(1);
            }
            else
            {
                start = ReportDate.Date;
                end = start.AddDays(1);
            }

            cmd.CommandText = @"
                SELECT a.Name, al.StartTime, al.EndTime
                FROM ActivityLogs al
                JOIN Activities a ON al.ActivityId = a.Id
                WHERE al.UserId = @userId AND al.StartTime BETWEEN @start AND @end
                ORDER BY al.StartTime";
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@start", start);
            cmd.Parameters.AddWithValue("@end", end);

            using var logReader = await cmd.ExecuteReaderAsync();
            Logs = new();

            while (await logReader.ReadAsync())
            {
                var activityName = logReader["Name"].ToString();
                var startTime = DateTime.Parse(logReader["StartTime"].ToString());
                var endTime = logReader["EndTime"] != DBNull.Value ? DateTime.Parse(logReader["EndTime"].ToString()) : DateTime.Now;
                var duration = endTime - startTime;

                Logs.Add(new ReportLog
                {
                    ActivityName = activityName,
                    StartTime = startTime,
                    EndTime = endTime,
                    DurationText = $"{(int)duration.TotalMinutes} min {duration.Seconds} s"
                });
            }

            return Page();
        }

        public class ReportLog
        {
            public string ActivityName { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public string DurationText { get; set; }
        }
    }
}
