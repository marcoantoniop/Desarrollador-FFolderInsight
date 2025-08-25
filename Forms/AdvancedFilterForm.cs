using ResportesParaDeveloper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResportesParaDeveloper.Forms
{
    /// <summary>
    /// Formulario para configurar filtros avanzados
    /// </summary>
    public partial class AdvancedFilterForm : Form
    {
        public FilterOptions FilterOptions { get; private set; }

        // Controles para filtros de tamaño
        private CheckBox chkEnableSizeFilter;
        private NumericUpDown nudMinSizeKB;
        private NumericUpDown nudMaxSizeMB;
        private ComboBox cmbMinSizeUnit;
        private ComboBox cmbMaxSizeUnit;

        // Controles para filtros de fecha
        private CheckBox chkEnableDateFilter;
        private DateTimePicker dtpMinCreated;
        private DateTimePicker dtpMaxCreated;
        private DateTimePicker dtpMinModified;
        private DateTimePicker dtpMaxModified;
        private CheckBox chkUseCreatedDate;
        private CheckBox chkUseModifiedDate;

        // Controles para filtros de nombre
        private CheckBox chkEnableNameFilter;
        private TextBox txtNamePattern;
        private CheckBox chkUseRegex;
        private CheckBox chkCaseSensitive;

        // Controles para exclusiones
        private CheckBox chkExcludeBinary;
        private CheckBox chkExcludeEmpty;
        private TextBox txtExcludedFolders;

        // Botones
        private Button btnOK;
        private Button btnCancel;
        private Button btnReset;

        public AdvancedFilterForm(FilterOptions currentOptions = null)
        {
            FilterOptions = currentOptions ?? new FilterOptions();
            InitializeComponent();
            LoadCurrentOptions();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Configuración del formulario
            this.Text = "Filtros Avanzados";
            this.Size = new Size(500, 650);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            int yPos = 10;
            const int leftMargin = 20;
            const int controlHeight = 25;
            const int spacing = 8;

            // === FILTROS POR TAMAÑO ===
            var grpSize = new GroupBox
            {
                Text = "Filtrar por Tamaño de Archivo",
                Location = new Point(10, yPos),
                Size = new Size(460, 120)
            };

            chkEnableSizeFilter = new CheckBox
            {
                Text = "Habilitar filtro por tamaño",
                Location = new Point(leftMargin, 25),
                Size = new Size(200, 20)
            };
            chkEnableSizeFilter.CheckedChanged += ChkEnableSizeFilter_CheckedChanged;

            // Tamaño mínimo
            var lblMinSize = new Label
            {
                Text = "Tamaño mínimo:",
                Location = new Point(leftMargin, 55),
                Size = new Size(100, 20)
            };

            nudMinSizeKB = new NumericUpDown
            {
                Location = new Point(leftMargin + 105, 53),
                Size = new Size(80, 23),
                Minimum = 0,
                Maximum = 999999,
                DecimalPlaces = 2
            };

            cmbMinSizeUnit = new ComboBox
            {
                Location = new Point(leftMargin + 190, 53),
                Size = new Size(60, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbMinSizeUnit.Items.AddRange(new[] { "B", "KB", "MB", "GB" });
            cmbMinSizeUnit.SelectedIndex = 1; // KB por defecto

            // Tamaño máximo
            var lblMaxSize = new Label
            {
                Text = "Tamaño máximo:",
                Location = new Point(leftMargin, 85),
                Size = new Size(100, 20)
            };

            nudMaxSizeMB = new NumericUpDown
            {
                Location = new Point(leftMargin + 105, 83),
                Size = new Size(80, 23),
                Minimum = 0,
                Maximum = 999999,
                DecimalPlaces = 2,
                Value = 100 // 100 MB por defecto
            };

            cmbMaxSizeUnit = new ComboBox
            {
                Location = new Point(leftMargin + 190, 83),
                Size = new Size(60, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbMaxSizeUnit.Items.AddRange(new[] { "B", "KB", "MB", "GB" });
            cmbMaxSizeUnit.SelectedIndex = 2; // MB por defecto

            grpSize.Controls.AddRange(new Control[] {
                chkEnableSizeFilter, lblMinSize, nudMinSizeKB, cmbMinSizeUnit,
                lblMaxSize, nudMaxSizeMB, cmbMaxSizeUnit
            });

            yPos += 130;

            // === FILTROS POR FECHA ===
            var grpDate = new GroupBox
            {
                Text = "Filtrar por Fecha",
                Location = new Point(10, yPos),
                Size = new Size(460, 180)
            };

            chkEnableDateFilter = new CheckBox
            {
                Text = "Habilitar filtro por fecha",
                Location = new Point(leftMargin, 25),
                Size = new Size(200, 20)
            };
            chkEnableDateFilter.CheckedChanged += ChkEnableDateFilter_CheckedChanged;

            // Fecha de creación
            chkUseCreatedDate = new CheckBox
            {
                Text = "Filtrar por fecha de creación",
                Location = new Point(leftMargin, 55),
                Size = new Size(200, 20)
            };

            var lblMinCreated = new Label
            {
                Text = "Desde:",
                Location = new Point(leftMargin + 20, 85),
                Size = new Size(50, 20)
            };

            dtpMinCreated = new DateTimePicker
            {
                Location = new Point(leftMargin + 75, 83),
                Size = new Size(120, 23),
                Format = DateTimePickerFormat.Short
            };

            var lblMaxCreated = new Label
            {
                Text = "Hasta:",
                Location = new Point(leftMargin + 210, 85),
                Size = new Size(50, 20)
            };

            dtpMaxCreated = new DateTimePicker
            {
                Location = new Point(leftMargin + 265, 83),
                Size = new Size(120, 23),
                Format = DateTimePickerFormat.Short
            };

            // Fecha de modificación
            chkUseModifiedDate = new CheckBox
            {
                Text = "Filtrar por fecha de modificación",
                Location = new Point(leftMargin, 115),
                Size = new Size(200, 20)
            };

            var lblMinModified = new Label
            {
                Text = "Desde:",
                Location = new Point(leftMargin + 20, 145),
                Size = new Size(50, 20)
            };

            dtpMinModified = new DateTimePicker
            {
                Location = new Point(leftMargin + 75, 143),
                Size = new Size(120, 23),
                Format = DateTimePickerFormat.Short
            };

            var lblMaxModified = new Label
            {
                Text = "Hasta:",
                Location = new Point(leftMargin + 210, 145),
                Size = new Size(50, 20)
            };

            dtpMaxModified = new DateTimePicker
            {
                Location = new Point(leftMargin + 265, 143),
                Size = new Size(120, 23),
                Format = DateTimePickerFormat.Short
            };

            grpDate.Controls.AddRange(new Control[] {
                chkEnableDateFilter, chkUseCreatedDate, lblMinCreated, dtpMinCreated,
                lblMaxCreated, dtpMaxCreated, chkUseModifiedDate, lblMinModified,
                dtpMinModified, lblMaxModified, dtpMaxModified
            });

            yPos += 190;

            // === FILTROS POR NOMBRE ===
            var grpName = new GroupBox
            {
                Text = "Filtrar por Nombre de Archivo",
                Location = new Point(10, yPos),
                Size = new Size(460, 120)
            };

            chkEnableNameFilter = new CheckBox
            {
                Text = "Habilitar filtro por nombre",
                Location = new Point(leftMargin, 25),
                Size = new Size(200, 20)
            };
            chkEnableNameFilter.CheckedChanged += ChkEnableNameFilter_CheckedChanged;

            var lblPattern = new Label
            {
                Text = "Patrón de búsqueda:",
                Location = new Point(leftMargin, 55),
                Size = new Size(120, 20)
            };

            txtNamePattern = new TextBox
            {
                Location = new Point(leftMargin + 125, 53),
                Size = new Size(200, 23),
                PlaceholderText = "Ej: test*.cs o ^[A-Z].*\\.txt$"
            };

            chkUseRegex = new CheckBox
            {
                Text = "Usar expresiones regulares",
                Location = new Point(leftMargin, 85),
                Size = new Size(200, 20)
            };

            chkCaseSensitive = new CheckBox
            {
                Text = "Sensible a mayúsculas/minúsculas",
                Location = new Point(leftMargin + 220, 85),
                Size = new Size(220, 20)
            };

            grpName.Controls.AddRange(new Control[] {
                chkEnableNameFilter, lblPattern, txtNamePattern, chkUseRegex, chkCaseSensitive
            });

            yPos += 130;

            // === EXCLUSIONES ===
            var grpExclusions = new GroupBox
            {
                Text = "Exclusiones",
                Location = new Point(10, yPos),
                Size = new Size(460, 120)
            };

            chkExcludeBinary = new CheckBox
            {
                Text = "Excluir archivos binarios",
                Location = new Point(leftMargin, 25),
                Size = new Size(200, 20),
                Checked = true
            };

            chkExcludeEmpty = new CheckBox
            {
                Text = "Excluir archivos vacíos",
                Location = new Point(leftMargin + 220, 25),
                Size = new Size(200, 20)
            };

            var lblExcludedFolders = new Label
            {
                Text = "Carpetas excluidas (separadas por comas):",
                Location = new Point(leftMargin, 55),
                Size = new Size(300, 20)
            };

            txtExcludedFolders = new TextBox
            {
                Location = new Point(leftMargin, 78),
                Size = new Size(420, 23),
                Text = ".git, .vs, node_modules, bin, obj, packages, .svn"
            };

            grpExclusions.Controls.AddRange(new Control[] {
                chkExcludeBinary, chkExcludeEmpty, lblExcludedFolders, txtExcludedFolders
            });

            yPos += 130;

            // === BOTONES ===
            var buttonPanel = new Panel
            {
                Location = new Point(10, yPos),
                Size = new Size(460, 40),
                Dock = DockStyle.Bottom
            };

            btnOK = new Button
            {
                Text = "Aplicar",
                Location = new Point(280, 8),
                Size = new Size(80, 25),
                DialogResult = DialogResult.OK
            };
            btnOK.Click += BtnOK_Click;

            btnCancel = new Button
            {
                Text = "Cancelar",
                Location = new Point(370, 8),
                Size = new Size(80, 25),
                DialogResult = DialogResult.Cancel
            };

            btnReset = new Button
            {
                Text = "Restablecer",
                Location = new Point(10, 8),
                Size = new Size(80, 25)
            };
            btnReset.Click += BtnReset_Click;

            buttonPanel.Controls.AddRange(new Control[] { btnReset, btnOK, btnCancel });

            // Agregar todos los controles al formulario
            this.Controls.AddRange(new Control[] {
                grpSize, grpDate, grpName, grpExclusions, buttonPanel
            });

            // Configurar valores por defecto
            SetDefaultValues();
            UpdateControlStates();

            this.ResumeLayout(false);
        }

        private void SetDefaultValues()
        {
            // Establecer fechas por defecto (último año)
            dtpMinCreated.Value = DateTime.Now.AddYears(-1);
            dtpMaxCreated.Value = DateTime.Now;
            dtpMinModified.Value = DateTime.Now.AddYears(-1);
            dtpMaxModified.Value = DateTime.Now;
        }

        private void LoadCurrentOptions()
        {
            // Cargar filtros de tamaño
            chkEnableSizeFilter.Checked = FilterOptions.EnableSizeFilter;

            if (FilterOptions.MinSizeBytes > 0)
            {
                var (value, unit) = ConvertBytesToUserFriendly(FilterOptions.MinSizeBytes);
                nudMinSizeKB.Value = (decimal)value;
                cmbMinSizeUnit.SelectedItem = unit;
            }

            if (FilterOptions.MaxSizeBytes < long.MaxValue)
            {
                var (value, unit) = ConvertBytesToUserFriendly(FilterOptions.MaxSizeBytes);
                nudMaxSizeMB.Value = (decimal)value;
                cmbMaxSizeUnit.SelectedItem = unit;
            }

            // Cargar filtros de fecha
            chkEnableDateFilter.Checked = FilterOptions.EnableDateFilter;

            if (FilterOptions.MinCreatedDate.HasValue)
            {
                chkUseCreatedDate.Checked = true;
                dtpMinCreated.Value = FilterOptions.MinCreatedDate.Value;
            }

            if (FilterOptions.MaxCreatedDate.HasValue)
            {
                dtpMaxCreated.Value = FilterOptions.MaxCreatedDate.Value;
            }

            if (FilterOptions.MinModifiedDate.HasValue)
            {
                chkUseModifiedDate.Checked = true;
                dtpMinModified.Value = FilterOptions.MinModifiedDate.Value;
            }

            if (FilterOptions.MaxModifiedDate.HasValue)
            {
                dtpMaxModified.Value = FilterOptions.MaxModifiedDate.Value;
            }

            // Cargar filtros de nombre
            chkEnableNameFilter.Checked = FilterOptions.EnableNameFilter;
            txtNamePattern.Text = FilterOptions.NamePattern;
            chkUseRegex.Checked = FilterOptions.UseRegex;
            chkCaseSensitive.Checked = FilterOptions.CaseSensitive;

            // Cargar exclusiones
            chkExcludeBinary.Checked = FilterOptions.ExcludeBinaryFiles;
            chkExcludeEmpty.Checked = FilterOptions.ExcludeEmptyFiles;
            txtExcludedFolders.Text = string.Join(", ", FilterOptions.ExcludedFolders);
        }

        private (double value, string unit) ConvertBytesToUserFriendly(long bytes)
        {
            if (bytes >= 1_073_741_824) // GB
                return (bytes / 1_073_741_824.0, "GB");
            if (bytes >= 1_048_576) // MB
                return (bytes / 1_048_576.0, "MB");
            if (bytes >= 1024) // KB
                return (bytes / 1024.0, "KB");
            return (bytes, "B");
        }

        private long ConvertToBytes(double value, string unit)
        {
            return unit switch
            {
                "GB" => (long)(value * 1_073_741_824),
                "MB" => (long)(value * 1_048_576),
                "KB" => (long)(value * 1024),
                "B" => (long)value,
                _ => (long)value
            };
        }

        private void ChkEnableSizeFilter_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControlStates();
        }

        private void ChkEnableDateFilter_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControlStates();
        }

        private void ChkEnableNameFilter_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControlStates();
        }

        private void UpdateControlStates()
        {
            // Habilitar/deshabilitar controles de tamaño
            nudMinSizeKB.Enabled = chkEnableSizeFilter.Checked;
            cmbMinSizeUnit.Enabled = chkEnableSizeFilter.Checked;
            nudMaxSizeMB.Enabled = chkEnableSizeFilter.Checked;
            cmbMaxSizeUnit.Enabled = chkEnableSizeFilter.Checked;

            // Habilitar/deshabilitar controles de fecha
            bool dateEnabled = chkEnableDateFilter.Checked;
            chkUseCreatedDate.Enabled = dateEnabled;
            chkUseModifiedDate.Enabled = dateEnabled;

            dtpMinCreated.Enabled = dateEnabled && chkUseCreatedDate.Checked;
            dtpMaxCreated.Enabled = dateEnabled && chkUseCreatedDate.Checked;
            dtpMinModified.Enabled = dateEnabled && chkUseModifiedDate.Checked;
            dtpMaxModified.Enabled = dateEnabled && chkUseModifiedDate.Checked;

            // Habilitar/deshabilitar controles de nombre
            txtNamePattern.Enabled = chkEnableNameFilter.Checked;
            chkUseRegex.Enabled = chkEnableNameFilter.Checked;
            chkCaseSensitive.Enabled = chkEnableNameFilter.Checked;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            try
            {
                // Guardar configuración de filtros de tamaño
                FilterOptions.EnableSizeFilter = chkEnableSizeFilter.Checked;
                if (chkEnableSizeFilter.Checked)
                {
                    FilterOptions.MinSizeBytes = ConvertToBytes((double)nudMinSizeKB.Value, cmbMinSizeUnit.SelectedItem.ToString());
                    FilterOptions.MaxSizeBytes = ConvertToBytes((double)nudMaxSizeMB.Value, cmbMaxSizeUnit.SelectedItem.ToString());

                    if (FilterOptions.MinSizeBytes > FilterOptions.MaxSizeBytes)
                    {
                        MessageBox.Show("El tamaño mínimo no puede ser mayor que el tamaño máximo.",
                            "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Guardar configuración de filtros de fecha
                FilterOptions.EnableDateFilter = chkEnableDateFilter.Checked;
                if (chkEnableDateFilter.Checked)
                {
                    FilterOptions.MinCreatedDate = chkUseCreatedDate.Checked ? dtpMinCreated.Value : null;
                    FilterOptions.MaxCreatedDate = chkUseCreatedDate.Checked ? dtpMaxCreated.Value : null;
                    FilterOptions.MinModifiedDate = chkUseModifiedDate.Checked ? dtpMinModified.Value : null;
                    FilterOptions.MaxModifiedDate = chkUseModifiedDate.Checked ? dtpMaxModified.Value : null;

                    // Validar fechas
                    if (chkUseCreatedDate.Checked && dtpMinCreated.Value > dtpMaxCreated.Value)
                    {
                        MessageBox.Show("La fecha mínima de creación no puede ser posterior a la fecha máxima.",
                            "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (chkUseModifiedDate.Checked && dtpMinModified.Value > dtpMaxModified.Value)
                    {
                        MessageBox.Show("La fecha mínima de modificación no puede ser posterior a la fecha máxima.",
                            "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Guardar configuración de filtros de nombre
                FilterOptions.EnableNameFilter = chkEnableNameFilter.Checked;
                FilterOptions.NamePattern = txtNamePattern.Text.Trim();
                FilterOptions.UseRegex = chkUseRegex.Checked;
                FilterOptions.CaseSensitive = chkCaseSensitive.Checked;

                // Validar expresión regular si está habilitada
                if (chkEnableNameFilter.Checked && chkUseRegex.Checked && !string.IsNullOrWhiteSpace(txtNamePattern.Text))
                {
                    try
                    {
                        var regex = new System.Text.RegularExpressions.Regex(txtNamePattern.Text);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"La expresión regular no es válida: {ex.Message}",
                            "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Guardar exclusiones
                FilterOptions.ExcludeBinaryFiles = chkExcludeBinary.Checked;
                FilterOptions.ExcludeEmptyFiles = chkExcludeEmpty.Checked;

                FilterOptions.ExcludedFolders.Clear();
                if (!string.IsNullOrWhiteSpace(txtExcludedFolders.Text))
                {
                    var folders = txtExcludedFolders.Text.Split(',');
                    foreach (var folder in folders)
                    {
                        var trimmedFolder = folder.Trim();
                        if (!string.IsNullOrEmpty(trimmedFolder))
                        {
                            FilterOptions.ExcludedFolders.Add(trimmedFolder);
                        }
                    }
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al aplicar los filtros: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("¿Está seguro de que desea restablecer todos los filtros a sus valores por defecto?",
                "Confirmar Restablecimiento", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                FilterOptions = new FilterOptions();
                LoadCurrentOptions();
                UpdateControlStates();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (this.DialogResult == DialogResult.None)
            {
                this.DialogResult = DialogResult.Cancel;
            }
            base.OnFormClosing(e);
        }
    }
}
