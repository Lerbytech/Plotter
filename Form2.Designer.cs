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
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      ((System.ComponentModel.ISupportInitialize)(this.imageBox1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.SuspendLayout();
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(0, 2);
      this.button1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(100, 28);
      this.button1.TabIndex = 0;
      this.button1.Text = "button1";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // ZDC_ColorMap
      // 
      this.ZDC_ColorMap.Location = new System.Drawing.Point(16, 38);
      this.ZDC_ColorMap.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
      this.ZDC_ColorMap.Name = "ZDC_ColorMap";
      this.ZDC_ColorMap.ScrollGrace = 0D;
      this.ZDC_ColorMap.ScrollMaxX = 0D;
      this.ZDC_ColorMap.ScrollMaxY = 0D;
      this.ZDC_ColorMap.ScrollMaxY2 = 0D;
      this.ZDC_ColorMap.ScrollMinX = 0D;
      this.ZDC_ColorMap.ScrollMinY = 0D;
      this.ZDC_ColorMap.ScrollMinY2 = 0D;
      this.ZDC_ColorMap.Size = new System.Drawing.Size(2480, 533);
      this.ZDC_ColorMap.TabIndex = 1;
      // 
      // imageBox1
      // 
      this.imageBox1.BackColor = System.Drawing.SystemColors.ControlDark;
      this.imageBox1.Location = new System.Drawing.Point(16, 578);
      this.imageBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
      this.imageBox1.Name = "imageBox1";
      this.imageBox1.Size = new System.Drawing.Size(2480, 414);
      this.imageBox1.TabIndex = 2;
      this.imageBox1.TabStop = false;
      // 
      // ZDC_OpticalPlot
      // 
      this.ZDC_OpticalPlot.Location = new System.Drawing.Point(16, 999);
      this.ZDC_OpticalPlot.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
      this.ZDC_OpticalPlot.Name = "ZDC_OpticalPlot";
      this.ZDC_OpticalPlot.ScrollGrace = 0D;
      this.ZDC_OpticalPlot.ScrollMaxX = 0D;
      this.ZDC_OpticalPlot.ScrollMaxY = 0D;
      this.ZDC_OpticalPlot.ScrollMaxY2 = 0D;
      this.ZDC_OpticalPlot.ScrollMinX = 0D;
      this.ZDC_OpticalPlot.ScrollMinY = 0D;
      this.ZDC_OpticalPlot.ScrollMinY2 = 0D;
      this.ZDC_OpticalPlot.Size = new System.Drawing.Size(2468, 268);
      this.ZDC_OpticalPlot.TabIndex = 3;
      // 
      // pictureBox1
      // 
      this.pictureBox1.Location = new System.Drawing.Point(235, 373);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(675, 675);
      this.pictureBox1.TabIndex = 4;
      this.pictureBox1.TabStop = false;
      // 
      // Form2
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1914, 1045);
      this.Controls.Add(this.pictureBox1);
      this.Controls.Add(this.ZDC_OpticalPlot);
      this.Controls.Add(this.imageBox1);
      this.Controls.Add(this.ZDC_ColorMap);
      this.Controls.Add(this.button1);
      this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
      this.Name = "Form2";
      this.Text = "Form2";
      ((System.ComponentModel.ISupportInitialize)(this.imageBox1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button button1;
    private ZedGraph.ZedGraphControl ZDC_ColorMap;
    private Emgu.CV.UI.ImageBox imageBox1;
    private ZedGraph.ZedGraphControl ZDC_OpticalPlot;
    private System.Windows.Forms.PictureBox pictureBox1;
  }
}