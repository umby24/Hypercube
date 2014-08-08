using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace Hypercube.Libraries {
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

        /// <summary>
        /// GZip Compresses a file at the given file path.
        /// </summary>
        /// <param name="Filepath">The path to the file to compress with gzip.</param>
        public static void CompressFile(string Filepath) {
            if (!File.Exists(Filepath))
                return;

            const int ChunkSize = 65536;

            try {
                using (var FS = new FileStream(Filepath, FileMode.Open)) {
                    using (var GS = new GZipStream(new FileStream("Temp.gz", FileMode.Create), CompressionMode.Compress)) {
                        var Buffer = new byte[ChunkSize];

                        while (true) {
                            var BytesRead = FS.Read(Buffer, 0, ChunkSize);

                            if (BytesRead == 0) break;

                            GS.Write(Buffer, 0, BytesRead);
                        }
                        Buffer = null;
                    }
                }

                File.Delete(Filepath);
                File.Move("Temp.gz", Filepath);
            } catch {
                GC.Collect();
                return;
            }
        }

        /// <summary>
        /// GZip Decompresses a file at the given file path.
        /// </summary>
        /// <param name="Filepath">The path to the file to decompress</param>
        public static void DecompressFile(string Filepath) {
            if (!File.Exists(Filepath))
                return;

            const int ChunkSize = 65536;

            try {
                using (var FS = new FileStream("Temp.hch", FileMode.Create)) {
                    using (var GS = new GZipStream(new FileStream(Filepath, FileMode.Open), CompressionMode.Decompress)) {
                        var Buffer = new byte[ChunkSize];

                        while (true) {
                            var BytesRead = GS.Read(Buffer, 0, ChunkSize);

                            if (BytesRead == 0) break;

                            FS.Write(Buffer, 0, BytesRead);
                        }
                        Buffer = null;
                    }
                }

                File.Delete(Filepath);
                File.Move("Temp.hch", Filepath);
            } catch {
                GC.Collect();
                return;
            }
        }
    }
}
