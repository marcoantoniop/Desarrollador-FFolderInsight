using System.Collections.Generic;

namespace ResportesParaDeveloper.Models
{
    public class AnalysisResult
    {
        public DirectoryAnalysisInfo RootDirectory { get; set; }
        public List<ExtensionInfo> Extensions { get; set; }
        public int TotalFiles { get; set; }
        public int TotalDirectories { get; set; }
        public long TotalSize { get; set; }

        public AnalysisResult()
        {
            Extensions = new List<ExtensionInfo>();
        }
    }
}
