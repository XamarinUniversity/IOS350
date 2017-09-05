using System;
using System.Drawing;

using Foundation;
using UIKit;
using Photos;
using CoreFoundation;
using CoreGraphics;

namespace PhotoExplorer
{
    public partial class PhotosViewController : UICollectionViewController
    {
		static readonly NSString CellIdentifier = new NSString("PhotoCell");
		readonly PhotoLibraryObserver observer;
		readonly PHImageManager imageManager;

		PHFetchResult images;
		public PHFetchResult Images {
			get {
				return images;
			}
			set {
				images = value;
				if (CollectionView != null) {
					CollectionView.ReloadData();
				}
			}
		}

        public PhotosViewController(IntPtr handle) : base(handle)
        {
			imageManager = new PHImageManager();
			Images = PHAsset.FetchAssets(PHAssetMediaType.Image, null);
			PHPhotoLibrary.SharedPhotoLibrary.RegisterChangeObserver(observer = new PhotoLibraryObserver(this));
        }

		public override nint GetItemsCount(UICollectionView collectionView, nint section)
		{
			return Images.Count;
		}

		public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = (PhotoViewCell) collectionView.DequeueReusableCell(CellIdentifier, indexPath);

			imageManager.RequestImageForAsset ((PHAsset)Images[indexPath.Item], 
				new SizeF(250, 250), PHImageContentMode.AspectFill, null, 
					(img, info) => {
					cell.TheImage.Image = img;
				});

			return cell;
		}

		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var photoController = UIStoryboard.FromName("MainStoryboard", null).InstantiateViewController("photoViewController") as PhotoViewController;
			photoController.Asset = (PHAsset)images[indexPath.Item];
			NavigationController.PushViewController(photoController, true);
		}

		class PhotoLibraryObserver : PHPhotoLibraryChangeObserver
		{
			readonly PhotosViewController controller;

			public PhotoLibraryObserver (PhotosViewController controller)
			{
				this.controller = controller;
			}

			public override void PhotoLibraryDidChange (PHChange changeInstance)
			{
				DispatchQueue.MainQueue.DispatchAsync (() => {
					var changes = changeInstance.GetFetchResultChangeDetails (controller.Images);
					controller.Images = changes.FetchResultAfterChanges;
				});
			}
		}
    }
}

