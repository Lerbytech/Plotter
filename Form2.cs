using System; 
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.Util;
using System.Diagnostics;
using ZedGraph;

namespace Plotter
{
  public partial class Form2 : Form
  {
    int LeftPoint = 0;
    int RightPoint = 0;

    Image<Bgr, Byte> ColorImg;
    MasterPane MP;
    bool isMousePressed = false;
    int posX;
    int posY;

    public Form2()
    {
      InitializeComponent();

      //ZDC_ColorMap.MouseDownEvent += new ZedGraphControl.ZedMouseEventHandler(zedGraph_MouseDownEvent);
      //ZDC_ColorMap.MouseUpEvent += new ZedGraphControl.ZedMouseEventHandler(zedGraph_MouseUpEvent);
    }

    #region функции для двиганья графиков, да

    private void BTN_AdjustZedGraphs_Click(object sender, EventArgs e)
    {
      double left = 0, right = 100;
      bool test = double.TryParse(TB_From.Text, out left);
      test = double.TryParse(TB_To.Text, out right);
      LeftPoint = (int)left;
      RightPoint = (int)right;
      MoveImage(LeftPoint, RightPoint);
    }

    private void Form2_Load(object sender, EventArgs e)
    {
      ZedGraph.MasterPane masterPane = ZDC_ColorMap.MasterPane;
      ZDC_ColorMap.IsSynchronizeXAxes = true;
      ZDC_ColorMap.IsSynchronizeYAxes = false;


      //masterPane.PaneList.Clear();
      masterPane.Add(ZDC_OpticalPlot.GraphPane);

      using (Graphics g = CreateGraphics())
      {
        // Графики будут размещены в один столбец друг под другом
        masterPane.SetLayout(g, PaneLayout.SingleColumn);
      }

      int topMargin = 0;
      int leftMargin = 25;
      int bottomMargin = 25;
      int rightMargin = 25;
      int spacingY = 1;
      float labelGapY = 1;
      float labelGapX = 1;


      //masterPane[0].Chart.Rect = new RectangleF(leftMargin, topMargin, ZDC_ColorMap.Width - leftMargin - rightMargin, ZDC_ColorMap.Height / 2 - topMargin - spacingY);
      //masterPane[1].Chart.Rect = new RectangleF(leftMargin, ZDC_ColorMap.Height / 2 + spacingY, ZDC_ColorMap.Width - leftMargin - rightMargin, ZDC_ColorMap.Height / 2 - spacingY - bottomMargin);

      masterPane[0].Margin.Top = 0.0f;
      masterPane[0].Margin.Left = 0.0f;
      masterPane[0].Margin.Right = 20.0f; //20
      masterPane[0].Margin.Bottom = 0.0f;

      masterPane[1].Margin.Top = 10.0f; //10
      masterPane[1].Margin.Left = 0.0f;
      masterPane[1].Margin.Right = 0.0f;
      masterPane[1].Margin.Bottom = 0.0f;

      masterPane[0].XAxis.Title.IsVisible = false;
      masterPane[0].YAxis.Title.IsVisible = false;
      masterPane[0].Title.IsVisible = false;
      masterPane[0].XAxis.Scale.IsVisible = false;
      masterPane[0].XAxis.Scale.LabelGap = labelGapY;
      masterPane.InnerPaneGap = 0.0f;
      masterPane[1].XAxis.Title.IsVisible = false;
      masterPane[1].YAxis.Title.IsVisible = false;
      masterPane[1].Title.IsVisible = false;
      masterPane[1].XAxis.Scale.LabelGap = labelGapX;
      masterPane[1].YAxis.Scale.LabelGap = labelGapY;
      ZDC_ColorMap.AxisChange();
      ZDC_ColorMap.Invalidate();

      //masterPane.Margin.Bottom = 5;
      int rr = (int)masterPane[0].Chart.Rect.Width;
      RectangleF rect = masterPane.Rect;

      ZDC_OpticalPlot.GraphPane.AxisChangeEvent += GraphPane_AxisChangeEvent;
    }

    private void GraphPane_AxisChangeEvent(GraphPane pane)
    {
      TB_From.Text = ZDC_ColorMap.MasterPane[0].XAxis.Scale.Min.ToString();
      TB_To.Text = ZDC_ColorMap.MasterPane[0].XAxis.Scale.Max.ToString();
      BTN_AdjustZedGraphs.PerformClick();
    }


    public bool zedGraph_MouseDownEvent(ZedGraphControl sender, MouseEventArgs e)
    {
      isMousePressed = true;
      posX = e.X;
      posY = e.Y;

      TB_From.Text = posX.ToString();
      TB_To.Text = e.X.ToString();
      return false;
    }

    public bool zedGraph_MouseUpEvent(ZedGraphControl sender, MouseEventArgs e)
    {
      if (isMousePressed)
      {
        // Координаты, которые переданы в событие
        PointF eventPoint = new PointF(e.X, e.Y);
        GraphPane pane = new GraphPane();
        object nearestObj = new object();
        int index = 0;

        using (Graphics g = sender.CreateGraphics())
        {
          MP.FindNearestPaneObject(eventPoint, g, out pane, out nearestObj, out index);
        }

        // Пересчитать координаты из системы координат, связанной с контролом zedGraph 
        // в систему координат, связанную с графиком
        double newX;
        double newY;
        double oldX;
        double oldY;

        pane.ReverseTransform(new PointF(posX, posY), out oldX, out oldY);
        pane.ReverseTransform(new PointF(e.X, e.Y), out newX, out newY);
        MoveImage(oldX, oldY, newX, newY);
      }
      return false;
    }

