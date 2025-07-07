using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Attendance.Data;

public class DBConnect 
{
    public static string ConnectionString { get; set; } = "Data Source=App_Data\\AttendanceDB.db";


    //public DbSet<User> Users { get; set; }
    //public DbSet<Activity> Activities { get; set; }
    //public DbSet<ActivityLog> ActivityLogs { get; set; }

    //public static SqliteConnection Connect()
    //     => new SqliteConnection(ConnectionString);

    public static SqliteConnection Connect()
    {
        var conn = new SqliteConnection(ConnectionString);
        conn.Open();
        return conn;
    }
}
          
