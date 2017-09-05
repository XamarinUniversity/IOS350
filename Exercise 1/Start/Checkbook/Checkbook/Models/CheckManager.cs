using System.Collections.Generic;
using System;

namespace Checkbook
{
	/// <summary>
	/// Manager to grab all the checks
	/// </summary>
	public static class CheckManager
	{
		public static IEnumerable<Check> Load()
		{
			return new[] {
				new Check { Number = 101, Date = new DateTime(2014,1,21), Payee = "Best Buy", Amount = -1599.23, Memo = "New HDTV", Cleared = true },
				new Check { Number = 102, Date = new DateTime(2014,2,4), Payee = "Kroger", Amount = -54.66, Memo = "Groceries", Cleared = false },
				new Check { Number = 103, Date = new DateTime(2014,2,9), Payee = "Amazon.com", Amount = -39.99, Memo = "Gift Card", Cleared = false },
				new Check { Number = 104, Date = new DateTime(2014,2,18), Payee = "Apple", Amount = -499.99, Memo = "Apple Watch", Cleared = true },
				new Check { Number = 105, Date = new DateTime(2014,3,6), Payee = "Geico", Amount = -100.00, Memo = "Insurance", Cleared = false },
				new Check { Number = 0, Date = new DateTime(2014,3,15), Payee = "Xamarin, Inc.", Amount = 3125.50, Memo = "Paycheck", Cleared = true },
			};
		}
	}
}
