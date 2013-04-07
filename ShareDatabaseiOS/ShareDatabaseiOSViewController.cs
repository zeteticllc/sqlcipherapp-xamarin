using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.MessageUI;

namespace ShareDatabaseiOS
{
	public partial class ShareDatabaseiOSViewController : UIViewController
	{
		public string Password {private get; set;}

		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public ShareDatabaseiOSViewController ()
			: base (UserInterfaceIdiomIsPhone ? "ShareDatabaseiOSViewController_iPhone" : "ShareDatabaseiOSViewController_iPad", null)
		{
		}
		
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Perform any additional setup after loading the view, typically from a nib.
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			if (UserInterfaceIdiomIsPhone) {
				return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
			} else {
				return true;
			}
		}

		partial void SendButtonClick(MonoTouch.Foundation.NSObject sender) {
		
			var alert = new UIAlertView("Test", "Test Message", new PasswordAlertDelegate(this), "OK", null);
			alert.AlertViewStyle = UIAlertViewStyle.SecureTextInput;
			alert.Show();

			if (MFMailComposeViewController.CanSendMail) 
			{
				var mail = new MFMailComposeViewController ();
				mail.SetMessageBody ("here is a database", false);

				
				PresentModalViewController (mail, true);
				
			} else {
				Console.WriteLine("cant send mail");
			}
		}
	}

	public class PasswordAlertDelegate : UIAlertViewDelegate
	{
		private ShareDatabaseiOSViewController _controller;

		public PasswordAlertDelegate(ShareDatabaseiOSViewController controller)
		{
			_controller = controller;
		}

		public override void Clicked (UIAlertView alertview, int buttonIndex)
		{
			Console.WriteLine("clicked password button" + alertview.GetTextField(0).Text);
			_controller.Password = alertview.GetTextField(0).Text;
		}
	}
}

