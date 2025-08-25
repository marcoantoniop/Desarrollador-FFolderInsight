namespace ResportesParaDeveloper
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            panel1 = new Panel();
            label1 = new Label();
            btnAdvancedFilters = new Button();
            lblActiveFilters = new Label();
            lblTotalStats = new Label();
            btnAnalyze = new Button();
            txtSelectedPath = new TextBox();
            btnSelectDirectory = new Button();
            panel2 = new Panel();
            progressBar = new ProgressBar();
            lblStatus = new Label();
            splitContainer1 = new SplitContainer();
            clbExtensions = new CheckedListBox();
            btnFilter = new Button();
            btnGenerateReport = new Button();
            lblExtensions = new Label();
            tvFilteredFiles = new TreeView();
            lblFileTree = new Label();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(label1);
            panel1.Controls.Add(btnAdvancedFilters);
            panel1.Controls.Add(lblActiveFilters);
            panel1.Controls.Add(lblTotalStats);
            panel1.Controls.Add(btnAnalyze);
            panel1.Controls.Add(txtSelectedPath);
            panel1.Controls.Add(btnSelectDirectory);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(927, 100);
            panel1.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(439, 25);
            label1.TabIndex = 6;
            label1.Text = "Desarrollador FFInsight - Analizador de archivos";
            // 
            // btnAdvancedFilters
            // 
            btnAdvancedFilters.Location = new Point(3, 42);
            btnAdvancedFilters.Name = "btnAdvancedFilters";
            btnAdvancedFilters.Size = new Size(125, 23);
            btnAdvancedFilters.TabIndex = 5;
            btnAdvancedFilters.Text = "Filtros Avanzados";
            btnAdvancedFilters.UseVisualStyleBackColor = true;
            btnAdvancedFilters.Click += btnAdvancedFilters_Click;
            // 
            // lblActiveFilters
            // 
            lblActiveFilters.AutoSize = true;
            lblActiveFilters.Location = new Point(513, 9);
            lblActiveFilters.Name = "lblActiveFilters";
            lblActiveFilters.Size = new Size(0, 15);
            lblActiveFilters.TabIndex = 4;
            // 
            // lblTotalStats
            // 
            lblTotalStats.AutoSize = true;
            lblTotalStats.Location = new Point(514, 75);
            lblTotalStats.Name = "lblTotalStats";
            lblTotalStats.Size = new Size(209, 15);
            lblTotalStats.TabIndex = 3;
            lblTotalStats.Text = "Seleccione una carpeta para comenzar";
            // 
            // btnAnalyze
            // 
            btnAnalyze.Location = new Point(433, 71);
            btnAnalyze.Name = "btnAnalyze";
            btnAnalyze.Size = new Size(75, 23);
            btnAnalyze.TabIndex = 2;
            btnAnalyze.Text = "Analizar";
            btnAnalyze.UseVisualStyleBackColor = true;
            btnAnalyze.Click += btnAnalyze_Click;
            // 
            // txtSelectedPath
            // 
            txtSelectedPath.Location = new Point(134, 71);
            txtSelectedPath.Name = "txtSelectedPath";
            txtSelectedPath.Size = new Size(293, 23);
            txtSelectedPath.TabIndex = 1;
            // 
            // btnSelectDirectory
            // 
            btnSelectDirectory.Location = new Point(3, 71);
            btnSelectDirectory.Name = "btnSelectDirectory";
            btnSelectDirectory.Size = new Size(125, 23);
            btnSelectDirectory.TabIndex = 0;
            btnSelectDirectory.Text = "Seleccionar Carpeta";
            btnSelectDirectory.UseVisualStyleBackColor = true;
            btnSelectDirectory.Click += btnSelectDirectory_Click;
            // 
            // panel2
            // 
            panel2.Controls.Add(progressBar);
            panel2.Controls.Add(lblStatus);
            panel2.Dock = DockStyle.Bottom;
            panel2.Location = new Point(0, 653);
            panel2.Name = "panel2";
            panel2.Size = new Size(927, 51);
            panel2.TabIndex = 1;
            // 
            // progressBar
            // 
            progressBar.Dock = DockStyle.Bottom;
            progressBar.Location = new Point(0, 28);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(927, 23);
            progressBar.TabIndex = 1;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(12, 3);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(0, 15);
            lblStatus.TabIndex = 0;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 100);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(clbExtensions);
            splitContainer1.Panel1.Controls.Add(btnFilter);
            splitContainer1.Panel1.Controls.Add(btnGenerateReport);
            splitContainer1.Panel1.Controls.Add(lblExtensions);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(tvFilteredFiles);
            splitContainer1.Panel2.Controls.Add(lblFileTree);
            splitContainer1.Size = new Size(927, 553);
            splitContainer1.SplitterDistance = 309;
            splitContainer1.TabIndex = 2;
            // 
            // clbExtensions
            // 
            clbExtensions.Dock = DockStyle.Fill;
            clbExtensions.FormattingEnabled = true;
            clbExtensions.Location = new Point(0, 15);
            clbExtensions.Name = "clbExtensions";
            clbExtensions.Size = new Size(309, 492);
            clbExtensions.TabIndex = 1;
            // 
            // btnFilter
            // 
            btnFilter.Dock = DockStyle.Bottom;
            btnFilter.Location = new Point(0, 507);
            btnFilter.Name = "btnFilter";
            btnFilter.Size = new Size(309, 23);
            btnFilter.TabIndex = 2;
            btnFilter.Text = "Filtrar";
            btnFilter.UseVisualStyleBackColor = true;
            btnFilter.Click += btnFilter_Click;
            // 
            // btnGenerateReport
            // 
            btnGenerateReport.Dock = DockStyle.Bottom;
            btnGenerateReport.Location = new Point(0, 530);
            btnGenerateReport.Name = "btnGenerateReport";
            btnGenerateReport.Size = new Size(309, 23);
            btnGenerateReport.TabIndex = 3;
            btnGenerateReport.Text = "Generar Reporte";
            btnGenerateReport.UseVisualStyleBackColor = true;
            btnGenerateReport.Click += btnGenerateReport_Click;
            // 
            // lblExtensions
            // 
            lblExtensions.AutoSize = true;
            lblExtensions.Dock = DockStyle.Top;
            lblExtensions.Location = new Point(0, 0);
            lblExtensions.Name = "lblExtensions";
            lblExtensions.Size = new Size(140, 15);
            lblExtensions.TabIndex = 0;
            lblExtensions.Text = "Extensiones encontradas:";
            // 
            // tvFilteredFiles
            // 
            tvFilteredFiles.Dock = DockStyle.Fill;
            tvFilteredFiles.Location = new Point(0, 15);
            tvFilteredFiles.Name = "tvFilteredFiles";
            tvFilteredFiles.Size = new Size(614, 538);
            tvFilteredFiles.TabIndex = 1;
            // 
            // lblFileTree
            // 
            lblFileTree.AutoSize = true;
            lblFileTree.Dock = DockStyle.Top;
            lblFileTree.Location = new Point(0, 0);
            lblFileTree.Name = "lblFileTree";
            lblFileTree.Size = new Size(102, 15);
            lblFileTree.TabIndex = 0;
            lblFileTree.Text = "Archivos filtrados:";
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(927, 704);
            Controls.Add(splitContainer1);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Main";
            Text = "Inicio";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Panel panel2;
        private SplitContainer splitContainer1;
        private Button btnSelectDirectory;
        private TextBox txtSelectedPath;
        private Button btnAnalyze;
        private Label lblTotalStats;
        private Label lblStatus;
        private ProgressBar progressBar;
        private Label lblExtensions;
        private CheckedListBox clbExtensions;
        private Button btnGenerateReport;
        private Button btnFilter;
        private Label lblFileTree;
        private TreeView tvFilteredFiles;
        private Label lblActiveFilters;
        private Label label1;
    }
}
