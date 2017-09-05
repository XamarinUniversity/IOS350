
using System;

using Foundation;
using UIKit;
using LocalAuthentication;

namespace TouchIDTest
{
    public partial class AuthViewController : UIViewController
    {
		public AuthViewController(IntPtr handle) : base(handle)
        {
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			authButton.TouchUpInside += PerformAuthentication;
		}

		void PerformAuthentication(object sender, EventArgs e)
		{
			NSError authError;
			var context = new LAContext();
			if (context.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out authError)) {
				var replyHandler = new LAContextReplyHandler((success, error) => this.InvokeOnMainThread(() => {
					if (success) {
						PerformSegue("AuthScreen", this);
					} 
					else {
						UIAlertController uac = UIAlertController.Create("Authentication Failed. Is this your phone?", "Security Failure", UIAlertControllerStyle.Alert);
						uac.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
						this.PresentViewController(uac, true, null);
					}
				})); 

				context.EvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, "Access Account", replyHandler);

			} else {
				UIAlertController uac = UIAlertController.Create("Biometrics not supported", "Policy not supported", UIAlertControllerStyle.Alert);
				uac.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
				this.PresentViewController(uac, true, null);
			}
		}
    }
}

