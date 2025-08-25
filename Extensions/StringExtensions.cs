using System;

namespace ResportesParaDeveloper.Extensions
{
    public static class StringExtensions
    {

        public static string Truncate(this string value, int maxLength, string suffix = "...")
        {
            if (string.IsNullOrEmpty(value))
                return value;

            if (value.Length <= maxLength)
                return value;

            return value.Substring(0, maxLength - suffix.Length) + suffix;
        }

        public static string RemoveInvalidPathChars(this string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            var invalidChars = System.IO.Path.GetInvalidPathChars();
            foreach (var invalidChar in invalidChars)
            {
                path = path.Replace(invalidChar, '_');
            }

            return path;
        }

        public static bool ContainsIgnoreCase(this string source, string value)
        {
            return source?.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>
        /// Formatea un tamaño de archivo en bytes a una representación legible
        /// </summary>
        public static string FormatFileSize(this long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        /// <summary>
        /// Formatea una fecha de manera amigable
        /// </summary>
        public static string ToFriendlyDateString(this DateTime date)
        {
            var timeSpan = DateTime.Now - date;

            if (timeSpan.TotalDays < 1)
                return "Hoy";
            if (timeSpan.TotalDays < 2)
                return "Ayer";
            if (timeSpan.TotalDays < 7)
                return $"Hace {(int)timeSpan.TotalDays} días";
            if (timeSpan.TotalDays < 30)
                return $"Hace {(int)(timeSpan.TotalDays / 7)} semanas";
            if (timeSpan.TotalDays < 365)
                return $"Hace {(int)(timeSpan.TotalDays / 30)} meses";

            return $"Hace {(int)(timeSpan.TotalDays / 365)} años";
        }
    }

}
