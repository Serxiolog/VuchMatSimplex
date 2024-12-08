namespace VuchMatSimplex
{
    partial class Form1
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
            dataGridViewMatrix = new DataGridView();
            btnLoadData = new Button();
            btnApplyGauss = new Button();
            btnClearData = new Button();
            numericUpDownStep = new NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)dataGridViewMatrix).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownStep).BeginInit();
            SuspendLayout();
            // 
            // dataGridViewMatrix
            // 
            dataGridViewMatrix.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewMatrix.Dock = DockStyle.Top;
            dataGridViewMatrix.Location = new Point(0, 0);
            dataGridViewMatrix.Margin = new Padding(5, 6, 5, 6);
            dataGridViewMatrix.Name = "dataGridViewMatrix";
            dataGridViewMatrix.RowHeadersWidth = 62;
            dataGridViewMatrix.Size = new Size(1207, 385);
            dataGridViewMatrix.TabIndex = 0;
            // 
            // btnLoadData
            // 
            btnLoadData.Location = new Point(20, 423);
            btnLoadData.Margin = new Padding(5, 6, 5, 6);
            btnLoadData.Name = "btnLoadData";
            btnLoadData.Size = new Size(125, 44);
            btnLoadData.TabIndex = 1;
            btnLoadData.Text = "Load Data";
            btnLoadData.UseVisualStyleBackColor = true;
            btnLoadData.Click += btnLoadData_Click;
            // 
            // btnApplyGauss
            // 
            btnApplyGauss.Location = new Point(155, 423);
            btnApplyGauss.Margin = new Padding(5, 6, 5, 6);
            btnApplyGauss.Name = "btnApplyGauss";
            btnApplyGauss.Size = new Size(125, 44);
            btnApplyGauss.TabIndex = 2;
            btnApplyGauss.Text = "Solve";
            btnApplyGauss.UseVisualStyleBackColor = true;
            btnApplyGauss.Click += btnApplyGauss_Click;
            // 
            // btnClearData
            // 
            btnClearData.Location = new Point(290, 423);
            btnClearData.Margin = new Padding(5, 6, 5, 6);
            btnClearData.Name = "btnClearData";
            btnClearData.Size = new Size(125, 44);
            btnClearData.TabIndex = 3;
            btnClearData.Text = "Clear Data";
            btnClearData.UseVisualStyleBackColor = true;
            btnClearData.Click += btnClearData_Click;
            // 
            // numericUpDownStep
            // 
            numericUpDownStep.Location = new Point(425, 423);
            numericUpDownStep.Margin = new Padding(5, 6, 5, 6);
            numericUpDownStep.Name = "numericUpDownStep";
            numericUpDownStep.Size = new Size(200, 31);
            numericUpDownStep.TabIndex = 4;
            numericUpDownStep.ValueChanged += numericUpDownStep_ValueChanged;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1207, 482);
            Controls.Add(numericUpDownStep);
            Controls.Add(btnClearData);
            Controls.Add(btnApplyGauss);
            Controls.Add(btnLoadData);
            Controls.Add(dataGridViewMatrix);
            Margin = new Padding(5, 6, 5, 6);
            Name = "Form1";
            Text = "Simplex";
            ((System.ComponentModel.ISupportInitialize)dataGridViewMatrix).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownStep).EndInit();
            ResumeLayout(false);
        }

        private System.Windows.Forms.DataGridView dataGridViewMatrix;
        private System.Windows.Forms.Button btnLoadData;
        private System.Windows.Forms.Button btnApplyGauss;
        private System.Windows.Forms.Button btnClearData;
        private System.Windows.Forms.NumericUpDown numericUpDownStep;
        #endregion
    }
}
