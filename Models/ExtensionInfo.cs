using System.Collections.Generic;

namespace ResportesParaDeveloper.Models
{
    public class ExtensionInfo
    {
        public string Extension { get; set; }
        public int FileCount { get; set; }
        public long TotalSize { get; set; }
        public bool IsSelected { get; set; }

        public string FormattedTotalSize
        {
            get
            {
                string[] sizes = { "B", "KB", "MB", "GB" };
                double len = TotalSize;
                int order = 0;
                while (len >= 1024 && order < sizes.Length - 1)
                {
                    order++;
                    len = len / 1024;
                }
                return $"{len:0.##} {sizes[order]}";
            }
        }
    }
}
