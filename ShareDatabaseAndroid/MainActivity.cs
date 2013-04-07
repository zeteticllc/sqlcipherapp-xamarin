using System;
using System.IO;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Mono.Data.Sqlite;

namespace ShareDatabaseAndroid
{
	[Activity (Label = "SendDatabaseAndroid", MainLauncher = true)]
	[IntentFilter (new[]{Intent.ActionView},
	Categories=new[]{"android.intent.category.DEFAULT"},
	DataMimeType="application/x-net-zetetic-dbfile")]
	public class Activity1 : Activity
	{
		private string Password {get;set;}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			var dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "message.db");

			File.Delete(dbPath);

			// Get our button from the layout resource,
			// and attach an event to it
			var button = FindViewById<Button> (Resource.Id.buttonSend);
			var emailTextView = FindViewById<TextView> (Resource.Id.editTextEmail);
			var messageTextView = FindViewById<TextView> (Resource.Id.editTextMessage);

			var input = new EditText(this);
			input.InputType = Android.Text.InputTypes.TextVariationPassword;
			
			var builder = new AlertDialog.Builder(this)
				.SetTitle("Enter Password")
					.SetMessage("Enter An Encryption Password")
					.SetView(input)
					.SetPositiveButton("Ok", (sender, args) => {
						Password = input.Text;
					});
			builder.Create().Show();

			Android.Net.Uri uri = this.Intent != null ? this.Intent.Data : null;
			if(uri != null) 
			{
				using(var istream = ContentResolver.OpenInputStream(uri)) {
					using(var ostream = new FileStream(dbPath, FileMode.OpenOrCreate)) {
						byte[] buffer = new byte[1024];
						int read;
						while ((read = istream.Read(buffer, 0, buffer.Length)) > 0)
						{
							ostream.Write (buffer, 0, read);
						}
					}
				}

				using(var connection = new SqliteConnection(String.Format("Data Source={0}",dbPath)))
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
						messageTextView.Text = message;
					}
				}
			}


			button.Click += delegate {



				using(var connection = new SqliteConnection(String.Format("Data Source={0}",dbPath)))
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
						p.Value = messageTextView.Text;
						command.Parameters.Add(p);
						command.ExecuteNonQuery();
					}
				}

				var intent = new Intent(Android.Content.Intent.ActionSend);
				intent.SetType("text/plain");
				intent.PutExtra(Intent.ExtraEmail, new String[] { emailTextView.Text });
				intent.PutExtra(Intent.ExtraSubject, "database file");
				intent.PutExtra(Intent.ExtraText, "please find a database attached");

				string url = DbFileProvider.CONTENT_URI.ToString() + "message.db";
				intent.PutExtra(Intent.ExtraStream, Android.Net.Uri.Parse(url));
				StartActivity(intent);
			};
		}
	}
}


