using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComicsViewer.Models
{
    class ComicsSettings
    {
        public int Page { get; private set; }
        public double TranslationX { get; private set; }
        public double TranslationY { get; private set; }
        public int ZoomStep { get; private set; }

        private ComicsSettings() {}

        public ComicsSettings(int page, float translX, float translY, int zoom)
        {
            Page = page;
            TranslationX = translX;
            TranslationY = translY;
            ZoomStep = zoom;
        }
    }
}
