using Attendance.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Attendance.Pages;

public class SelectActivityModel : PageModel
{
    private object userFirstName;
    private object connection;

    //private readonly DBConnect connect;

    //public SelectActivityModel(DBConnect connect) => this.connect = connect;

    [BindProperty(SupportsGet = true)]
    public int UserId { get; set; }

    public string UserFirstName { get; set; }
    public string Greeting { get; set; }

    [BindProperty]
    public int SelectedActivityId { get; set; }

    public List<SelectListItem> ActivityOptions { get; set; } = new();

    public void OnGet()
    {
        
        LoadUserFirstName();
        GenerateRandomGreeting();


        using var conn = DBConnect.Connect();
        using var cmd = new SqliteCommand("SELECT Id, Name FROM Activities", conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            ActivityOptions.Add(new SelectListItem
            {
                Value = reader["Id"].ToString(),
                Text = reader["Name"].ToString()
            });
        }
    }

    private void LoadUserFirstName()
    {
        using var conn = DBConnect.Connect();
        using var cmd = new SqliteCommand("SELECT FirstName FROM Users WHERE Id = @id", conn);
        cmd.Parameters.AddWithValue("@id", UserId);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            UserFirstName = reader["FirstName"].ToString();
        }
    }


    private void GenerateRandomGreeting()
    {
        var greetings = new[]
        {
        "😎 Tebe jsem nečekal",
        "🧑‍💼 Zdravím tě",
        "🤴 Vaše výsost",
        "👨‍💼 No nazdar, to zase bude průs...",
        "🤖 Hezký den ti přeje AI, naše vzpoura se blíží ",
        "🧭 Kapitán Amerika, teda pardon ",
        "🧑‍💼 Pane kolego",
        "🕵️ Zpátky v akci",
        "😎 To je dost že se ukážeš",
        "🎯 Zase ty?"
    };

        var random = new Random();
        Greeting = greetings[random.Next(greetings.Length)];
    }

    public IActionResult OnPost()
    {
        if (SelectedActivityId == 0)
        {
            ModelState.AddModelError(string.Empty, "Vyberte činnost.");
            OnGet(); // znovu načti aktivity
            return Page();
        }

        using var conn = DBConnect.Connect();
        conn.Open();
        using var checkCmd = conn.CreateCommand();
        checkCmd.CommandText = @"
            SELECT COUNT(*) 
            FROM ActivityLogs 
            WHERE UserId = $UserId AND EndTime IS NULL";
        checkCmd.Parameters.AddWithValue("$UserId", UserId);

        long activeCount = (long)checkCmd.ExecuteScalar();

        if (activeCount > 0)
        {
            // Uživatel už má aktivní činnost => přeruš odeslání
            ModelState.AddModelError("", "Uživatel už má aktivní činnost. Nejdříve ji ukončete.");
            OnGet();
            return Page();
        }

       

        // Záznam do ActivityLog
        using var insertCmd = new SqliteCommand(
            "INSERT INTO ActivityLogs (UserId, ActivityId, StartTime) VALUES (@userId, @activityId, @startTime); SELECT last_insert_rowid();",
            conn);
        insertCmd.Parameters.AddWithValue("@userId", UserId);
        insertCmd.Parameters.AddWithValue("@activityId", SelectedActivityId);
        insertCmd.Parameters.AddWithValue("@startTime", DateTime.Now);

        var logId = (long)insertCmd.ExecuteScalar();

        return RedirectToPage("ActiveUsers", new { logId = logId });
    }
}
