using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Checkbook
{
	/// <summary>
	/// Simple class to model a single check
	/// </summary>
	public class Check : INotifyPropertyChanged
    {
		public event PropertyChangedEventHandler PropertyChanged = delegate {};

		public int Number { get; set; }

		private DateTime date;
		public DateTime Date {
			get { return date; }
			set { SetValue(ref date, value); }
		}
		private string payee;
		public string Payee {
			get { return payee; }
			set { SetValue(ref payee, value); }
		}
		private double amount;
		public double Amount {
			get { return amount; }
			set { SetValue(ref amount, value); }
		}
		private string memo;
		public string Memo {
			get { return memo; }
			set { SetValue(ref memo, value); }
		}
		private bool cleared;
		public bool Cleared {
			get { return cleared; }
			set { SetValue(ref cleared, value); }
		}

		private bool SetValue<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
		{
			if (!Equals(field, value)) {
				field = value;
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
				return true;
			}
			return false;
		}

		public override string ToString()
		{
			return string.Format("{0} - {1:C}", Payee, Amount);
		}
    }
}

