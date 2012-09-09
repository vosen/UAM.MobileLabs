using System;
using System.IO;

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
            // deploy our superterrible comics into unsuspecting user's file system
            string ubunchuPath = Path.Combine(ComicsViewer.Controllers.FileBrowserController.GetStoragePath(null), "ubunchu.cbz");
            if (!File.Exists(ubunchuPath))
            {
                try
                {
                    using (Stream ubunchuSource = Application.Context.Assets.Open("ubunchu.cbz"))
                    {
                        using (FileStream ubunchuTarget = File.Create(ubunchuPath))
                        {
                            StreamExtensions.Copy(ubunchuSource, ubunchuTarget, new byte[8192]);
                        }
                    }
                }
                catch (IOException ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                }
            }
            // continue as usual
            var prefs = GetSharedPreferences("Global", FileCreationMode.Private);
            if (prefs.Contains("ComicsPath"))
                StartActivity(typeof(ViewerActivity));
            else
                StartActivity(typeof(FileBrowserActivity));
        }
    }
}

