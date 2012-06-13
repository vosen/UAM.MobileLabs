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
    [Activity(Label = "My Activity")]
    public class ViewerActivity : Activity
    {
        private TouchImageView MainImage { get; set; }
        private ZoomControls Zoom { get; set; }
        private ImageButton OpenButton { get; set; }
        private ImageButton LeftButton { get; set; }
        private ImageButton RightButton { get; set; }

        public event EventHandler TurnLeftClicked;
        public event EventHandler TurnRightClicked;
        public event EventHandler OpenClicked;

        protected override void OnCreate(Bundle bundle)
        {
            RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Viewer);
            // associate the views
            MainImage = FindViewById<TouchImageView>(Resource.Id.MainImage);
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
            if (Intent.Extras != null && Intent.Extras.ContainsKey("ComicsPath"))
                new ViewerController(this, Intent.Extras.GetString("ComicsPath"));
            else
                new ViewerController(this, null);
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
    }
}