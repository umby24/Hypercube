using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace Hypercube_Classic.Libraries {
    class GZip {
        /// <summary>
        /// GZip Compresses (Deflate method) the given data.
        /// </summary>
        /// <param name="Data"></param>
        /// <returns>Compressed version of the input data array.</returns>
        public static byte[] Compress(byte[] Data) {
            byte[] CompressedData;

            using (MemoryStream mem = new MemoryStream()) {
                using (GZipStream zip = new GZipStream(mem, CompressionMode.Compress)) {
                    zip.Write(Data, 0, Data.Length);
                }
                CompressedData = mem.ToArray();
            }

            return CompressedData;
        }
    }
}