    private void MoveImage(double oldX, double oldY, double newX, double newY)
    {
      double Mul = ColorImageBox.Width * 8700;
      LeftPoint = (int)Math.Round(Mul * oldX);
      RightPoint = (int)Math.Round(Mul * newX);

      ColorImageBox.Image = ColorImg.Copy(new Rectangle(LeftPoint, 0, RightPoint, ColorImageBox.Height));
      ColorImageBox.SizeMode = PictureBoxSizeMode.StretchImage;
      TB_From.Text = LeftPoint.ToString();
      TB_To.Text = RightPoint.ToString();
    }

    private void MoveImage(int L, int R)
    {/*
      double Mul = ColorImageBox.Width * 8700;
      LeftPoint = (int)Math.Round(Mul * oldX);
      RightPoint = (int)Math.Round(Mul * newX);
      */
      //ColorImageBox.Image = ColorImg.Copy(new Rectangle(L, 0, R - L, ColorImageBox.Height));
      ColorImageBox.Image = ColorIMG_Copy(ColorImg, L, R);

      ColorImageBox.SizeMode = PictureBoxSizeMode.StretchImage;
    }

    #endregion


    private void button1_Click(object sender, EventArgs e)
    {
      //SmoothSparkles();
      //DrawGray();

      Image<Bgr, Int32> Img = GetCorrelationImage();
      Img.Save(@"C:\Users\Admin\Desktop\TempFolder\CorrelationMatrix.png");
      //imageBox2.Image = Img;
      //pictureBox1.Image = Img.Resize(45, Inter.Nearest).Bitmap;
      //ColorMap();
      ColorImg = IMGColorMap();
      RightPoint = ColorImg.Width;
      LeftPoint = 0;
      TB_From.Text = Left.ToString(); TB_To.Text = RightPoint.ToString();

      //ColorImageBox.Image = ColorImg.Copy(new Rectangle(LeftPoint, 0, RightPoint, ColorImageBox.Height));
      ColorImageBox.Image = ColorIMG_Copy(ColorImg, LeftPoint, RightPoint);

      ColorImageBox.SizeMode = PictureBoxSizeMode.StretchImage;
      PlotNormalisedSignals();
      Tuple<double[], double[]> corr_data = ClusterLines();

      //TestCorr();
      //Correlate(corr_data);
               
      Form3 window_x = new Form3();
      window_x.Show();
            
      //Stuff();
    }


    public Image<Bgr, Byte> ColorIMG_Copy(Image<Bgr, Byte> src, int L, int R)
    {
      Image<Bgr, Byte> res = new Image<Bgr, byte>(R - L + 1, src.Height, new Bgr(0,0,0));


      int width = res.Width;
      int height = res.Height;
      int N = 0;
      byte[, ,] srcDATA = src.Data;
      byte[, ,] resDATA = res.Data;

      for (int y = 0; y < height && y < src.Height; y++)
        for (int x = 0; x < width && x < src.Width; x++)
        {
          resDATA[y, x, 0] = srcDATA[y, x + L, 0];
          resDATA[y, x, 1] = srcDATA[y, x + L, 1];
          resDATA[y, x, 2] = srcDATA[y, x + L, 2];
        }                        

      return res;
    }



    //private void ColorMap()
    //{
    //  // настроить ZedGraph
    //  Stopwatch timer1 = new Stopwatch();
    //  long l = 0;
    //  GraphPane pane = ZDC_ColorMap.GraphPane;
    //  pane.CurveList.Clear();
    //  pane.XAxis.Title.Text = "Ось X";
    //  pane.XAxis.Title.Text = "Ось Y";


    //  LineItem myCurve;
    //  // разбить на линии zedgraph

    //  Dictionary<int, SingleNeuron> Neurons = NeuronDataManager.Neurons;

    //  //List<List<PointD>>[] Sparkles = new List<List<PointD>>[Neurons.Count];

    //  int HeightOfLine = 1; // ZDC_ColorMap.Height / Neurons.Count;
    //  int VerticalSpacing = HeightOfLine;
    //  int DiscretisationRate = 5;

    //  double[] x = new double[5];
    //  double[] y = new double[5];
    //  double LocalMax;
    //  double TotalMax;
    //  double candidate;
    //  double r;
    //  double rel;
    //  double x0;
    //  double y0;
    //  Color col;
    //  List<PointD> tmpList = new List<PointD>();
    //  PointD[] tmpArr;
    //  foreach (KeyValuePair<int, SingleNeuron> pair in Neurons) // НЕЙРОНЫ
    //  {
    //    timer1.Start();
    //    /*
    //    double TotalMax = double.MinValue;
    //    double candidate = TotalMax;
    //    for (int i = 0; i < pair.Value.Sparkles.Count; i++)
    //    {
    //      candidate = pair.Value.Sparkles[i].Max(p => p.Y);
    //      if ( TotalMax < candidate )
    //      TotalMax = candidate;
    //    }
    //    */

        
    //    for (int sparkleID = 0; sparkleID < pair.Value.Sparkles.Count; sparkleID++) // ВСПЫШКИ
    //    {
    //      //tmpList = pair.Value.Sparkles[sparkleID];
    //      tmpArr = pair.Value.Sparkles[sparkleID].ToArray();
    //      TotalMax = tmpArr.Max(p => p.Y);


