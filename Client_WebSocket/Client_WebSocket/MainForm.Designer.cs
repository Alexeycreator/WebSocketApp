namespace Client_WebSocket
{
    partial class MainForm
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chartPrintData = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.cmbxVariableData = new System.Windows.Forms.ComboBox();
            this.chbxSeriesGraph = new System.Windows.Forms.CheckedListBox();
            this.btnCheckedAll = new System.Windows.Forms.Button();
            this.btnUnCheckedAll = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.chartPrintData)).BeginInit();
            this.SuspendLayout();
            // 
            // chartPrintData
            // 
            chartArea1.Name = "ChartArea1";
            this.chartPrintData.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chartPrintData.Legends.Add(legend1);
            this.chartPrintData.Location = new System.Drawing.Point(12, 45);
            this.chartPrintData.Name = "chartPrintData";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chartPrintData.Series.Add(series1);
            this.chartPrintData.Size = new System.Drawing.Size(967, 652);
            this.chartPrintData.TabIndex = 0;
            this.chartPrintData.Text = "chart1";
            // 
            // cmbxVariableData
            // 
            this.cmbxVariableData.Font = new System.Drawing.Font("Times New Roman", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cmbxVariableData.FormattingEnabled = true;
            this.cmbxVariableData.Location = new System.Drawing.Point(12, 12);
            this.cmbxVariableData.Name = "cmbxVariableData";
            this.cmbxVariableData.Size = new System.Drawing.Size(272, 27);
            this.cmbxVariableData.TabIndex = 1;
            this.cmbxVariableData.SelectedIndexChanged += new System.EventHandler(this.cmbxVariableData_SelectedIndexChanged);
            // 
            // chbxSeriesGraph
            // 
            this.chbxSeriesGraph.Font = new System.Drawing.Font("Times New Roman", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.chbxSeriesGraph.FormattingEnabled = true;
            this.chbxSeriesGraph.Location = new System.Drawing.Point(985, 12);
            this.chbxSeriesGraph.Name = "chbxSeriesGraph";
            this.chbxSeriesGraph.Size = new System.Drawing.Size(182, 576);
            this.chbxSeriesGraph.TabIndex = 3;
            // 
            // btnCheckedAll
            // 
            this.btnCheckedAll.Font = new System.Drawing.Font("Times New Roman", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnCheckedAll.Location = new System.Drawing.Point(985, 593);
            this.btnCheckedAll.Name = "btnCheckedAll";
            this.btnCheckedAll.Size = new System.Drawing.Size(182, 49);
            this.btnCheckedAll.TabIndex = 4;
            this.btnCheckedAll.Text = "Выделить все серии";
            this.btnCheckedAll.UseVisualStyleBackColor = true;
            this.btnCheckedAll.Click += new System.EventHandler(this.btnCheckedAll_Click);
            // 
            // btnUnCheckedAll
            // 
            this.btnUnCheckedAll.Font = new System.Drawing.Font("Times New Roman", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnUnCheckedAll.Location = new System.Drawing.Point(985, 648);
            this.btnUnCheckedAll.Name = "btnUnCheckedAll";
            this.btnUnCheckedAll.Size = new System.Drawing.Size(182, 49);
            this.btnUnCheckedAll.TabIndex = 5;
            this.btnUnCheckedAll.Text = "Снять выделение со всех серий";
            this.btnUnCheckedAll.UseVisualStyleBackColor = true;
            this.btnUnCheckedAll.Click += new System.EventHandler(this.btnUnCheckedAll_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1179, 709);
            this.Controls.Add(this.btnUnCheckedAll);
            this.Controls.Add(this.btnCheckedAll);
            this.Controls.Add(this.chbxSeriesGraph);
            this.Controls.Add(this.cmbxVariableData);
            this.Controls.Add(this.chartPrintData);
            this.Name = "MainForm";
            this.Text = "Клиент";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chartPrintData)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Button btnUnCheckedAll;

        private System.Windows.Forms.Button button1;

        private System.Windows.Forms.Button btnCheckedAll;
        
        private System.Windows.Forms.ComboBox cmbxVariableData;
        private System.Windows.Forms.CheckedListBox chbxSeriesGraph;

        private System.Windows.Forms.DataVisualization.Charting.Chart chartPrintData;

        #endregion
    }
}