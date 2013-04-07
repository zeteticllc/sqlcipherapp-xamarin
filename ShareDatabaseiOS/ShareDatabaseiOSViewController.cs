using System;
using System.Drawing;
using System.IO;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.MessageUI;

using Mono.Data.Sqlite;

namespace ShareDatabaseiOS
{
	public partial class ShareDatabaseiOSViewController : UIViewController
	{
		public static readonly string DB_PATH = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "message.db");

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
			base.DidReceiveMemoryWarning ();
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			LoadDatabase();
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return false;
		}

		partial void SendButtonClick(MonoTouch.Foundation.NSObject sender) 
		{
			//var alert = new UIAlertView("Test", "Test Message", new PasswordAlertDelegate(this), "OK", null);
			//alert.AlertViewStyle = UIAlertViewStyle.SecureTextInput;
			//alert.Show();
			SaveDatabase();
		}

		public void LoadDatabase()
		{
			if(File.Exists(DB_PATH)) 
			{
				using(var connection = new SqliteConnection(String.Format("Data Source={0}", DB_PATH)))
				{
					connection.Open();
					
					if(!string.IsNullOrEmpty(Password)) 
					{
						System.Console.WriteLine("Password: " + Password);
					}
					
					using (var command = connection.CreateCommand())
					{
						command.CommandText = "SELECT content FROM message WHERE id = 0";
						string message = (string) command.ExecuteScalar();

						textViewMessage.Text = message;
					}
				}
			}
		}

		public void SaveDatabase() 
		{
			using(var connection = new SqliteConnection(String.Format("Data Source={0}",DB_PATH)))
			{
				connection.Open();
				
				if(!string.IsNullOrEmpty(Password)) 
				{
					System.Console.WriteLine("Password: " + Password);
				}
				
				using (var command = connection.CreateCommand())
				{
					command.CommandText = "CREATE TABLE IF NOT EXISTS message(id INTEGER PRIMARY KEY, content TEXT)";
					command.ExecuteNonQuery();
				}
				
				using (var command = connection.CreateCommand())
				{
					command.CommandText = "INSERT OR REPLACE INTO message (id, content) VALUES (0, @content)";
					var p = command.CreateParameter();
					p.ParameterName = "@content";
					p.Value = textViewMessage.Text;
					command.Parameters.Add(p);
					command.ExecuteNonQuery();
				}
			}
		}

		public void SendDatabase() 
		{
			if (MFMailComposeViewController.CanSendMail) 
			{
				var mail = new MFMailComposeViewController ();
				mail.SetSubject("Test Message");
				mail.SetToRecipients(new string[]{textFieldEmail.Text});
				mail.SetMessageBody (textViewMessage.Text, false);
				mail.AddAttachmentData(NSData.FromFile(DB_PATH), "application/x-net-zetetic-dbfile", "message.db");
				PresentViewController(mail, true, null);
				
			} else {
				Console.WriteLine("cant send mail");
			}
		}

		public void HandleOpenUrl(NSUrl url) 
		{
			File.Copy(url.Path, DB_PATH, true);
			LoadDatabase();
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

