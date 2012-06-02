using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComicsViewer.Events
{
    public class ItemClickedEventArgs : EventArgs
    {
        public int Postion { get; private set; }
        public ItemClickedEventArgs(int pos)
        {
            Postion = pos;
        }
    }
}
