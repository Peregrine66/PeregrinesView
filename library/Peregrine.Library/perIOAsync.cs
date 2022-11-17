using System;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading;

namespace Peregrine.Library
{
    /// <summary>
    /// A Collection of IO operations that perform in an async manner.
    /// </summary>
    public static class perIOAsync
    {
        /// <summary>
        /// Copy from one stream to another
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="bytesToCopy"></param>
        /// <returns></returns>
        public static Task<long> CopyStreamAsync(Stream source, Stream target, long bytesToCopy = -1)
        {
            return CopyStreamAsync(source, target, CancellationToken.None, bytesToCopy);
        }

        /// <summary>
        /// Copy from one stream to another, with the specified CancellationTokenSource
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="bytesToCopy"></param>
        /// <returns></returns>
        public static async Task<long> CopyStreamAsync(Stream source, Stream target, CancellationToken cancellationToken, long bytesToCopy = -1)
        {
            if (bytesToCopy < 0)
            {
                bytesToCopy = source.Length;
                source.Position = 0;
            }

            var bufferSize = (int)Math.Min(bytesToCopy, 32768);
            var buffer = new byte[bufferSize];
            var bytesRead = 1;
            long totalBytesRead = 0;

            while (bytesToCopy > 0 && bytesRead != 0)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var bytesToReadThisTime = Math.Min(bufferSize, (int)bytesToCopy);

                bytesRead = await source.ReadAsync(buffer, 0, bytesToReadThisTime, cancellationToken).ConfigureAwait(false);

                cancellationToken.ThrowIfCancellationRequested();

                await target.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);

                cancellationToken.ThrowIfCancellationRequested();

                totalBytesRead += bytesRead;
                bytesToCopy -= bytesRead;
            }

            await target.FlushAsync(cancellationToken).ConfigureAwait(false);

