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

using Java.IO;
using ComicsViewer.Controllers;
using System.IO;

namespace ComicsViewer.Adapters
{
    class FileBrowserAdapter : BaseAdapter<string>
    {
        public FileBrowserController Controller { get; private set; }
        private LayoutInflater inflater;

        public FileBrowserAdapter(Context ctx, FileBrowserController contr)
        {
            Controller = contr;
            inflater = LayoutInflater.From(ctx);
        }

        public override int ViewTypeCount
        {
            get
            {
                return 3;
            }
        }

        public override bool HasStableIds
        {
            get
            {
                return false;
            }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View currentView;
            if (Controller.IsParentRow(position))
            {
                currentView = inflater.Inflate(Resource.Layout.ParentRow, parent, false);
                return currentView;
            }
            else if (Controller.IsDirectoryRow(position))
            {
                currentView = inflater.Inflate(Resource.Layout.DirectoryRow, parent, false);
            }
            else
            {
                currentView = inflater.Inflate(Resource.Layout.FileRow, parent, false);
            }
            currentView.FindViewById<TextView>(Resource.Id.RowText).Text = this[position];
            return currentView;
        }

        public override string this[int position]
        {
            get 
            {
                return Controller.FileDirectoryName(position);
            }
        }

        public override int Count
        {
            get { return Controller.Directories.Length + Controller.Files.Length + (Controller.IsRoot ? 0 : 1); }
        }

        public override long GetItemId(int position)
        {
            return position;
        }
    }
}