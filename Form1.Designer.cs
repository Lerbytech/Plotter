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
      this.SuspendLayout();
      // 
      // zedGraphControl
      // 
      this.zedGraphControl.Location = new System.Drawing.Point(12, 12);
      this.zedGraphControl.Name = "zedGraphControl";
      this.zedGraphControl.ScrollGrace = 0D;
      this.zedGraphControl.ScrollMaxX = 0D;
      this.zedGraphControl.ScrollMaxY = 0D;
      this.zedGraphControl.ScrollMaxY2 = 0D;
      this.zedGraphControl.ScrollMinX = 0D;
      this.zedGraphControl.ScrollMinY = 0D;
      this.zedGraphControl.ScrollMinY2 = 0D;
      this.zedGraphControl.Size = new System.Drawing.Size(981, 646);
      this.zedGraphControl.TabIndex = 1;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1005, 670);
      this.Controls.Add(this.zedGraphControl);
      this.Name = "Form1";
      this.Text = "Form1";
      this.ResumeLayout(false);

    }

    #endregion

    private ZedGraph.ZedGraphControl zedGraphControl;

  }
}

