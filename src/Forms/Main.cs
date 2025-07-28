using ResportesParaDeveloper.Extensions;
using ResportesParaDeveloper.Forms;
using ResportesParaDeveloper.Models;
using ResportesParaDeveloper.Services;
using ResportesParaDeveloper.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ResportesParaDeveloper
{
    public partial class Main : Form
    {
        private DirectoryAnalyzerService _analyzerService;
        private AnalysisResult _analysisResult;
        private FilterOptions _filterOptions;
        private CancellationTokenSource _cancellationTokenSource;

        private Button btnAdvancedFilters;
        //private Label lblActiveFilters;


        public Main()
        {
            InitializeComponent();
            _analyzerService = new DirectoryAnalyzerService();
            _analyzerService.ProgressChanged += OnProgressChanged;

            // Inicializar sistema de iconos
            var imageList = IconHelper.InitializeImageList();
            tvFilteredFiles.ImageList = imageList;
            _filterOptions = new FilterOptions();

            SetupButtonIcons();
            SetupAdditionalControls();
        }

        private void splitContainer2_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private async void btnSelectDirectory_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Seleccione el directorio a analizar";
                folderDialog.ShowNewFolderButton = false;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    txtSelectedPath.Text = folderDialog.SelectedPath;
                    btnAnalyze.Enabled = true;

                    // Reset UI
                    /*
                    clbExtensions.Items.Clear();
                    tvFilteredFiles.Nodes.Clear();
                    btnFilter.Enabled = false;
                    btnGenerateReport.Enabled = false;
                    lblTotalStats.Text = "Directorio seleccionado. Haga clic en 'Analizar' para comenzar.";
                    */

                    // Reset UI
                    ResetAnalysisUI();
                    lblTotalStats.Text = "Directorio seleccionado. Haga clic en 'Analizar' para comenzar.";
                    lblStatus.Text = $"Directorio seleccionado: {folderDialog.SelectedPath}";
                }
            }
            
        }

        private async void btnAnalyze_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSelectedPath.Text))
                return;

            // Crear token de cancelación
            _cancellationTokenSource = new CancellationTokenSource();

            SetUIEnabled(false);
            ShowProgress(true);

            try
            {
                lblStatus.Text = "Iniciando análisis...";

                // Ejecutar análisis con soporte para cancelación
                _analysisResult = await Task.Run(() =>
                    _analyzerService.AnalyzeDirectory(txtSelectedPath.Text),
                    _cancellationTokenSource.Token);

                // Update UI with results
                UpdateExtensionsList();
                UpdateStatsLabel();
                btnFilter.Enabled = true;
                btnAdvancedFilters.Enabled = true;
                btnGenerateReport.Enabled = true;

                lblStatus.Text = $"Análisis completado. Encontrados {_analysisResult.TotalFiles} archivos en {_analysisResult.TotalDirectories} carpetas.";

                // Aplicar filtro inicial si hay extensiones seleccionadas
                ApplyCurrentFilters();
            }
            catch (OperationCanceledException)
            {
                lblStatus.Text = "Análisis cancelado por el usuario";
                ResetAnalysisUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error durante el análisis: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = $"Error durante el análisis: {ex.Message}";
                ResetAnalysisUI();
            }
            finally
            {
                SetUIEnabled(true);
                ShowProgress(false);
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }

        private void ShowProgress(bool show)
        {
            progressBar.Visible = show;
            if (show)
            {
                progressBar.Style = ProgressBarStyle.Marquee;
                progressBar.MarqueeAnimationSpeed = 50;
            }
            else
            {
                progressBar.Style = ProgressBarStyle.Blocks;
                progressBar.MarqueeAnimationSpeed = 0;
            }
        }

        private void UpdateExtensionsList()
        {
            clbExtensions.Items.Clear();

            foreach (var extension in _analysisResult.Extensions)
            {
                clbExtensions.Items.Add(extension);
            }

            clbExtensions.DisplayMember = "Extension";
        }

        private void UpdateStatsLabel()
        {
            var totalSizeFormatted = FormatBytes(_analysisResult.TotalSize);
            lblTotalStats.Text = $"Total: {_analysisResult.TotalDirectories} carpetas, " +
                $"{_analysisResult.TotalFiles} archivos, {totalSizeFormatted}";
        }

        private string GetRelativePath(string fullPath)
        {
            if (_analysisResult?.RootDirectory?.FullPath == null)
                return fullPath;

            var basePath = _analysisResult.RootDirectory.FullPath;
            if (fullPath.StartsWith(basePath, StringComparison.OrdinalIgnoreCase))
            {
                var relativePath = fullPath.Substring(basePath.Length).TrimStart('\\', '/');
                return string.IsNullOrEmpty(relativePath) ? "[Directorio Raíz]" : relativePath;
            }

            return fullPath;
        }

        private void UpdateFileTree(List<FileAnalysisInfo> filteredFiles)
        {
            tvFilteredFiles.Nodes.Clear();

            if (filteredFiles == null || !filteredFiles.Any())
            {
                var emptyNode = new TreeNode("No se encontraron archivos que coincidan con los filtros")
                {
                    ForeColor = Color.Gray,
                    ImageIndex = -1,
                    SelectedImageIndex = -1
                };
                tvFilteredFiles.Nodes.Add(emptyNode);
                return;
            }

            var groupedByDirectory = filteredFiles
                .GroupBy(f => f.DirectoryPath)
                .OrderBy(g => g.Key);

            foreach (var group in groupedByDirectory)
            {
                var relativePath = GetRelativePath(group.Key);
                var dirNode = new TreeNode(relativePath)
                {
                    ImageIndex = IconHelper.GetIconIndex("", true), // Icono de carpeta
                    SelectedImageIndex = IconHelper.GetIconIndex("", true),
                    ToolTipText = group.Key
                };

                long totalDirSize = 0;
                int fileCount = 0;

                foreach (var file in group.OrderBy(f => f.Name))
                {
                    var fileDisplayText = $"{file.Name} ({file.FormattedSize})";
                    var modifiedDate = file.ModifiedDate.ToFriendlyDateString();

                    var fileNode = new TreeNode(fileDisplayText)
                    {
                        Tag = file,
                        ImageIndex = IconHelper.GetIconIndex(file.Extension),
                        SelectedImageIndex = IconHelper.GetIconIndex(file.Extension),
                        ToolTipText = $"Tamaño: {file.FormattedSize}\n" +
                                     $"Modificado: {modifiedDate}\n" +
                                     $"Creado: {file.CreatedDate.ToFriendlyDateString()}\n" +
                                     $"Ruta: {file.FullPath}"
                    };

                    dirNode.Nodes.Add(fileNode);
                    totalDirSize += file.Size;
                    fileCount++;
                }

                // Actualizar texto del directorio con estadísticas
                dirNode.Text = $"{relativePath} ({fileCount} archivos, {totalDirSize.FormatFileSize()})";
                tvFilteredFiles.Nodes.Add(dirNode);
            }

            // Expandir nodos si no hay muchos
            if (tvFilteredFiles.Nodes.Count <= 10)
            {
                tvFilteredFiles.ExpandAll();
            }
            else
            {
                // Solo expandir el primer nivel
                foreach (TreeNode node in tvFilteredFiles.Nodes)
                {
                    node.Expand();
                }
            }
        }

        

        private void OnProgressChanged(object sender, string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => lblStatus.Text = message));
            }
            else
            {
                lblStatus.Text = message;
            }
        }

        private void SetUIEnabled(bool enabled)
        {
            btnSelectDirectory.Enabled = enabled;
            btnAnalyze.Enabled = enabled && !string.IsNullOrEmpty(txtSelectedPath.Text);
            btnFilter.Enabled = enabled && _analysisResult != null;
            btnGenerateReport.Enabled = enabled && _analysisResult != null;
            clbExtensions.Enabled = enabled;
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

        private void btnFilter_Click(object sender, EventArgs e)
        {
            ApplyCurrentFilters();

    
        }

        private void ResetAnalysisUI()
        {
            clbExtensions.Items.Clear();
            tvFilteredFiles.Nodes.Clear();
            btnFilter.Enabled = false;
            btnAdvancedFilters.Enabled = false;
            btnGenerateReport.Enabled = false;
            lblActiveFilters.Text = "Sin filtros adicionales";
            _analysisResult = null;
        }

        private void btnGenerateReport_Click(object sender, EventArgs e)
        {
            if (_analysisResult == null)
                return;

            var selectedExtensions = new List<string>();
            foreach (int index in clbExtensions.CheckedIndices)
            {
                var extensionInfo = (ExtensionInfo)clbExtensions.Items[index];
                selectedExtensions.Add(extensionInfo.Extension);
            }

            if (selectedExtensions.Count == 0)
            {
                MessageBox.Show("Seleccione al menos una extensión para generar el reporte.", "Información",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var reportForm = new ReportForm(_analysisResult, selectedExtensions, _analyzerService);
            reportForm.Show();
        }

        private void SetupButtonIcons()
        {
            // Agregar iconos a los botones principales
            btnSelectDirectory.Image = IconHelper.ButtonIcons.CreateFolderSelectIcon();
            btnSelectDirectory.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnSelectDirectory.ImageAlign = ContentAlignment.MiddleLeft;

            btnAnalyze.Image = IconHelper.ButtonIcons.CreateAnalyzeIcon();
            btnAnalyze.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnAnalyze.ImageAlign = ContentAlignment.MiddleLeft;

            btnFilter.Image = IconHelper.ButtonIcons.CreateFilterIcon();
            btnFilter.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnFilter.ImageAlign = ContentAlignment.MiddleLeft;

            btnGenerateReport.Image = IconHelper.ButtonIcons.CreateReportIcon();
            btnGenerateReport.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnGenerateReport.ImageAlign = ContentAlignment.MiddleLeft;
        }

        private void SetupAdditionalControls()
        {
            // Botón para filtros avanzados
            btnAdvancedFilters = new Button
            {
                Text = "Filtros Avanzados",
                Location = new Point(btnFilter.Location.X, btnFilter.Location.Y + btnFilter.Height + 5),
                Size = new Size(btnFilter.Width, 23),
                Enabled = false,
                UseVisualStyleBackColor = true
            };
            btnAdvancedFilters.Click += btnAdvancedFilters_Click;

            // Label para mostrar filtros activos
            lblActiveFilters = new Label
            {
                Text = "Sin filtros adicionales",
                Location = new Point(btnAdvancedFilters.Location.X, btnAdvancedFilters.Location.Y + btnAdvancedFilters.Height + 10),
                Size = new Size(splitContainer1.Panel1.Width - 20, 40),
                ForeColor = Color.DarkGreen,
                Font = new Font(this.Font.FontFamily, 7.5f, FontStyle.Italic),
                AutoSize = false
            };

            // Agregar controles al panel izquierdo
            splitContainer1.Panel1.Controls.Add(btnAdvancedFilters);
            splitContainer1.Panel1.Controls.Add(lblActiveFilters);

            // Ajustar posición de otros controles
            btnGenerateReport.Location = new Point(btnGenerateReport.Location.X,
                lblActiveFilters.Location.Y + lblActiveFilters.Height + 10);
        }

        private void btnAdvancedFilters_Click(object sender, EventArgs e)
        {
            using (var filterForm = new AdvancedFilterForm(_filterOptions))
            {
                if (filterForm.ShowDialog() == DialogResult.OK)
                {
                    _filterOptions = filterForm.FilterOptions;
                    UpdateActiveFiltersLabel();
                    ApplyCurrentFilters();
                }
            }
        }

        private void UpdateActiveFiltersLabel()
        {
            var activeFilters = new List<string>();

            if (_filterOptions.EnableSizeFilter)
            {
                var minSize = _filterOptions.MinSizeBytes.FormatFileSize();
                var maxSize = _filterOptions.MaxSizeBytes == long.MaxValue ? "∞" : _filterOptions.MaxSizeBytes.FormatFileSize();
                activeFilters.Add($"Tamaño: {minSize} - {maxSize}");
            }

            if (_filterOptions.EnableDateFilter)
            {
                if (_filterOptions.MinCreatedDate.HasValue || _filterOptions.MaxCreatedDate.HasValue)
                {
                    activeFilters.Add("Filtro por fecha de creación");
                }
                if (_filterOptions.MinModifiedDate.HasValue || _filterOptions.MaxModifiedDate.HasValue)
                {
                    activeFilters.Add("Filtro por fecha de modificación");
                }
            }

            if (_filterOptions.EnableNameFilter && !string.IsNullOrWhiteSpace(_filterOptions.NamePattern))
            {
                var patternType = _filterOptions.UseRegex ? "Regex" : "Texto";
                activeFilters.Add($"Nombre ({patternType}): {_filterOptions.NamePattern.Truncate(20)}");
            }

            if (_filterOptions.ExcludeBinaryFiles)
            {
                activeFilters.Add("Sin binarios");
            }

            if (_filterOptions.ExcludeEmptyFiles)
            {
                activeFilters.Add("Sin archivos vacíos");
            }

            if (_filterOptions.ExcludedFolders.Any())
            {
                activeFilters.Add($"Excluye {_filterOptions.ExcludedFolders.Count} carpetas");
            }

            lblActiveFilters.Text = activeFilters.Any()
                ? $"Filtros activos: {string.Join(", ", activeFilters)}"
                : "Sin filtros adicionales";
        }

        private void ApplyCurrentFilters()
        {
            if (_analysisResult == null)
                return;

            // Obtener extensiones seleccionadas
            var selectedExtensions = new List<string>();
            foreach (int index in clbExtensions.CheckedIndices)
            {
                var extensionInfo = (ExtensionInfo)clbExtensions.Items[index];
                selectedExtensions.Add(extensionInfo.Extension);
            }

            // Actualizar filtros con extensiones seleccionadas
            _filterOptions.SelectedExtensions = selectedExtensions;

            // Aplicar todos los filtros
            var allFiles = _analyzerService.GetFilteredFiles(_analysisResult.RootDirectory, selectedExtensions);
            var filteredFiles = allFiles.Where(_filterOptions.MatchesFilter).ToList();

            UpdateFileTree(filteredFiles);

            // Actualizar mensaje de estado
            var totalSelected = selectedExtensions.Count;
            var totalExtensions = _analysisResult.Extensions.Count;
            var filteredCount = filteredFiles.Count;
            var totalFiles = allFiles.Count;

            lblStatus.Text = $"Mostrando {filteredCount} de {totalFiles} archivos " +
                           $"({totalSelected}/{totalExtensions} extensiones seleccionadas)";

            // Actualizar título del TreeView con estadísticas
            var filteredSize = filteredFiles.Sum(f => f.Size);
            lblFileTree.Text = $"Archivos filtrados: {filteredCount} archivos ({filteredSize.FormatFileSize()})";
        }

    }
}
