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
            this.chartPrintData.Size = new System.Drawing.Size(689, 488);
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
            this.chbxSeriesGraph.Location = new System.Drawing.Point(707, 45);
            this.chbxSeriesGraph.Name = "chbxSeriesGraph";
            this.chbxSeriesGraph.Size = new System.Drawing.Size(182, 488);
            this.chbxSeriesGraph.TabIndex = 3;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(898, 541);
            this.Controls.Add(this.chbxSeriesGraph);
            this.Controls.Add(this.cmbxVariableData);
            this.Controls.Add(this.chartPrintData);
            this.Name = "MainForm";
            this.Text = "Клиент";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chartPrintData)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.ComboBox cmbxVariableData;
        private System.Windows.Forms.CheckedListBox chbxSeriesGraph;

        private System.Windows.Forms.DataVisualization.Charting.Chart chartPrintData;

        #endregion
    }
}