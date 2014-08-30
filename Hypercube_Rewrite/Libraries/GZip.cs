using System;
using System.IO;
using System.IO.Compression;

namespace Hypercube.Libraries {
    class GZip {
        /// <summary>
        /// GZip Compresses (Deflate method) the given data.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Compressed version of the input data array.</returns>
        public static byte[] Compress(byte[] data) {
            byte[] compressedData;

            using (var mem = new MemoryStream()) {
                using (var zip = new GZipStream(mem, CompressionMode.Compress)) {
                    zip.Write(data, 0, data.Length);
                }
                compressedData = mem.ToArray();
            }

            return compressedData;
        }

        /// <summary>
        /// GZip Compresses a file at the given file path.
        /// </summary>
        /// <param name="filepath">The path to the file to compress with gzip.</param>
        public static void CompressFile(string filepath) {
            if (!File.Exists(filepath))
                return;

            const int chunkSize = 65536;

            try {
                using (var fs = new FileStream(filepath, FileMode.Open)) {
                    using (var gs = new GZipStream(new FileStream("Temp.gz", FileMode.Create), CompressionMode.Compress)) {
                        var buffer = new byte[chunkSize];

                        while (true) {
                            var bytesRead = fs.Read(buffer, 0, chunkSize);

                            if (bytesRead == 0) break;

                            gs.Write(buffer, 0, bytesRead);
                        }
                    }
                }

                File.Delete(filepath);
                File.Move("Temp.gz", filepath);
            } catch {
                GC.Collect();
            }
        }

        /// <summary>
        /// GZip Decompresses a file at the given file path.
        /// </summary>
        /// <param name="filepath">The path to the file to decompress</param>
        public static void DecompressFile(string filepath) {
            if (!File.Exists(filepath))
                return;

            const int chunkSize = 65536;

            try {
                using (var fs = new FileStream("Temp.hch", FileMode.Create)) {
                    using (var gs = new GZipStream(new FileStream(filepath, FileMode.Open), CompressionMode.Decompress)) {
                        var buffer = new byte[chunkSize];

                        while (true) {
                            var bytesRead = gs.Read(buffer, 0, chunkSize);

                            if (bytesRead == 0) break;

                            fs.Write(buffer, 0, bytesRead);
                        }
                    }
                }

                File.Delete(filepath);
                File.Move("Temp.hch", filepath);
            } catch {
                GC.Collect();
            }
        }
    }
}
