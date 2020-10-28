namespace Maxstupo.Fsu.Providers {

    using System;
    using System.IO;
    using HeyRed.Mime;
    using Maxstupo.Fsu.Core.Detail;
    using Maxstupo.Fsu.Core.Processor;
    using Maxstupo.Fsu.Utility;
    using UnitsNet.Units;

    public sealed class ExtendedFilePropertyProvider : IPropertyProvider {

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
                double numericValue = Convert.ChangeType(value, typeof(double));

                Enum unit = null;

                if (detail == FileDetail.Duration) {
                    numericValue /= 10000;
                    unit = DurationUnit.Millisecond;

                } else if (detail == FileDetail.TotalBitrate) {
                    unit = BitRateUnit.BitPerSecond;
                }

                return new PropertyItem(numericValue, unit);
            }
        }

    }

}
