using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Provider;
using Geolocator.Plugin;
using Android.Support.V4.App;
using Android;
using Android.Content.PM;
using System;
using System.Collections.Generic;

namespace permmsionsTest
{
	[Activity(Label = "permmsionsTest", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity , ActivityCompat.IOnRequestPermissionsResultCallback
	{
		int count = 1;
		RuntimePermissions.PermissionsHelper helper;

		protected override  void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Main);
			Button button = FindViewById<Button>(Resource.Id.myButton);

			button.Click +=  delegate 
			{
				var l = new string[] { Manifest.Permission.ReadContacts };
			helper = new RuntimePermissions.PermissionsHelper(this, l ,"",()=>{
					RunOnUiThread(()=> { Toast.MakeText(this,"yes",ToastLength.Long).Show();});
				},()=>{
					this.RunOnUiThread(() => { Toast.MakeText(this, "n", ToastLength.Long).Show();});
				});
			};
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			helper.OnRequestPermissionsResult(requestCode,permissions,grantResults);
		}


}
}


