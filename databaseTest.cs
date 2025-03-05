using System;
using Npgsql;

class Programi
{
    static void Main()
    {
        string connString = "Host=dpg-cv405bdumphs73en9dkg-a.oregon-postgres.render.com;Database=order_db_o3in;Username=db_admin;Password=kPTGwNYwyB5rWY8Lcov62qUAXszLfTxZ;Port=5432;SSL Mode=Require;Trust Server Certificate=true;";

        try
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                Console.WriteLine("✅ PostgreSQL Database Connected Successfully!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Connection Failed: " + ex.Message);
        }
    }
}
