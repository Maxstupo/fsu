namespace Maxstupo.Fsu.Providers {
    
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;
    using HeyRed.Mime;
    using Maxstupo.Fsu.Core.Detail;
    using Maxstupo.Fsu.Core.Processor;
    using Maxstupo.Fsu.Core.Utility;
    using UnitsNet.Units;

    public class BasicFilePropertyProvider : IPropertyProvider {


        public void Begin() {

        }

        public void End() {
            foreach (HashAlgorithm ha in algorithms.Values)
                ha.Dispose();
            algorithms.Clear();
        }

        public PropertyItem GetProperty(IPropertyProvider providerList, ProcessorItem item, string propertyName) {
            string filepath = item.Value;
            HashAlgorithm ha;
            switch (propertyName) {
                case "name":
                    return new PropertyItem(Path.GetFileNameWithoutExtension(filepath));

                case "ext":
                case "extension":
                    return new PropertyItem(Path.GetExtension(filepath).SafeSubstring(1));

                case "filename":
                    return new PropertyItem(Path.GetFileName(filepath));

                case "dir":
                case "directory":
                    return new PropertyItem(Path.GetDirectoryName(filepath));

                case "value":
                case "path":
                case "filepath":
                    return new PropertyItem(item.Value);

                case "ivalue":
                case "initalvalue":
                case "initalpath":
                case "ipath":
                case "initalfilepath":
                case "ifilepath":
                    return new PropertyItem(item.InitialValue);

                case "origin":
                    return item.Origin == null ? null : new PropertyItem(item.Origin);

                case "relpath":
                    if (item.Origin == null || !item.Value.StartsWith(item.Origin))
                        return null;
                    return new PropertyItem(item.Value.Substring(item.Origin.Length + 1));

                case "size":
                    if (!File.Exists(item.Value))
                        break;
                    long size = new FileInfo(item.Value).Length;
                    return new PropertyItem(size) { Unit = InformationUnit.Byte };

                case "megapixels": // Composite property
                    PropertyItem width = item.GetProperty(providerList, "width");
                    PropertyItem height = item.GetProperty(providerList, "height");
                    if (!width.IsNumeric || !height.IsNumeric)
                        break;
                    return new PropertyItem(width.ValueNumber * height.ValueNumber / 1000000.0);
                case "mime":
                    return new PropertyItem(MimeTypesMap.GetMimeType(filepath));

                case "md5":
                    ha = TryGetOrAddAlgorithm(propertyName, () => MD5.Create());
                    return new PropertyItem(ComputeHash(ha, filepath));
                case "sha256":
                    ha = TryGetOrAddAlgorithm(propertyName, () => SHA256.Create());
                    return new PropertyItem(ComputeHash(ha, filepath));
                case "sha1":
                    ha = TryGetOrAddAlgorithm(propertyName, () => SHA1.Create());
                    return new PropertyItem(ComputeHash(ha, filepath));
            }
            return null;
        }

        private readonly Dictionary<string, HashAlgorithm> algorithms = new Dictionary<string, HashAlgorithm>();

        private HashAlgorithm TryGetOrAddAlgorithm(string key, Func<HashAlgorithm> creation) {
            if (!algorithms.TryGetValue(key, out HashAlgorithm sha1)) {
                sha1 = creation();
                algorithms[key] = sha1;
            }
            return sha1;
        }

        private static string ComputeHash(HashAlgorithm a, string filepath) {

            using (Stream sr = File.OpenRead(filepath)) {
                byte[] hash = a.ComputeHash(sr);
                return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
            }

        }

    }
}
