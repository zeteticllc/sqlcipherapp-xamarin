using System;
using System.IO;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Mono.Data.Sqlite;

namespace ShareDatabase
{
	[Activity (Label = "SendDatabaseAndroid", MainLauncher = true)]
	[IntentFilter (new[]{Intent.ActionView},
	Categories=new[]{"android.intent.category.DEFAULT"},
	DataMimeType="application/x-net-zetetic-dbfile")]
	public class Activity1 : Activity
	{
		private MessageDbFile _messageDb = new MessageDbFile(
			Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "message.db")
			);

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			Android.Net.Uri uri = this.Intent != null ? this.Intent.Data : null;
			if(uri != null) 
			{
				using(var istream = ContentResolver.OpenInputStream(uri)) {
					using(var ostream = new FileStream(_messageDb.FilePath, FileMode.OpenOrCreate)) {
						CopyStream(istream, ostream);
					}
				}
			}

			Load ();
			
			var button = FindViewById<Button> (Resource.Id.buttonSend);
			button.Click += delegate {
				Save ();
				/*
				var emailTextView = FindViewById<TextView> (Resource.Id.editTextEmail);
				var intent = new Intent(Android.Content.Intent.ActionSend);
				intent.SetType("text/plain");
				intent.PutExtra(Intent.ExtraEmail, new String[] { emailTextView.Text });
				intent.PutExtra(Intent.ExtraSubject, "database file");
				intent.PutExtra(Intent.ExtraText, "please find a database attached");

				string url = DbFileProvider.CONTENT_URI.ToString() + "message.db";
				intent.PutExtra(Intent.ExtraStream, Android.Net.Uri.Parse(url));
				StartActivity(intent);
				*/
			};
		}

		private void Load()
		{
			var messageTextView = FindViewById<TextView> (Resource.Id.editTextMessage);
			messageTextView.Text = _messageDb.LoadMessage();
			/*
			var input = new EditText(this) {
				InputType = Android.Text.InputTypes.TextVariationPassword
			};
			var builder = new AlertDialog.Builder(this)
				.SetTitle("Enter Password")
					.SetMessage("Enter An Encryption Password")
					.SetView(input)
					.SetPositiveButton("OK", (sender, args) => {
						Password = input.Text;
					});
			builder.Create().Show();
			*/
		}

		private void Save()
		{
			var messageTextView = FindViewById<TextView> (Resource.Id.editTextMessage);
			_messageDb.SaveMessage(messageTextView.Text);
		}

		private void CopyStream(Stream istream, Stream ostream) {
			byte[] buffer = new byte[1024];
			int read;
			while ((read = istream.Read(buffer, 0, buffer.Length)) > 0)
			{
				ostream.Write (buffer, 0, read);
			}
		}
	}
}


