using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace ComicsViewer.Actvities
{
    [Activity(MainLauncher = true, Icon = "@drawable/icon", NoHistory = true)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            var prefs = GetSharedPreferences("Global", FileCreationMode.Private);
            if (prefs.Contains("ComicsPath"))
                StartActivity(typeof(ViewerActivity));
            else
                StartActivity(typeof(FileBrowserActivity));
        }
    }
}

