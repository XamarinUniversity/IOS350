using System;
using UIKit;
using Foundation;

namespace PhotoExplorer
{
	[Register("PhotoViewCell")]
	public class PhotoViewCell : UICollectionViewCell
	{
		[Outlet]
		public UIImageView TheImage { get; set; }

		public PhotoViewCell (IntPtr handle) : base (handle)
		{
		}
	}
}

