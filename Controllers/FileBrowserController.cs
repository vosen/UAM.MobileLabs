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
                if ((directoryInfo == null || (directoryInfo != null && value.FullName != directoryInfo.FullName)) && OnCurrentPathChanged(value))
                {
                    directoryInfo = value;
                    Activity.CurrentPath = directoryInfo.FullName;
                    Adapter.NotifyDataSetChanged();
                }
            }
        }

        private bool OnCurrentPathChanged(DirectoryInfo newDir)
        {
            try
            {
                Files = newDir.GetFiles().Where(fi => fi.Extension.Equals(".zip", StringComparison.OrdinalIgnoreCase) || fi.Extension.Equals(".cbz", StringComparison.OrdinalIgnoreCase)).ToArray();
                Directories = newDir.EnumerateDirectories().Where(dir => !dir.Attributes.HasFlag(FileAttributes.Hidden)).ToArray();
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                OnDirectoryAccessFailure(newDir.FullName);
                return false;
            }
        }

        public bool IsRoot { get { return CurrentDirectory.Root.ToString() == CurrentDirectory.ToString(); } }

        public FileBrowserController(FileBrowserActivity activity, string path)
        {
            Activity = activity;
            Adapter = new FileBrowserAdapter(activity, this);
            CurrentDirectory = new DirectoryInfo(GetStoragePath(path));
            Activity.ListAdapter = Adapter;
            Activity.ItemClicked += (src, arg) => OnListItemClicked(arg.Postion);
        }

        internal static string ParentPath(string path)
        {
            return new DirectoryInfo(path).Parent.FullName;
        }

        internal static string GetStoragePath(string oldPath)
        {
            Console.WriteLine(oldPath);
            Console.WriteLine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal));
            // we check for personal folder path because otherwise there's a risk of user being stuck in /data/data/<ProgramName>
            if (oldPath == null || oldPath.StartsWith(ParentPath(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal))))
            {
                string storageState = Android.OS.Environment.ExternalStorageState;
                if (storageState == Android.OS.Environment.MediaMounted)
                    return Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
                if(oldPath == null)
                    return System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            }
            return oldPath;
        }

        private void OnListItemClicked(int position)
        {
            if (IsParentRow(position))
                CurrentDirectory = CurrentDirectory.Parent;
            else if (IsRoot && IsDirectoryRow(position))
                CurrentDirectory = Directories[position];
            else if (!IsRoot && IsDirectoryRow(position))
                CurrentDirectory = Directories[position - 1];
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

        private void OnDirectoryAccessFailure(string path)
        {
            Toast.MakeText(Activity, String.Format("Access to path {0} denied", path), ToastLength.Short).Show();
        }
    }
}