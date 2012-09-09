using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComicsViewer.Models;
using ComicsViewer.Actvities;
using Android.Graphics;
using Android.Content;
using Android.OS;

namespace ComicsViewer.Controllers
{
    class ViewerController
    {
        private Comics Comics { get; set; }
        public ViewerActivity Activity { get; private set; }
        public Bitmap Current { get; private set; }
        public string CurrentPath { get { return Comics.Path; } }
        public bool IsComicsOpen { get { return Comics != null; } }
        private int Index { get; set; }
        private Database db;

        public ViewerController(ViewerActivity activity, string path)
        {
            Activity = activity;
            activity.OpenClicked += (src, args) => OpenFileBrowser();
            activity.TurnLeftClicked += (src, args) => TurnLeft();
            activity.TurnRightClicked += (src, args) => TurnRight();
            activity.Stopped += (src, args) => SaveComics();
            db = new Database();
            if (path != null)
            {
                LoadComics(path);
            }
        }

        private void SaveComics()
        {
            if (Comics != null)
            {
                db.SaveSettingsForPath(Comics.Path,
                    new ComicsSettings(
                        Index,
                        Activity.MainImage.TranslationX,
                        Activity.MainImage.TranslationY,
                        Activity.MainImage.CurrentStep));
            }
        }

        void OpenFileBrowser()
        {
            var intent = new Intent(Activity.ApplicationContext, typeof(FileBrowserActivity));
            Activity.StartActivity(intent);
        }

        public void LoadComics(string path)
        {
            // load the comics
            Comics = Comics.FromPath(path);

            // load new settings
            ComicsSettings settings = db.GetSettingsForPath(path);
            if (settings != null)
            {
                OpenPage(settings.Page);
                Activity.MainImage.CurrentStep = settings.ZoomStep;
                Activity.MainImage.TranslationX = (float)settings.TranslationX;
                Activity.MainImage.TranslationY = (float)settings.TranslationY;
                Activity.MainImage.RefreshLayout();
            }
            else
            {
                OpenPage(0);
            }
        }

        private void OpenPage(int idx)
        {
            if (Current != null)
            {
                Current.Recycle();
                Current.Dispose();
            }
            Current = BitmapFactory.DecodeByteArray(Comics.Bitmaps[idx], 0, Comics.Bitmaps[idx].Length);
            Activity.SetBitmap(Current);
            Index = idx;

            if (Index == 0)
                Activity.LeftButton.Visibility = Android.Views.ViewStates.Invisible;
            else
                Activity.LeftButton.Visibility = Android.Views.ViewStates.Visible;

            if (Index == (Comics.Bitmaps.Length - 1))
                Activity.RightButton.Visibility = Android.Views.ViewStates.Invisible;
            else
                Activity.RightButton.Visibility = Android.Views.ViewStates.Visible;
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
