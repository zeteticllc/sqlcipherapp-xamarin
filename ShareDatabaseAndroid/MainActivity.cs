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
	[Activity (Label = "ShareDatabase", MainLauncher = true)]
	[IntentFilter (
		new[]{Intent.ActionView},
		Categories=new[]{"android.intent.category.DEFAULT"},
		DataMimeType=MessageDb.MIME_TYPE)]
	public class MainActivity : Activity
	{
		private MessageDb _messageDb = new MessageDb(
			Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "message.db")
			);

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

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

			var button = FindViewById<Button> (Resource.Id.buttonSave);
			button.Click += delegate {
				Save ();
			};

			button = FindViewById<Button> (Resource.Id.buttonSend);
			button.Click += delegate {
				Save ();
				var intent = new Intent(Android.Content.Intent.ActionSend);
				intent.SetType("text/plain");
				intent.PutExtra(Intent.ExtraEmail, new String[] { });
				intent.PutExtra(Intent.ExtraSubject, "Zetetic Message Database");
				intent.PutExtra(Intent.ExtraText, "Please find a database attached");

				string url = MessageDbProvider.CONTENT_URI.ToString() + "message.db";
				intent.PutExtra(Intent.ExtraStream, Android.Net.Uri.Parse(url));
				StartActivity(intent);
			};

			Load ();
		}

		private void Load()
		{
			var textViewMessage = FindViewById<TextView> (Resource.Id.editTextMessage);
			textViewMessage.Text = _messageDb.LoadMessage();
			/*
			var input = new EditText(this) {
				InputType = Android.Text.InputTypes.TextVariationPassword
			};
			var builder = new AlertDialog.Builder(this)
				.SetTitle("Enter Password")
					.SetMessage("Password")
					.SetView(input)
					.SetPositiveButton("OK", (sender, args) => {
						_messageDb = input.Text;
						try
						{
							messageTextView.Text = _messageDb.LoadMessage();
						} catch (SqliteException e) 
						{
							messageTextView.Text = e.Message;
						}
					});
			builder.Create().Show();
			*/
		}

		private void Save()
		{
			var textViewMessage = FindViewById<TextView> (Resource.Id.editTextMessage);
			_messageDb.SaveMessage(textViewMessage.Text);
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


