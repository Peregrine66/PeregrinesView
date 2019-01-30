using System;
using System.Threading.Tasks;

namespace Peregrine.Library
{
    using System.IO;
    using System.Net.Http;
    using System.Threading;

    /// <summary>
    /// A Collection of IO operations that perform in an async manner.
    /// </summary>
    public static class perIOAsync
    {
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
            return new FileStream(
                fileName,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                bufferSize,
                useAsync: true);
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
            return new FileStream(
                fileName,
                FileMode.Append,
                FileAccess.Write,
                FileShare.None,
                bufferSize,
                useAsync: true);
        }

        /// <summary>
        /// Read the contents of a file into a byte array
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Task<perTaskResult<byte[]>> ReadAllBytesFromFileAsync(string fileName)
        {
            return ReadAllBytesFromFileAsync(fileName, 0);
        }

        /// <summary>
        /// Read the contents of a file into a byte array, with the specified timeout
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="timeoutSeconds"></param>
        /// <returns></returns>
        public static Task<perTaskResult<byte[]>> ReadAllBytesFromFileAsync(string fileName, int timeoutSeconds)
        {
            return ReadAllBytesFromFileAsync(fileName, timeoutSeconds, new CancellationTokenSource());
        }

        /// <summary>
        /// Read the contents of a file into a byte array, with the specified CancellationTokenSource
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="tokenSource"></param>
        /// <returns></returns>
        public static Task<perTaskResult<byte[]>> ReadAllBytesFromFileAsync(
            string fileName,
            CancellationTokenSource tokenSource)
        {
            return ReadAllBytesFromFileAsync(fileName, 0, tokenSource);
        }

        /// <summary>
        /// Read the contents of a file into a byte array, with the specified timeout and CancellationTokenSource
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="timeoutSeconds"></param>
        /// <param name="tokenSource"></param>
        /// <returns></returns>
        public static async Task<perTaskResult<byte[]>> ReadAllBytesFromFileAsync(
            string fileName,
            int timeoutSeconds,
            CancellationTokenSource tokenSource)
        {
            using (var file = OpenFileForAsyncReading(fileName))
            {
                var bytes = new byte[file.Length];

                var response = await file.ReadAsync(bytes, 0, (int)file.Length, tokenSource.Token)
                                   .RunTaskWithTimeoutAsync(timeoutSeconds, tokenSource).ConfigureAwait(false);

                var result = new perTaskResult<byte[]>
                                 {
                                     Status = response.Status,
                                     ErrorMessage = response.ErrorMessage,
                                     Exception = response.Exception
                                 };

                if (response.Status == perTaskStatus.CompletedOk)
                    result.Data = bytes;

                return result;
            }
        }

        /// <summary>
        /// Write a byte array to a file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static Task<perTaskResult> WriteBytesToFileAsync(string fileName, byte[] bytes)
        {
            return WriteBytesToFileAsync(fileName, bytes, 0);
        }

        /// <summary>
        /// Write a byte array to a file, with the specified timeout
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="bytes"></param>
        /// <param name="timeoutSeconds"></param>
        /// <returns></returns>
        public static Task<perTaskResult> WriteBytesToFileAsync(string fileName, byte[] bytes, int timeoutSeconds)
        {
            return WriteBytesToFileAsync(fileName, bytes, timeoutSeconds, new CancellationTokenSource());
        }

        /// <summary>
        /// Write a byte array to a file, with the specified CancellationTokenSource 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="bytes"></param>
        /// <param name="tokenSource"></param>
        /// <returns></returns>
        public static Task<perTaskResult> WriteBytesToFileAsync(
            string fileName,
            byte[] bytes,
            CancellationTokenSource tokenSource)
        {
            return WriteBytesToFileAsync(fileName, bytes, 0, tokenSource);
        }