    //      for (int intsparkleID = 0; intsparkleID < tmpArr.Length; intsparkleID += DiscretisationRate) // ВНУТРИ ВСПЫШЕК
    //      {
    //        //
    //        LocalMax = tmpArr[intsparkleID].Y;
    //        candidate = LocalMax;
    //        for (int k = intsparkleID + 1; k < DiscretisationRate && k < tmpArr.Length; k++)
    //        {
    //          candidate = tmpArr[k].Y;
    //          if (LocalMax < candidate)
    //            LocalMax = candidate;
    //        }
            
    //        //
    //        x0 = tmpArr[intsparkleID].X;
    //        x[0] = x0;
    //        x[1] = x0 + DiscretisationRate;
    //        x[2] = x0 + DiscretisationRate;
    //        x[3] = x0;
    //        x[4] = x0;

    //        y0 = pair.Value.ID;
    //        y[0] = ( y0 + 1 )* HeightOfLine;
    //        y[1] = (y0 + 1) * HeightOfLine;
    //        y[2] = y0 * HeightOfLine;
    //        y[3] = y0 * HeightOfLine;
    //        y[4] = (y0 + 1) * HeightOfLine;

    //        //
    //        r = LocalMax / TotalMax * (740 - 380) + 380;
    //        col =  Plotter.Colors.waveToColor(r);
    //        myCurve = pane.AddCurve("", x, y, col, SymbolType.None);
    //        myCurve.Line.Fill = new ZedGraph.Fill(col);
            
    //        //ZDC_ColorMap.AxisChange();
    //        //ZDC_ColorMap.Invalidate();
    //        //ZDC_ColorMap.GetImage().Save(@"C:\Users\Admin\Desktop\Антон\EXPERIMENTS\Processed\Processed\2CH_65 part 3\img_" + sparkleID.ToString() + "_" + intsparkleID.ToString() + ".png");
    //      }
    //    }
     
    //    //ZDC_ColorMap.GetImage().Save(@"C:\Users\Admin\Desktop\Антон\EXPERIMENTS\Processed\Processed\2CH_65 part 3\img_" + pair.Value.ID.ToString() + ".png");
    //    //ZDC_ColorMap.GetImage().Save(@"C:\Users\Админ\Desktop\НИР\EXPERIMENTS\Separated\TEST\img_" + pair.Value.ID.ToString() + ".png");
    //    timer1.Stop();
    //    l += timer1.ElapsedMilliseconds;
    //  }

    //  timer1.Stop();
      
    //  // подписать оси
    //  // для каждого нейрона получить окраску вспышек
    //  // нарисовать тепловую карту 
    //  //ZDC_ColorMap.GetImage().Save(@"C:\Users\Admin\Desktop\Антон\EXPERIMENTS\Processed\Processed\2CH_65 part 3\img_FINAL.png");
    //  timer1.Start();
    //  Size oldSize = ZDC_ColorMap.Size;
    //  ZDC_ColorMap.Size = new Size(8700, 92 * 50);
    //  ZDC_ColorMap.AxisChange();
    //  ZDC_ColorMap.Invalidate();
    //  ZDC_ColorMap.GetImage().Save(@"C:\Users\Admin\Desktop\Антон\EXPERIMENTS\Processed\Processed\img_FINAL.png");
    //  ZDC_ColorMap.Size = oldSize;
    //  timer1.Stop();
    //  l = timer1.ElapsedMilliseconds;
    //  //5740121
    //  //1771624
      
    //}

#region
    private Image<Bgr, Byte> IMGColorMap()
    {
      // настроить ZedGraph
      Stopwatch timer1 = new Stopwatch();
      long l = 0; 
     
      Dictionary<int, SingleNeuron> Neurons = NeuronDataManager.Neurons;


      int PixelHeight = 20; 
      int PixelWidth = 5;
      int DiscretisationRate = 5;

      double LocalMax;
      double TotalMax;
      double candidate;

      List<PointD> tmpList = new List<PointD>();
      PointD[] tmpArr;

      Image<Bgr, Byte> RESIMG = new Image<Bgr, byte>(NeuronDataManager.Neurons[0].IntensityCleanData.Count * PixelWidth / DiscretisationRate,
                                                     NeuronDataManager.Neurons.Count * PixelHeight,
                                                     new Bgr(255, 255, 255));
      int[] _colors = new int[3];
      byte[] colors = new byte[3];

      int width = RESIMG.Width;
      int height = RESIMG.Height;
      int N = 0;
      byte[, ,] DATA = RESIMG.Data;

      foreach (KeyValuePair<int, SingleNeuron> pair in Neurons) // НЕЙРОНЫ
      {
        timer1.Start();
        
        for (int sparkleID = 0; sparkleID < pair.Value.Sparkles.Count; sparkleID++) // ВСПЫШКИ
        {
          tmpArr = pair.Value.Sparkles[sparkleID].ToArray();
          TotalMax = tmpArr.Max(p => p.Y);

          for (int intsparkleID = 0; intsparkleID < tmpArr.Length; intsparkleID += DiscretisationRate) // ВНУТРИ ВСПЫШЕК
          {
            //
            LocalMax = tmpArr[intsparkleID].Y;
            candidate = LocalMax;
            for (int k = intsparkleID + 1; k < DiscretisationRate && k < tmpArr.Length; k++)
            {
              candidate = tmpArr[k].Y;
              if (LocalMax < candidate)
                LocalMax = candidate;
            }

            //
            _colors = Plotter.Colors.waveLengthToRGB(LocalMax / TotalMax * (740 - 380) + 380);
            colors[0] = (byte)_colors[2];
            colors[1] = (byte)_colors[1];
            colors[2] = (byte)_colors[0];


            for (int x = (int)tmpArr[intsparkleID].X; x < tmpArr[intsparkleID].X + DiscretisationRate && x < width; x++)
              for (int y = pair.Value.ID * PixelHeight; y < pair.Value.ID * PixelHeight + PixelHeight && y < height; y++)
              {
                //IMG_DATA[y, x, 0] = (byte)colors[2];
                //IMG_DATA[y, x, 1] = (byte)colors[1];
                //IUMG_DATA[y, x, 2] = (byte)colors[0];
                DATA[y, x, 0] = colors[0];
                DATA[y, x, 1] = colors[1];
                DATA[y, x, 2] = colors[2];
              }
          }
        }
        
        
        timer1.Stop();
        l += timer1.ElapsedMilliseconds;
      }

        l = 10;
        return RESIMG;
    }
    
