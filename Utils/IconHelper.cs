using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResportesParaDeveloper.Utils
{
    /// <summary>
    /// Helper para manejo de iconos en la aplicación
    /// </summary>
    public static class IconHelper
    {
        private static ImageList _fileImageList;
        private static Dictionary<string, int> _extensionIconMap;

        /// <summary>
        /// Inicializa el sistema de iconos
        /// </summary>
        public static ImageList InitializeImageList()
        {
            if (_fileImageList != null)
                return _fileImageList;

            _fileImageList = new ImageList
            {
                ImageSize = new Size(16, 16),
                ColorDepth = ColorDepth.Depth32Bit
            };

            _extensionIconMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            // Agregar iconos básicos
            AddBasicIcons();

            return _fileImageList;
        }

        private static void AddBasicIcons()
        {
            // Icono de carpeta (índice 0)
            _fileImageList.Images.Add("folder", CreateFolderIcon());

            // Icono de archivo genérico (índice 1)
            _fileImageList.Images.Add("file", CreateFileIcon(Color.Gray));

            // Iconos específicos por extensión
            var iconDefinitions = new Dictionary<string[], Color>
            {
                // Código fuente - Verde
                { new[] { ".cs", ".vb" }, Color.Green },
                { new[] { ".cpp", ".c", ".h", ".hpp" }, Color.Blue },
                { new[] { ".java" }, Color.Orange },
                { new[] { ".js", ".ts" }, Color.Yellow },
                { new[] { ".py" }, Color.Blue },
                { new[] { ".php" }, Color.Purple },
                { new[] { ".rb" }, Color.Red },
                { new[] { ".go" }, Color.Cyan },
                
                // Web - Naranja
                { new[] { ".html", ".htm" }, Color.Orange },
                { new[] { ".css", ".scss", ".sass" }, Color.Blue },
                { new[] { ".xml", ".xsl" }, Color.Green },
                { new[] { ".json", ".yaml", ".yml" }, Color.DarkGreen },
                
                // Documentos - Azul
                { new[] { ".txt", ".md" }, Color.Blue },
                { new[] { ".pdf" }, Color.Red },
                { new[] { ".doc", ".docx" }, Color.Blue },
                { new[] { ".xls", ".xlsx" }, Color.Green },
                
                // Imágenes - Violeta
                { new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" }, Color.Purple },
                
                // Ejecutables - Rojo
                { new[] { ".exe", ".dll" }, Color.Red },
                
                // Comprimidos - Marrón
                { new[] { ".zip", ".rar", ".7z" }, Color.Brown },
                
                // Scripts - Verde oscuro
                { new[] { ".bat", ".cmd", ".ps1", ".sh" }, Color.DarkGreen },
                
                // Configuración - Gris oscuro
                { new[] { ".ini", ".cfg", ".conf", ".config" }, Color.DarkGray },
                
                // Logs - Amarillo oscuro
                { new[] { ".log" }, Color.DarkGoldenrod },
                
                // SQL - Azul oscuro
                { new[] { ".sql" }, Color.DarkBlue }
            };

            int index = 2; // Empezar después de folder y file genérico

            foreach (var definition in iconDefinitions)
            {
                var extensions = definition.Key;
                var color = definition.Value;

                var icon = CreateFileIcon(color);
                var iconKey = $"ext_{index}";
                _fileImageList.Images.Add(iconKey, icon);

                foreach (var ext in extensions)
                {
                    _extensionIconMap[ext] = index;
                }

                index++;
            }
        }

        /// <summary>
        /// Obtiene el índice de icono para una extensión de archivo
        /// </summary>
        public static int GetIconIndex(string extension, bool isDirectory = false)
        {
            if (isDirectory)
                return 0; // Icono de carpeta

            if (string.IsNullOrEmpty(extension))
                return 1; // Archivo sin extensión

            if (_extensionIconMap.TryGetValue(extension, out int iconIndex))
                return iconIndex;

            return 1; // Icono de archivo genérico
        }

        /// <summary>
        /// Crea un icono de carpeta simple
        /// </summary>
        private static Bitmap CreateFolderIcon()
        {
            var bitmap = new Bitmap(16, 16);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Fondo de la carpeta
                using (var brush = new SolidBrush(Color.FromArgb(255, 255, 206, 84)))
                {
                    g.FillRectangle(brush, 1, 4, 14, 10);
                }

                // Pestaña de la carpeta
                using (var brush = new SolidBrush(Color.FromArgb(255, 255, 223, 128)))
                {
                    g.FillRectangle(brush, 1, 2, 6, 3);
                }

                // Borde
                using (var pen = new Pen(Color.FromArgb(255, 204, 164, 61)))
                {
                    g.DrawRectangle(pen, 1, 4, 13, 9);
                    g.DrawRectangle(pen, 1, 2, 5, 2);
                }
            }
            return bitmap;
        }

        /// <summary>
        /// Crea un icono de archivo con el color especificado
        /// </summary>
        private static Bitmap CreateFileIcon(Color color)
        {
            var bitmap = new Bitmap(16, 16);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Fondo del archivo
                using (var brush = new SolidBrush(Color.FromArgb(200, color)))
                {
                    g.FillRectangle(brush, 2, 2, 10, 12);
                }

                // Esquina doblada
                using (var brush = new SolidBrush(Color.FromArgb(255,
                    Math.Max(0, color.R - 40),
                    Math.Max(0, color.G - 40),
                    Math.Max(0, color.B - 40))))
                {
                    Point[] triangle = { new Point(10, 2), new Point(12, 2), new Point(12, 4) };
                    g.FillPolygon(brush, triangle);
                }

                // Borde
                using (var pen = new Pen(Color.FromArgb(255,
                    Math.Max(0, color.R - 60),
                    Math.Max(0, color.G - 60),
                    Math.Max(0, color.B - 60))))
                {
                    g.DrawRectangle(pen, 2, 2, 9, 11);
                    g.DrawLine(pen, 10, 2, 12, 4);
                    g.DrawLine(pen, 10, 2, 10, 4);
                    g.DrawLine(pen, 10, 4, 12, 4);
                }

                // Líneas de texto simuladas
                using (var pen = new Pen(Color.FromArgb(100, Color.Black)))
                {
                    g.DrawLine(pen, 4, 6, 9, 6);
                    g.DrawLine(pen, 4, 8, 10, 8);
                    g.DrawLine(pen, 4, 10, 8, 10);
                }
            }
            return bitmap;
        }

        /// <summary>
        /// Crea iconos para botones de la aplicación
        /// </summary>
        public static class ButtonIcons
        {
            public static Bitmap CreateFolderSelectIcon()
            {
                var bitmap = new Bitmap(24, 24);
                using (var g = Graphics.FromImage(bitmap))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                    // Carpeta base
                    using (var brush = new SolidBrush(Color.FromArgb(255, 255, 206, 84)))
                    {
                        g.FillRectangle(brush, 2, 8, 18, 12);
                    }

                    // Pestaña
                    using (var brush = new SolidBrush(Color.FromArgb(255, 255, 223, 128)))
                    {
                        g.FillRectangle(brush, 2, 6, 8, 3);
                    }

                    // Borde
                    using (var pen = new Pen(Color.FromArgb(255, 204, 164, 61), 2))
                    {
                        g.DrawRectangle(pen, 2, 8, 17, 11);
                        g.DrawRectangle(pen, 2, 6, 7, 2);
                    }
                }
                return bitmap;
            }

            public static Bitmap CreateAnalyzeIcon()
            {
                var bitmap = new Bitmap(24, 24);
                using (var g = Graphics.FromImage(bitmap))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                    // Lupa - círculo
                    using (var pen = new Pen(Color.FromArgb(255, 50, 150, 255), 3))
                    {
                        g.DrawEllipse(pen, 4, 4, 12, 12);
                    }

                    // Lupa - mango
                    using (var pen = new Pen(Color.FromArgb(255, 100, 100, 100), 3))
                    {
                        g.DrawLine(pen, 14, 14, 19, 19);
                    }

                    // Elemento dentro de la lupa
                    using (var brush = new SolidBrush(Color.FromArgb(255, 50, 150, 255)))
                    {
                        g.FillEllipse(brush, 8, 8, 4, 4);
                    }
                }
                return bitmap;
            }

            public static Bitmap CreateFilterIcon()
            {
                var bitmap = new Bitmap(24, 24);
                using (var g = Graphics.FromImage(bitmap))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                    // Embudo
                    using (var brush = new SolidBrush(Color.FromArgb(255, 100, 200, 100)))
                    {
                        Point[] funnel = {
                            new Point(4, 6),
                            new Point(20, 6),
                            new Point(14, 12),
                            new Point(10, 12)
                        };
                        g.FillPolygon(brush, funnel);

                        // Tubo del embudo
                        g.FillRectangle(brush, 10, 12, 4, 6);
                    }

                    // Borde
                    using (var pen = new Pen(Color.FromArgb(255, 70, 140, 70), 2))
                    {
                        Point[] funnel = {
                            new Point(4, 6),
                            new Point(20, 6),
                            new Point(14, 12),
                            new Point(10, 12),
                            new Point(10, 18),
                            new Point(14, 18),
                            new Point(14, 12)
                        };
                        g.DrawLines(pen, funnel);
                    }
                }
                return bitmap;
            }

            public static Bitmap CreateReportIcon()
            {
                var bitmap = new Bitmap(24, 24);
                using (var g = Graphics.FromImage(bitmap))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                    // Documento
                    using (var brush = new SolidBrush(Color.White))
                    {
                        g.FillRectangle(brush, 5, 3, 12, 16);
                    }

                    // Esquina doblada
                    using (var brush = new SolidBrush(Color.FromArgb(255, 220, 220, 220)))
                    {
                        Point[] triangle = { new Point(14, 3), new Point(17, 3), new Point(17, 6) };
                        g.FillPolygon(brush, triangle);
                    }

                    // Borde
                    using (var pen = new Pen(Color.FromArgb(255, 100, 100, 100), 2))
                    {
                        g.DrawRectangle(pen, 5, 3, 11, 15);
                        g.DrawLine(pen, 14, 3, 17, 6);
                        g.DrawLine(pen, 14, 3, 14, 6);
                        g.DrawLine(pen, 14, 6, 17, 6);
                    }

                    // Líneas de texto
                    using (var pen = new Pen(Color.FromArgb(255, 150, 150, 150), 1))
                    {
                        g.DrawLine(pen, 7, 8, 13, 8);
                        g.DrawLine(pen, 7, 10, 15, 10);
                        g.DrawLine(pen, 7, 12, 12, 12);
                        g.DrawLine(pen, 7, 14, 14, 14);
                    }
                }
                return bitmap;
            }
        }

        /// <summary>
        /// Libera recursos del ImageList
        /// </summary>
        public static void Dispose()
        {
            _fileImageList?.Dispose();
            _fileImageList = null;
            _extensionIconMap?.Clear();
        }
    }
}
