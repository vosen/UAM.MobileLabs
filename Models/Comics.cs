using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Java.Util.Zip;
using Java.Util;
using Android.Graphics;
using Android.Runtime;

namespace ComicsViewer.Models
{
    class Comics
    {
        public string Path {get; private set;}
        public byte[][] Bitmaps { get; private set; }

        private Comics(string path)
        {
            Path = path;
        }

        public static Comics FromPath(string path)
        {
            Comics comics = new Comics(path);
            using(var zip = new ZipFile(path))
            {
                comics.Bitmaps = GetBitmapEntries(zip)
                    .OrderBy(entry => entry.Name)
                    .Select(entry =>
                        {
                            using (var stream = zip.GetInputStream(entry))
                            {
                                return stream.ReadFully();
                            }
                        })
                    .ToArray();
            }
            return comics;
        }

        private static IEnumerable<ZipEntry> GetBitmapEntries(ZipFile file)
        {
            using(IEnumeration entries = file.Entries())
            {
                while(entries.HasMoreElements)
                {
                    var entry = (ZipEntry)entries.NextElement();
                    if (!entry.IsDirectory
                        && (entry.Name.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) 
                            || entry.Name.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                            || entry.Name.EndsWith(".gif", StringComparison.OrdinalIgnoreCase)
                            || entry.Name.EndsWith(".png", StringComparison.OrdinalIgnoreCase)))
                    {
                        yield return entry;
                    }
                }
            }
        }
    }
}
