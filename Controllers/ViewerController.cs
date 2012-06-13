using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComicsViewer.Models;
using ComicsViewer.Actvities;
using Android.Graphics;
using Android.Content;

namespace ComicsViewer.Controllers
{
    class ViewerController
    {
        public Comics Comics { get; private set; }
        public ViewerActivity Activity { get; private set; }
        public Bitmap Current { get; private set; }
        private int Index { get; set; }

        public ViewerController(ViewerActivity activity, string path)
        {
            Activity = activity;
            activity.OpenClicked += (src, args) => OpenFileBrowser();
            activity.TurnLeftClicked += (src, args) => TurnLeft();
            activity.TurnRightClicked += (src, args) => TurnRight();
            if (path != null)
            {
                LoadComics(path);
                OpenPage(0);
            }
        }

        void OpenFileBrowser()
        {
            Activity.StartActivity(new Intent(Activity.ApplicationContext, typeof(FileBrowserActivity)));
        }

        private void LoadComics(string path)
        {
            Comics = Comics.FromPath(path);
        }

        private void OpenPage(int idx)
        {
            if (Current != null)
                Current.Dispose();
            Current = BitmapFactory.DecodeByteArray(Comics.Bitmaps[idx], 0, Comics.Bitmaps[idx].Length);
            Activity.SetBitmap(Current);
            Index = idx;
        }

        private void TurnLeft()
        {
            if (Index - 1 >= 0)
                OpenPage(Index - 1);
        }

        private void TurnRight()
        {
            if (Index + 1 < Comics.Bitmaps.Length)
                OpenPage(Index + 1);
        }
    }
}
