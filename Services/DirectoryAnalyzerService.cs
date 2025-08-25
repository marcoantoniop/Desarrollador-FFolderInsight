using ResportesParaDeveloper.Models;
using ResportesParaDeveloper.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace ResportesParaDeveloper.Services
{
    public class DirectoryAnalyzerService
    {
        public event EventHandler<string> ProgressChanged;
        private int _processedItems = 0;
        private int _totalItems = 0;
        public AnalysisResult AnalyzeDirectory(string rootPath, FilterOptions filterOptions = null, CancellationToken token = default)
        {
            if (!Directory.Exists(rootPath))
                throw new DirectoryNotFoundException($"El directorio no existe: {rootPath}");

            var result = new AnalysisResult();
            var extensionDict = new Dictionary<string, ExtensionInfo>();
            filterOptions = filterOptions ?? new FilterOptions();

            _processedItems = 0;
            _totalItems = CountTotalItems(rootPath, filterOptions);

            OnProgressChanged($"Analizando directorio: {rootPath}");

            // MODIFICADO: Se pasa el token al método recursivo
            result.RootDirectory = AnalyzeDirectoryRecursive(rootPath, extensionDict, filterOptions, token);
            result.Extensions = extensionDict.Values.OrderBy(e => e.Extension).ToList();
            result.TotalFiles = CountFiles(result.RootDirectory);
            result.TotalDirectories = CountDirectories(result.RootDirectory) - 1; // No contar el root
            result.TotalSize = CalculateTotalSize(result.RootDirectory);

            OnProgressChanged("Análisis completado");

            return result;
        }

        private int CountTotalItems(string path, FilterOptions filterOptions)
        {
            int count = 0;
            try
            {
                // Contar archivos en el directorio actual
                count += Directory.GetFiles(path).Length;

                // Contar subdirectorios recursivamente
                foreach (var subDir in Directory.GetDirectories(path))
                {
                    var dirName = Path.GetFileName(subDir);
                    if (!filterOptions.IsFolderExcluded(dirName))
                    {
                        count++; // Contar el directorio
                        count += CountTotalItems(subDir, filterOptions);
                    }
                }
            }
            catch
            {
                // Ignorar errores de acceso
            }
            return count;
        }

        // MODIFICADO: La firma del método ahora acepta un CancellationToken
        private DirectoryAnalysisInfo AnalyzeDirectoryRecursive(string path, Dictionary<string, ExtensionInfo> extensionDict, FilterOptions filterOptions, CancellationToken token)
        {
            token.ThrowIfCancellationRequested(); 
            var dirInfo = new DirectoryAnalysisInfo
            {
                FullPath = path,
                Name = Path.GetFileName(path) ?? path
            };

            try
            {
                // Analizar archivos
                var files = Directory.GetFiles(path);
                OnProgressChanged($"Procesando {files.Length} archivos en: {GetShortPath(path)}");

                foreach (var filePath in files)
                {
                    token.ThrowIfCancellationRequested();
                    try
                    {
                        var fileInfo = new System.IO.FileInfo(filePath);

                        // Aplicar filtros básicos durante el análisis
                        if (filterOptions.ExcludeEmptyFiles && fileInfo.Length == 0)
                        {
                            _processedItems++;
                            continue;
                        }

                        var extension = fileInfo.Extension.ToLower();
                        if (filterOptions.ExcludeBinaryFiles && FileHelper.IsBinaryFile(extension))
                        {
                            _processedItems++;
                            continue;
                        }

                        var fileAnalysis = new FileAnalysisInfo
                        {
                            FullPath = filePath,
                            Name = fileInfo.Name,
                            Extension = extension,
                            Size = fileInfo.Length,
                            CreatedDate = fileInfo.CreationTime,
                            ModifiedDate = fileInfo.LastWriteTime,
                            DirectoryPath = path
                        };

                        dirInfo.Files.Add(fileAnalysis);

                        // Actualizar estadísticas de extensiones
                        var extensionKey = string.IsNullOrEmpty(extension) ? "[sin extensión]" : extension;

                        if (!extensionDict.ContainsKey(extensionKey))
                        {
                            extensionDict[extensionKey] = new ExtensionInfo
                            {
                                Extension = extensionKey,
                                FileCount = 0,
                                TotalSize = 0,
                                IsSelected = false
                            };
                        }

                        extensionDict[extensionKey].FileCount++;
                        extensionDict[extensionKey].TotalSize += fileAnalysis.Size;

                        _processedItems++;

                        // Actualizar progreso cada 100 archivos
                        if (_processedItems % 100 == 0)
                        {
                            var percentage = _totalItems > 0 ? (_processedItems * 100) / _totalItems : 0;
                            OnProgressChanged($"Procesados {_processedItems}/{_totalItems} elementos ({percentage}%)");
                        }
                    }
                    catch (Exception ex)
                    {
                        OnProgressChanged($"Error al acceder al archivo {filePath}: {ex.Message}");
                        _processedItems++;
                    }
                }

                // Analizar subdirectorios
                var directories = Directory.GetDirectories(path);
                foreach (var subDirPath in directories)
                {
                    token.ThrowIfCancellationRequested(); 
                    try
                    {
                        var dirName = Path.GetFileName(subDirPath);

                        // Aplicar filtros de exclusión de carpetas
                        if (filterOptions.IsFolderExcluded(dirName))
                        {
                            OnProgressChanged($"Omitiendo carpeta excluida: {dirName}");
                            continue;
                        }

                        OnProgressChanged($"Analizando subdirectorio: {GetShortPath(subDirPath)}");
                        // MODIFICADO: Se pasa el token en la llamada recursiva
                        var subDir = AnalyzeDirectoryRecursive(subDirPath, extensionDict, filterOptions, token);
                        dirInfo.SubDirectories.Add(subDir);
                    }
                    catch (Exception ex)
                    {
                        OnProgressChanged($"Error al acceder al directorio {subDirPath}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                OnProgressChanged($"Error al analizar directorio {path}: {ex.Message}");
            }

            return dirInfo;
        }

        private string GetShortPath(string fullPath, int maxLength = 60)
        {
            if (fullPath.Length <= maxLength)
                return fullPath;

            var parts = fullPath.Split(Path.DirectorySeparatorChar);
            if (parts.Length <= 2)
                return fullPath;

            // Mostrar la primera parte y las últimas partes
            var result = parts[0] + Path.DirectorySeparatorChar + "..." + Path.DirectorySeparatorChar;
            var remainingLength = maxLength - result.Length;

            for (int i = parts.Length - 1; i >= 1; i--)
            {
                var part = parts[i];
                if (result.Length + part.Length + 1 <= maxLength)
                {
                    result += Path.DirectorySeparatorChar + part;
                }
                else
                {
                    break;
                }
            }

            return result;
        }

        public List<FileAnalysisInfo> GetFilteredFiles(DirectoryAnalysisInfo rootDir, List<string> selectedExtensions)
        {
            var filteredFiles = new List<FileAnalysisInfo>();
            GetFilteredFilesRecursive(rootDir, selectedExtensions, filteredFiles);
            return filteredFiles;
        }

        public List<FileAnalysisInfo> GetFilteredFiles(DirectoryAnalysisInfo rootDir, FilterOptions filterOptions)
        {
            var allFiles = new List<FileAnalysisInfo>();
            GetAllFilesRecursive(rootDir, allFiles);

            return allFiles.Where(filterOptions.MatchesFilter).ToList();
        }

        private void GetAllFilesRecursive(DirectoryAnalysisInfo dir, List<FileAnalysisInfo> result)
        {
            result.AddRange(dir.Files);

            foreach (var subDir in dir.SubDirectories)
            {
                GetAllFilesRecursive(subDir, result);
            }
        }

        private void GetFilteredFilesRecursive(DirectoryAnalysisInfo dir, List<string> selectedExtensions, List<FileAnalysisInfo> result)
        {
            // Agregar archivos que coincidan con las extensiones seleccionadas
            foreach (var file in dir.Files)
            {
                if (selectedExtensions.Contains(file.Extension) ||
                    (string.IsNullOrEmpty(file.Extension) && selectedExtensions.Contains("[sin extensión]")))
                {
                    result.Add(file);
                }
            }

            // Procesar subdirectorios
            foreach (var subDir in dir.SubDirectories)
            {
                GetFilteredFilesRecursive(subDir, selectedExtensions, result);
            }
        }

        public string ReadFileContent(string filePath)
        {
            try
            {
                // Verificar si el archivo existe
                if (!File.Exists(filePath))
                {
                    return "[El archivo no existe o no se puede acceder]";
                }

                var fileInfo = new FileInfo(filePath);

                // Verificar tamaño del archivo (limitar a 10MB para evitar problemas de memoria)
                const long maxSize = 10 * 1024 * 1024; // 10MB
                if (fileInfo.Length > maxSize)
                {
                    return $"[Archivo demasiado grande para mostrar - Tamaño: {FileHelper.FormatFileSize(fileInfo.Length)}]";
                }

                // Detectar si el archivo es binario
                if (!FileHelper.IsTextFile(filePath))
                {
                    return "[Archivo binario - contenido no mostrado]";
                }

                // Intentar leer con diferentes codificaciones
                return ReadFileWithEncoding(filePath);
            }
            catch (UnauthorizedAccessException)
            {
                return "[Acceso denegado al archivo]";
            }
            catch (Exception ex)
            {
                return $"[Error al leer el archivo: {ex.Message}]";
            }
        }

        private string ReadFileWithEncoding(string filePath)
        {
            // Lista de codificaciones a probar
            var encodings = new[]
            {
                Encoding.UTF8,
                Encoding.Default,
                Encoding.ASCII,
                Encoding.Unicode,
                Encoding.UTF32
            };

            foreach (var encoding in encodings)
            {
                try
                {
                    var content = File.ReadAllText(filePath, encoding);

                    // Verificar si la decodificación fue exitosa
                    // (sin demasiados caracteres de reemplazo)
                    var replacementCount = content.Count(c => c == '\uFFFD');
                    if (replacementCount < content.Length * 0.05) // Menos del 5% son caracteres de reemplazo
                    {
                        return content;
                    }
                }
                catch
                {
                    continue;
                }
            }

            // Si ninguna codificación funcionó bien, usar UTF-8 como predeterminada
            try
            {
                return File.ReadAllText(filePath, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                return $"[Error de codificación: {ex.Message}]";
            }
        }

        private int CountFiles(DirectoryAnalysisInfo dir)
        {
            int count = dir.Files.Count;
            foreach (var subDir in dir.SubDirectories)
            {
                count += CountFiles(subDir);
            }
            return count;
        }

        private int CountDirectories(DirectoryAnalysisInfo dir)
        {
            int count = 1; // Contar el directorio actual
            foreach (var subDir in dir.SubDirectories)
            {
                count += CountDirectories(subDir);
            }
            return count;
        }

        private long CalculateTotalSize(DirectoryAnalysisInfo dir)
        {
            long size = dir.Files.Sum(f => f.Size);
            foreach (var subDir in dir.SubDirectories)
            {
                size += CalculateTotalSize(subDir);
            }
            return size;
        }

        /// <summary>
        /// Genera estadísticas adicionales del análisis
        /// </summary>
        public AnalysisStatistics GenerateStatistics(AnalysisResult analysisResult)
        {
            var allFiles = new List<FileAnalysisInfo>();
            GetAllFilesRecursive(analysisResult.RootDirectory, allFiles);

            var stats = new AnalysisStatistics
            {
                TotalFiles = allFiles.Count,
                TotalDirectories = analysisResult.TotalDirectories,
                TotalSize = analysisResult.TotalSize,
                AverageFileSize = allFiles.Count > 0 ? (long)allFiles.Average(f => f.Size) : 0,
                LargestFile = allFiles.OrderByDescending(f => f.Size).FirstOrDefault(),
                SmallestFile = allFiles.Where(f => f.Size > 0).OrderBy(f => f.Size).FirstOrDefault(),
                NewestFile = allFiles.OrderByDescending(f => f.ModifiedDate).FirstOrDefault(),
                OldestFile = allFiles.OrderBy(f => f.ModifiedDate).FirstOrDefault(),
                EmptyFiles = allFiles.Count(f => f.Size == 0),
                BinaryFiles = allFiles.Count(f => FileHelper.IsBinaryFile(f.Extension)),
                TextFiles = allFiles.Count(f => FileHelper.IsCodeFile(f.Extension)),
                ExtensionDistribution = analysisResult.Extensions.ToDictionary(e => e.Extension, e => e.FileCount)
            };

            return stats;
        }

        protected virtual void OnProgressChanged(string message)
        {
            ProgressChanged?.Invoke(this, message);
        }
    }

    /// <summary>
    /// Estadísticas adicionales del análisis
    /// </summary>
    public class AnalysisStatistics
    {
        public int TotalFiles { get; set; }
        public int TotalDirectories { get; set; }
        public long TotalSize { get; set; }
        public long AverageFileSize { get; set; }
        public FileAnalysisInfo LargestFile { get; set; }
        public FileAnalysisInfo SmallestFile { get; set; }
        public FileAnalysisInfo NewestFile { get; set; }
        public FileAnalysisInfo OldestFile { get; set; }
        public int EmptyFiles { get; set; }
        public int BinaryFiles { get; set; }
        public int TextFiles { get; set; }
        public Dictionary<string, int> ExtensionDistribution { get; set; }
    }
}