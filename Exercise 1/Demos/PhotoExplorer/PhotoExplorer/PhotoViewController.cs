using System;
using UIKit;
using Photos;
using CoreImage;
using Foundation;

namespace PhotoExplorer
{
	[Register("PhotoViewController")]
	public class PhotoViewController : UIViewController
	{
		[Outlet]
		public UIImageView TheImage { get; set; }

		public PHAsset Asset { get; set; }

		public PhotoViewController (IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad ()
		{
			PHImageManager.DefaultManager.RequestImageForAsset (Asset, View.Frame.Size, 
				PHImageContentMode.AspectFit, new PHImageRequestOptions (), (img, info) => {
					TheImage.Image = img;
				});

			NavigationItem.RightBarButtonItem.Clicked += OnApplyFilter;
		}

		void OnApplyFilter (object sender, EventArgs e)
		{
			Asset.RequestContentEditingInput(new PHContentEditingInputRequestOptions(), 
				(input, options) => {
					var image = CIImage.FromUrl (input.FullSizeImageUrl);
					image = image.CreateWithOrientation(input.FullSizeImageOrientation);

					var updatedPhoto = new CIPhotoEffectNoir { Image = image };
					var ciContext = CIContext.FromOptions (null);
					var output = updatedPhoto.OutputImage;

					// Get the upated image
					var uiImage = UIImage.FromImage (ciContext.CreateCGImage (output, output.Extent));
					TheImage.Image = uiImage;

					// Save the image data to a PHContentEditingOutput instance
					var editingOutput = new PHContentEditingOutput (input);

					NSError error;
					var data = uiImage.AsJPEG();
					data.Save (editingOutput.RenderedContentUrl, false, out error);
					editingOutput.AdjustmentData = new PHAdjustmentData();;

					// Request to publish the changes form the editing output back to the photo library
					PHPhotoLibrary.SharedPhotoLibrary.PerformChanges (
						() => {
							PHAssetChangeRequest request = PHAssetChangeRequest.ChangeRequest (Asset);
							request.ContentEditingOutput = editingOutput;
						},
						(ok, err) => Console.WriteLine ("Photo updated : {0}, {1}", ok, err));
				});
		}
	}
}

