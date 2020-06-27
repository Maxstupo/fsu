namespace Maxstupo.Fsu.Utility {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using Shell32;

    public enum FileDetail {
        FrameWidth,
        FrameHeight,
        FrameRate,

        /// <summary>
        /// Indicates the total data rate in "bits per second" for all video and audio streams.
        /// </summary>
        TotalBitrate,

        HorizontalAspectRatio,
        VerticalAspectRatio,

        /// <summary>
        /// The media duration, in 100ns units. (Divide by 10,000 to get milliseconds)
        /// </summary>
        Duration,

        Width,
        Height,
        BitDepth,


        PageCount
    }

    public sealed class ExtendedFile {

        // https://docs.microsoft.com/en-us/windows/win32/properties/props
        private static readonly Lazy<IReadOnlyDictionary<FileDetail, string>> scids = new Lazy<IReadOnlyDictionary<FileDetail, string>>(() => new Dictionary<FileDetail, string> {
            { FileDetail.FrameWidth,            "{64440491-4C8B-11D1-8B70-080036B11A03} 3"  },   // Video: System.Video.FrameWidth
            { FileDetail.FrameHeight,           "{64440491-4C8B-11D1-8B70-080036B11A03} 4"  },   // Video: System.Video.FrameHeight
            { FileDetail.FrameRate,             "{64440491-4C8B-11D1-8B70-080036B11A03} 6"  },   // Video: System.Video.FrameRate
            { FileDetail.TotalBitrate,          "{64440491-4C8B-11D1-8B70-080036B11A03} 43" },   // Video: System.Video.TotalBitrate
            { FileDetail.HorizontalAspectRatio, "{64440491-4C8B-11D1-8B70-080036B11A03} 42" },   // Video: System.Video.HorizontalAspectRatio
            { FileDetail.VerticalAspectRatio,   "{64440491-4C8B-11D1-8B70-080036B11A03} 45" },   // Video: System.Video.VerticalAspectRatio
            { FileDetail.Duration,              "{64440490-4C8B-11D1-8B70-080036B11A03} 3"  },   // Media: System.Media.Duration
                                                                                                 
            { FileDetail.Width,                 "{6444048F-4C8B-11D1-8B70-080036B11A03} 3"  },   // Image: System.Image.HorizontalSize
            { FileDetail.Height,                "{6444048F-4C8B-11D1-8B70-080036B11A03} 4"  },   // Image: System.Image.VerticalSize
            { FileDetail.BitDepth,              "{6444048F-4C8B-11D1-8B70-080036B11A03} 7"  },   // Image: System.Image.BitDepth
        
            { FileDetail.PageCount,             "{F29F85E0-4FF9-1068-AB91-08002B27B3D9} 14" },   // Document: System.Document.PageCount
        });

        public static IReadOnlyDictionary<FileDetail, string> Scids => scids.Value;


        private static readonly Shell shell = new Shell();

        private readonly Folder folder;
        private readonly ShellFolderItem folderItem;

        private bool isDisposed = false;

        public ExtendedFile(string path) {
            folder = shell.NameSpace(Path.GetDirectoryName(path));
            folderItem = (ShellFolderItem) folder.ParseName(Path.GetFileName(path));
        }

        ~ExtendedFile() {
            Dispose();
        }

        public dynamic GetExtendedProperty(string bstrPropName) {
            return folderItem.ExtendedProperty(bstrPropName);
        }

        public dynamic GetExtendedProperty(FileDetail detail) {
            if (scids.Value.TryGetValue(detail, out string scid))
                return folderItem.ExtendedProperty(scid);
            return null;
        }

        public T GetExtendedProperty<T>(FileDetail detail) {
            dynamic item = GetExtendedProperty(detail);
            return item == null ? default : (T) Convert.ChangeType(item, typeof(T));
        }


        public void Dispose() {
            if (isDisposed)
                return;
            Marshal.ReleaseComObject(folderItem);
            Marshal.ReleaseComObject(folder);
            isDisposed = true;
        }



    }

}
