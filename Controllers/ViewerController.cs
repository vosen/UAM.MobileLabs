using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComicsViewer.Models;
using ComicsViewer.Actvities;
using Android.Graphics;

namespace ComicsViewer.Controllers
{
    class ViewerController
    {
        public Comics Comics { get; private set; }
        public ViewerActivity Activity { get; private set; }
        public Bitmap Current { get; private set; }

        public ViewerController(ViewerActivity activity, string path)
        {
            Activity = activity;
            if (path != null)
            {
                Comics = Comics.FromPath(path);
                Current = BitmapFactory.DecodeByteArray(Comics.Bitmaps[0], 0, Comics.Bitmaps[0].Length);
                activity.SetBitmap(Current);
            }
        }
    }
}
