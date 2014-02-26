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

            using (var mem = new MemoryStream()) {
                using (var zip = new GZipStream(mem, CompressionMode.Compress)) {
                    zip.Write(Data, 0, Data.Length);
                }
                CompressedData = mem.ToArray();
            }

            return CompressedData;
        }

        public static void CompressFile(string Filepath) {
            if (!File.Exists(Filepath))
                return;

            //using (var stream = new FileStream(Filepath, FileMode.Open)) {
            //    using (var zip = new GZipStream(stream, CompressionMode.Compress)) {
            //        zip.Write(Temp, 0, Temp.Length);
            //        Temp = null;
            //    }
            //}

        }

        public static void DecompressFile(string Filepath) {
            if (!File.Exists(Filepath))
                return;

            //using (var stream = new FileStream(Filepath, FileMode.Open)) {
            //    using (var zip = new GZipStream(stream, CompressionMode.Decompress)) {
            //        var Temp = new byte[stream.Length];
            //        stream.Read(Temp, 0, Temp.Length);

            //        zip.Write(Temp, 0, Temp.Length);
            //        Temp = null;
            //    }
            //}
        }
    }
}
