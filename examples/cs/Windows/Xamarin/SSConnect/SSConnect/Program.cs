/*
 * This C# Console Application with .NET framework connects to SQL Server 2016 Express Edition using Windows
 * authentication.
 */

using System;
using System.Data.SqlClient;

namespace SSConnect
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			try
			{
				// Build connection string
				SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
				builder.DataSource = "localhost\\SQLEXPRESS";
				builder.UserID = "usr";
				//builder.Password = "your_password";		// Not required for Windows Authentication
				builder.InitialCatalog = "master";
				builder.IntegratedSecurity = true;          // Use Windows Authentication

				// Connect to SQL Server
				Console.Write("Connecting to SQL Server ... ");
				using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
				{
					connection.Open();
					Console.WriteLine("Done. Connection established!");
				}
			}
			catch (SqlException e)
			{
				Console.WriteLine(e.ToString());
			}

			Console.WriteLine("All done. Press any key to finish...");
			Console.ReadKey(true);
		}
	}
}
