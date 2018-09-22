
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Busidex.Mobile
{
	[Activity (Label = "AddCard", MainLauncher = false)]			
	public class AddCard : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create your application here
			SetContentView (Resource.Layout.AddCard);

			Button btnBackMyBusidex = FindViewById<Button> (Resource.Id.btnBackMyBusidex);
			btnBackMyBusidex.Click += delegate {
				var aMyBusidex = new Intent(this, typeof(MyBusidex));
				StartActivity (aMyBusidex);
			};
		}
	}
}

