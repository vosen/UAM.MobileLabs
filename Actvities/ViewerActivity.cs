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
using ComicsViewer.Views;
using Android.Graphics;
using Android.Graphics.Drawables;
using ComicsViewer.Controllers;

namespace ComicsViewer.Actvities
{
    [Activity(Label = "", LaunchMode = Android.Content.PM.LaunchMode.SingleInstance)]
    public class ViewerActivity : Activity
    {
        public ZoomImageView MainImage { get; private set; }
        private ZoomControls Zoom { get; set; }
        private ImageButton OpenButton { get; set; }
        internal ImageButton LeftButton { get; set; }
        internal ImageButton RightButton { get; set; }
        private ViewerController Controller { get; set; }

        public event EventHandler TurnLeftClicked;
        public event EventHandler TurnRightClicked;
        public event EventHandler OpenClicked;
        public event EventHandler Stopped;

        protected override void OnCreate(Bundle bundle)
        {
            RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Viewer);
            // associate the views
            MainImage = FindViewById<ZoomImageView>(Resource.Id.MainImage);
            Zoom = FindViewById<ZoomControls>(Resource.Id.ZoomButtons);
            OpenButton = FindViewById<ImageButton>(Resource.Id.OpenButton);
            LeftButton = FindViewById<ImageButton>(Resource.Id.LeftButton);
            RightButton = FindViewById<ImageButton>(Resource.Id.RightButton);
            // wire the views
            Zoom.ZoomInClick += (src, args) => MainImage.ZoomIn();
            Zoom.ZoomOutClick += (src, args) => MainImage.ZoomOut();
            OpenButton.Click += (src, args) => OnOpenButtonClick(args);
            LeftButton.Click += (src, args) => OnLeftButtonClick(args);
            RightButton.Click += (src, args) => OnRightButtonClick(args);
            // get preferences
            var prefs = GetSharedPreferences("Global", FileCreationMode.Private);
            // check bundles and prefs
            if (Intent.Extras != null && Intent.Extras.ContainsKey("ComicsPath"))
                Controller = new ViewerController(this, Intent.Extras.GetString("ComicsPath"));
            else if (bundle != null && bundle.ContainsKey("ComicsPath"))
                Controller = new ViewerController(this, bundle.GetString("ComicsPath"));
            else
                Controller = new ViewerController(this, prefs.GetString("ComicsPath", null));
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            if (intent.Extras != null && intent.Extras.ContainsKey("ComicsPath"))
                Controller.LoadComics(intent.Extras.GetString("ComicsPath"));
        }

        protected override void OnStop()
        {
            if(Controller.IsComicsOpen)
            {
                var editor = GetSharedPreferences("Global", FileCreationMode.Private).Edit();
                editor.PutString("ComicsPath", Controller.CurrentPath);
                editor.Commit();
            }

            if (Stopped != null)
                Stopped(this, new EventArgs());

            base.OnStop();
        }

        public void SetBitmap(Bitmap bitmap)
        {
            MainImage.SetImageBitmap(bitmap);
        }

        void OnLeftButtonClick(EventArgs e)
        {
            if (TurnLeftClicked != null)
                TurnLeftClicked(this, e);
        }

        void OnRightButtonClick(EventArgs e)
        {
            if (TurnRightClicked != null)
                TurnRightClicked(this, e);
        }

        void OnOpenButtonClick(EventArgs e)
        {
            if (OpenClicked != null)
                OpenClicked(this, e);
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if(Controller.IsComicsOpen)
                outState.PutString("ComicsPath", Controller.CurrentPath);
            base.OnSaveInstanceState(outState);
        }
    }
}