    private List<List<double>> GetNormalisedData()
    {
      List<List<double>> Normalised = new List<List<double>>();
      foreach (KeyValuePair<int, SingleNeuron> pair in NeuronDataManager.Neurons)
      {
        Normalised.Add(pair.Value.IntensityCleanData);
      }

      double MaxVal = 0;
      for (int i = 0; i < Normalised.Count; i++)
      {
        MaxVal = Normalised[i].Max();
        for (int j = 0; j < Normalised[i].Count; j++)
          Normalised[i][j] /= MaxVal;
      }

      return Normalised;
    }

    private List<List<List<PointD>>> GetNormalisedSparkles()
    {
      double TotalMax = double.MinValue;
      double candidate = TotalMax;
      for (int i = 0; i < NeuronDataManager.Neurons.Count; i++)
      {
        for (int j = 0; j < NeuronDataManager.Neurons[i].Sparkles.Count; j++)
        {
          candidate = NeuronDataManager.Neurons[i].Sparkles[j].Max(p => p.Y);
          if (TotalMax < candidate) TotalMax = candidate;
        }
      }

      List<List<List<PointD>>> res = new List<List<List<PointD>>>();
      List<List<PointD>> TMP1 = new List<List<PointD>>();
      List<PointD> TMP2 = new List<PointD>();

      for (int i = 0; i < NeuronDataManager.Neurons.Count; i++)
      {
        for (int j = 0; j < NeuronDataManager.Neurons[i].Sparkles.Count; j++)
        {
          TMP1 = new List<List<PointD>>();
          TMP2 = new List<PointD>();
          for (int k = 0; k < NeuronDataManager.Neurons[i].Sparkles[j].Count; k++)
            TMP2.Add ( new PointD( NeuronDataManager.Neurons[i].Sparkles[j][k].X, NeuronDataManager.Neurons[i].Sparkles[j][k].Y / TotalMax));
          TMP1.Add(TMP2);
        }
        res.Add(TMP1);
      }

      return res;
    }
  
    private void PlotNormalisedSignals()
    {
      List<List<double>> Normalised = GetNormalisedData();

      List<double> X = new List<double>();
      for (int i = 0; i < Normalised[0].Count; i++)
        X.Add(i);

      GraphPane pane1 = ZDC_ColorMap.GraphPane;
      pane1.XAxis.Title.Text = "Время (мс)";
      pane1.YAxis.Title.Text = "Номер нейрона";

      pane1.CurveList.Clear();
      //pane1.XAxis.Scale.Max = X.Count + 5;
      pane1.XAxis.Scale.Max = X.Count + 5;

      pane1.YAxis.Scale.Max = 1;

      for (int i = 0; i < Normalised.Count; i++)
      {
        PointPairList list1 = new PointPairList(X.ToArray(), Normalised[i].ToArray());
        LineItem myCurve1 = pane1.AddCurve("", list1, Plotter.Colors.waveToColor(i * (740 - 380)/Normalised.Count + 380), SymbolType.None);
     
      
        //bp1.Save(Path_toSave + filename + ".png");
      }
      //Image bp1 = ZDC_ColorMap.GetImage();

      ZDC_ColorMap.AxisChange();
      ZDC_ColorMap.Invalidate();
    }

    private Image<Bgr, Int32> GetCorrelationImage()
    {
      double[,] CorrMatrix = GetCorrelationMatrix();
      int N = (int)Math.Sqrt(CorrMatrix.Length);

      double Max = double.MinValue;
      for (int i = 0; i < N; i++)
        for (int j = 0; j < N; j++)
        {
          if (CorrMatrix[i, j] > Max) Max = CorrMatrix[i, j];
        }

      Image<Bgr, Int32> CORRimg = new Image<Bgr, Int32>(N,N);
      for (int i = 0; i < N; i++)
      {
        for (int j = 0; j < N; j++)
        {
          int[] rr = Plotter.Colors.waveLengthToRGB(CorrMatrix[i, j] / Max * (740 - 380) + 380);
          CORRimg[i, j] = new Bgr(rr[2], rr[1], rr[0]);
        }
      }
      /*
      for (int i = 0; i < N; i++)
        CORRimg[i, i] = new Bgr(0, 0, 0);
      */
      return CORRimg;
    }

