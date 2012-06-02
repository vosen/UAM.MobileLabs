using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ComicsViewer.Events;
using ComicsViewer.Adapters;
using ComicsViewer.Controllers;

namespace ComicsViewer.Actvities
{
    [Activity()]
    public class FileBrowserActivity : ListActivity
    {
        private TextView CurrentPathView;
        //private ListView EntriesList;
        //private BaseAdapter EntriesListAdapter;
        private string currentPath;

        public string CurrentPath 
        { 
            get
            {
                return currentPath;
            }
            set
            {
                CurrentPathView.Text = value;
                currentPath = value;
                OnCurrentPathChanged(currentPath);
            }
        }

        public event EventHandler<PathChangedEventArgs> CurrentPathChanged;
        public event EventHandler<ItemClickedEventArgs> ItemClicked;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.FileBrowser);
            CurrentPathView = FindViewById<TextView>(Resource.Id.CurrentPath);
            new FileBrowserController(this);
        }

        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            base.OnListItemClick(l, v, position, id);
            var temp = ItemClicked;
            if (temp != null)
            {
                temp(this, new ItemClickedEventArgs(position));
            }
        }

        private void OnCurrentPathChanged(string newPath)
        {
            var temp = CurrentPathChanged;
            if (temp != null)
            {
                temp(this, new PathChangedEventArgs(newPath));
            }
        }
    }
}