namespace Plotter
{
  partial class Form1
  {
    /// <summary>
    /// Требуется переменная конструктора.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Освободить все используемые ресурсы.
    /// </summary>
    /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Код, автоматически созданный конструктором форм Windows

    /// <summary>
    /// Обязательный метод для поддержки конструктора - не изменяйте
    /// содержимое данного метода при помощи редактора кода.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      this.zedGraphControl = new ZedGraph.ZedGraphControl();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.NeuronSelector = new System.Windows.Forms.ComboBox();
      this.AllSignalZedGraph = new ZedGraph.ZedGraphControl();
      this.label1 = new System.Windows.Forms.Label();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.label2 = new System.Windows.Forms.Label();
      this.WindowWindthNUD = new System.Windows.Forms.NumericUpDown();
      this.DrawSparklesChB = new System.Windows.Forms.CheckBox();
      this.DrawSelectLevelChB = new System.Windows.Forms.CheckBox();
      this.DrawSigmaChB = new System.Windows.Forms.CheckBox();
      this.drawAverageChB = new System.Windows.Forms.CheckBox();
      this.groupBox1.SuspendLayout();
      this.groupBox2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.WindowWindthNUD)).BeginInit();
      this.SuspendLayout();
      // 
      // zedGraphControl
      // 
      this.zedGraphControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.zedGraphControl.Location = new System.Drawing.Point(2, 76);
      this.zedGraphControl.Name = "zedGraphControl";
      this.zedGraphControl.ScrollGrace = 0D;
      this.zedGraphControl.ScrollMaxX = 0D;
      this.zedGraphControl.ScrollMaxY = 0D;
      this.zedGraphControl.ScrollMaxY2 = 0D;
      this.zedGraphControl.ScrollMinX = 0D;
      this.zedGraphControl.ScrollMinY = 0D;
      this.zedGraphControl.ScrollMinY2 = 0D;
      this.zedGraphControl.Size = new System.Drawing.Size(989, 303);
      this.zedGraphControl.TabIndex = 1;
      this.zedGraphControl.Load += new System.EventHandler(this.zedGraphControl_Load);
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.NeuronSelector);
      this.groupBox1.Location = new System.Drawing.Point(9, 12);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(138, 58);
      this.groupBox1.TabIndex = 2;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Нейрон";
      // 
      // NeuronSelector
      // 
      this.NeuronSelector.FormattingEnabled = true;
      this.NeuronSelector.Location = new System.Drawing.Point(6, 19);
      this.NeuronSelector.Name = "NeuronSelector";
      this.NeuronSelector.Size = new System.Drawing.Size(121, 21);
      this.NeuronSelector.TabIndex = 0;
      this.NeuronSelector.SelectedValueChanged += new System.EventHandler(this.NeuronSelector_SelectedValueChanged);
      // 
      // AllSignalZedGraph
      // 
      this.AllSignalZedGraph.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.AllSignalZedGraph.Location = new System.Drawing.Point(2, 398);
      this.AllSignalZedGraph.Name = "AllSignalZedGraph";
      this.AllSignalZedGraph.ScrollGrace = 0D;
      this.AllSignalZedGraph.ScrollMaxX = 0D;
      this.AllSignalZedGraph.ScrollMaxY = 0D;
      this.AllSignalZedGraph.ScrollMaxY2 = 0D;
      this.AllSignalZedGraph.ScrollMinX = 0D;
      this.AllSignalZedGraph.ScrollMinY = 0D;
      this.AllSignalZedGraph.ScrollMinY2 = 0D;
      this.AllSignalZedGraph.Size = new System.Drawing.Size(989, 149);
      this.AllSignalZedGraph.TabIndex = 1;
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(-1, 382);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(70, 13);
      this.label1.TabIndex = 3;
      this.label1.Text = "Весь сигнал";
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.label2);
      this.groupBox2.Controls.Add(this.WindowWindthNUD);
      this.groupBox2.Controls.Add(this.DrawSparklesChB);
      this.groupBox2.Controls.Add(this.DrawSelectLevelChB);
      this.groupBox2.Controls.Add(this.DrawSigmaChB);
      this.groupBox2.Controls.Add(this.drawAverageChB);
      this.groupBox2.Location = new System.Drawing.Point(153, 12);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(537, 57);
      this.groupBox2.TabIndex = 4;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Параметры нейрона";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(394, 22);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(82, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "Окно подсчета";
      // 
      // WindowWindthNUD
      // 
      this.WindowWindthNUD.Location = new System.Drawing.Point(493, 18);
      this.WindowWindthNUD.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
      this.WindowWindthNUD.Minimum = new decimal(new int[] {
            3,
            0,
            0,
            0});
      this.WindowWindthNUD.Name = "WindowWindthNUD";
      this.WindowWindthNUD.Size = new System.Drawing.Size(38, 20);
      this.WindowWindthNUD.TabIndex = 1;
      this.WindowWindthNUD.Value = new decimal(new int[] {
            40,
            0,
            0,
            0});
      this.WindowWindthNUD.ValueChanged += new System.EventHandler(this.WindowWindthNUD_ValueChanged);
      // 
      // DrawSparklesChB
      // 
      this.DrawSparklesChB.AutoSize = true;
      this.DrawSparklesChB.Checked = true;
      this.DrawSparklesChB.CheckState = System.Windows.Forms.CheckState.Checked;
      this.DrawSparklesChB.Location = new System.Drawing.Point(273, 19);
      this.DrawSparklesChB.Name = "DrawSparklesChB";
      this.DrawSparklesChB.Size = new System.Drawing.Size(73, 17);
      this.DrawSparklesChB.TabIndex = 0;
      this.DrawSparklesChB.Text = "Вспышки";
      this.DrawSparklesChB.UseVisualStyleBackColor = true;
      this.DrawSparklesChB.CheckedChanged += new System.EventHandler(this.DrawSparklesChB_CheckedChanged);
      // 
      // DrawSelectLevelChB
      // 
      this.DrawSelectLevelChB.AutoSize = true;
      this.DrawSelectLevelChB.Checked = true;
      this.DrawSelectLevelChB.CheckState = System.Windows.Forms.CheckState.Checked;
      this.DrawSelectLevelChB.Location = new System.Drawing.Point(154, 19);
      this.DrawSelectLevelChB.Name = "DrawSelectLevelChB";
      this.DrawSelectLevelChB.Size = new System.Drawing.Size(113, 17);
      this.DrawSelectLevelChB.TabIndex = 0;
      this.DrawSelectLevelChB.Text = "Среднее + Сигма";
      this.DrawSelectLevelChB.UseVisualStyleBackColor = true;
      this.DrawSelectLevelChB.CheckedChanged += new System.EventHandler(this.DrawSelectLevelChB_CheckedChanged);
      // 
      // DrawSigmaChB
      // 
      this.DrawSigmaChB.AutoSize = true;
      this.DrawSigmaChB.Checked = true;
      this.DrawSigmaChB.CheckState = System.Windows.Forms.CheckState.Checked;
      this.DrawSigmaChB.Location = new System.Drawing.Point(90, 19);
      this.DrawSigmaChB.Name = "DrawSigmaChB";
      this.DrawSigmaChB.Size = new System.Drawing.Size(58, 17);
      this.DrawSigmaChB.TabIndex = 0;
      this.DrawSigmaChB.Text = "Сигма";
      this.DrawSigmaChB.UseVisualStyleBackColor = true;
      this.DrawSigmaChB.CheckedChanged += new System.EventHandler(this.DrawSigmaChB_CheckedChanged);
      // 
      // drawAverageChB
      // 
      this.drawAverageChB.AutoSize = true;
      this.drawAverageChB.Checked = true;
      this.drawAverageChB.CheckState = System.Windows.Forms.CheckState.Checked;
      this.drawAverageChB.Location = new System.Drawing.Point(15, 19);
      this.drawAverageChB.Name = "drawAverageChB";
      this.drawAverageChB.Size = new System.Drawing.Size(69, 17);
      this.drawAverageChB.TabIndex = 0;
      this.drawAverageChB.Text = "Среднее";
      this.drawAverageChB.UseVisualStyleBackColor = true;
      this.drawAverageChB.CheckedChanged += new System.EventHandler(this.drawAverageChB_CheckedChanged);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1003, 545);
      this.Controls.Add(this.groupBox2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.AllSignalZedGraph);
      this.Controls.Add(this.zedGraphControl);
      this.Name = "Form1";
      this.Text = "Form1";
      this.Load += new System.EventHandler(this.Form1_Load);
      this.groupBox1.ResumeLayout(false);
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.WindowWindthNUD)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private ZedGraph.ZedGraphControl zedGraphControl;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.ComboBox NeuronSelector;
    private ZedGraph.ZedGraphControl AllSignalZedGraph;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.NumericUpDown WindowWindthNUD;
    private System.Windows.Forms.CheckBox DrawSparklesChB;
    private System.Windows.Forms.CheckBox DrawSelectLevelChB;
    private System.Windows.Forms.CheckBox DrawSigmaChB;
    private System.Windows.Forms.CheckBox drawAverageChB;

  }
}