    private double[,] GetCorrelationMatrix()
    {
      List<List<double>> Normalised = GetNormalisedData();
     
      //подсчет матрицы
      int N = Normalised.Count;
      int L = Normalised[0].Count;
      double[,] Corr = new double[N, N];

      for (int i = 0; i < N; i++)
      {
        for (int j = i; j < N; j++)
        {
          double C = 0;
          for (int R = 0; R < L; R++)
          {
            C += Normalised[i][R] * Normalised[j][R];
          }
          C /= L;
          Corr[i, j] = C;
          Corr[j, i] = C;
        }
      }
      
      // нормализация
      double tmp_val;
      for (int i = 0; i < N; i++)
      {
        tmp_val = Corr[i, i];
        for (int j = i; j < N; j++)
        {
          Corr[i, j] /= tmp_val;
          Corr[j, i] = Corr[i, j];
        }
      }

      int jj;
      using (StreamWriter SW = new StreamWriter(@"C:\Users\Admin\Desktop\Антон\EXPERIMENTS\Processed\Processed\CorrMatrix.txt"))
      {
        for (int i = 0; i < N - 1; i++)
          SW.Write(i.ToString() + " ");
        SW.Write((N - 1 ).ToString());

        for (int i = 1; i < N; i++)
        {
          SW.Write(i.ToString() + " ");
        for (jj = 0; jj < N - 1; jj++)
        {
          SW.Write(Corr[i,jj].ToString());
          SW.Write(" ");
        }
          SW.WriteLine(Corr[i,jj].ToString());
        }
      }
      jj = 2;
      
      /*
      double Max = -1;
      for (int i = 0; i < Normalised.Count; i++)
      {
        for (int j = 0; j < Normalised.Count; j++)
        {
          if (Corr[i, j] > Max) Max = Corr[i, j];
        }
      }

      for (int i = 0; i < Normalised.Count; i++)
      {
        for (int j = 0; j < Normalised.Count; j++)
        {
          Corr[i, j] /= Max;
        }
      }
      */
      return Corr;
    }

    private void DrawGray()
    {
      double[] IntensityData = NeuronDataManager.Neurons[0].IntensityCleanData.ToArray();
      int L = NeuronDataManager.Neurons[0].IntensityCleanData.Count;
      int N = NeuronDataManager.Neurons.Count;
      List<List<double>> AllData = GetNormalisedData();
      //List<List<List<PointD>>> TMP = GetNormalisedSparkles();

      
      GraphPane pane = ZDC_ColorMap.GraphPane;
      pane.CurveList.Clear();
      pane.XAxis.Scale.Max = L + 50;
      PointPairList list;
      LineItem myCurve = new LineItem("");
      myCurve.Line.Width = 1.0F;
      double lambda;
      //Grays
      double []indexes;
      int Left = 0;
      int Right = 1;
      for (int i = 0; i < N; i++)
      {
        // точки до первой вспышки
        list = new PointPairList();
        myCurve.Line.Width = 2.0F;
        IntensityData = AllData[i].ToArray();
         for (int k = 0; k <= NeuronDataManager.Neurons[i].SparkleIndexes[0][0]; k++)
            list.Add(k, IntensityData[k]);
         list.Add(PointPairBase.Missing, PointPairBase.Missing);
          myCurve = pane.AddCurve("", list, Color.Gray  , SymbolType.None);
          
        //

        lambda = i * (740 - 380) / N + 380;
        for (int j = 1; j < NeuronDataManager.Neurons[i].SparkleIndexes.Count; j++ )
        {
          myCurve.Line.Width = 1.0F;
          list = new PointPairList();
          indexes = NeuronDataManager.Neurons[i].SparkleIndexes[j - 1];
          Left = (int)indexes[0];
          Right = (int)indexes[1];
          for (int k = Left; k <= Right && ( k <  L); k++)
            list.Add(k, IntensityData[k]);
          list.Add(PointPairBase.Missing, PointPairBase.Missing);
          myCurve = pane.AddCurve("", list, Colors.waveToColor(lambda), SymbolType.None);

          myCurve.Line.Width = 2.0F;
          list = new PointPairList();
          indexes = NeuronDataManager.Neurons[i].SparkleIndexes[j];
          Left = (int)indexes[0];
          //Right = (int)indexes[0];
          for (int k = Right + 1; k < Left && (k < L); k++)
            list.Add(k, IntensityData[k]);
          list.Add(PointPairBase.Missing, PointPairBase.Missing);
          myCurve = pane.AddCurve("", list, Color.Gray, SymbolType.None);
        }
      }
      ZDC_ColorMap.AxisChange();
      ZDC_ColorMap.Invalidate();
      
    }

    private void SmoothSparkles()
    {
      double[] IntensityData = NeuronDataManager.Neurons[0].IntensityCleanData.ToArray();
      int L = NeuronDataManager.Neurons[0].IntensityCleanData.Count;
      int N = NeuronDataManager.Neurons.Count;
      List<List<double>> AllData = GetNormalisedData();

      GraphPane pane = ZDC_ColorMap.GraphPane;
      pane.CurveList.Clear();
      pane.XAxis.Scale.Max = L + 50;
      PointPairList list;
      LineItem myCurve = new LineItem("");
      myCurve.Line.Width = 1.0F;
      double lambda;
      List<double> tmpY;
      List<double> tmpX;

      for (int i = 0; i < NeuronDataManager.Neurons.Count; i++)
      {
        lambda = i * (740 - 380) / N + 380;
        for (int j = 0; j < NeuronDataManager.Neurons[i].Sparkles.Count; j++)
        {
          tmpY = new List<double>();
          tmpX = new List<double>();
          for (int k = 0; k < NeuronDataManager.Neurons[i].Sparkles[j].Count; k++)
          { tmpY.Add( NeuronDataManager.Neurons[i].Sparkles[j][k].Y);
            tmpX.Add( NeuronDataManager.Neurons[i].Sparkles[j][k].X);
          }

          myCurve = pane.AddCurve("", tmpX.ToArray(), tmpY.ToArray(), Color.Gray, SymbolType.None);
          tmpY = CurveProcessingTools.WindowAVGC(tmpY, 5);
          myCurve = pane.AddCurve("", tmpX.ToArray(), tmpY.ToArray(), Colors.waveToColor(lambda), SymbolType.None);
        }

      }
    }
#endregion


