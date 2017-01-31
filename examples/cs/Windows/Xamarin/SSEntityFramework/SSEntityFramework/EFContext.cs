/*
 * This C# class inherits from the base class, DbContext
 */

using System;
using System.Data.Entity;

namespace SSEntityFramework
{
	// Derived Class: EFContext
	public class EFContext : DbContext	// Inherits Base Class, DbContext
	{
		public EFContext(string connectionString)   // Constructor
		{
			Database.SetInitializer(new DropCreateDatabaseAlways<EFContext>());
			this.Database.Connection.ConnectionString = connectionString;
		}

		public DbSet<User> Users { get; set; }
		public DbSet<Task> Tasks { get; set; }
	}
}
