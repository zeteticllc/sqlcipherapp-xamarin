using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace ShareDatabaseiOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		UIWindow window;
		ShareDatabaseiOSViewController viewController;

		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			
			viewController = new ShareDatabaseiOSViewController ();
			window.RootViewController = viewController;
			window.MakeKeyAndVisible ();

			if(options != null)
			{
				var url = (NSUrl) options["UIApplicationLaunchOptionsURL"];
				if (url != null && url.IsFileUrl) 
				{
					viewController.HandleOpenUrl(url);
					Console.WriteLine(url);
				}
			}
			return true;
		}

		public override bool OpenUrl (UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
		{
			if(url.IsFileUrl) 
			{
				Console.WriteLine(url);
				viewController.HandleOpenUrl(url);
				return true;
			}
			return false;
		}

	}
}

