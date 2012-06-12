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
            OpenButton.Click += (src, args) => StartActivity(new Intent(ApplicationContext, typeof(FileBrowserActivity)));
            LeftButton.Click += new EventHandler(LeftButton_Click);
            if (Intent.Extras != null && Intent.Extras.ContainsKey("ComicsPath"))
                new ViewerController(this, Intent.Extras.GetString("ComicsPath"));
            else
                new ViewerController(this, null);
            //MainImage.SetImageBitmap(((BitmapDrawable)Resources.GetDrawable(Resource.Drawable.speech_balloon)).Bitmap);
            //MainImage.SetImageBitmap();
        }

        public void SetBitmap(Bitmap bitmap)
        {
            MainImage.SetImageBitmap(bitmap);
        }

        void LeftButton_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}