namespace Plotter
{
  partial class Form2
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
      this.components = new System.ComponentModel.Container();
      this.BTN_Process = new System.Windows.Forms.Button();
      this.ZDC_ColorMap = new ZedGraph.ZedGraphControl();
      this.ColorImageBox = new Emgu.CV.UI.ImageBox();
      this.TB_From = new System.Windows.Forms.TextBox();
      this.TB_To = new System.Windows.Forms.TextBox();
      this.BTN_AdjustZedGraphs = new System.Windows.Forms.Button();
      this.ZDC_OpticalPlot = new ZedGraph.ZedGraphControl();
      this.export = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.ColorImageBox)).BeginInit();
      this.SuspendLayout();
      // 
      // BTN_Process
      // 
      this.BTN_Process.Location = new System.Drawing.Point(0, 2);
      this.BTN_Process.Name = "BTN_Process";
      this.BTN_Process.Size = new System.Drawing.Size(75, 23);
      this.BTN_Process.TabIndex = 0;
      this.BTN_Process.Text = "Process";
      this.BTN_Process.UseVisualStyleBackColor = true;
      this.BTN_Process.Click += new System.EventHandler(this.button1_Click);
      // 
      // ZDC_ColorMap
      // 
      this.ZDC_ColorMap.Location = new System.Drawing.Point(0, 32);
      this.ZDC_ColorMap.Margin = new System.Windows.Forms.Padding(4);
      this.ZDC_ColorMap.Name = "ZDC_ColorMap";
      this.ZDC_ColorMap.ScrollGrace = 0D;
      this.ZDC_ColorMap.ScrollMaxX = 0D;
      this.ZDC_ColorMap.ScrollMaxY = 0D;
      this.ZDC_ColorMap.ScrollMaxY2 = 0D;
      this.ZDC_ColorMap.ScrollMinX = 0D;
      this.ZDC_ColorMap.ScrollMinY = 0D;
      this.ZDC_ColorMap.ScrollMinY2 = 0D;
      this.ZDC_ColorMap.Size = new System.Drawing.Size(2539, 757);
      this.ZDC_ColorMap.TabIndex = 1;
      // 
      // ColorImageBox
      // 
      this.ColorImageBox.BackColor = System.Drawing.SystemColors.ActiveBorder;
      this.ColorImageBox.Location = new System.Drawing.Point(33, 796);
      this.ColorImageBox.Name = "ColorImageBox";
      this.ColorImageBox.Size = new System.Drawing.Size(2491, 606);
      this.ColorImageBox.TabIndex = 2;
      this.ColorImageBox.TabStop = false;
      // 
      // TB_From
      // 
      this.TB_From.Location = new System.Drawing.Point(124, 5);
      this.TB_From.Name = "TB_From";
      this.TB_From.Size = new System.Drawing.Size(53, 20);
      this.TB_From.TabIndex = 4;
      // 
      // TB_To
      // 
      this.TB_To.Location = new System.Drawing.Point(193, 4);
      this.TB_To.Name = "TB_To";
      this.TB_To.Size = new System.Drawing.Size(53, 20);
      this.TB_To.TabIndex = 5;
      // 
      // BTN_AdjustZedGraphs
      // 
      this.BTN_AdjustZedGraphs.Location = new System.Drawing.Point(253, 5);
      this.BTN_AdjustZedGraphs.Name = "BTN_AdjustZedGraphs";
      this.BTN_AdjustZedGraphs.Size = new System.Drawing.Size(56, 23);
      this.BTN_AdjustZedGraphs.TabIndex = 6;
      this.BTN_AdjustZedGraphs.Text = "Adjust";
      this.BTN_AdjustZedGraphs.UseVisualStyleBackColor = true;
      this.BTN_AdjustZedGraphs.Click += new System.EventHandler(this.BTN_AdjustZedGraphs_Click);
      // 
      // ZDC_OpticalPlot
      // 
      this.ZDC_OpticalPlot.Location = new System.Drawing.Point(1, 1117);
      this.ZDC_OpticalPlot.Margin = new System.Windows.Forms.Padding(4);
      this.ZDC_OpticalPlot.Name = "ZDC_OpticalPlot";
      this.ZDC_OpticalPlot.ScrollGrace = 0D;
      this.ZDC_OpticalPlot.ScrollMaxX = 0D;
      this.ZDC_OpticalPlot.ScrollMaxY = 0D;
      this.ZDC_OpticalPlot.ScrollMaxY2 = 0D;
      this.ZDC_OpticalPlot.ScrollMinX = 0D;
      this.ZDC_OpticalPlot.ScrollMinY = 0D;
      this.ZDC_OpticalPlot.ScrollMinY2 = 0D;
      this.ZDC_OpticalPlot.Size = new System.Drawing.Size(1, 1);
      this.ZDC_OpticalPlot.TabIndex = 3;
      // 
      // export
      // 
      this.export.Location = new System.Drawing.Point(367, 5);
      this.export.Name = "export";
      this.export.Size = new System.Drawing.Size(75, 23);
      this.export.TabIndex = 7;
      this.export.Text = "BTN_Export";
      this.export.UseVisualStyleBackColor = true;
      this.export.Click += new System.EventHandler(this.export_Click);
      // 
      // Form2
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(2544, 1402);
      this.Controls.Add(this.export);
      this.Controls.Add(this.BTN_AdjustZedGraphs);
      this.Controls.Add(this.TB_To);
      this.Controls.Add(this.TB_From);
      this.Controls.Add(this.ColorImageBox);
      this.Controls.Add(this.ZDC_OpticalPlot);
      this.Controls.Add(this.ZDC_ColorMap);
      this.Controls.Add(this.BTN_Process);
      this.Name = "Form2";
      this.Text = "Form2";
      this.Load += new System.EventHandler(this.Form2_Load);
      ((System.ComponentModel.ISupportInitialize)(this.ColorImageBox)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button BTN_Process;
    private ZedGraph.ZedGraphControl ZDC_ColorMap;
    private Emgu.CV.UI.ImageBox ColorImageBox;
    private System.Windows.Forms.TextBox TB_From;
    private System.Windows.Forms.TextBox TB_To;
    private System.Windows.Forms.Button BTN_AdjustZedGraphs;
    private ZedGraph.ZedGraphControl ZDC_OpticalPlot;
    private System.Windows.Forms.Button export;
  }
}