/*
 * Making your C# app up to 100x faster using Columnstore
 * 
 * In this module we will show you a simple example of Columnstore Indexes and how they can improve data processing
 * speeds. Columnstore Indexes can achieve up to 100x better performance on analytical workloads and up to 10x better
 * data compression than traditional rowstore indexes.
 * 
 * Note:
 * ----
 * With the introduction of Service Pack 1 for SQL Server 2016, features in the database engine related to application
 * development are now available across all editions of SQL Server (from Express through Enterprise). This includes 
 * innovations that can significantly improve your application's throughput, latency, and security. Examples include the
 * iin-memory columnstore used in this tutorial, in-memory OLTP, data compression, table partitioning, Hadoop 
 * integration with PolyBase, Always Encrypted, row-level security, and data masking.
 * 
 * To showcase the capabilities of Columnstore indexes, this C# application creates a sample database and a sample table
 * with 5 million rows and then runs a simple query before and after adding a Columnstore index.
 */

using System;
using System.Data.SqlClient;
using System.Text;

namespace SSColumnstore
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			try
			{
				Console.WriteLine("*** SQL Server Columnstore demo ***");

				// Build connection string
				SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
				builder.DataSource = "localhost\\SQLEXPRESS";
				builder.UserID = "usr";
				//builder.Password = "your_password";		// Not required when using Windows Authentication
				builder.InitialCatalog = "master";
				builder.IntegratedSecurity = true;          // Using Windows Authentication

				// Connect to SQL
				Console.Write("\nConnecting to SQL Server ... ");
				using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
				{
					connection.Open();
					Console.WriteLine("Done. Connection established!");

					// Create a sample database
					Console.Write("\nDropping and creating database 'SampleDB' ... ");
					String sql = "DROP DATABASE IF EXISTS [SampleDB]; CREATE DATABASE [SampleDB]";
					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.ExecuteNonQuery();
						Console.WriteLine("Done. Database created!");
					}

					// Insert 5 million rows into the table 'Table_with_5M_rows'
					Console.Write("\nInserting 5 million rows into table 'Table_with_5M_rows'. Please wait ... ");
					StringBuilder sb = new StringBuilder();
					sb.Append("USE SampleDB; ");
					sb.Append("WITH a AS (SELECT * FROM (VALUES(1),(2),(3),(4),(5),(6),(7),(8),(9),(10)) AS a(a))");
					sb.Append("SELECT TOP(5000000)");
					sb.Append("ROW_NUMBER() OVER (ORDER BY a.a) AS OrderItemId ");
					sb.Append(",a.a + b.a + c.a + d.a + e.a + f.a + g.a + h.a AS OrderId ");
					sb.Append(",a.a * 10 AS Price ");
					sb.Append(",CONCAT(a.a, N' ', b.a, N' ', c.a, N' ', d.a, N' ', e.a, N' ', f.a, ' ', g.a, N' ', h.a)" +
							  " AS ProductName ");
					sb.Append("INTO Table_with_5M_rows ");
					sb.Append("FROM a, a AS b, a AS c, a AS d, a AS e, a AS f, a AS g, a AS h;");
					sql = sb.ToString();
					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.CommandTimeout = 0;     // No timeout limit
						command.ExecuteNonQuery();
						Console.WriteLine("Done. 5 million rows loaded into table!");
					}

					// Execute SQL query without columnstore index
					double elapsedTimeWithoutIndex = SumPrice(connection);
					Console.WriteLine("\nQuery time WITHOUT columnstore index: " + elapsedTimeWithoutIndex + "ms");

					// Add a Columnstore Index
					Console.Write("\nAdding a columnstore index to table 'Table_with_5M_rows'. Please wait ... ");
					sql = "CREATE CLUSTERED COLUMNSTORE INDEX columnstoreindex ON Table_with_5M_rows;";
					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.CommandTimeout = 0;     // No timeout limit
						command.ExecuteNonQuery();
						Console.WriteLine("Done. Columnstore index added!");
					}

					// Execute the same SQL query again after columnstore index was added
					double elapsedTimeWithIndex = SumPrice(connection);
					Console.WriteLine("\nQuery time WITH columnstore index: " + elapsedTimeWithIndex + "ms");

					// Calculate performance gain from adding columnstore index
					Console.WriteLine("\nPerformance improvement with columnstore index: "
									  + Math.Round(elapsedTimeWithoutIndex / elapsedTimeWithIndex) + "x!");
				}
				Console.WriteLine("\nAll done. Press any key to finish...");
				Console.ReadKey(true);
			}
			catch (Exception e)
			{
				DisplayException(e);
			}
		}


		public static double SumPrice(SqlConnection connection)
		{
			String sql = "SELECT SUM(Price) FROM Table_with_5M_rows";
			long startTicks = DateTime.Now.Ticks;
			using (SqlCommand command = new SqlCommand(sql, connection))
			{
				try
				{
					var sum = command.ExecuteScalar();
					TimeSpan elapsed = TimeSpan.FromTicks(DateTime.Now.Ticks) - TimeSpan.FromTicks(startTicks);
					return elapsed.TotalMilliseconds;
				}
				catch (Exception e)
				{
					DisplayException(e);
				}
			}
			return 0;
		}

		public static void DisplayException(Exception e)
		{
			Console.WriteLine(e.ToString());
			Console.WriteLine("\nException. Press any key to finish...");
			Console.ReadKey(true);
		}
	}
}
