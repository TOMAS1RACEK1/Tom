using Attendance.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;

namespace Attendance.Pages;

public class LoginModel : PageModel
{
    //private readonly DBConnect connect;

    //public LoginModel(DBConnect connect) => this.connect = connect;

    [BindProperty] 
   
    public string Nick { get; set; }
    public object UserId { get; private set; }

    public async Task<IActionResult> OnPostAsync()
    {


        if (!ModelState.IsValid || string.IsNullOrWhiteSpace(Nick))
            return Page();

        User user = null;

        using var conn = DBConnect.Connect(); // Pøedpokládá se metoda Connect() vrací otevøený SQLiteConnection

        // Hledání uživatele podle pøezdívky
        using (var checkCmd = new SqliteCommand("SELECT Id, Nick FROM Users WHERE Nick = @nick", conn))
        {
            checkCmd.Parameters.AddWithValue("@nick", Nick);
            using var reader = await checkCmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                user = new User
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Nick = reader["Nick"].ToString()
                };
            }
        }


       


        // Pokud uživatel neexistuje, vložíme ho
        if (user == null)
        {
            using var insertCmd = new SqliteCommand("INSERT INTO Users (Nick) VALUES (@nick);", conn);
            insertCmd.Parameters.AddWithValue("@nick", Nick);
            await insertCmd.ExecuteNonQueryAsync();

            // Získání ID právì vloženého uživatele
            using var idCmd = new SqliteCommand("SELECT last_insert_rowid();", conn);
            var newId = (long)await idCmd.ExecuteScalarAsync();
            user = new User { Id = (int)newId, Nick = Nick };
        }

        // Pøesmìrování na stránku s výbìrem èinnosti
        return RedirectToPage("SelectActivity", new { userId = user.Id });
    }

   


}
