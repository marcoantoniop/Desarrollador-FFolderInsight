namespace ResportesParaDeveloper.Forms
{
    partial class ReportesForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportesForm));
            panel1 = new Panel();
            btnClose = new Button();
            btnSave = new Button();
            btnPrint = new Button();
            label1 = new Label();
            panel2 = new Panel();
            progressBar = new ProgressBar();
            lblStatus = new Label();
            rtbReport = new RichTextBox();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(btnClose);
            panel1.Controls.Add(btnSave);
            panel1.Controls.Add(btnPrint);
            panel1.Controls.Add(label1);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(742, 68);
            panel1.TabIndex = 0;
            // 
            // btnClose
            // 
            btnClose.Location = new Point(270, 37);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(126, 23);
            btnClose.TabIndex = 3;
            btnClose.Text = "Cerrar";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(141, 37);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(126, 23);
            btnSave.TabIndex = 2;
            btnSave.Text = "Guardar";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnPrint
            // 
            btnPrint.Location = new Point(12, 37);
            btnPrint.Name = "btnPrint";
            btnPrint.Size = new Size(126, 23);
            btnPrint.TabIndex = 1;
            btnPrint.Text = "Imprimir";
            btnPrint.UseVisualStyleBackColor = true;
            btnPrint.Click += btnPrint_Click;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(112, 25);
            label1.TabIndex = 0;
            label1.Text = "Mi Reporte";
            // 
            // panel2
            // 
            panel2.Controls.Add(progressBar);
            panel2.Controls.Add(lblStatus);
            panel2.Dock = DockStyle.Bottom;
            panel2.Location = new Point(0, 523);
            panel2.Name = "panel2";
            panel2.Size = new Size(742, 49);
            panel2.TabIndex = 1;
            // 
            // progressBar
            // 
            progressBar.Dock = DockStyle.Fill;
            progressBar.Location = new Point(0, 15);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(742, 34);
            progressBar.TabIndex = 1;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Dock = DockStyle.Top;
            lblStatus.Location = new Point(0, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(42, 15);
            lblStatus.TabIndex = 0;
            lblStatus.Text = "Estado";
            // 
            // rtbReport
            // 
            rtbReport.Dock = DockStyle.Fill;
            rtbReport.Location = new Point(0, 68);
            rtbReport.Name = "rtbReport";
            rtbReport.Size = new Size(742, 455);
            rtbReport.TabIndex = 2;
            rtbReport.Text = "";
            // 
            // ReportesForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(742, 572);
            Controls.Add(rtbReport);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "ReportesForm";
            Text = "Reporte";
            FormClosing += ReportesForm_FormClosing;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Panel panel2;
        private Label label1;
        private Button btnClose;
        private Button btnSave;
        private Button btnPrint;
        private RichTextBox rtbReport;
        private Label lblStatus;
        private ProgressBar progressBar;
    }
}