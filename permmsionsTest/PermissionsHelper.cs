using Android.App;
using Android.OS;
using Android.Content;
using Android.Support.V4.App;
using Android.Content.PM;
using System;
using System.Collections.Generic;

namespace RuntimePermissions
{
	public class PermissionsHelper : Java.Lang.Object
	{
		//List off permmersions you have to ask for as off 23 , can change with new versions

		//READ_CALENDAR
		//WRITE_CALENDAR
		//CAMERA
		//CAMERA
		//CONTACTS
		//READ_CONTACTS
		//WRITE_CONTACTS
		//GET_ACCOUNTS
		//LOCATION
		//ACCESS_FINE_LOCATION
		//ACCESS_COARSE_LOCATION
		//MICROPHONE
		//RECORD_AUDIO
		//PHONE
		//READ_PHONE_STATE
		//CALL_PHONE
		//READ_CALL_LOG
		//WRITE_CALL_LOG
		//ADD_VOICEMAIL
		//USE_SIP
		//PROCESS_OUTGOING_CALLS
		//SENSORS
		//BODY_SENSORS
		//SMS
		//SEND_SMS
		//RECEIVE_SMS
		//READ_SMS
		//RECEIVE_WAP_PUSH
		//RECEIVE_MMS
		//STORAGE
		//READ_EXTERNAL_STORAGE
		//WRITE_EXTERNAL_STORAGE


		public string ReasonDialogTitle { set; get; } = "Permission";
		public string ReasonDialogConfrim { set; get; } = "Ok";
		public string ReasonDialogDecline { set; get; } = "Cancel";

		private const int _requestCode = 123;
		private Activity _context;
		private Action _goHead;
		private Action _notAccpted;
		private string[] _perms;
		private string _reason;



		public PermissionsHelper(Activity context, string[] perms, string reason, Action accpeted, Action notaccpted)
		{
			_context = context;
			_goHead = accpeted;
			_notAccpted = notaccpted;
			_perms = perms;
			_reason = reason;

			CheckIfHavePermmsion();


		}


		public void CheckIfHavePermmsion()
		{
			//if before marchmello dont worry about it
			if ((int)Build.VERSION.SdkInt < 23)
			{
				_goHead?.Invoke();
			}
			else
			{
				var havePermmsion = false;

				foreach (var perm in _perms)
				{
					havePermmsion = Android.Support.V4.Content.ContextCompat.CheckSelfPermission(_context, perm) == Permission.Granted;
				}


				if (havePermmsion)
				{
					//if we have it carry on
					_goHead?.Invoke();
				}
				else
				{
					GetPermmsionsFor(_reason);
				}

			}

		}

		void GetPermmsionsFor(string reason)
		{

			bool shouldShow = false;


			foreach (var perm in _perms)
			{
				shouldShow = ActivityCompat.ShouldShowRequestPermissionRationale(_context, perm);
			}


			//if they have said no in the past but want to use the feature again show them this

			if (shouldShow && !string.IsNullOrEmpty(reason))
			{
#pragma warning disable RECS0091 // Use 'var' keyword when possible : is complex type 
				AlertDialog.Builder alert = new AlertDialog.Builder(_context);
#pragma warning restore RECS0091 // Use 'var' keyword when possible

				alert.SetTitle(ReasonDialogTitle);
				alert.SetMessage(reason);
				alert.SetPositiveButton(ReasonDialogConfrim, (senderAlert, args) =>
				{
					RequestPerms();
				});

				alert.SetNegativeButton(ReasonDialogDecline, (object sender, DialogClickEventArgs e) =>
				{
					var s = (Dialog)sender;
					s?.Dismiss();
					_notAccpted?.Invoke();
				});

				var dialog = alert.Create();
				dialog.Show();
			}
			else
			{
				RequestPerms();
			}
		}

		void RequestPerms()
		{
			var list = new List<string>();

			foreach (var perm in _perms)
			{
				list.Add(perm);
			}

			//brings up dialog box for for user to accpet perm
			_context.RunOnUiThread(()=> {
				ActivityCompat.RequestPermissions(_context, list.ToArray(), _requestCode);
			});

		}

		public void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			if (VerifyPermissions(grantResults) && requestCode == _requestCode)
			{
				_goHead?.Invoke();
			}
			else
			{
				_notAccpted?.Invoke();
			}
		}

		public bool VerifyPermissions(Permission[] grantResults)
		{
			
			// At least one result must be checked.
			if (grantResults == null || grantResults.Length < 1)
				return false;

			// Verify that each required permission has been granted, otherwise return false.
			foreach (Permission result in grantResults)
			{
				if (result != Permission.Granted)
				{
					return false;
				}
			}
			return true;
		}
	}
}

