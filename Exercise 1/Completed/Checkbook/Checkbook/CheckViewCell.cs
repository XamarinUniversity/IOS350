using System;
using Foundation;
using UIKit;

namespace Checkbook
{
	[Register("CheckViewCell")]
    public class CheckViewCell : UITableViewCell
    {
		[Outlet]
		public UILabel DateLabel { get; set; }

		[Outlet]
		public UILabel PayeeText { get; set; }

		[Outlet]
		public UILabel Amount { get; set; }

		public CheckViewCell(IntPtr handle) : base(handle)
        {
        }
    }
}

