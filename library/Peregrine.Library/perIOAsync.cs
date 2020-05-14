using System;
using System.Threading.Tasks;
using System.IO;
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
            return new FileStream(fileName, FileMode.CreateNew, FileAccess.Write, FileShare.None, bufferSize, useAsync: true);
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
        public static Task<perAsyncFunctionResponse<byte[]>> ReadAllBytesFromFileAsync(string fileName)
        {
            return ReadAllBytesFromFileAsync(fileName, perTimeSpanHelper.Forever);
        }

        /// <summary>
        /// Read the contents of a file into a byte array, with the specified timeout
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static Task<perAsyncFunctionResponse<byte[]>> ReadAllBytesFromFileAsync(string fileName, TimeSpan timeout)
        {
            return ReadAllBytesFromFileAsync(fileName, timeout, new CancellationTokenSource());
        }

        /// <summary>
        /// Read the contents of a file into a byte array, with the specified CancellationTokenSource
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="tokenSource"></param>
        /// <returns></returns>
        public static Task<perAsyncFunctionResponse<byte[]>> ReadAllBytesFromFileAsync(string fileName, CancellationTokenSource tokenSource)
        {
            return ReadAllBytesFromFileAsync(fileName, perTimeSpanHelper.Forever, tokenSource);
        }

        /// <summary>
        /// Read the contents of a file into a byte array, with the specified timeout and CancellationTokenSource
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="timeout"></param>
        /// <param name="tokenSource"></param>
        /// <returns></returns>
        public static async Task<perAsyncFunctionResponse<byte[]>> ReadAllBytesFromFileAsync(string fileName, TimeSpan timeout, CancellationTokenSource tokenSource)
        {
            using (var file = OpenFileForAsyncReading(fileName))
            {
                var bytes = new byte[file.Length];

                var response = await file.ReadAsync(bytes, 0, (int) file.Length, tokenSource.Token)
                    .EvaluateFunctionWithTimeoutAsync(timeout, tokenSource).ConfigureAwait(false);

                var result = response.CloneAsFunctionResponse<byte[]>();

                if (response.IsCompletedOk)
                    result.Data = bytes;

                return result;
            }
        }

        /// <summary>
        /// Read the contents of a file into a byte array, without any wrapper around the task.
        /// </summary>
        /// <param name="fileName"></param>
        public static Task<byte[]> ReadAllBytesFromFileRawAsync(string fileName)
        {
            return ReadAllBytesFromFileRawAsync(fileName, CancellationToken.None);
        }

        /// <summary>
        /// Read the contents of a file into a byte array, without any wrapper around the task.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="token"></param>
        public static async Task<byte[]> ReadAllBytesFromFileRawAsync(string fileName, CancellationToken token)
        {
            byte[] bytes;

            using (var file = OpenFileForAsyncReading(fileName))
            {
                bytes = new byte[file.Length];
                await file.ReadAsync(bytes, 0, (int)file.Length, token).ConfigureAwait(false);
            }

            return bytes;
        }

        /// <summary>
        /// Write a byte array to a file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static Task<perAsyncActionResponse> WriteBytesToFileAsync(string fileName, byte[] bytes)
        {
            return WriteBytesToFileAsync(fileName, bytes, perTimeSpanHelper.Forever);
        }

        /// <summary>
        /// Write a byte array to a file, with the specified timeout
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="bytes"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static Task<perAsyncActionResponse> WriteBytesToFileAsync(string fileName, byte[] bytes, TimeSpan timeout)
        {
            return WriteBytesToFileAsync(fileName, bytes, timeout, new CancellationTokenSource());
        }

        /// <summary>
        /// Write a byte array to a file, with the specified CancellationTokenSource 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="bytes"></param>
        /// <param name="tokenSource"></param>
        /// <returns></returns>
        public static Task<perAsyncActionResponse> WriteBytesToFileAsync(string fileName, byte[] bytes, CancellationTokenSource tokenSource)
        {
            return WriteBytesToFileAsync(fileName, bytes, perTimeSpanHelper.Forever, tokenSource);
        }

        /// <summary>
        /// Write a byte array to a file, with the specified timeout and CancellationTokenSource
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="bytes"></param>
        /// <param name="timeout"></param>
        /// <param name="tokenSource"></param>
        /// <returns></returns>
        public static async Task<perAsyncActionResponse> WriteBytesToFileAsync(string fileName, byte[] bytes, TimeSpan timeout, CancellationTokenSource tokenSource)
        {
            using (var file = CreateFileForAsyncWriting(fileName))
            {
                return await file.WriteAsync(bytes, 0, bytes.Length, tokenSource.Token)
                    .ExecuteActionWithTimeoutAsync(timeout, tokenSource).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Read a UTF8 format file as a string
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Task<perAsyncFunctionResponse<string>> ReadUtf8TextFromFileAsync(string fileName)
        {
            return ReadUtf8TextFromFileAsync(fileName, perTimeSpanHelper.Forever);
        }

        /// <summary>
        /// Read a UTF8 format file as a string, with the specified timeout
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static Task<perAsyncFunctionResponse<string>> ReadUtf8TextFromFileAsync(string fileName, TimeSpan timeout)
        {
            return ReadUtf8TextFromFileAsync(fileName, timeout, new CancellationTokenSource());
        }

        /// <summary>
        /// Read a UTF8 format file as a string, with the specified CancellationTokenSource
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="tokenSource"></param>
        /// <returns></returns>
        public static Task<perAsyncFunctionResponse<string>> ReadUtf8TextFromFileAsync(string fileName, CancellationTokenSource tokenSource)
        {
            return ReadUtf8TextFromFileAsync(fileName, perTimeSpanHelper.Forever, tokenSource);
        }

        /// <summary>
        /// Read a UTF8 format file as a string, with the specified timeout and CancellationTokenSource
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="timeout"></param>
        /// <param name="tokenSource"></param>
        /// <returns></returns>
        public static async Task<perAsyncFunctionResponse<string>> ReadUtf8TextFromFileAsync(string fileName, TimeSpan timeout, CancellationTokenSource tokenSource)
        {
            var response = await ReadAllBytesFromFileAsync(fileName, timeout, tokenSource).ConfigureAwait(false);

            var result = response.CloneAsFunctionResponse<string>();

            if (response.IsCompletedOk)
            {
                result.Data = response.Data.ToUtf8String();
            }

            return result;
        }

        /// <summary>
        /// Write a string as to a file in UTF8 format
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Task<perAsyncActionResponse> WriteUtf8TextToFileAsync(string fileName, string text)
        {
            return WriteUtf8TextToFileAsync(fileName, text, perTimeSpanHelper.Forever);
        }

        /// <summary>
        /// Write a string as to a file in UTF8 format, with the specified timeout
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="text"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static Task<perAsyncActionResponse> WriteUtf8TextToFileAsync(string fileName, string text, TimeSpan timeout)
        {
            return WriteUtf8TextToFileAsync(fileName, text, timeout, new CancellationTokenSource());
        }

        /// <summary>
        /// Write a string as to a file in UTF8 format, with the specified CancellationTokenSource
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="text"></param>
        /// <param name="tokenSource"></param>
        /// <returns></returns>
        public static Task<perAsyncActionResponse> WriteUtf8TextToFileAsync(string fileName, string text, CancellationTokenSource tokenSource)
        {
            return WriteUtf8TextToFileAsync(fileName, text, perTimeSpanHelper.Forever, tokenSource);
        }

        /// <summary>
        /// Write a string as to a file in UTF8 format, with the specified timeout and CancellationTokenSource
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="text"></param>
        /// <param name="timeout"></param>
        /// <param name="tokenSource"></param>
        /// <returns></returns>
        public static Task<perAsyncActionResponse> WriteUtf8TextToFileAsync(string fileName, string text, TimeSpan timeout, CancellationTokenSource tokenSource)
        {
            var bytes = text.Utf8ToByteArray();
            return WriteBytesToFileAsync(fileName, bytes, timeout, tokenSource);
        }

        /// <summary>
        /// Read a ASCII format file as a string
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Task<perAsyncFunctionResponse<string>> ReadAsciiTextFromFileAsync(string fileName)
        {
            return ReadAsciiTextFromFileAsync(fileName, perTimeSpanHelper.Forever);
        }

        /// <summary>
        /// Read a ASCII format file as a string, with the specified timeout
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static Task<perAsyncFunctionResponse<string>> ReadAsciiTextFromFileAsync(string fileName, TimeSpan timeout)
        {
            return ReadAsciiTextFromFileAsync(fileName, timeout, new CancellationTokenSource());
        }

        /// <summary>
        /// Read a ASCII format file as a string, with the specified CancellationTokenSource
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="tokenSource"></param>
        /// <returns></returns>
        public static Task<perAsyncFunctionResponse<string>> ReadAsciiTextFromFileAsync(string fileName, CancellationTokenSource tokenSource)
        {
            return ReadAsciiTextFromFileAsync(fileName, perTimeSpanHelper.Forever, tokenSource);
        }

        /// <summary>
        /// Read a ASCII format file as a string, with the specified timeout and CancellationTokenSource
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="timeout"></param>
        /// <param name="tokenSource"></param>
        /// <returns></returns>
        public static async Task<perAsyncFunctionResponse<string>> ReadAsciiTextFromFileAsync(string fileName, TimeSpan timeout,
            CancellationTokenSource tokenSource)
        {
            var response = await ReadAllBytesFromFileAsync(fileName, timeout, tokenSource).ConfigureAwait(false);

            var result = response.CloneAsFunctionResponse<string>();

            if (response.IsCompletedOk)
            {
                result.Data = response.Data.ToAsciiString();
            }

            return result;
        }

        /// <summary>
        /// Write a string to a file in ASCII format 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Task<perAsyncActionResponse> WriteAsciiTextToFileAsync(string fileName, string text)
        {
            return WriteAsciiTextToFileAsync(fileName, text, perTimeSpanHelper.Forever);
        }

        /// <summary>
        /// Write a string to a file in ASCII format, with the specified timeout
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="text"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static Task<perAsyncActionResponse> WriteAsciiTextToFileAsync(string fileName, string text, TimeSpan timeout)
        {
            return WriteAsciiTextToFileAsync(fileName, text, timeout, new CancellationTokenSource());
        }

        /// <summary>
        /// Write a string to a file in ASCII format, with the specified CancellationTokenSource
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="text"></param>
        /// <param name="tokenSource"></param>
        /// <returns></returns>
        public static Task<perAsyncActionResponse> WriteAsciiTextToFileAsync(string fileName, string text, CancellationTokenSource tokenSource)
        {
            return WriteAsciiTextToFileAsync(fileName, text, perTimeSpanHelper.Forever, tokenSource);
        }

        /// <summary>
        /// Write a string to a file in ASCII format, with the specified timeout and CancellationTokenSource
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="text"></param>
        /// <param name="timeout"></param>
        /// <param name="tokenSource"></param>
        /// <returns></returns>
        public static Task<perAsyncActionResponse> WriteAsciiTextToFileAsync(string fileName, string text, TimeSpan timeout, CancellationTokenSource tokenSource)
        {
            var bytes = text.AsciiToByteArray();
            return WriteBytesToFileAsync(fileName, bytes, timeout, tokenSource);
        }

        /// <summary>
        /// Copy from one stream to another
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="bytesToCopy"></param>
        /// <returns></returns>
        public static Task<perAsyncFunctionResponse<long>> CopyStreamAsync(Stream source, Stream target, long bytesToCopy = -1)
        {
            return CopyStreamAsync(source, target, perTimeSpanHelper.Forever, bytesToCopy);
        }

        /// <summary>
        /// Copy from one stream to another, with the specified timeout
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="timeout"></param>
        /// <param name="bytesToCopy"></param>
        /// <returns></returns>
        public static Task<perAsyncFunctionResponse<long>> CopyStreamAsync(Stream source, Stream target, TimeSpan timeout, long bytesToCopy = -1)
        {
            return CopyStreamAsync(source, target, timeout, new CancellationTokenSource(), bytesToCopy);
        }

        /// <summary>
        /// Copy from one stream to another, with the specified CancellationTokenSource
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="tokenSource"></param>
        /// <param name="bytesToCopy"></param>
        /// <returns></returns>
        public static Task<perAsyncFunctionResponse<long>> CopyStreamAsync(Stream source, Stream target, CancellationTokenSource tokenSource, long bytesToCopy = -1)
        {
            return CopyStreamAsync(source, target, perTimeSpanHelper.Forever, tokenSource, bytesToCopy);
        }

        /// <summary>
        /// Copy from one stream to another, with the specified timeout and CancellationTokenSource
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="timeout"></param>
        /// <param name="tokenSource"></param>
        /// <param name="bytesToCopy"></param>
        /// <returns></returns>
        public static async Task<perAsyncFunctionResponse<long>> CopyStreamAsync(Stream source, Stream target, TimeSpan timeout, CancellationTokenSource tokenSource, long bytesToCopy = -1)
        {
            if (bytesToCopy < 0)
            {
                bytesToCopy = source.Length;
                source.Position = 0;
            }

            var bufferSize = (int) Math.Min(bytesToCopy, 32768);
            var buffer = new byte[bufferSize];
            var bytesRead = 1;
            long totalBytesRead = 0;

            while (bytesToCopy > 0 && bytesRead != 0)
            {
                var bytesToReadThisTime = Math.Min(bufferSize, (int) bytesToCopy);

                var response1 = await source.ReadAsync(buffer, 0, bytesToReadThisTime)
                    .EvaluateFunctionWithTimeoutAsync(timeout, tokenSource).ConfigureAwait(false);

                if (!response1.IsCompletedOk)
                {
                    return response1.CloneAsFunctionResponse<long>();
                }

                bytesRead = response1.Data;

                var response2 = await target.WriteAsync(buffer, 0, bytesRead)
                    .ExecuteActionWithTimeoutAsync(timeout, tokenSource).ConfigureAwait(false);

                if (!response2.IsCompletedOk)
                {
                    return response2.CloneAsFunctionResponse<long>();
                }

                totalBytesRead += bytesRead;
                bytesToCopy -= bytesRead;
            }

            return new perAsyncFunctionResponse<long>(perTaskStatus.CompletedOk)
            {
                Data = totalBytesRead
            };
        }

        /// <summary>
        /// Download the contents of a Http url into a byte array
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Task<perAsyncFunctionResponse<byte[]>> ReadAllBytesFromUrlAsync(string url)
        {
            return ReadAllBytesFromUrlAsync(url, perTimeSpanHelper.Forever);
        }

        /// <summary>
        /// Download the contents of a Http url into a byte array, with the specified timeout
        /// </summary>
        /// <param name="url"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static Task<perAsyncFunctionResponse<byte[]>> ReadAllBytesFromUrlAsync(string url, TimeSpan timeout)
        {
            return ReadAllBytesFromUrlAsync(url, timeout, new CancellationTokenSource());
        }

        /// <summary>
        /// Download the contents of a Http url into a byte array, with the specified CancellationTokenSource
        /// </summary>
        /// <param name="url"></param>
        /// <param name="tokenSource"></param>
        /// <returns></returns>
        public static Task<perAsyncFunctionResponse<byte[]>> ReadAllBytesFromUrlAsync(string url, CancellationTokenSource tokenSource)
        {
            return ReadAllBytesFromUrlAsync(url, perTimeSpanHelper.Forever, tokenSource);
        }

        /// <summary>
        /// Download the contents of a Http url into a byte array, with the specified timeout and CancellationTokenSource
        /// </summary>
        /// <param name="url"></param>
        /// <param name="timeout"></param>
        /// <param name="tokenSource"></param>
        /// <returns></returns>
        public static async Task<perAsyncFunctionResponse<byte[]>> ReadAllBytesFromUrlAsync(string url, TimeSpan timeout, CancellationTokenSource tokenSource)
        {
            return await ReadAllBytesFromUrlRawAsync(url)
                .EvaluateFunctionWithTimeoutAsync(timeout, tokenSource).ConfigureAwait(false);
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
        /// Download the contents of a Http url into a byte array, without any wrapper around the task.
        /// </summary>
        /// <param name="url"></param>
        public static Task<byte[]> ReadAllBytesFromUrlRawAsync(string url)
        {
            return SharedHttpClient.GetByteArrayAsync(url);

            // NOTE: Don't dispose of HttpClient instance after use.
        }
    }
}