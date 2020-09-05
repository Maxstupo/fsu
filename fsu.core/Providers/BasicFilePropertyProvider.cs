namespace Maxstupo.Fsu.Core.Providers {

    using System.IO;
    using Maxstupo.Fsu.Core.Detail;
    using Maxstupo.Fsu.Core.Processor;
    using Maxstupo.Fsu.Core.Utility;

    /// <summary>
    /// Provides basic metadata for ProcessorItems representing a file or directory.
    /// </summary>
    public sealed class BasicFilePropertyProvider : IPropertyProvider {

        public void Begin() { }

        public void End() { }

        public PropertyItem GetProperty(IPropertyProvider propertyProvider, ProcessorItem item, string propertyName) {
            string filepath = item.Value;

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
                    return new PropertyItem(item.Value);

                case "ivalue":
                    return new PropertyItem(item.InitialValue);

                case "relpath":
                    PropertyItem originItem = item.GetProperty(propertyProvider, "origin");
                    if (originItem.IsNumeric || originItem == null)
                        return null;

                    string origin = originItem.ValueText;
                    if (!item.Value.StartsWith(origin))
                        return null;
                    return new PropertyItem(item.Value.Substring(origin.Length + 1));

                case "size":
                case "filesize":
                    if (!File.Exists(item.Value))
                        return null;
                    long size = new FileInfo(item.Value).Length;
                    return new PropertyItem(size, null);

                default:
                    return null;
            }

        }

    }

}