/*
 * This C# Console Application with .NET Framework connects to SQL Server 2016 Express Edition and executes the
 * following queries:
 * 
 * 1) List the name of all the databases
 * 2) Creates a database, 'SampleDB'
 * 3) List the name of all the databases
 * 4) Creates a table, 'Employees' inside the database, 'SampleDB'
 * 5) Loads some sample data into the table, 'Employees'
 * 6) Insert a row into the table (insert a new employee)
 * 7) Update the location of an employee
 * 8) Delete a row from the table (delete an employee)
 * 9) Display all the employees
 */

using System;
using System.Text;
using System.Data.SqlClient;

namespace SSQueries
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			try
			{
				Console.WriteLine("Connect to SQL Server and demo Create, Read, Update and Delete operations.");

				// Build connection string
				SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
				builder.DataSource = "localhost\\SQLEXPRESS";
				builder.UserID = "usr";
				//builder.Password = "your_password";		// Not required when using Windows Authentication
				builder.InitialCatalog = "master";
				builder.IntegratedSecurity = true;          // Use Windows Authentication

				// Connect to SQL
				Console.Write("\nConnecting to SQL Server ... ");
				using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
				{
					connection.Open();
					Console.WriteLine("Done. Connection established!");

					// Display the name of all of the databases
					DisplayDatabases(connection);

					// Create a sample database
					DropCreateDatabase(connection);

					// Display the name of all of the databases
					DisplayDatabases(connection);

					// Create a Table
					CreateTable(connection);

					// Loading some sample data
					LoadTable(connection);

					// Display data in the table
					DisplayTableData(connection);

					// INSERT demo
					InsertRow(connection);

					// Display data in the table
					DisplayTableData(connection);

					// UPDATE demo
					UpdateData(connection, "Nikita", "United States");

					// Display data in the table
					DisplayTableData(connection);

					// DELETE demo
					DeleteRow(connection, "Jared");

					// Display data in the table
					DisplayTableData(connection);
				}
			}
			catch (SqlException e)
			{
				Console.WriteLine(e.ToString());
			}

			Console.WriteLine("\nAll done. Press any key to finish...");
			Console.ReadKey(true);
		}


		public static void DisplayDatabases(SqlConnection connection)
		{
			// Display the name of all of the databases
			Console.WriteLine("\nDisplay the name of all of the databases ...");
			String sql = "SELECT Name FROM sys.Databases;";
			using (SqlCommand command = new SqlCommand(sql, connection))
			{
				// FETCH Cursor
				using (SqlDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						Console.WriteLine("{0}", reader.GetString(0));
					}
				}
			}
		}

		public static void DisplayTableData(SqlConnection connection)
		{
			// READ demo
			Console.WriteLine("\nReading data from table, press any key to continue...");
			Console.ReadKey(true);

			String sql = "SELECT Id, Name, Location FROM Employees;";
			using (SqlCommand command = new SqlCommand(sql, connection))
			{
				// FETCH Cursor
				using (SqlDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						Console.WriteLine("{0} {1} {2}", reader.GetInt32(0), reader.GetString(1),
										  reader.GetString(2));
					}
				}
			}
		}

		public static void DropCreateDatabase(SqlConnection connection)
		{
			// Create a sample database
			Console.Write("\nDropping and creating database 'SampleDB' ...");
			String sql = "DROP DATABASE IF EXISTS [SampleDB]; CREATE DATABASE [SampleDB]";
			using (SqlCommand command = new SqlCommand(sql, connection))
			{
				command.ExecuteNonQuery();
				Console.WriteLine("Done. Database created!");
			}
		}

		public static void CreateTable(SqlConnection connection)
		{
			// Create a Table
			Console.Write("\nCreating table 'Employees', press any key to continue...");
			Console.ReadKey(true);

			// Create a Table named 'Employees'
			StringBuilder sb = new StringBuilder();
			sb.Append("USE SampleDB; ");
			sb.Append("CREATE TABLE Employees ( ");
			sb.Append(" Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY, ");
			sb.Append(" Name NVARCHAR(50), ");
			sb.Append(" Location NVARCHAR(50) ");
			sb.Append("); ");

			// Convert to string and assign to string variable, 'sql'
			String sql = sb.ToString();
			using (SqlCommand command = new SqlCommand(sql, connection))
			{
				command.ExecuteNonQuery();
				Console.WriteLine("Done. Table created!");
			}
		}

		public static void LoadTable(SqlConnection connection)
		{
			// Loading some sample data
			Console.Write("\nLoading some sample data into table, press any key to continue...");
			Console.ReadKey(true);

			// Load some sample data into Table, Employees
			StringBuilder sb = new StringBuilder();
			//sb.Clear();
			sb.Append("INSERT INTO Employees (Name, Location) VALUES ");
			sb.Append("(N'Jared', N'Australia'), ");
			sb.Append("(N'Nikita', N'India'), ");
			sb.Append("(N'Tom', N'Germany'); ");

			// Convert to string and assign to string variable, 'sql'
			String sql = sb.ToString();
			using (SqlCommand command = new SqlCommand(sql, connection))
			{
				command.ExecuteNonQuery();
				Console.WriteLine("Done. Sample data loaded!");
			}
		}

		public static void InsertRow(SqlConnection connection)
		{
			Console.Write("\nInserting a new row into table, press any key to continue...");
			Console.ReadKey(true);

			StringBuilder sb = new StringBuilder();
			//sb.Clear();
			sb.Append("INSERT Employees (Name, Location) ");
			sb.Append("VALUES (@name, @location);");

			String sql = sb.ToString();
			using (SqlCommand command = new SqlCommand(sql, connection))
			{
				command.Parameters.AddWithValue("@name", "Jake");
				command.Parameters.AddWithValue("@location", "United States");
				int rowsAffected = command.ExecuteNonQuery();
				Console.WriteLine(rowsAffected + " row(s) inserted");
			}
		}

		public static void UpdateData(SqlConnection connection, String userToUpdate, String userToUpdateLocation)
		{
			Console.Write("\nUpdating 'Location' for user '" + userToUpdate + "', press any key to continue...");
			Console.ReadKey(true);

			StringBuilder sb = new StringBuilder();
			//sb.Clear();
			//sb.Append("UPDATE Employees SET Location = N'United States' WHERE Name = @name");
			sb.Append("UPDATE Employees SET Location = @location WHERE Name = @name");

			String sql = sb.ToString();
			using (SqlCommand command = new SqlCommand(sql, connection))
			{
				command.Parameters.AddWithValue("@location", userToUpdateLocation);
				command.Parameters.AddWithValue("@name", userToUpdate);
				int rowsAffected = command.ExecuteNonQuery();
				Console.WriteLine(rowsAffected + " row(s) updated");
			}
		}

		public static void DeleteRow(SqlConnection connection, String userToDelete)
		{
			Console.Write("\nDeleting user '" + userToDelete + "', press any key to continue...");
			Console.ReadKey(true);

			StringBuilder sb = new StringBuilder();
			//sb.Clear();
			sb.Append("DELETE FROM Employees WHERE Name = @name;");

			String sql = sb.ToString();
			using (SqlCommand command = new SqlCommand(sql, connection))
			{
				command.Parameters.AddWithValue("@name", userToDelete);
				int rowsAffected = command.ExecuteNonQuery();
				Console.WriteLine(rowsAffected + " row(s) deleted");
			}
		}
	}
}
