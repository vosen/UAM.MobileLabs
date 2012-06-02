using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace ComicsViewer.Actvities
{
    [Activity(Label = "Otwórz komiks", MainLauncher = true, Icon = "@drawable/icon")]
    public class Activity1 : FileBrowserActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
        }
    }
}

