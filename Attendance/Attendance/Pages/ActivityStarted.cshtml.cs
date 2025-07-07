using Attendance.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;

namespace Attendance.Pages
{
    public class ActivityStartedModel : PageModel
    {
        private object logId;

        //private readonly DBConnect connect;

        //public ActivityStartedModel(DBConnect connect) => this.connect = connect;

        [BindProperty(SupportsGet = true)]
        public int LogId { get; set; }

        public string Nick { get; set; }
        public string ActivityName { get; set; }
        public DateTime StartTime { get; set; }

        public void  OnGet()
        {
            using var conn = DBConnect.Connect();

            using var cmd = new SqliteCommand(@"
            SELECT u.Nick, a.Name AS ActivityName, l.StartTime
            FROM ActivityLogs l
            JOIN Users u ON l.UserId = u.Id
            JOIN Activities a ON l.ActivityId = a.Id
            WHERE l.Id = @logId", conn);

            cmd.Parameters.AddWithValue("@logId", LogId);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                Nick = reader["Nick"].ToString();
                ActivityName = reader["ActivityName"].ToString();
                StartTime = DateTime.Parse(reader["StartTime"].ToString());
            }
             //RedirectToPage("ActivityRunning", new { logId = logId });
        }
    }
}