    private Tuple<double[], double[]> ClusterLines()
    {
      //GraphPane pane1 = ZDC_OpticalPlot.GraphPane;
      //pane1.XAxis.Title.Text = "t (cек)";
      //pane1.YAxis.Title.Text = "Номер кластера";


      //if (GlobalVar.channel_id == 65) pane1.XAxis.Scale.Max = 3187 + 5;
      //else pane1.XAxis.Scale.Max = 9519;

      //pane1.CurveList.Clear();
      //double[] X = new double[2];
      //double[] Y = new double[2];

      //double T = GlobalVar.clustTimer / GlobalVar.FPS;
      ////double T = GlobalVar.clustTimer;


      //int Width;
      //double Disp;
      //Stuff(out Width, out Disp);

      //int inc = 0;
      //double[] X_Blur = new double[2 * Width + 1];
      //double[] Y_Blur = new double[2 * Width + 1];

      //double[] TotalY = new double[(int)pane1.XAxis.Scale.Max];

      //PointPairList list1;
      //LineItem myCurve1;
      //List<List<double>> clusters = GetClustersFromFile(GlobalVar.channel_id);
      //for (int i = 0; i < clusters.Count; i++)
      //{
      //  for (int j = 0; j < clusters[i].Count; j++)
      //  {
      //    X[0] = X[1] = Math.Round(clusters[i][j] / T);
      //    Y[0] = i;
      //    Y[1] = i + 1;
      //    list1 = new PointPairList(X.ToArray(), Y.ToArray());
      //    myCurve1 = pane1.AddCurve("", list1, Color.Black, SymbolType.None);

      //    inc = 0;
      //    //--------------- строим гауссову кривую для 
      //    for (int n = -Width; n < Width + 1; n++)
      //    //for (int n = 0; n < 2 * Width; n++)
      //    {
      //      X_Blur[inc] = X[0];
      //      Y_Blur[inc] = Math.Exp(-0.5 * Math.Pow((X_Blur[n + Width] - X[0])/Disp, 2)) / (Disp * Math.Sqrt(2 * Math.PI));
      //      inc++;
      //    }

      //     // нормализуем
      //    double max = Y_Blur.ToList().Max();
      //    for (int n = 0; n < Y_Blur.Length; n++)
      //      Y_Blur[n] = Y_Blur[n] / max;

      //    list1 = new PointPairList(X_Blur.ToArray(), Y_Blur.ToArray());
      //    //myCurve1 = pane1.AddCurve("", list1, Color.Green, SymbolType.None);

      //    for (int n = 0; n < X_Blur.Length; n++)
      //      TotalY[(int)X_Blur[n]] += Y_Blur[n];


      //  }
      //}

      //double[] TotalX = new double[(int)pane1.XAxis.Scale.Max];
      //for (int i = 0; i < TotalX.Length; i++) TotalX[i] = i;
      //list1 = new PointPairList(TotalX.ToArray(), TotalY.ToArray());
      //myCurve1 = pane1.AddCurve("", list1, Color.Red, SymbolType.None);


      //ZDC_OpticalPlot.AxisChange();
      //ZDC_OpticalPlot.Invalidate();

      //return new Tuple<double[], double[]>(TotalX, TotalY);

      GraphPane pane1 = ZDC_OpticalPlot.GraphPane;
      pane1.XAxis.Title.Text = "t (cек)";
      pane1.YAxis.Title.Text = "Номер кластера";


      if (GlobalVar.channel_id == 65) pane1.XAxis.Scale.Max = 3187 + 5;
      else pane1.XAxis.Scale.Max =  9519;

      pane1.CurveList.Clear();
      double[] X = new double[2];
      double[] Y = new double[2];

      double T = GlobalVar.clustTimer / GlobalVar.FPS;
      //double T = GlobalVar.clustTimer;


      int Width;
      double Disp;
      Stuff(out Width, out Disp);

      int inc = 0;
      double[] X_Blur = new double[2 * Width + 1];
      double[] Y_Blur = new double[2 * Width + 1];

      double[] TotalY = new double[(int)pane1.XAxis.Scale.Max];

      PointPairList list1;
      LineItem myCurve1;
      List<List<double>> clusters = GetClustersFromFile(GlobalVar.channel_id);
      for (int i = 0; i < clusters.Count; i++)
      {
        for (int j = 0; j < clusters[i].Count; j++)
        {
          X[0] = X[1] = Math.Round(clusters[i][j] / T);
          Y[0] = i;
          Y[1] = i + 1;
          list1 = new PointPairList(X.ToArray(), Y.ToArray());
          myCurve1 = pane1.AddCurve("", list1, Color.Black, SymbolType.None);

          inc = 0;
          //--------------- строим гауссову кривую для 
          for (int n = -Width; n < Width + 1; n++)
          {
            X_Blur[inc] = X[0] + n;
            Y_Blur[inc] = Math.Exp(-0.5 * Math.Pow((X_Blur[n + Width] - X[0]) / Disp, 2)) / (Disp * Math.Sqrt(2 * Math.PI));
            inc++;
          }

          // нормализуем
          double max = Y_Blur.ToList().Max();
          for (int n = 0; n < Y_Blur.Length; n++)
            Y_Blur[n] = Y_Blur[n] / max;

          list1 = new PointPairList(X_Blur.ToArray(), Y_Blur.ToArray());
          //myCurve1 = pane1.AddCurve("", list1, Color.Green, SymbolType.None);

          for (int n = 0; n < X_Blur.Length; n++)
            TotalY[(int)X_Blur[n]] += Y_Blur[n];


        }
      }

      double[] TotalX = new double[(int)pane1.XAxis.Scale.Max];
      for (int i = 0; i < TotalX.Length; i++) TotalX[i] = i;
      list1 = new PointPairList(TotalX.ToArray(), TotalY.ToArray());
      myCurve1 = pane1.AddCurve("", list1, Color.Red, SymbolType.None);


      ZDC_OpticalPlot.AxisChange();
      ZDC_OpticalPlot.Invalidate();
      return new Tuple<double[], double[]>(TotalX, TotalY);
    }

