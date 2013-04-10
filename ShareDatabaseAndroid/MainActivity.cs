using System;
using System.IO;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Text;

namespace ShareDatabase
{
	[Activity (Label = "ShareDatabase", MainLauncher = true)]
	[IntentFilter (
		new[]{Intent.ActionView},
		Categories=new[]{"android.intent.category.DEFAULT"},
		DataMimeType="*/*")]
	public class MainActivity : Activity
	{
		private String databasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "message.db");
		private MessageDb _messageDb;
		private TextView textViewMessage;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);
			_messageDb = new MessageDb(databasePath);
			textViewMessage = FindViewById<TextView> (Resource.Id.editTextMessage);
			var button = FindViewById<Button> (Resource.Id.buttonSave);
			button.Click += delegate {
				Save ();
			};

			button = FindViewById<Button> (Resource.Id.buttonSend);
			button.Click += delegate {
				var intent = new Intent(Android.Content.Intent.ActionSend);
				intent.SetType("text/plain");
				intent.PutExtra(Intent.ExtraEmail, new String[] { });
				intent.PutExtra(Intent.ExtraSubject, "Zetetic Message Database");
				intent.PutExtra(Intent.ExtraText, "Please find a database attached");

				string url = MessageDbProvider.CONTENT_URI.ToString() + "message.db";
				intent.PutExtra(Intent.ExtraStream, Android.Net.Uri.Parse(url));
				StartActivity(intent);
			};

		}

		protected override void OnStart ()
		{
			base.OnStart();
			textViewMessage.Text = "";
			Android.Net.Uri uri = this.Intent != null ? this.Intent.Data : null;
			if(uri != null) 
			{
				using(var istream = ContentResolver.OpenInputStream(uri)) {
					using(var ostream = new FileStream(_messageDb.FilePath, FileMode.OpenOrCreate)) {
						istream.CopyTo(ostream);
					}
				}
			}

			if(File.Exists(databasePath)){
				Load ();
			}
		}

		private void Load()
		{

			/*
			textViewMessage.Text = _messageDb.LoadMessage();
			*/
			var input = new EditText(this);
			input.InputType = (InputTypes.ClassText | InputTypes.TextVariationPassword);
			var builder = new AlertDialog.Builder(this)
				.SetTitle("Enter Password")
					.SetMessage("Password")
					.SetView(input)
					.SetCancelable(false)
					.SetPositiveButton("OK", (sender, args) => {
						_messageDb.Password = input.Text;
						try
						{
							textViewMessage.Text = _messageDb.LoadMessage();
						} catch (Exception e) 
						{
							textViewMessage.Text = e.Message;
						}
					});
			builder.Create().Show();

		}

		private void Save()
		{
			var textViewMessage = FindViewById<TextView> (Resource.Id.editTextMessage);
			/*
			_messageDb.SaveMessage(textViewMessage.Text);
			*/

			var input = new EditText(this);
			input.InputType = (InputTypes.ClassText | InputTypes.TextVariationPassword);
			var builder = new AlertDialog.Builder(this)
				.SetTitle("Enter Password")
					.SetMessage("Password")
					.SetView(input)
					.SetCancelable(false)
					.SetPositiveButton("OK", (sender, args) => {
						_messageDb.Password = input.Text;
						_messageDb.SaveMessage(textViewMessage.Text);
					});
			builder.Create().Show();
		}
	}
}


