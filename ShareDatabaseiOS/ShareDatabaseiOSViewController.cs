using System;
using System.Drawing;
using System.IO;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.MessageUI;

using Mono.Data.Sqlite;

namespace ShareDatabase
{
	public partial class ShareDatabaseiOSViewController : UIViewController
	{
		private MessageDb _messageDb = new MessageDb(
			Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "message.db"));

		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public ShareDatabaseiOSViewController ()
			: base (UserInterfaceIdiomIsPhone ? "ShareDatabaseiOSViewController_iPhone" : "ShareDatabaseiOSViewController_iPad", null)
		{
		}
		
		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			Load ();
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.Portrait;
		}

		private void Load() 
		{
			textViewMessage.Text = _messageDb.LoadMessage();
			/*
			var alert = new UIAlertView("Enter Password", "Password", null, "OK", null) {
				AlertViewStyle = UIAlertViewStyle.SecureTextInput
			};
			alert.Clicked += (s, a) =>  {
				_messageDb.Password = alert.GetTextField(0).Text;
				try
				{
					textViewMessage.Text = _messageDb.LoadMessage();
				} catch (SqliteException e) 
				{
					textViewMessage.Text = e.Message;
				}
				
			};
			alert.Show();
			*/
		}

		private void Save() 
		{
			_messageDb.SaveMessage(textViewMessage.Text);
			/*
			var alert = new UIAlertView("Enter Password", "Password", null, "OK", null) {
				AlertViewStyle = UIAlertViewStyle.SecureTextInput
			};
			alert.Clicked += (s, a) =>  {
				_messageDb.Password = alert.GetTextField(0).Text;
				_messageDb.SaveMessage(textViewMessage.Text);
			};
			alert.Show();
			*/
		}

		partial void SaveButtonClick(MonoTouch.Foundation.NSObject sender) 
		{
			Save ();
		}

		partial void SendButtonClick(MonoTouch.Foundation.NSObject sender) 
		{
			Save ();
			if (MFMailComposeViewController.CanSendMail) 
			{
				var mail = new MFMailComposeViewController ();
				mail.SetSubject( "Zetetic Message Database");
				mail.SetToRecipients(new string[]{textFieldEmail.Text});
				mail.SetMessageBody ("Please find a database attached", false);
				mail.AddAttachmentData(_messageDb.FilePath, MessageDb.MIME_TYPE, Path.GetFileName(_messageDb.FilePath));
				PresentViewController(mail, true, null);				
			}
		}

		public void HandleOpenUrl(NSUrl url) 
		{
			File.Copy(url.Path, _messageDb.FilePath, true);
			Load ();
		}
	}
}

