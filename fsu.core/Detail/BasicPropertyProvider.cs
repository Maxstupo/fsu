﻿using HeyRed.Mime;
using Maxstupo.Fsu.Core.Processor;
using Maxstupo.Fsu.Core.Utility;
using System.IO;

namespace Maxstupo.Fsu.Core.Detail {

    public class BasicPropertyProvider : IFilePropertyProvider {

        public void Begin() { }

        public void End() { }

        public Property GetFileProperty(ProcessorItem item, string propertyName) {
            string filepath = item.Value;

            switch (propertyName) {
                case "name":
                    return new Property(Path.GetFileNameWithoutExtension(filepath));

                case "ext":
                case "extension":
                    return new Property(Path.GetExtension(filepath).SafeSubstring(1));

                case "filename":
                    return new Property(Path.GetFileName(filepath));

                case "dir":
                case "directory":
                    return new Property(Path.GetDirectoryName(filepath));

                case "value":
                case "path":
                case "filepath":
                    return new Property(item.Value);

                case "ivalue":
                case "initalvalue":
                case "initalpath":
                case "ipath":
                case "initalfilepath":
                case "ifilepath":
                    return new Property(item.InitalValue);

                case "origin":
                    return item.Origin == null ? null : new Property(item.Origin);

                case "relpath":
                    if (item.Origin == null || !item.Value.StartsWith(item.Origin))
                        return null;
                    return new Property(item.Value.Substring(item.Origin.Length + 1));

                case "size":
                    if (!File.Exists(filepath))
                        return null;
                    return new Property(new FileInfo(filepath).Length);

                case "mime":
                    return new Property(MimeTypesMap.GetMimeType(filepath));

                default:
                    return null;
            }
        }

    }

}
