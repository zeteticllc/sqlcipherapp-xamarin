// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace ShareDatabaseiOS
{
	[Register ("ShareDatabaseiOSViewController")]
	partial class ShareDatabaseiOSViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton buttonSend { get; set; }

		[Action ("SendButtonClick:")]
		partial void SendButtonClick (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (buttonSend != null) {
				buttonSend.Dispose ();
				buttonSend = null;
			}
		}
	}
}