            return totalBytesRead;
        }

        /// <summary>
        /// Read the specified number of bytes from the source stream
        /// </summary>
        /// <param name="source"></param>
        /// <param name="bytesToCopy"></param>
        /// <returns></returns>
        public static Task<byte[]> ReadBytesFromStreamAsync(Stream source, long bytesToCopy = -1)
        {
            return ReadBytesFromStreamAsync(source, CancellationToken.None, bytesToCopy);
        }

        /// <summary>
        /// Read the specified number of bytes from the source stream, with the CancellationToken
        /// </summary>
        /// <param name="source"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="bytesToCopy"></param>
        /// <returns></returns>
        public static async Task<byte[]> ReadBytesFromStreamAsync(Stream source, CancellationToken cancellationToken, long bytesToCopy = -1)
        {
            using (var targetStream = new MemoryStream(Convert.ToInt32(source.Length)))
            {
                await CopyStreamAsync(source, targetStream, cancellationToken, bytesToCopy).ConfigureAwait(false);
                var result = targetStream.ToArray();
                return result;
            }
        }

        /// <summary>
        /// Open a file for reading in an async manner
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static FileStream OpenFileForAsyncReading(string fileName)
        {
            return OpenFileForAsyncReading(fileName, 4096);
        }

        /// <summary>
        /// Open a file for reading in an async manner, with the specified buffer size
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="bufferSize"></param>
        /// <returns></returns>
        public static FileStream OpenFileForAsyncReading(string fileName, int bufferSize)
        {
            return new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, useAsync: true);
        }

        /// <summary>
        /// Create a new file for writing in an async manner
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static FileStream CreateFileForAsyncWriting(string fileName)
        {
            return CreateFileForAsyncWriting(fileName, 4096);
        }

        /// <summary>
        /// Create a new file for writing in an async manner, with the specified buffer size
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="bufferSize"></param>
        /// <returns></returns>
        public static FileStream CreateFileForAsyncWriting(string fileName, int bufferSize)
        {
            return new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, useAsync: true);
        }

        /// <summary>
        /// Open an existing file, or create a new one if it doesn't exist, for writing in an async manner
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static FileStream OpenExistingFileForAsyncWriting(string fileName)
        {
            return OpenExistingFileForAsyncWriting(fileName, 4096);
        }

        /// <summary>
        /// Open an existing file, or create a new one if it doesn't exist, for writing in an async manner, with the specified buffer size
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="bufferSize"></param>
        /// <returns></returns>
        public static FileStream OpenExistingFileForAsyncWriting(string fileName, int bufferSize)
        {
            return new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.None, bufferSize, useAsync: true);
        }

        /// <summary>
        /// Read the contents of a file into a byte array
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Task<byte[]> ReadAllBytesFromFileAsync(string fileName)
        {
            return ReadAllBytesFromFileAsync(fileName, CancellationToken.None);
        }

        /// <summary>
        /// Read the contents of a file into a byte array, with the specified CancellationTokenSource
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<byte[]> ReadAllBytesFromFileAsync(string fileName, CancellationToken cancellationToken)
        {
            using (var sourceFileStream = OpenFileForAsyncReading(fileName))
            {
                var result = await ReadBytesFromStreamAsync(sourceFileStream, cancellationToken).ConfigureAwait(false);
                return result;
            }
        }

        /// <summary>
        /// Write a byte array to a file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static Task WriteBytesToFileAsync(string fileName, byte[] bytes)
        {
            return WriteBytesToFileAsync(fileName, bytes, CancellationToken.None);
        }

        /// <summary>
        /// Write a byte array to a file, with the specified CancellationTokenSource 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="bytes"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task WriteBytesToFileAsync(string fileName, byte[] bytes, CancellationToken cancellationToken)
        {
            using (var sourceStream = new MemoryStream(bytes))
            {
                using (var targetFileStream = CreateFileForAsyncWriting(fileName))
                {
                    await CopyStreamAsync(sourceStream, targetFileStream, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Read a UTF8 format file as a string
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Task<string> ReadUTF8TextFromFileAsync(string fileName)
        {
            return ReadUTF8TextFromFileAsync(fileName, CancellationToken.None);
        }

        /// <summary>
        /// Read a UTF8 format file as a string, with the specified CancellationToken
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<string> ReadUTF8TextFromFileAsync(string fileName, CancellationToken cancellationToken)
        {
            var bytes = await ReadAllBytesFromFileAsync(fileName, cancellationToken).ConfigureAwait(false);

            var result = bytes.ToUtf8String();

            return result;
        }

        /// <summary>
        /// Write a string as to a file in UTF8 format
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Task WriteUTF8TextToFileAsync(string fileName, string text)
        {
            return WriteUTF8TextToFileAsync(fileName, text, CancellationToken.None);
        }

        /// <summary>
        /// Write a string as to a file in UTF8 format, with the specified CancellationToken
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="text"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task WriteUTF8TextToFileAsync(string fileName, string text, CancellationToken cancellationToken)
        {
            var bytes = text.Utf8ToByteArray();
            return WriteBytesToFileAsync(fileName, bytes, cancellationToken);
        }

        /// <summary>
        /// Read a ASCII format file as a string
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Task<string> ReadASCIITextFromFileAsync(string fileName)
        {
            return ReadASCIITextFromFileAsync(fileName, CancellationToken.None);
        }

        /// <summary>
        /// Read a ASCII format file as a string, with the specified CancellationToken
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<string> ReadASCIITextFromFileAsync(string fileName, CancellationToken cancellationToken)
        {
            var bytes = await ReadAllBytesFromFileAsync(fileName, cancellationToken).ConfigureAwait(false);

            var result = bytes.ToAsciiString();

            return result;
        }

        /// <summary>
        /// Write a string to a file in ASCII format 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Task WriteASCIITextToFileAsync(string fileName, string text)
        {
            return WriteASCIITextToFileAsync(fileName, text, CancellationToken.None);
        }

        /// <summary>
        /// Write a string to a file in ASCII format, with the specified CancellationTokenSource
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="text"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task WriteASCIITextToFileAsync(string fileName, string text, CancellationToken cancellationToken)
        {
            var bytes = text.AsciiToByteArray();
            return WriteBytesToFileAsync(fileName, bytes, cancellationToken);
        }

        /// <summary>
        /// Download the contents of a Http url into a byte array
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Task<byte[]> ReadAllBytesFromUrlAsync(string url)
        {
            return SharedHttpClient.GetByteArrayAsync(url);

            // NOTE: Don't dispose of HttpClient instance after use.
        }

        private static HttpClient _sharedHttpClient;

        private static HttpClient SharedHttpClient
        {
            get
            {
                if (_sharedHttpClient == null)
                {
                    _sharedHttpClient = new HttpClient();
                    _sharedHttpClient.DefaultRequestHeaders.ConnectionClose = true;
                }

                return _sharedHttpClient;
            }
        }

        /// <summary>
        /// Read all text from a stream that was compressed with GZip
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static async Task<string> ReadAllTextFromGZipStreamAsync(this Stream stream)
        {
            using (var gzipStream = new GZipStream(stream, CompressionMode.Decompress))
            {
                var result = await gzipStream.ReadAllTextFromStreamAsync().ConfigureAwait(false);
                return result;
            }
        }

        /// <summary>
        /// Read all text from a stream that was compressed with Deflate
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static async Task<string> ReadAllTextFromDeflateStreamAsync(this Stream stream)
        {
            using (var deflateStream = new DeflateStream(stream, CompressionMode.Decompress))
            {
                var result = await deflateStream.ReadAllTextFromStreamAsync().ConfigureAwait(false);
                return result;
            }
        }

        /// <summary>
        /// Read all text from a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static async Task<string> ReadAllTextFromStreamAsync(this Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                var result = await reader.ReadToEndAsync().ConfigureAwait(false);
                return result;
            }
        }

        /// <summary>
        /// Copy a file in an Async manner
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="targetPath"></param>
        /// <returns></returns>
        public static async Task CopyFileAsync(string sourcePath, string targetPath)
        {
            var sourceFileInfo = new FileInfo(sourcePath);
            var targetFileInfo = new FileInfo(targetPath);

            if (!targetFileInfo.Exists || targetFileInfo.LastWriteTimeUtc != sourceFileInfo.LastWriteTimeUtc)
            {
                var targetFolder = Path.GetDirectoryName(targetPath);
                Directory.CreateDirectory(targetFolder ?? "");

                using (var sourceStream = OpenFileForAsyncReading(sourcePath))
                {
                    using (var targetStream = CreateFileForAsyncWriting(targetPath))
                    {
                        await CopyStreamAsync(sourceStream, targetStream).ConfigureAwait(false);
                        await targetStream.FlushAsync().ConfigureAwait(false);
                        targetStream.Close();
                    }

                    sourceStream.Close();
                }

                targetFileInfo.CreationTime = sourceFileInfo.CreationTime;
                targetFileInfo.LastWriteTime = sourceFileInfo.LastWriteTime;
                targetFileInfo.LastAccessTime = sourceFileInfo.LastAccessTime;
            }
        }
    }
}