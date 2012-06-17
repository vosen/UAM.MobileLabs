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

using ComicsViewer.Actvities;
using ComicsViewer.Adapters;
using System.IO;

namespace ComicsViewer.Controllers
{
    class FileBrowserController
    {
        private FileBrowserActivity Activity { get; set; }
        private FileBrowserAdapter Adapter { get; set; }
        public DirectoryInfo[] Directories { get; private set; }
        public FileInfo[] Files { get; private set; }
        private DirectoryInfo directoryInfo;

        public DirectoryInfo CurrentDirectory
        {
            get
            {
                return directoryInfo;
            }
            private set
            {
                if (directoryInfo != null && value.FullName == directoryInfo.FullName)
                    return;
                directoryInfo = value;
                OnCurrentPathChanged();
            }
        }

        private void OnCurrentPathChanged()
        {
            Files = CurrentDirectory.GetFiles().Where(fi => fi.Extension.Equals(".zip", StringComparison.OrdinalIgnoreCase) ).ToArray();
            Directories = CurrentDirectory.EnumerateDirectories().Where(dir => !dir.Attributes.HasFlag(FileAttributes.Hidden)).ToArray();
            Activity.CurrentPath = CurrentDirectory.FullName;
            Adapter.NotifyDataSetChanged();
        }

        public bool IsRoot { get { return CurrentDirectory.Root.ToString() == CurrentDirectory.ToString(); } }

        public FileBrowserController(FileBrowserActivity activity, string path)
        {
            Activity = activity;
            Adapter = new FileBrowserAdapter(activity, this);
            CurrentDirectory = (path == null) ? new DirectoryInfo(GetStoragePath()) : new DirectoryInfo(path);
            Activity.ListAdapter = Adapter;
            Activity.ItemClicked += (src, arg) => OnListItemClicked(arg.Postion);
        }

        private string GetStoragePath()
        {
            var androidPath = Android.OS.Environment.ExternalStorageDirectory;
            if(androidPath != null)
                return androidPath.AbsolutePath;
            return System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        }

        private void OnListItemClicked(int position)
        {
            if (IsParentRow(position))
                CurrentDirectory = CurrentDirectory.Parent;
            else if (IsRoot && IsDirectoryRow(position))
                CurrentDirectory = Directories[position];
            else if (!IsRoot && IsDirectoryRow(position))
                CurrentDirectory = Directories[position - 1];
            // do something on click
            else if (!IsRoot)
                OpenFile(Files[position - Directories.Length - 1].FullName);
            else
                OpenFile(Files[position - Directories.Length].FullName);
        }

        private void OpenFile(string path)
        {
            ComicsViewer.Models.Comics.FromPath(path);
            var intent = new Intent(Activity.ApplicationContext, typeof(ViewerActivity));
            intent.PutExtra("ComicsPath", path);
            Activity.StartActivity(intent);

        }

        public string FileDirectoryName(int position)
        {
            if (IsParentRow(position))
                return "..";
            else if (!IsRoot && position <= Directories.Length)
                return Directories[position - 1].Name;
            else if (IsRoot && position < Directories.Length)
                return Directories[position].Name;
            else if (!IsRoot)
                return Files[position - Directories.Length - 1].Name;
            else
                return Files[position - Directories.Length].Name;
        }

        public bool IsParentRow(int position)
        {
            return (position == 0) && !IsRoot;
        }

        public bool IsDirectoryRow(int position)
        {
            return (position < Directories.Length || (position == Directories.Length & !IsRoot));
        }
    }
}