    //load clusters from file
    private List<List<double>> GetClustersFromFile(int Channel)
    {
      string[] lines = File.ReadAllLines(GlobalVar.cluster_path);

      List<List<double>> res = new List<List<double>>();
      List<double> tmp = new List<double>();
      
      string[] s;
      for (int i = 0; i < lines.Length; i++)
      {
        s = lines[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var II in s) tmp.Add(Double.Parse(II));
        tmp.RemoveAt(0);
        res.Add(tmp);
        tmp = new List<double>();
      }

      return res;
    }


    private void imageBox1_Click(object sender, EventArgs e)
    {

    }

    //------------------------
    private void Stuff(out int par1, out double par2)
    {

      // Шаг 1 - ищем дисперсию длин промежуток вспышек.
      List<double> neurons_sparkle_dispersions = new List<double>();
      List<double> neurons_sparkle_averages = new List<double>();

      double avg = 0;
      double disp = 0;
                       /*
      List<List<PointD>> tmp = new List<List<PointD>>();
      List<double> valList = new List<double>();
      for (int i = 0; i < NeuronDataManager.Neurons.Count; i++)
      {
        tmp = NeuronDataManager.Neurons[i].Sparkles;
        
        foreach (var I in tmp) avg += I.Count;
        avg /= tmp.Count;
        valList.Add(avg);
      }

      double width_stddev = Tools.Statistics.StdDev(valList);
      double width_avg = valList.Sum() / valList.Count;
                            */

      for (int i = 0; i < NeuronDataManager.Neurons.Count; i++)
      {
        avg = 0;
        disp = 0;
      
        List<double[]> sp_indexes = NeuronDataManager.Neurons[i].SparkleIndexes;
        if (sp_indexes.Count == 0)
        {
          neurons_sparkle_dispersions.Add(0);
          neurons_sparkle_averages.Add(0);
          continue;
        }

        for (int j = 0; j < sp_indexes.Count; j++)
          avg += sp_indexes[j][1] - sp_indexes[j][0];
        avg /= sp_indexes.Count;


        for (int j = 0; j < sp_indexes.Count; j++)
          disp += Math.Pow(sp_indexes[j][1] - sp_indexes[j][0] - avg, 2);

        disp /= sp_indexes.Count;
        neurons_sparkle_dispersions.Add(Math.Sqrt(disp));
        neurons_sparkle_averages.Add(avg);
      }


      //собственно, средняя полуширина вспышки 
      double avg_width = neurons_sparkle_averages.Sum() / neurons_sparkle_averages.Count;
      double avg_width_disp = neurons_sparkle_dispersions.Sum() / neurons_sparkle_dispersions.Count;


      //  дисперсия окон ширины
      for (int i = 0; i < neurons_sparkle_averages.Count; i++)
      {
        disp += Math.Pow(neurons_sparkle_averages[i] - avg_width, 2);
      }
      disp = Math.Sqrt(disp / neurons_sparkle_averages.Count);


        //
      int avgW2 = (int)Math.Round(avg_width / 2);

      par1 = Convert.ToInt32(avgW2);
      //par2 = avg_width_disp;
      par2 = disp;

      //double FPS = 24;
      //double clustTimer = 25000;
      //double T = clustTimer / FPS;


      
      //List<List<double>> Cluster = GetClustersFromFile(GlobalVar.channel_id);




    }

    private void Correlate(Tuple<double[], double[]> electro_sigXY)
    {
      List<double> res_corr_values = new List<double>();
      List<List<double>> Normalised = GetNormalisedData();

      double corrVal;
      corrVal = 0;


      List<double> sign_tmp;
      List<double> electro_sig = new List<double>(electro_sigXY.Item2);

      double m = electro_sig.Max();
      for (int i = 0; i < electro_sig.Count; i++)
        electro_sig[i] /= m;


      List<double> tmp = new List<double>();

      double img_height = 0;

      double[] x = new double[250];
      for (int i = 0; i < 250; i++) x[i] = -250 + i;
                       /*
      for (int i = 0; i < Normalised.Count; i++)
        tmp.Add(Tools.Statistics.crossCorrelation(electro_sig.ToArray(), Normalised[i].ToArray(), true));
                     */





      GraphPane pane1 = ZDC_ColorMap.GraphPane;
      pane1.XAxis.Title.Text = "Время (с)";
      pane1.YAxis.Title.Text = "Номер нейрона";

      pane1.CurveList.Clear();

      // для всех нейронов
      for (int i = 0; i < Normalised.Count; i++)
      {
        tmp.Clear();
        // осуществляем сдвиги и ищем на каждом корреляцию
                           /*
          int maxShift = (int)(Math.Min(pattern.Length, signal.Length) * .20);
          double corr;
          double maxCorr = double.MinValue;
          double selfCorr1 = 0;
          double selfCorr2 = 0;
          int ShiftofMax = 0;
          for (int i = 0; i < pattern.Length; i++)
          {
            selfCorr1 += pattern[i] * pattern[i];
          }
          for (int i = 0; i < signal.Length; i++)
          {
            selfCorr2 += signal[i] * signal[i];
          }
          selfCorr1 = Math.Max(selfCorr1, selfCorr2);
          maxCorr = 0;
          for (int shift = 0; shift < maxShift; shift++)
          {
            corr = 0;
            for (int i = 0; i < pattern.Length && i + shift < signal.Length; i++)
            {
              corr += pattern[i] * signal[i + shift];
            }

            if (corr > maxCorr)
            {
              maxCorr = corr;
              ShiftofMax = shift;
            }
            corr = 0;
            for (int i = 0; i + shift < pattern.Length && i < signal.Length; i++)
            {
              corr += pattern[i + shift] * signal[i];
            }

            if (corr > maxCorr)
            {
              maxCorr = corr;
              ShiftofMax = -shift;
            }



                            */



         //записываем в список
          tmp.Add(corrVal);
        }

        //рисуем график
        img_height = (img_height > tmp.Max()) ? img_height : tmp.Max();
        PointPairList list1 = new PointPairList(x, tmp.ToArray());

        //LineItem myCurve1 = pane1.AddCurve("", list1, Plotter.Colors.waveToColor(i * (740 - 380) / Normalised.Count + 380), SymbolType.None);

        pane1.XAxis.Scale.Max = 250 + 5;
        pane1.YAxis.Scale.Max = img_height * 1.05;
        ZDC_ColorMap.AxisChange();
        ZDC_ColorMap.Invalidate();
        //pane1.GetImage().Save(@"C:\Users\Admin\Desktop\Антон\EXPERIMENTS\correlations" + i.ToString() + ".png");
      }
    

