using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NablaUtils
{
    public class ResourceUtils
    {
        public static byte[] GetEmbeddedResourceAsBytes(string resourcePath)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using Stream stream = assembly.GetManifestResourceStream(resourcePath);
            using BinaryReader reader = new BinaryReader(stream);
            long length = stream.Length;
            return reader.ReadBytes((int)length);
        }
    }
}
