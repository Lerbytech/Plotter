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
      //TT();
      ColorMap();
    }

    private void ColorMap()
    {
      // настроить ZedGraph
      Stopwatch timer1 = new Stopwatch();
      long l = 0;
      GraphPane pane = ZDC_ColorMap.GraphPane;
      pane.CurveList.Clear();
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
        ZDC_ColorMap.GetImage().Save(@"C:\Users\Admin\Desktop\Антон\EXPERIMENTS\Processed\Processed\2CH_65 part 3\img_" + pair.Value.ID.ToString() + ".png");
        timer1.Stop();
        l += timer1.ElapsedMilliseconds;
      }

      timer1.Stop();
      
      // подписать оси
      // для каждого нейрона получить окраску вспышек
      // нарисовать тепловую карту 
      ZDC_ColorMap.GetImage().Save(@"C:\Users\Admin\Desktop\Антон\EXPERIMENTS\Processed\Processed\2CH_65 part 3\img_FINAL.png");
      l = 10;
      //5740121
      //1771624
      
    }

    private void TT()
    {
      List<List<double>> Normalised = new List<List<double>>();
      foreach (KeyValuePair<int, SingleNeuron> pair in NeuronDataManager.Neurons)
      {
        Normalised.Add(pair.Value.IntensityCleanData);
      }


      double meanVal = 0;
      for (int i = 0; i < Normalised.Count; i++)
      {
        meanVal = Normalised[i].Sum() / Normalised[i].Count;
        for (int j = 0; j < Normalised[i].Count; j++)
          Normalised[i][j] -= meanVal;
      }

      double MaxVal = 0;
      for (int i = 0; i < Normalised.Count; i++)
      {
        MaxVal = Normalised[i].Max();
        for (int j = 0; j < Normalised[i].Count; j++)
          Normalised[i][j] /= MaxVal;
      }

    
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

      double [,]Conv = new double[Normalised.Count,Normalised.Count];

      for (int i = 0; i < Normalised.Count; i++)
      {
        for (int j = 0; j < Normalised.Count; j++)
        {
          double C = 0;
          for (int R = 0; R < Normalised[i].Count; R++)
          {
            C += Normalised[i][R] * Normalised[j][R];
            //C += Math.Abs(Normalised[i][R] - Normalised[j][R]);
          }
          C /= Normalised.Count;
          Conv[i, j] = C;
          Conv[j, i] = C;
        }
      }

      double Max = - 1;
      for (int i = 0; i < Normalised.Count; i++)
      {
        for (int j = 0; j < Normalised.Count; j++)
        {
          if (Conv[i, j] > Max) Max = Conv[i, j];
        }
      }

      Image<Bgr, Int32> CORRimg = new Image<Bgr, Int32>(Normalised.Count, Normalised.Count);
      for (int i = 0; i < Normalised.Count; i++)
      {
        for (int j = 0; j < Normalised.Count; j++)
        {
          
          int []rr = Plotter.Colors.waveLengthToRGB( Conv[i, j] / Max * (740 - 380) + 380);
          CORRimg[i, j] = new Bgr(rr[2], rr[1], rr[0]);
        }
      }

      for (int i = 0; i < Normalised.Count; i++) 
        CORRimg[i, i] = new Bgr(0, 0, 0);
    }


  }
}
