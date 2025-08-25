using ResportesParaDeveloper.Extensions;
using System;

namespace ResportesParaDeveloper.Models
{
    public class FileAnalysisInfo
    {
        public string FullPath { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public long Size { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string DirectoryPath { get; set; }

        public string FormattedSize
        {
            get
            {
                string[] sizes = { "B", "KB", "MB", "GB" };
                double len = Size;
                int order = 0;
                while (len >= 1024 && order < sizes.Length - 1)
                {
                    order++;
                    len = len / 1024;
                }
                //return $"{len:0.##} {sizes[order]}";
                return this.Size.FormatFileSize();
            }
        }
    }
}
