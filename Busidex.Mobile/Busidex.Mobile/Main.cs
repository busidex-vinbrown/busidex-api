using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;

namespace Busidex.Mobile
{
	[Activity (Label = "Busidex.Mobile", MainLauncher = true)]
	public class Main : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			var btnMyBusidex = FindViewById<Button> (Resource.Id.btnMyBusidex);
			var btnAddCard = FindViewById<Button> (Resource.Id.btnAddCard);
			           
			btnMyBusidex.Click += delegate {
				var aMyBusidex = new Intent(this, typeof(MyBusidex));
				StartActivity (aMyBusidex);
			};
			btnAddCard.Click += delegate {
				var aAddCard = new Intent(this, typeof(AddCard));
				StartActivity (aAddCard);
			};

           

		}
	}
}


