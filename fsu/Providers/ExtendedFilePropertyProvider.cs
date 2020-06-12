using HeyRed.Mime;
using Maxstupo.Fsu.Core.Detail;
using Maxstupo.Fsu.Core.Processor;
using Maxstupo.Fsu.Utility;
using System;
using System.IO;

namespace Maxstupo.Fsu.Providers {

    public class ExtendedFilePropertyProvider : IPropertyProvider {

        private bool isBulkMode = false;

        private ExtendedFile file;

        public void Begin() {
            isBulkMode = true;
        }

        public void End() {
            isBulkMode = false;
            file?.Dispose();
            file = null;
        }

        public PropertyItem GetProperty(IPropertyProvider providerList, ProcessorItem item, string propertyName) {
            if (!File.Exists(item.Value))
                return null;

            if (!Enum.TryParse(propertyName, true, out FileDetail detail))
                return null;

            string mime = MimeTypesMap.GetMimeType(item.Value);

            // HACK: Work out a better solution for file detail naming translation (e.g. width -> framewidth, if video, etc..)
            // Might be better being embedded in the ExtendedFile class.
            if (mime.StartsWith("video")) {
                if (detail == FileDetail.Width) {
                    detail = FileDetail.FrameWidth;
                } else if (detail == FileDetail.Height) {
                    detail = FileDetail.FrameHeight;
                }
            } else if (mime.StartsWith("image")) {
                if (detail == FileDetail.FrameWidth) {
                    detail = FileDetail.Width;
                } else if (detail == FileDetail.FrameHeight) {
                    detail = FileDetail.Height;
                }
            }

            if (!isBulkMode || file == null)
                file = new ExtendedFile(item.Value);

            dynamic value = file.GetExtendedProperty(detail);

            if (!isBulkMode) {
                file?.Dispose();
                file = null;
            }

            if (value == null)
                return null;

            if (value is string str) {
                return new PropertyItem(str);
            } else {
                if (detail == FileDetail.Duration) // HACK: Work out a better solution for file detail value translation.
                    value /= 10000;
                else if (detail == FileDetail.TotalBitrate)
                    value /= 1000;

                return new PropertyItem(Convert.ChangeType(value, typeof(double)));
            }
        }

    }

}
