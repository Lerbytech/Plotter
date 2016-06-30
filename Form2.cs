using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
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
    public Form2()
    {
      InitializeComponent();
    }

    private void button1_Click(object sender, EventArgs e)
    {
      //SmoothSparkles();
      //DrawGray();
      Image<Bgr, Int32> Img = GetCorrelationImage();
      //imageBox2.Image = Img;
      pictureBox1.Image = Img.Resize(45, Inter.Nearest).Bitmap;
      //ColorMap();
      //PlotNormalisedSignals();
    }

    private void ColorMap()
    {
      // настроить ZedGraph
      Stopwatch timer1 = new Stopwatch();
      long l = 0;
      GraphPane pane = ZDC_ColorMap.GraphPane;
      pane.CurveList.Clear();
      pane.XAxis.Title.Text = "Ось X";
      pane.XAxis.Title.Text = "Ось X";


      LineItem myCurve;
      // разбить на линии zedgraph

      Dictionary<int, SingleNeuron> Neurons = NeuronDataManager.Neurons;

      //List<List<PointD>>[] Sparkles = new List<List<PointD>>[Neurons.Count];

      int HeightOfLine = 10; // ZDC_ColorMap.Height / Neurons.Count;
      int VerticalSpacing = HeightOfLine;
      int DiscretisationRate = 5;

      double[] x = new double[5];
      double[] y = new double[5];
      double LocalMax;
      double TotalMax;
      double candidate;
      double r;
      double rel;
      double x0;
      double y0;
      Color col;
      List<PointD> tmpList = new List<PointD>();
      PointD[] tmpArr;
      foreach (KeyValuePair<int, SingleNeuron> pair in Neurons) // НЕЙРОНЫ
      {
        timer1.Start();
        /*
        double TotalMax = double.MinValue;
        double candidate = TotalMax;
        for (int i = 0; i < pair.Value.Sparkles.Count; i++)
        {
          candidate = pair.Value.Sparkles[i].Max(p => p.Y);
          if ( TotalMax < candidate )
          TotalMax = candidate;
        }
        */

        
        for (int sparkleID = 0; sparkleID < pair.Value.Sparkles.Count; sparkleID++) // ВСПЫШКИ
        {
          //tmpList = pair.Value.Sparkles[sparkleID];
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
            x0 = tmpArr[intsparkleID].X;
            x[0] = x0;
            x[1] = x0 + DiscretisationRate;
            x[2] = x0 + DiscretisationRate;
            x[3] = x0;
            x[4] = x0;

            y0 = pair.Value.ID;
            y[0] = ( y0 + 1 )* HeightOfLine;
            y[1] = (y0 + 1) * HeightOfLine;
            y[2] = y0 * HeightOfLine;
            y[3] = y0 * HeightOfLine;
            y[4] = (y0 + 1) * HeightOfLine;

            //
            r = LocalMax / TotalMax * (740 - 380) + 380;
            col =  Plotter.Colors.waveToColor(r);
            myCurve = pane.AddCurve("", x, y, col, SymbolType.None);
            myCurve.Line.Fill = new ZedGraph.Fill(col);
            
            //ZDC_ColorMap.AxisChange();
            //ZDC_ColorMap.Invalidate();
            //ZDC_ColorMap.GetImage().Save(@"C:\Users\Admin\Desktop\Антон\EXPERIMENTS\Processed\Processed\2CH_65 part 3\img_" + sparkleID.ToString() + "_" + intsparkleID.ToString() + ".png");
          }
        }
        ZDC_ColorMap.AxisChange();
        ZDC_ColorMap.Invalidate();
        //ZDC_ColorMap.GetImage().Save(@"C:\Users\Admin\Desktop\Антон\EXPERIMENTS\Processed\Processed\2CH_65 part 3\img_" + pair.Value.ID.ToString() + ".png");
        ZDC_ColorMap.GetImage().Save(@"C:\Users\Админ\Desktop\НИР\EXPERIMENTS\Separated\TEST\img_" + pair.Value.ID.ToString() + ".png");
        timer1.Stop();
        l += timer1.ElapsedMilliseconds;
      }

      timer1.Stop();
      
      // подписать оси
      // для каждого нейрона получить окраску вспышек
      // нарисовать тепловую карту 
      //ZDC_ColorMap.GetImage().Save(@"C:\Users\Admin\Desktop\Антон\EXPERIMENTS\Processed\Processed\2CH_65 part 3\img_FINAL.png");
      ZDC_ColorMap.GetImage().Save(@"C:\Users\Админ\Desktop\НИР\EXPERIMENTS\Separated\TEST\\img_FINAL.png");
      l = 10;
      //5740121
      //1771624
      
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
      pane1.CurveList.Clear();
      pane1.XAxis.Scale.Max = X.Count + 50;


      for (int i = 0; i < Normalised.Count; i++)
      {
        PointPairList list1 = new PointPairList(X.ToArray(), Normalised[i].ToArray());
        LineItem myCurve1 = pane1.AddCurve("", list1, Plotter.Colors.waveToColor(i * (740 - 380)/Normalised.Count + 380), SymbolType.None);
        ZDC_ColorMap.AxisChange();
        ZDC_ColorMap.Invalidate();
      
        //bp1.Save(Path_toSave + filename + ".png");
      }
      //Image bp1 = ZDC_ColorMap.GetImage();

     
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
      /*
      List<List<double>> Normalised = new List<List<double>>();
      for (int i = 0; i < NeuronDataManager.Neurons.Count; i++)
      {
        Normalised.Add(NeuronDataManager.Neurons[i].IntensityCleanData);
      }*/

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

  }
}
