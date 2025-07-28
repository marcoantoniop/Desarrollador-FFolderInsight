using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace ResportesParaDeveloper.Utils
{
    public static class FileHelper
    {
        private static readonly HashSet<string> CodeExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".cs", ".vb", ".cpp", ".c", ".h", ".hpp", ".java", ".js", ".ts", ".py", ".php",
            ".rb", ".go", ".rs", ".swift", ".kt", ".scala", ".pl", ".sh", ".bat", ".ps1",
            ".html", ".htm", ".css", ".scss", ".sass", ".less", ".xml", ".json", ".yaml",
            ".yml", ".sql", ".md", ".txt", ".ini", ".cfg", ".conf", ".log"
        };

        private static readonly HashSet<string> BinaryExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".exe", ".dll", ".bin", ".zip", ".rar", ".7z", ".tar", ".gz", ".bz2",
            ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx",
            ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".ico", ".tiff", ".svg",
            ".mp3", ".mp4", ".avi", ".mov", ".wmv", ".flv", ".mkv", ".wav", ".flac",
            ".obj", ".lib", ".a", ".so", ".dylib", ".pdb", ".mdb"
        };

        public static bool IsCodeFile(string extension)
        {
            return CodeExtensions.Contains(extension ?? string.Empty);
        }

        public static bool IsBinaryFile(string extension)
        {
            return BinaryExtensions.Contains(extension ?? string.Empty);
        }

        public static string GetFileTypeDescription(string extension)
        {
            if (string.IsNullOrEmpty(extension))
                return "Sin extensión";

            return extension.ToLower() switch
            {
                ".cs" => "C# Source",
                ".vb" => "Visual Basic",
                ".cpp" or ".c" => "C/C++ Source",
                ".h" or ".hpp" => "C/C++ Header",
                ".java" => "Java Source",
                ".js" => "JavaScript",
                ".ts" => "TypeScript",
                ".py" => "Python",
                ".php" => "PHP",
                ".html" or ".htm" => "HTML",
                ".css" => "CSS",
                ".xml" => "XML",
                ".json" => "JSON",
                ".txt" => "Text File",
                ".md" => "Markdown",
                ".sql" => "SQL Script",
                ".bat" => "Batch File",
                ".ps1" => "PowerShell",
                ".exe" => "Executable",
                ".dll" => "Dynamic Library",
                ".zip" => "ZIP Archive",
                ".pdf" => "PDF Document",
                ".jpg" or ".jpeg" => "JPEG Image",
                ".png" => "PNG Image",
                ".gif" => "GIF Image",
                _ => extension.ToUpper().TrimStart('.') + " File"
            };
        }

        public static bool IsTextFile(string filePath)
        {
            try
            {
                var extension = Path.GetExtension(filePath);

                // Si es una extensión conocida de código, es texto
                if (IsCodeFile(extension))
                    return true;

                // Si es una extensión binaria conocida, no es texto
                if (IsBinaryFile(extension))
                    return false;

                // Para archivos sin extensión o extensiones desconocidas, 
                // verificar los primeros bytes
                return IsTextFileByContent(filePath);
            }
            catch
            {
                return false;
            }
        }

        private static bool IsTextFileByContent(string filePath)
        {
            try
            {
                const int bytesToCheck = 1024;
                var buffer = new byte[bytesToCheck];

                using (var fileStream = File.OpenRead(filePath))
                {
                    var bytesRead = fileStream.Read(buffer, 0, bytesToCheck);

                    // Verificar si contiene bytes nulos (indicativo de archivo binario)
                    for (int i = 0; i < bytesRead; i++)
                    {
                        if (buffer[i] == 0)
                            return false;
                    }

                    // Contar caracteres no imprimibles
                    int nonPrintableCount = 0;
                    for (int i = 0; i < bytesRead; i++)
                    {
                        byte b = buffer[i];
                        if (b < 9 || (b > 13 && b < 32) || b == 127)
                        {
                            nonPrintableCount++;
                        }
                    }

                    // Si más del 10% son caracteres no imprimibles, probablemente es binario
                    return (double)nonPrintableCount / bytesRead < 0.1;
                }
            }
            catch
            {
                return false;
            }
        }

        public static string FormatFileSize(long bytes)
        {
            const long KB = 1024;
            const long MB = KB * 1024;
            const long GB = MB * 1024;
            const long TB = GB * 1024;

            return bytes switch
            {
                < KB => $"{bytes} B",
                < MB => $"{bytes / (double)KB:F2} KB",
                < GB => $"{bytes / (double)MB:F2} MB",
                < TB => $"{bytes / (double)GB:F2} GB",
                _ => $"{bytes / (double)TB:F2} TB"
            };
        }

        public static string SanitizeFileName(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var sanitized = string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));

            // Limitar longitud
            if (sanitized.Length > 200)
                sanitized = sanitized.Substring(0, 200);

            return sanitized;
        }
    }
}
