using System.Collections.Generic;

namespace ResportesParaDeveloper.Models
{
    public class DirectoryAnalysisInfo
    {
        public string FullPath { get; set; }
        public string Name { get; set; }
        public List<DirectoryAnalysisInfo> SubDirectories { get; set; }
        public List<FileAnalysisInfo> Files { get; set; }
        public int TotalFiles { get; set; }
        public int TotalDirectories { get; set; }

        public DirectoryAnalysisInfo()
        {
            SubDirectories = new List<DirectoryAnalysisInfo>();
            Files = new List<FileAnalysisInfo>();
        }
    }
}
