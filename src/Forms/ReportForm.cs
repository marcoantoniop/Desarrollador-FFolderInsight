using ResportesParaDeveloper.Models;
using ResportesParaDeveloper.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ResportesParaDeveloper.Forms
{
    public partial class ReportForm : Form
    {
        private readonly AnalysisResult _analysisResult;
        private readonly List<string> _selectedExtensions;
        private readonly DirectoryAnalyzerService _analyzerService;

        // Controls
        private RichTextBox rtbReport;
        private Button btnPrint;
        private Button btnSave;
        private Button btnClose;
        private ProgressBar progressBar;
        private Label lblStatus;

        public ReportForm(AnalysisResult analysisResult, List<string> selectedExtensions, DirectoryAnalyzerService analyzerService)
        {
            _analysisResult = analysisResult ?? throw new ArgumentNullException(nameof(analysisResult));
            _selectedExtensions = selectedExtensions ?? throw new ArgumentNullException(nameof(selectedExtensions));
            _analyzerService = analyzerService ?? throw new ArgumentNullException(nameof(analyzerService));

            InitializeComponent();
            GenerateReportAsync();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form properties
            this.Text = "Reporte de Análisis";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(800, 600);

            // Top panel for buttons
            var topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                Padding = new Padding(10)
            };

            btnPrint = new Button
            {
                Text = "Imprimir",
                Location = new Point(10, 10),
                Size = new Size(100, 30),
                Enabled = false
            };
            btnPrint.Click += BtnPrint_Click;

            btnSave = new Button
            {
                Text = "Guardar",
                Location = new Point(120, 10),
                Size = new Size(100, 30),
                Enabled = false
            };
            btnSave.Click += BtnSave_Click;

            btnClose = new Button
            {
                Text = "Cerrar",
                Location = new Point(230, 10),
                Size = new Size(100, 30)
            };
            btnClose.Click += (s, e) => this.Close();

            topPanel.Controls.AddRange(new Control[] { btnPrint, btnSave, btnClose });

            // Main text area
            rtbReport = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Font = new Font("Consolas", 9),
                WordWrap = false,
                ScrollBars = RichTextBoxScrollBars.Both
            };

            // Bottom status panel
            var bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 40,
                Padding = new Padding(10)
            };

            lblStatus = new Label
            {
                Location = new Point(10, 10),
                Size = new Size(400, 20),
                Text = "Generando reporte..."
            };

            progressBar = new ProgressBar
            {
                Location = new Point(420, 10),
                Size = new Size(300, 20),
                Style = ProgressBarStyle.Marquee
            };

            bottomPanel.Controls.AddRange(new Control[] { lblStatus, progressBar });

            // Add all controls to form
            this.Controls.AddRange(new Control[] { rtbReport, topPanel, bottomPanel });

            this.ResumeLayout(false);
        }

        private async void GenerateReportAsync()
        {
            try
            {
                var report = await Task.Run(() => GenerateReport());

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

        private string GenerateReport()
        {
            var sb = new StringBuilder();

            // Header del reporte
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

            // Extensiones incluidas en el reporte
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

            // Estructura de directorios
            sb.AppendLine("=".PadRight(80, '='));
            sb.AppendLine("ESTRUCTURA DE DIRECTORIOS Y ARCHIVOS");
            sb.AppendLine("=".PadRight(80, '='));
            sb.AppendLine();

            GenerateDirectoryStructure(_analysisResult.RootDirectory, sb, "", _selectedExtensions);

            sb.AppendLine();
            sb.AppendLine("=".PadRight(80, '='));
            sb.AppendLine("CONTENIDO DE ARCHIVOS");
            sb.AppendLine("=".PadRight(80, '='));
            sb.AppendLine();

            // Contenido de archivos
            GenerateFileContents(_analysisResult.RootDirectory, sb, _selectedExtensions);

            return sb.ToString();
        }

        private void GenerateDirectoryStructure(DirectoryAnalysisInfo directory, StringBuilder sb, string indent, List<string> selectedExtensions)
        {
            sb.AppendLine($"{indent}📁 {directory.Name}/");

            // Mostrar archivos que coincidan con las extensiones seleccionadas
            var filteredFiles = directory.Files.Where(f =>
                selectedExtensions.Contains(f.Extension) ||
                (string.IsNullOrEmpty(f.Extension) && selectedExtensions.Contains("[sin extensión]"))
            ).OrderBy(f => f.Name);

            foreach (var file in filteredFiles)
            {
                sb.AppendLine($"{indent}  📄 {file.Name} ({file.FormattedSize})");
            }

            // Procesar subdirectorios
            foreach (var subDir in directory.SubDirectories.OrderBy(d => d.Name))
            {
                GenerateDirectoryStructure(subDir, sb, indent + "  ", selectedExtensions);
            }
        }

        private void GenerateFileContents(DirectoryAnalysisInfo directory, StringBuilder sb, List<string> selectedExtensions)
        {
            // Mostrar contenido de archivos en el directorio actual
            var filteredFiles = directory.Files.Where(f =>
                selectedExtensions.Contains(f.Extension) ||
                (string.IsNullOrEmpty(f.Extension) && selectedExtensions.Contains("[sin extensión]"))
            ).OrderBy(f => f.Name);

            if (filteredFiles.Any())
            {
                sb.AppendLine($"Carpeta: {directory.FullPath}");
                sb.AppendLine("-".PadRight(80, '-'));
                sb.AppendLine();

                foreach (var file in filteredFiles)
                {
                    sb.AppendLine($"Archivo: {file.Name}");
                    sb.AppendLine($"Tamaño: {file.FormattedSize}");
                    sb.AppendLine($"Ruta completa: {file.FullPath}");
                    sb.AppendLine("Contenido:");
                    sb.AppendLine();

                    // Leer contenido del archivo
                    var content = _analyzerService.ReadFileContent(file.FullPath);
                    sb.AppendLine(content);

                    sb.AppendLine();
                    sb.AppendLine("".PadRight(60, '-'));
                    sb.AppendLine();
                }
            }

            // Procesar subdirectorios recursivamente
            foreach (var subDir in directory.SubDirectories.OrderBy(d => d.Name))
            {
                GenerateFileContents(subDir, sb, selectedExtensions);
            }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
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

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                using (var saveDialog = new SaveFileDialog())
                {
                    saveDialog.Filter = "Archivo de texto (*.txt)|*.txt|Todos los archivos (*.*)|*.*";
                    saveDialog.DefaultExt = "txt";
                    saveDialog.FileName = $"Reporte_Analisis_{DateTime.Now:yyyyMMdd_HHmmss}.txt";

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllText(saveDialog.FileName, rtbReport.Text, Encoding.UTF8);
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

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Confirmar cierre si el reporte se está generando
            if (progressBar.Visible)
            {
                var result = MessageBox.Show("El reporte se está generando. ¿Está seguro que desea cerrar?",
                    "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }

            base.OnFormClosing(e);
        }
    }
}
