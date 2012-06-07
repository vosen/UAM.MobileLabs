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

namespace ComicsViewer.Actvities
{
    [Activity(Label = "My Activity")]
    public class ViewerActivity : Activity
    {
        private TouchImageView MainImage { get; set; }
        private ZoomControls Zoom { get; set; }

        protected override void OnCreate(Bundle bundle)
        {
            RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Viewer);
            MainImage = FindViewById<TouchImageView>(Resource.Id.MainImage);
            Zoom = FindViewById<ZoomControls>(Resource.Id.ZoomButtons);
            Zoom.ZoomInClick += (src, args) => MainImage.Scale(0.3f);
            MainImage.SetImageBitmap(((BitmapDrawable)Resources.GetDrawable(Resource.Drawable.speech_balloon)).Bitmap);
            //MainImage.SetImageBitmap();
        }
    }
}