        /// <summary>
        /// Write a byte array to a file, with the specified timeout and CancellationTokenSource
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="bytes"></param>
        /// <param name="timeoutSeconds"></param>
        /// <param name="tokenSource"></param>
        /// <returns></returns>
        public static async Task<perTaskResult> WriteBytesToFileAsync(
            string fileName,
            byte[] bytes,
            int timeoutSeconds,
            CancellationTokenSource tokenSource)
        {
            using (var file = CreateFileForAsyncWriting(fileName))
            {
                return await file.WriteAsync(bytes, 0, bytes.Length, tokenSource.Token)
                           .RunTaskWithTimeoutAsync(timeoutSeconds, tokenSource).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Read a UTF8 format file as a string
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Task<perTaskResult<string>> ReadUtf8TextFromFileAsync(string fileName)
        {
            return ReadUtf8TextFromFileAsync(fileName, 0);
        }

        /// <summary>
        /// Read a UTF8 format file as a string, with the specified timeout
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="timeoutSeconds"></param>
        /// <returns></returns>
        public static Task<perTaskResult<string>> ReadUtf8TextFromFileAsync(string fileName, int timeoutSeconds)
        {
            return ReadUtf8TextFromFileAsync(fileName, timeoutSeconds, new CancellationTokenSource());
        }

        /// <summary>
        /// Read a UTF8 format file as a string, with the specified CancellationTokenSource
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="tokenSource"></param>
        /// <returns></returns>
        public static Task<perTaskResult<string>> ReadUtf8TextFromFileAsync(
            string fileName,
            CancellationTokenSource tokenSource)
        {
            return ReadUtf8TextFromFileAsync(fileName, 0, tokenSource);
        }

        /// <summary>
        /// Read a UTF8 format file as a string, with the specified timeout and CancellationTokenSource
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="timeoutSeconds"></param>
        /// <param name="tokenSource"></param>
        /// <returns></returns>
        public static async Task<perTaskResult<string>> ReadUtf8TextFromFileAsync(
            string fileName,
            int timeoutSeconds,
            CancellationTokenSource tokenSource)
        {
            var response = await ReadAllBytesFromFileAsync(fileName, timeoutSeconds, tokenSource).ConfigureAwait(false);

            return new perTaskResult<string>
                       {
                           ErrorMessage = response.ErrorMessage,
                           Exception = response.Exception,
                           Status = response.Status,
                           Data = response.Status == perTaskStatus.CompletedOk
                                      ? response.Data.ToUtf8String()
                                      : null
                       };
        }

        /// <summary>
        /// Write a string as to a file in UTF8 format
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Task<perTaskResult> WriteUtf8TextToFileAsync(string fileName, string text)
        {
            return WriteUtf8TextToFileAsync(fileName, text, 0);
        }

        /// <summary>
        /// Write a string as to a file in UTF8 format, with the specified timeout
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="text"></param>
        /// <param name="timeoutSeconds"></param>
        /// <returns></returns>
        public static Task<perTaskResult> WriteUtf8TextToFileAsync(string fileName, string text, int timeoutSeconds)
        {
            return WriteUtf8TextToFileAsync(fileName, text, timeoutSeconds, new CancellationTokenSource());
        }

        /// <summary>
        /// Write a string as to a file in UTF8 format, with the specified CancellationTokenSource
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="text"></param>
        /// <param name="tokenSource"></param>
        /// <returns></returns>
        public static Task<perTaskResult> WriteUtf8TextToFileAsync(
            string fileName,
            string text,
            CancellationTokenSource tokenSource)
        {
            return WriteUtf8TextToFileAsync(fileName, text, 0, tokenSource);
        }

        /// <summary>
        /// Write a string as to a file in UTF8 format, with the specified timeout and CancellationTokenSource
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="text"></param>
        /// <param name="timeoutSeconds"></param>
        /// <param name="tokenSource"></param>
        /// <returns></returns>
        public static Task<perTaskResult> WriteUtf8TextToFileAsync(
            string fileName,
            string text,
            int timeoutSeconds,
            CancellationTokenSource tokenSource)
        {
            var bytes = text.Utf8ToByteArray();
            return WriteBytesToFileAsync(fileName, bytes, timeoutSeconds, tokenSource);
        }

        /// <summary>
        /// Read a ASCII format file as a string
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Task<perTaskResult<string>> ReadAsciiTextFromFileAsync(string fileName)
        {
            return ReadAsciiTextFromFileAsync(fileName, 0);
        }

        /// <summary>
        /// Read a ASCII format file as a string, with the specified timeout
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="timeoutSeconds"></param>
        /// <returns></returns>
        public static Task<perTaskResult<string>> ReadAsciiTextFromFileAsync(string fileName, int timeoutSeconds)
        {
            return ReadAsciiTextFromFileAsync(fileName, timeoutSeconds, new CancellationTokenSource());
        }

        /// <summary>
        /// Read a ASCII format file as a string, with the specified CancellationTokenSource
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="tokenSource"></param>
        /// <returns></returns>
        public static Task<perTaskResult<string>> ReadAsciiTextFromFileAsync(
            string fileName,
            CancellationTokenSource tokenSource)
        {
            return ReadAsciiTextFromFileAsync(fileName, 0, tokenSource);
        }

        /// <summary>
        /// Read a ASCII format file as a string, with the specified timeout and CancellationTokenSource
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="timeoutSeconds"></param>
        /// <param name="tokenSource"></param>
        /// <returns></returns>
        public static async Task<perTaskResult<string>> ReadAsciiTextFromFileAsync(
            string fileName,
            int timeoutSeconds,
            CancellationTokenSource tokenSource)
        {
            var response = await ReadAllBytesFromFileAsync(fileName, timeoutSeconds, tokenSource).ConfigureAwait(false);

            return new perTaskResult<string>
                       {
                           ErrorMessage = response.ErrorMessage,
                           Exception = response.Exception,
                           Status = response.Status,
                           Data = response.Status == perTaskStatus.CompletedOk
                                      ? response.Data.ToAsciiString()
                                      : null
                       };
        }

        /// <summary>
        /// Write a string to a file in ASCII format 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Task<perTaskResult> WriteAsciiTextToFileAsync(string fileName, string text)
        {
            return WriteAsciiTextToFileAsync(fileName, text, 0);
        }

        /// <summary>
        /// Write a string to a file in ASCII format, with the specified timeout
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="text"></param>
        /// <param name="timeoutSeconds"></param>
        /// <returns></returns>
        public static Task<perTaskResult> WriteAsciiTextToFileAsync(string fileName, string text, int timeoutSeconds)
        {
            return WriteAsciiTextToFileAsync(fileName, text, timeoutSeconds, new CancellationTokenSource());
        }

        /// <summary>
        /// Write a string to a file in ASCII format, with the specified CancellationTokenSource
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="text"></param>
        /// <param name="tokenSource"></param>
        /// <returns></returns>
        public static Task<perTaskResult> WriteAsciiTextToFileAsync(
            string fileName,
            string text,
            CancellationTokenSource tokenSource)
        {
            return WriteAsciiTextToFileAsync(fileName, text, 0, tokenSource);
        }

        /// <summary>
        /// Write a string to a file in ASCII format, with the specified timeout and CancellationTokenSource
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="text"></param>
        /// <param name="timeoutSeconds"></param>
        /// <param name="tokenSource"></param>
        /// <returns></returns>
        public static Task<perTaskResult> WriteAsciiTextToFileAsync(
            string fileName,
            string text,
            int timeoutSeconds,
            CancellationTokenSource tokenSource)
        {
            var bytes = text.AsciiToByteArray();
            return WriteBytesToFileAsync(fileName, bytes, timeoutSeconds, tokenSource);
        }

        /// <summary>
        /// Copy from one stream to another
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="bytesToCopy"></param>
        /// <returns></returns>
        public static Task<perTaskResult<long>> CopyStreamAsync(Stream source, Stream target, long bytesToCopy = -1)
        {
            return CopyStreamAsync(source, target, 0, bytesToCopy);
        }

        /// <summary>
        /// Copy from one stream to another, with the specified timeout
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="timeoutSeconds"></param>
        /// <param name="bytesToCopy"></param>
        /// <returns></returns>
        public static Task<perTaskResult<long>> CopyStreamAsync(
            Stream source,
            Stream target,
            int timeoutSeconds,
            long bytesToCopy = -1)
        {
            return CopyStreamAsync(source, target, timeoutSeconds, new CancellationTokenSource(), bytesToCopy);
        }

        /// <summary>
        /// Copy from one stream to another, with the specified CancellationTokenSource
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="tokenSource"></param>
        /// <param name="bytesToCopy"></param>
        /// <returns></returns>
        public static Task<perTaskResult<long>> CopyStreamAsync(
            Stream source,
            Stream target,
            CancellationTokenSource tokenSource,
            long bytesToCopy = -1)
        {
            return CopyStreamAsync(source, target, 0, tokenSource, bytesToCopy);
        }

        /// <summary>
        /// Copy from one stream to another, with the specified timeout and CancellationTokenSource
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="timeoutSeconds"></param>
        /// <param name="tokenSource"></param>
        /// <param name="bytesToCopy"></param>
        /// <returns></returns>
        public static async Task<perTaskResult<long>> CopyStreamAsync(
            Stream source,
            Stream target,
            int timeoutSeconds,
            CancellationTokenSource tokenSource,
            long bytesToCopy = -1)
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
                var bytesToReadThisTime = Math.Min(bufferSize, (int)bytesToCopy);

                var response1 = await source.ReadAsync(buffer, 0, bytesToReadThisTime)
                                    .GetTaskResultWithTimeoutAsync(timeoutSeconds, tokenSource).ConfigureAwait(false);

                if (response1.Status != perTaskStatus.CompletedOk)
                    return new perTaskResult<long>
                               {
                                   ErrorMessage = response1.ErrorMessage,
                                   Exception = response1.Exception,
                                   Status = response1.Status
                               };

                bytesRead = response1.Data;

                var response2 = await target.WriteAsync(buffer, 0, bytesRead)
                                    .RunTaskWithTimeoutAsync(timeoutSeconds, tokenSource).ConfigureAwait(false);

                if (response2.Status != perTaskStatus.CompletedOk)
                    return new perTaskResult<long>
                               {
                                   ErrorMessage = response2.ErrorMessage,
                                   Exception = response2.Exception,
                                   Status = response2.Status
                               };

                totalBytesRead += bytesRead;
                bytesToCopy -= bytesRead;
            }

            return new perTaskResult<long> { Status = perTaskStatus.CompletedOk, Data = totalBytesRead };
        }

