using System;
using System.Collections.Generic;


namespace ResportesParaDeveloper.Models
{
    /// <summary>
    /// Opciones de filtrado para el análisis de archivos
    /// </summary>
    public class FilterOptions
    {
        public List<string> SelectedExtensions { get; set; } = new List<string>();

        // Filtros por tamaño
        public bool EnableSizeFilter { get; set; } = false;
        public long MinSizeBytes { get; set; } = 0;
        public long MaxSizeBytes { get; set; } = long.MaxValue;

        // Filtros por fecha
        public bool EnableDateFilter { get; set; } = false;
        public DateTime? MinCreatedDate { get; set; }
        public DateTime? MaxCreatedDate { get; set; }
        public DateTime? MinModifiedDate { get; set; }
        public DateTime? MaxModifiedDate { get; set; }

        // Filtros por nombre
        public bool EnableNameFilter { get; set; } = false;
        public string NamePattern { get; set; } = string.Empty;
        public bool UseRegex { get; set; } = false;
        public bool CaseSensitive { get; set; } = false;

        // Filtros de exclusión
        public List<string> ExcludedFolders { get; set; } = new List<string>
        {
            ".git", ".vs", "node_modules", "bin", "obj", "packages", ".svn"
        };

        public bool ExcludeBinaryFiles { get; set; } = true;
        public bool ExcludeEmptyFiles { get; set; } = false;

        /// <summary>
        /// Valida si un archivo cumple con todos los filtros establecidos
        /// </summary>
        public bool MatchesFilter(FileAnalysisInfo file)
        {
            // Filtro por extensión
            if (SelectedExtensions.Count > 0)
            {
                var extension = string.IsNullOrEmpty(file.Extension) ? "[sin extensión]" : file.Extension;
                if (!SelectedExtensions.Contains(extension))
                    return false;
            }

            // Filtro por tamaño
            if (EnableSizeFilter)
            {
                if (file.Size < MinSizeBytes || file.Size > MaxSizeBytes)
                    return false;
            }

            // Filtro por fecha de creación
            if (EnableDateFilter)
            {
                if (MinCreatedDate.HasValue && file.CreatedDate < MinCreatedDate.Value)
                    return false;
                if (MaxCreatedDate.HasValue && file.CreatedDate > MaxCreatedDate.Value)
                    return false;
                if (MinModifiedDate.HasValue && file.ModifiedDate < MinModifiedDate.Value)
                    return false;
                if (MaxModifiedDate.HasValue && file.ModifiedDate > MaxModifiedDate.Value)
                    return false;
            }

            // Filtro por nombre
            if (EnableNameFilter && !string.IsNullOrWhiteSpace(NamePattern))
            {
                if (UseRegex)
                {
                    try
                    {
                        var regex = new System.Text.RegularExpressions.Regex(
                            NamePattern,
                            CaseSensitive ?
                                System.Text.RegularExpressions.RegexOptions.None :
                                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                        if (!regex.IsMatch(file.Name))
                            return false;
                    }
                    catch
                    {
                        // Si hay error en regex, usar búsqueda simple
                        var comparison = CaseSensitive ?
                            StringComparison.Ordinal :
                            StringComparison.OrdinalIgnoreCase;
                        if (file.Name.IndexOf(NamePattern, comparison) == -1)
                            return false;
                    }
                }
                else
                {
                    var comparison = CaseSensitive ?
                        StringComparison.Ordinal :
                        StringComparison.OrdinalIgnoreCase;
                    if (file.Name.IndexOf(NamePattern, comparison) == -1)
                        return false;
                }
            }

            // Filtro de archivos vacíos
            if (ExcludeEmptyFiles && file.Size == 0)
                return false;

            return true;
        }

        /// <summary>
        /// Valida si una carpeta debe ser excluida
        /// </summary>
        public bool IsFolderExcluded(string folderName)
        {
            return ExcludedFolders.Contains(folderName, StringComparer.OrdinalIgnoreCase);
        }
    }
}
