using UIKit;
using MonoTouch.Dialog;
using System;
using Foundation;

namespace Checkbook
{
	[Register ("CheckDetailsViewController")]
	public class CheckDetailsViewController : DialogViewController
    {
		DateElement datePicker;
		EntryElement payeePicker, amountPicker, memoPicker;
		BooleanElement clearedPicker;

		Check check;
		public Check Check {
			get {
				return check;
			}
			set {
				if (check != value) {
					check = value;
					ConfigureView();
				}
			}
		}

		public CheckDetailsViewController() : base(null, true)
		{
		}

		public CheckDetailsViewController(IntPtr handle) : base(handle)
        {
			Pushing = true;
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			if (Root == null)
				ConfigureView();
		}

		void ConfigureView()
		{
			if (Check == null) {
				Root = null;
				return;
			}

			Root = new RootElement("Details") {
				new Section() {
					new StyledStringElement("Check #", Check.Number.ToString()) {
						TextColor = UIColor.LightGray
					},
					(datePicker = new DateElement("Date", Check.Date)),
					(payeePicker = new EntryElement("Payee", "", Check.Payee)),
					(amountPicker = new EntryElement("Amount", "", Check.Amount.ToString()) {
						KeyboardType = UIKeyboardType.DecimalPad
					}),
					(memoPicker = new EntryElement("Memo", "", Check.Memo)),
					(clearedPicker = new BooleanElement("Cleared?", Check.Cleared)),
				}
			};

			datePicker.DateSelected += dt => Check.Date = dt.DateValue;
			payeePicker.EntryEnded = (s,e) => Check.Payee = payeePicker.Value;
			amountPicker.EntryEnded = (s, e) => Check.Amount = Double.Parse(amountPicker.Value);
			memoPicker.EntryEnded = (s, e) => Check.Memo = memoPicker.Value;
			clearedPicker.ValueChanged += (sender, e) => Check.Cleared = clearedPicker.Value;
		}
    }
}

