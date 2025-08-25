using ResportesParaDeveloper.Models;
using ResportesParaDeveloper.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ResportesParaDeveloper.Forms
{
    public partial class ReportesForm : Form
    {
        private readonly AnalysisResult _analysisResult;
        private readonly List<string> _selectedExtensions;
        private readonly DirectoryAnalyzerService _analyzerService;
        private List<FileAnalysisInfo> _filesToReport; // AÑADIDO: Para tener la lista de archivos a reportar

        public ReportesForm(AnalysisResult analysisResult, List<string> selectedExtensions, DirectoryAnalyzerService analyzerService)
        {
            _analysisResult = analysisResult ?? throw new ArgumentNullException(nameof(analysisResult));
            _selectedExtensions = selectedExtensions ?? throw new ArgumentNullException(nameof(selectedExtensions));
            _analyzerService = analyzerService ?? throw new ArgumentNullException(nameof(analyzerService));

            InitializeComponent();

            // AÑADIDO: Mover la generación al evento Load para que el formulario sea visible primero.
            this.Load += ReportesForm_Load;
        }

        // AÑADIDO: Evento Load para iniciar la generación del reporte.
        private void ReportesForm_Load(object sender, EventArgs e)
        {
            // Obtenemos la lista de archivos una sola vez.
            _filesToReport = GetFilesForReport();
            GenerateReportAsync();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            // (Sin cambios en este método)
            try
            {
                var printDialog = new PrintDialog();
                var document = new System.Drawing.Printing.PrintDocument();

                document.PrintPage += (s, args) =>
                {
                    var font = new Font("Consolas", 8);
                    var brush = new SolidBrush(Color.Black);
                    var lines = rtbReport.Text.Split('\n');
                    var y = args.MarginBounds.Top;
                    var lineHeight = font.GetHeight(args.Graphics);

                    foreach (var line in lines)
                    {
                        if (y + lineHeight > args.MarginBounds.Bottom)
                            break;

                        args.Graphics.DrawString(line, font, brush, args.MarginBounds.Left, y);
                        y += (int)lineHeight;
                    }
                };

                printDialog.Document = document;

                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    document.Print();
                    lblStatus.Text = "Documento enviado a impresora";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al imprimir: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // (Sin cambios en este método)
            try
            {
                using (var saveDialog = new SaveFileDialog())
                {
                    saveDialog.Filter = "Archivo de texto (*.txt)|*.txt|Todos los archivos (*.*)|*.*";
                    saveDialog.DefaultExt = "txt";
                    saveDialog.FileName = $"Reporte_Analisis_{DateTime.Now:yyyyMMdd_HHmmss}.txt";

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        System.IO.File.WriteAllText(saveDialog.FileName, rtbReport.Text, Encoding.UTF8);
                        lblStatus.Text = $"Reporte guardado en: {saveDialog.FileName}";

                        MessageBox.Show($"Reporte guardado exitosamente en:\n{saveDialog.FileName}",
                                       "Guardado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el archivo: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ReportesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // (Sin cambios en este método)
            if (progressBar.Visible)
            {
                var result = MessageBox.Show("El reporte se está generando. ¿Está seguro que desea cerrar?",
                                           "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private async void GenerateReportAsync()
        {
            bool includeContent = true;
            const int fileWarningThreshold = 50; // Umbral para mostrar advertencia

            if (_filesToReport.Count > fileWarningThreshold)
            {
                var dialogResult = MessageBox.Show(
                    $"Ha seleccionado {_filesToReport.Count} archivos. Incluir el contenido de todos los archivos puede tardar y generar un reporte muy grande.\n\n¿Desea incluir el contenido de los archivos?",
                    "Confirmación de Reporte Extenso",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (dialogResult == DialogResult.No)
                {
                    includeContent = false;
                }
            }

            rtbReport.Text = "Generando reporte, por favor espera...";
            lblStatus.Text = "Generando...";
            btnPrint.Enabled = false;
            btnSave.Enabled = false;
            progressBar.Visible = true;
            progressBar.Style = ProgressBarStyle.Marquee;
            progressBar.MarqueeAnimationSpeed = 50;

            try
            {
                var report = await Task.Run(() => GenerateReport(includeContent)); // Se pasa el parámetro

                rtbReport.Text = report;
                btnPrint.Enabled = true;
                btnSave.Enabled = true;
                progressBar.Visible = false;
                lblStatus.Text = "Reporte generado exitosamente";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar el reporte: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = $"Error: {ex.Message}";
                progressBar.Visible = false;
            }
        }

        // MODIFICADO: Ahora acepta un booleano para decidir si incluir el contenido.
        private string GenerateReport(bool includeContent)
        {
            var sb = new StringBuilder();
            // (La sección del encabezado no cambia)
            sb.AppendLine("=".PadRight(80, '='));
            sb.AppendLine("REPORTE DE ANÁLISIS DE DIRECTORIO");
            sb.AppendLine("=".PadRight(80, '='));
            sb.AppendLine();
            sb.AppendLine($"Directorio analizado: {_analysisResult.RootDirectory.FullPath}");
            sb.AppendLine($"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            sb.AppendLine($"Total de carpetas: {_analysisResult.TotalDirectories}");
            sb.AppendLine($"Total de archivos: {_analysisResult.TotalFiles}");
            sb.AppendLine($"Tamaño total: {FormatBytes(_analysisResult.TotalSize)}");
            sb.AppendLine();

            sb.AppendLine("Extensiones incluidas en este reporte:");
            foreach (var ext in _selectedExtensions.OrderBy(e => e))
            {
                var extInfo = _analysisResult.Extensions.FirstOrDefault(e => e.Extension == ext);
                if (extInfo != null)
                {
                    sb.AppendLine($"  {ext} ({extInfo.FileCount} archivos, {extInfo.FormattedTotalSize})");
                }
            }
            sb.AppendLine();

            sb.AppendLine("=".PadRight(80, '='));
            sb.AppendLine("ESTRUCTURA DE DIRECTORIOS Y ARCHIVOS");
            sb.AppendLine("=".PadRight(80, '='));
            sb.AppendLine();

            GenerateDirectoryStructure(_analysisResult.RootDirectory, sb);

            // MODIFICADO: Esta sección ahora es condicional
            if (includeContent)
            {
                sb.AppendLine();
                sb.AppendLine("=".PadRight(80, '='));
                sb.AppendLine("CONTENIDO DE ARCHIVOS");
                sb.AppendLine("=".PadRight(80, '='));
                sb.AppendLine();

                GenerateFileContents(_analysisResult.RootDirectory, sb);
            }
            else
            {
                sb.AppendLine();
                sb.AppendLine("[Contenido de archivos omitido por el usuario]");
            }

            return sb.ToString();
        }

        private string FormatBytes(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        // MODIFICADO: Ahora usa la lista de archivos pre-calculada
        private void GenerateDirectoryStructure(DirectoryAnalysisInfo directory, StringBuilder sb, string indent = "")
        {
            sb.AppendLine($"{indent}📁 {directory.Name}/");

            var filesInDir = _filesToReport.Where(f => f.DirectoryPath == directory.FullPath).OrderBy(f => f.Name);

            foreach (var file in filesInDir)
            {
                sb.AppendLine($"{indent}  📄 {file.Name} ({file.FormattedSize})");
            }

            foreach (var subDir in directory.SubDirectories.OrderBy(d => d.Name))
            {
                GenerateDirectoryStructure(subDir, sb, indent + "  ");
            }
        }

        // MODIFICADO: Ahora usa la lista de archivos pre-calculada
        private void GenerateFileContents(DirectoryAnalysisInfo directory, StringBuilder sb)
        {
            var filesInDir = _filesToReport.Where(f => f.DirectoryPath == directory.FullPath).OrderBy(f => f.Name);

            if (filesInDir.Any())
            {
                sb.AppendLine($"Carpeta: {directory.FullPath}");
                sb.AppendLine("-".PadRight(80, '-'));
                sb.AppendLine();

                foreach (var file in filesInDir)
                {
                    sb.AppendLine($"Archivo: {file.Name}");
                    sb.AppendLine($"Tamaño: {file.FormattedSize}");
                    sb.AppendLine($"Ruta completa: {file.FullPath}");
                    sb.AppendLine("Contenido:");
                    sb.AppendLine();

                    var content = _analyzerService.ReadFileContent(file.FullPath);
                    sb.AppendLine(content);

                    sb.AppendLine();
                    sb.AppendLine("".PadRight(60, '-'));
                    sb.AppendLine();
                }
            }

            foreach (var subDir in directory.SubDirectories.OrderBy(d => d.Name))
            {
                GenerateFileContents(subDir, sb);
            }
        }

        // AÑADIDO: Método para obtener la lista de archivos a reportar.
        private List<FileAnalysisInfo> GetFilesForReport()
        {
            var allFiles = new List<FileAnalysisInfo>();

            Action<DirectoryAnalysisInfo> collectFiles = null;
            collectFiles = (dir) => {
                allFiles.AddRange(dir.Files.Where(f => _selectedExtensions.Contains(f.Extension)));
                foreach (var subDir in dir.SubDirectories)
                {
                    collectFiles(subDir);
                }
            };

            collectFiles(_analysisResult.RootDirectory);
            return allFiles;
        }
    }
}