    private double CountCorr(List<double> small, List<double> big, int pos0)
    {
      //if (pos0 >= big.Count || pos0 < 0) throw new Exception("wrong pos");
      //if (small.Count > big.Count || Math.Abs(pos0) >= small.Count) throw new Exception("wrong sizes");
       
      double res = 0;

      if (pos0 < 0)
      {
        for (int i = 0; i < small.Count - Math.Abs(pos0); i++)
        {
          res += big[i] * small[Math.Abs(pos0) + i];
        }

        res /= small.Count - Math.Abs(pos0);
      } 

      if ( pos0 == 0)
      {
        for (int i = 0; i < big.Count && i < small.Count; i++)
        {
          res += big[i] * small[i];
        }

        res /= small.Count;
      }
      if ( pos0 > 0) 
      {
        for (int i = pos0; i < big.Count && i < small.Count; i++)
          {
            res += big[i] * small[i - pos0];
          }
        res /= big.Count - pos0;

      
      }
      /*
        for (int big_iter = pos0; big_iter < big.Count && big_iter - pos0 < small.Count; big_iter++)
        {
          res += small[big_iter - pos0] * big[big_iter];
        }
      }

      return res / ( (small.Count + pos0 < big.Count) ? small.Count : big.Count - pos0 );
       * */

      return res;

    }

    private double PearsonCountCorr(List<double> small, List<double> big, int pos0)
    {
      if (pos0 <= -small.Count || pos0 >= big.Count) throw new Exception("!!");

      double res = 0;
      List<double> A = new List<double>();
      List<double> B = new List<double>();

      if (pos0 < 0)
      {
        A = small.GetRange(-pos0, small.Count + pos0);
        B = big.GetRange(0, small.Count + pos0);
        return Tools.Statistics.PearsonCor(A, B);
      }
      else if (pos0 == 0)
      {
        B = big.GetRange(0, small.Count);
        return Tools.Statistics.PearsonCor(small, B);
      }
      else if (pos0 >= 0 && pos0 + small.Count < big.Count)
      {
        B = big.GetRange(pos0, small.Count);
        return Tools.Statistics.PearsonCor(small, B);
      }
      else if (pos0 > 0 && pos0 + small.Count >= big.Count)
      {
        A = small.GetRange(pos0, big.Count - pos0);
        B = big.GetRange(pos0, big.Count - pos0);
        return Tools.Statistics.PearsonCor(A, B);
      }
      
      return 0;
    }

    private void TestCorr()
    {
      List<double> A = new List<double>();
      List<double> B = new List<double>();
      int N = 4;

                   /*
      for (int i = 0; i < N; i++)
        A.Add(2));

      for (int i = 0; i < N; i++)
        B.Add(2);*/

      A.Add(1); A.Add(2); A.Add(3); A.Add(4); A.Add(1); A.Add(2); A.Add(3); A.Add(4); A.Add(1); A.Add(2); A.Add(3); A.Add(4);
      B.Add(1); B.Add(2); B.Add(3); B.Add(4); B.Add(1); B.Add(2); B.Add(3); B.Add(4); B.Add(1); B.Add(2); B.Add(3); B.Add(4);

      B.Reverse();
      double U = Tools.Statistics.crossCorrelation(A.ToArray(), B.ToArray());

      double T = Tools.Statistics.PearsonCor(A, B);
          /*
      B.RemoveAt(0);
      B.RemoveAt(0);
      B.RemoveAt(0);
      B.RemoveAt(0);
      B.RemoveAt(0);
        */
      double TT = PearsonCountCorr(A, B, 4);
          /*
      A.RemoveAt(0);
      A.RemoveAt(0);
      A.RemoveAt(0);
      A.RemoveAt(0);
      A.RemoveAt(0);
        */
      T = Tools.Statistics.PearsonCor(A, B);

                                           //-0.37796447300922725

      double V = CountCorr(A, B, -1);

    }


   
    private void export_Click(object sender, EventArgs e)
    {

    }


  
  }
}

