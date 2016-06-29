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
      this.button1 = new System.Windows.Forms.Button();
      this.ZDC_ColorMap = new ZedGraph.ZedGraphControl();
      this.imageBox1 = new Emgu.CV.UI.ImageBox();
      this.ZDC_OpticalPlot = new ZedGraph.ZedGraphControl();
      ((System.ComponentModel.ISupportInitialize)(this.imageBox1)).BeginInit();
      this.SuspendLayout();
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(0, 2);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(75, 23);
      this.button1.TabIndex = 0;
      this.button1.Text = "button1";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // ZDC_ColorMap
      // 
      this.ZDC_ColorMap.Location = new System.Drawing.Point(12, 31);
      this.ZDC_ColorMap.Name = "ZDC_ColorMap";
      this.ZDC_ColorMap.ScrollGrace = 0D;
      this.ZDC_ColorMap.ScrollMaxX = 0D;
      this.ZDC_ColorMap.ScrollMaxY = 0D;
      this.ZDC_ColorMap.ScrollMaxY2 = 0D;
      this.ZDC_ColorMap.ScrollMinX = 0D;
      this.ZDC_ColorMap.ScrollMinY = 0D;
      this.ZDC_ColorMap.ScrollMinY2 = 0D;
      this.ZDC_ColorMap.Size = new System.Drawing.Size(1860, 433);
      this.ZDC_ColorMap.TabIndex = 1;
      // 
      // imageBox1
      // 
      this.imageBox1.BackColor = System.Drawing.SystemColors.ControlDark;
      this.imageBox1.Location = new System.Drawing.Point(12, 470);
      this.imageBox1.Name = "imageBox1";
      this.imageBox1.Size = new System.Drawing.Size(1860, 336);
      this.imageBox1.TabIndex = 2;
      this.imageBox1.TabStop = false;
      // 
      // ZDC_OpticalPlot
      // 
      this.ZDC_OpticalPlot.Location = new System.Drawing.Point(12, 812);
      this.ZDC_OpticalPlot.Name = "ZDC_OpticalPlot";
      this.ZDC_OpticalPlot.ScrollGrace = 0D;
      this.ZDC_OpticalPlot.ScrollMaxX = 0D;
      this.ZDC_OpticalPlot.ScrollMaxY = 0D;
      this.ZDC_OpticalPlot.ScrollMaxY2 = 0D;
      this.ZDC_OpticalPlot.ScrollMinX = 0D;
      this.ZDC_OpticalPlot.ScrollMinY = 0D;
      this.ZDC_OpticalPlot.ScrollMinY2 = 0D;
      this.ZDC_OpticalPlot.Size = new System.Drawing.Size(1851, 218);
      this.ZDC_OpticalPlot.TabIndex = 3;
      // 
      // Form2
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1884, 1042);
      this.Controls.Add(this.ZDC_OpticalPlot);
      this.Controls.Add(this.imageBox1);
      this.Controls.Add(this.ZDC_ColorMap);
      this.Controls.Add(this.button1);
      this.Name = "Form2";
      this.Text = "Form2";
      ((System.ComponentModel.ISupportInitialize)(this.imageBox1)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button button1;
    private ZedGraph.ZedGraphControl ZDC_ColorMap;
    private Emgu.CV.UI.ImageBox imageBox1;
    private ZedGraph.ZedGraphControl ZDC_OpticalPlot;
  }
}