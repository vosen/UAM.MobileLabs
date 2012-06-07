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

namespace ComicsViewer.Views
{
    class TouchImageViewState : View.BaseSavedState
    {
        internal float CenterX { get; private set; }
        internal float CenterY { get; private set; }
        internal int ScaleStep { get; private set; }

        public TouchImageViewState(IParcelable superState, float x, float y, int step)
            : base(superState)
        {
            CenterX = x;
            CenterY = y;
            ScaleStep = step;
        }
    }
}