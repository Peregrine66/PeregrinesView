using System.Collections.Generic;
using System.IO;

namespace Peregrine.Library
{
    /// <summary>
    /// IO operations that can't be done in an async manner
    /// </summary>
    public static class PerIO
    {
        private static IReadOnlyCollection<FileInfo> ListFilesInternal(string rootPath, string subPath, string fileSpec, bool iterateSubFolders)
        {
            var result = new List<FileInfo>();

            var searchPath = Path.Combine(rootPath, subPath);

            if (Directory.Exists(searchPath))
            {
                var sourceDirectoryInfo = new DirectoryInfo(searchPath);
                if (iterateSubFolders)
                    foreach (var subFolderDirectoryInfo in sourceDirectoryInfo.GetDirectories())
                        result.AddRange(ListFilesInternal(searchPath, subFolderDirectoryInfo.Name, fileSpec, true));

                result.AddRange(sourceDirectoryInfo.GetFiles(fileSpec, SearchOption.TopDirectoryOnly));
            }

            return result.AsReadOnly();
        }

        public static IReadOnlyCollection<FileInfo> ListFiles(string rootPath, string fileSpec, bool iterateSubFolders)
        { return ListFilesInternal(rootPath, "", fileSpec, iterateSubFolders); }

        public static IReadOnlyCollection<FileInfo> ListFiles(string rootPath, bool iterateSubFolders)
        { return ListFiles(rootPath, "*.*", iterateSubFolders); }

        public static IReadOnlyCollection<FileInfo> ListFiles(string rootPath, string fileSpec)
        { return ListFiles(rootPath, fileSpec, true); }

        public static IReadOnlyCollection<FileInfo> ListFiles(string rootPath)
        { return ListFiles(rootPath, true); }
    }
}