        /// <summary>
        /// Download the contents of a Http url into a byte array
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Task<perTaskResult<byte[]>> ReadAllBytesFromUrlAsync(string url)
        {
            return ReadAllBytesFromUrlAsync(url, 0);
        }

        /// <summary>
        /// Download the contents of a Http url into a byte array, with the specified timeout
        /// </summary>
        /// <param name="url"></param>
        /// <param name="timeoutSeconds"></param>
        /// <returns></returns>
        public static Task<perTaskResult<byte[]>> ReadAllBytesFromUrlAsync(string url, int timeoutSeconds)
        {
            return ReadAllBytesFromUrlAsync(url, timeoutSeconds, new CancellationTokenSource());
        }

        /// <summary>
        /// Download the contents of a Http url into a byte array, with the specified CancellationTokenSource
        /// </summary>
        /// <param name="url"></param>
        /// <param name="tokenSource"></param>
        /// <returns></returns>
        public static Task<perTaskResult<byte[]>> ReadAllBytesFromUrlAsync(
            string url,
            CancellationTokenSource tokenSource)
        {
            return ReadAllBytesFromUrlAsync(url, 0, tokenSource);
        }

        /// <summary>
        /// Download the contents of a Http url into a byte array, with the specified timeout and CancellationTokenSource
        /// </summary>
        /// <param name="url"></param>
        /// <param name="timeoutSeconds"></param>
        /// <param name="tokenSource"></param>
        /// <returns></returns>
        public static async Task<perTaskResult<byte[]>> ReadAllBytesFromUrlAsync(
            string url,
            int timeoutSeconds,
            CancellationTokenSource tokenSource)
        {
            using (var httpClient = new HttpClient())
            {
                return await httpClient.GetByteArrayAsync(url)
                           .GetTaskResultWithTimeoutAsync(timeoutSeconds, tokenSource).ConfigureAwait(false);
            }
        }
    }
}