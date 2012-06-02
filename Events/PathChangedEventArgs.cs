using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace ComicsViewer.Events
{
    public class PathChangedEventArgs : EventArgs
    {
        public string NewPath { get; private set; }
        public PathChangedEventArgs(string newPath)
        {
            NewPath = NewPath;
        }
    }
}