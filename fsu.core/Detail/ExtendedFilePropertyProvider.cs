using HeyRed.Mime;
using Maxstupo.Fsu.Core.Processor;
using Maxstupo.Fsu.Core.Utility;
using System;
using System.IO;

namespace Maxstupo.Fsu.Core.Detail {

    public class ExtendedFilePropertyProvider : IFilePropertyProvider {

        private bool isBulkMode = false;

        private ExtendedFile file;

        public void Begin() {
            if (isBulkMode)
                return;
#if DEBUG
            Console.WriteLine("\t#BeginBulkPropertyRequest");
#endif
            isBulkMode = true;
        }

        public void End() {
            if (!isBulkMode)
                return;
#if DEBUG
            Console.WriteLine("\t#EndBulkPropertyRequest");
#endif
            isBulkMode = false;
            file?.Dispose();
            file = null;
        }


        public Property GetFileProperty(ProcessorItem item, string propertyName) {
            if (!File.Exists(item.Value))
                return null;

            string mime = MimeTypesMap.GetMimeType(item.Value);
            bool isVideo = mime.StartsWith("video");
            bool isImage = mime.StartsWith("image");

            if (isVideo || isImage || mime.StartsWith("audio")) {

                if (!Enum.TryParse(propertyName, true, out FileDetail detail))
                    return null;

                if (isVideo) {
                    if (detail == FileDetail.Width) {
                        detail = FileDetail.FrameWidth;
                    } else if (detail == FileDetail.Height) {
                        detail = FileDetail.FrameHeight;
                    }
                } else if (isImage) {
                    if (detail == FileDetail.FrameWidth) {
                        detail = FileDetail.Width;
                    } else if (detail == FileDetail.FrameHeight) {
                        detail = FileDetail.Height;
                    }
                }

                if (!isBulkMode || file == null) {
#if DEBUG
                    if (isBulkMode)
                        ColorConsole.WriteLine($"\t    #File: '&-e;{item.Value}&-^;'");
#endif
                    file = new ExtendedFile(item.Value);
                }

#if DEBUG
                ColorConsole.WriteLine($"\t\t#GetProperty '&-e;{propertyName}&-^;");
#endif


                dynamic value = file.GetExtendedProperty(detail);

                if (!isBulkMode) {
                    file?.Dispose();
                    file = null;
                }

                if (value == null)
                    return null;

                if (value is string str) {
                    return new Property(str);
                } else {
                    if (detail == FileDetail.Duration)
                        value /= 10000;
                    else if (detail == FileDetail.TotalBitrate)
                        value /= 1000;
                    return new Property(Convert.ChangeType(value, typeof(double)));
                }

            }
            return null;
        }

    }

}
