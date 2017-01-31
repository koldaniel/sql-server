/*
 * This C# Class holds data about "users".
 */

using System;
using System.Collections.Generic;

namespace SSEntityFramework
{
	public class User
	{
		// Getters and Setters
		public int UserId { get; set; }
		public String FirstName { get; set; }
		public String LastName { get; set; }
		public virtual IList<Task> Tasks { get; set; }

		public String GetFullName()
		{
			return this.FirstName + " " + this.LastName;
		}

		public override string ToString()
		{
			return "User [id=" + this.UserId + ", name=" + this.GetFullName() + "]";
		}
	}
}
