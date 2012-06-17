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

namespace ComicsViewer.Actvities
{
    class FileBrowserState : View.BaseSavedState
    {
        public string ComicsPath { get; private set; }

        public FileBrowserState(IParcelable superState, string path)
            : base(superState)
        {
            ComicsPath = path;
        }
    }
}