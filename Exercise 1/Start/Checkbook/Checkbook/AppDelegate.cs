using Foundation;
using UIKit;
using CoreGraphics;

namespace Checkbook
{
    [Register("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        public override UIWindow Window { get; set; }
        
		public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
		{
			// Override point for customization after application launch.
			var splitViewController = Window.RootViewController as UISplitViewController;

			// Set the navigation up for the split view.
			var detailNavigationController = (UINavigationController)splitViewController.ViewControllers[1];
			var detailViewController = (CheckDetailsViewController)detailNavigationController.TopViewController;
			detailViewController.NavigationItem.LeftBarButtonItem = splitViewController.DisplayModeButtonItem;

			splitViewController.Delegate = new SplitViewDelegate();

			return true;
		}
		#region Split View Delegate
		sealed class SplitViewDelegate : UISplitViewControllerDelegate
		{
			/// <summary>
			/// Gets the primary view controller for expanding split view controller.
			/// When the split expands, it sets its current (and only) VC as the new primary. 
			/// If you want to set another VC as the primary, return it from this method
			/// </summary>
			/// <returns>The primary view controller for expanding split view controller.</returns>
			/// <param name="splitViewController">Split view controller.</param>
			public override UIViewController GetPrimaryViewControllerForExpandingSplitViewController(UISplitViewController splitViewController)
			{
				return null;
			}

			/// <summary>
			/// Gets the primary view controller for collapsing split view controller.
			/// When the split collapses it uses its current primary VC as the new single VC. 
			/// If you want it to set a different vc as the new VC, return it from this method
			/// </summary>
			/// <returns>The primary view controller for collapsing split view controller.</returns>
			/// <param name="splitViewController">Split view controller.</param>
			public override UIViewController GetPrimaryViewControllerForCollapsingSplitViewController(UISplitViewController splitViewController)
			{
				return null;
			}

			/// <summary>
			/// Separates the secondary view controller. When the split expands and this 
			/// method return null the split calls the primary VC method 
			/// SeparateSecondaryViewControllerForSplitViewController to obtain the 
			/// new secondary (the NavigationController reutrn the last vc popped 
			/// from the stack as the new secondary), otherwise it uses this VC as the 
			/// new secondary
			/// </summary>
			/// <returns>The secondary view controller.</returns>
			/// <param name="splitViewController">Split view controller.</param>
			/// <param name="primaryViewController">Primary view controller.</param>
			public override UIViewController SeparateSecondaryViewController(UISplitViewController splitViewController, UIViewController primaryViewController)
			{
				return null;
			}

			/// <summary>
			/// Collapses the second view controller. This is called just before the split 
			/// is collapsing. If this method return false, the split calls the primary's 
			/// CollapseSecondaryViewController method to give it a chance to do something 
			/// with the secondary VC (who is disappearing). i.e. the NavigationController 
			/// uses CollapseSecondaryViewController to push the secondary as the new primary.
			/// 
			/// If you return true the split does nothing and the primary will be the 
			/// single VC displayed.
			/// </summary>
			/// <returns><c>true</c>, if second view controller was collapsed, <c>false</c> otherwise.</returns>
			/// <param name="splitViewController">Split view controller.</param>
			/// <param name="secondaryViewController">Secondary view controller.</param>
			/// <param name="primaryViewController">Primary view controller.</param>
			public override bool CollapseSecondViewController(UISplitViewController splitViewController, UIViewController secondaryViewController, UIViewController primaryViewController)
			{
				var secondaryNav = secondaryViewController as UINavigationController;
				if (secondaryNav != null)
				{
					var dvc = secondaryNav.TopViewController as CheckDetailsViewController;
					if (dvc != null && dvc.Check == null)
						// Return true to indicate we have handled the collapse by doing nothing;
						// the secondary controller will be discarded.
						return true;
				}
				return false;
			}
		}
		#endregion
    }
}

