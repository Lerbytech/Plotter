using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

using ZedGraph;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.Util;

namespace Plotter
{
  public partial class Form1 : Form
  {
    public List<double> X;
    public List<double> Y;
    public List<Image<Gray, Byte>> Images;

    // 27 38 38normal  47 65
    //C:\Users\Leica\Documents\Visual Studio 2012\Projects\SimpleCameraSaveFiles\CH65_Kalman
    //public string PATH = @"C:\Users\Leica\Documents\Visual Studio 2012\Projects\SimpleCameraSaveFiles\CH65";
    public string PATH = @"C:\Users\Админ\Desktop\НИР\EXPERIMENTS\Separated\M-Movie012\Raw";
    //public string savePATH = @"C:\Users\Leica\Documents\Visual Studio 2012\Projects\SimpleCameraSaveFiles\CH37_Kalman.png";
    public string savePATH = @"C:\Users\Админ\Desktop\НИР\EXPERIMENTS\Separated\M-Movie012\SigmaReject2";
    //public Image<Gray, Byte> min = new Image<Gray, byte>(@"C:\Users\Админ\Desktop\НИР\EXPERIMENTS\Separated\M-Movie012\Z-Project Gray\MIN_M-Movie0012.tif");
    public string savePATH2 = @"C:\Users\Leica\Documents\Visual Studio 2012\Projects\SimpleCameraSaveFiles\CH65_Mean20.png";
    public Form1()
    {
      InitializeComponent();
      List<double> X = new List<double>();
      List<double> Y = new List<double>();

      /*Images = GetImages(GetFiles(PATH));
      Tools.Denoise.PrepareDenoiseFunctions(Images[0].Width, Images[0].Height);
      //SaveToVideo(Images);
      for (int i = 0; i < Images.Count; i++) { Images[i] = Tools.Denoise.SigmaReject2(Images[i] - min); Images[i].Save(savePATH + "\\" + i.ToString() + ".png");  }
      
      Y = GetIntensity(Images);
       */

      string[] lines = System.IO.File.ReadAllLines(@"C:\Users\Admin\Desktop\Антон\EXPERIMENTS\M-Movie\Raw results\Neurons Data\Images\Neuron_1.txt");
      for (int i = 0; i < lines.Length; i++)
      { 
        X.Add(i);
        Y.Add(double.Parse(lines[i]));
      }

      List<double> AVG = WindowAVG(Y, 200);
      
      List<double> diff = new List<double>();
      for (int i = 0; i < AVG.Count; i++) diff.Add( Y[i] - AVG[i]);

      #region ZEDGRAPH 
      ///*
      ///

      PlotData(Y, "Signal", "bp1");

      PlotData(AVG, "AVG", "bp2");

      PlotData(diff, "Corrected", "bp3");

      List<double> avgDiff = WindowAVG(diff, 50);
      
      PlotData(avgDiff, "Smoothed", "bp4");
      
      List<double> minDiff = new List<double>();
      minDiff.AddRange(diff);
      minDiff = WindowAVG(minDiff, 20);

      PlotData(minDiff, "Smooth", "bp5");
      double Min = minDiff.Min();
      for (int i = 0; i < minDiff.Count; i++) minDiff[i] -= Min;
      
      PlotData(minDiff, "Minus Min + smooth", "bp6");


      #region Мишин метод

      List<double> disp = WindowDispersion(minDiff, 50);
      PlotData(disp, "DISPERSION", "DISP1");

      disp = WindowAVG(disp, 50);
      List<double> disp2 = new List<double>(); disp2.AddRange(disp);
      PlotData(disp, "DISPERSION + AVG", "DISP2");

      //disp = WindowDispersion(disp, 50);
      //PlotData(disp, "DISPERSIONS + AVG + DISPERSIONS", "DISP3");


      X = new List<double>();
      for (int i = 0; i < disp.Count; i++) X.Add(i);

      GraphPane pane1 = zedGraphControl.GraphPane;

      pane1.CurveList.Clear();
      pane1.XAxis.Scale.Max = X.Count + 50;
      PointPairList list1 = new PointPairList(X.ToArray(), disp.ToArray());
      LineItem myCurve1 = pane1.AddCurve("", list1, Color.Black, SymbolType.None);
      zedGraphControl.AxisChange();
      zedGraphControl.Invalidate();
      list1 = new PointPairList(X.ToArray(), disp2.ToArray());
      myCurve1 = pane1.AddCurve("", list1, Color.Blue, SymbolType.None);
      zedGraphControl.AxisChange();
      zedGraphControl.Invalidate();
      //pane1.Title.Text = Title;
      Image bp1 = zedGraphControl.GetImage();
      bp1.Save(@"C:\Users\Admin\Desktop\Антон\EXPERIMENTS\M-Movie\Raw results\" + "DISP1_2" + ".png");
     

      /*
      List<double> sign_square = new List<double>();
      for (int i = 0; i < minDiff.Count; i++) sign_square.Add( minDiff[i] * minDiff[i] );

      double AA = 0; double BB = 0;
      for (int i = 0; i < minDiff.Count; i++)
      {
        AA += minDiff[i];
        BB += sign_square[i];
      }
      AA /= minDiff.Count;
      BB /= minDiff.Count;

      double dispersion = Math.Sqrt(BB * BB - AA * AA);
       * */
      #endregion  






      List<double> medDiff = new List<double>(); medDiff.AddRange(minDiff);
      medDiff.Sort();
      double median = 0.5 * (medDiff[medDiff.Count / 2 - 1] + medDiff[medDiff.Count / 2 - 1]);

      double meanVal = 0;
      for (int i = 0; i < minDiff.Count; i++) meanVal += minDiff[i];
      meanVal /= minDiff.Count;

      List<double> separated = new List<double>();
      separated.AddRange(minDiff);
      for (int i = 0; i < separated.Count; i++)
        if (separated[i] < median) separated[i] = 0;
        else separated[i] = 1;

      //FLTR
      /*
      int FLTR_rate = 5;
      int A = 0;
      int Z = 0;
      int i = 0;
      for (int i = 0; i < separated.Count; i++) if (separated[i] = 1) { A = i; break; }
      for ( ; i < separated.Count; i++)
      {
        if (separated[i] == 0) Z = i;
        else if (Z = 0 && separated[i] == 1) A = 

        if ( Z - A <= FLTR_rate )
          for (int j = A; j < Z; j++)
          {
            separated[j] = 0;
            A = Z = 0;
          }


      }
      */
      /*
      #region      

      X = new List<double>();
      //for (int i = 0; i < separated.Count; i++) X.Add(i);

      GraphPane pane1 = zedGraphControl.GraphPane;

      pane1.CurveList.Clear();
      pane1.XAxis.Scale.Max = separated.Count + 50;
      LineItem myCurve1;
      PointPairList list1;
      
      Y = new List<double>();
      Y.Add( minDiff[0] );
      for (int i = 1; i < separated.Count; i++)
      {
        if (separated[i] != separated[i - 1])
        {
          list1 = new PointPairList(X.ToArray(), Y.ToArray());
          if (separated[i - 1] == 0 )
            myCurve1 = pane1.AddCurve("", list1, Color.Black, SymbolType.None);
          else 
            myCurve1 = pane1.AddCurve("", list1, Color.Red, SymbolType.None);

          X = new List<double>();
          Y = new List<double>();
        }
        else 
        { 
          Y.Add ( minDiff[i]);
          X.Add( i );
        }
      }

      zedGraphControl.AxisChange();
      zedGraphControl.Invalidate();
      pane1.Title.Text = "";
      Image bp1 = zedGraphControl.GetImage();
      bp1.Save(@"C:\Users\Админ\Desktop\НИР\EXPERIMENTS\Separated\" +  "Separated BY MEDIAN" + ".png");
      
      #endregion*/



      PlotData(separated, "Separated", "bp8");

      //----------
      //LocalMINMAX(ZP_minDiff, 10);

#endregion
      int y = 5;

      // */
      // GraphPane pane2 = zedGraphControl.GraphPane;
      // pane2.CurveList.Clear();
      // PointPairList list2 = new PointPairList(X.ToArray(), WindowAVG(Y, 20).ToArray());
      // LineItem myCurve2 = pane2.AddCurve("", list2, Color.Red, SymbolType.None);
      // zedGraphControl.AxisChange();
      // zedGraphControl.Invalidate();
      // Image bp2 = zedGraphControl.GetImage();
      // bp2.Save(savePATH2);
       

      //#endregion
      // int k = 2;
      // k++;
    }

    public List<string> GetFiles(string path)
    {
      List<string> allfiles = Directory.GetFiles(path).ToList<string>();
      //for (int i = 0; i < allfiles.Count; i++) allfiles[i] = allfiles[i].Replace(path, String.Empty).Replace(".Png", String.Empty);

      int path_len = path.Length;
      int exten_lem = 4;

      List<string> res = new List<string>();

      for (int i = 0; i < allfiles.Count; i++) if (allfiles[i].Length == (path_len + exten_lem + 1)) res.Add(allfiles[i]);
      for (int i = 0; i < allfiles.Count; i++) if (allfiles[i].Length == (path_len + exten_lem + 2)) res.Add(allfiles[i]);
      for (int i = 0; i < allfiles.Count; i++) if (allfiles[i].Length == (path_len + exten_lem + 3)) res.Add(allfiles[i]);
      for (int i = 0; i < allfiles.Count; i++) if (allfiles[i].Length == (path_len + exten_lem + 4)) res.Add(allfiles[i]);
      for (int i = 0; i < allfiles.Count; i++) if (allfiles[i].Length == (path_len + exten_lem + 5)) res.Add(allfiles[i]);
      for (int i = 0; i < allfiles.Count; i++) if (allfiles[i].Length == (path_len + exten_lem + 6)) res.Add(allfiles[i]);
      for (int i = 0; i < allfiles.Count; i++) if (allfiles[i].Length == (path_len + exten_lem + 7)) res.Add(allfiles[i]);
      for (int i = 0; i < allfiles.Count; i++) if (allfiles[i].Length == (path_len + exten_lem + 8)) res.Add(allfiles[i]);
      for (int i = 0; i < allfiles.Count; i++) if (allfiles[i].Length == (path_len + exten_lem + 9)) res.Add(allfiles[i]);
      for (int i = 0; i < allfiles.Count; i++) if (allfiles[i].Length == (path_len + exten_lem + 10)) res.Add(allfiles[i]);

      return res;
    }

    public List<Image<Gray, Byte>> GetImages(List<string> input)
    {
      Image<Gray, Byte> tmp = new Image<Gray, byte>(1,1);
      List<Image<Gray, Byte>> res = new List<Image<Gray,byte>>();
      foreach (var value in input)
      {
        tmp = new Image<Gray, byte>(value);
        res.Add(tmp);
      }

      return res;
    }

    public List<double> GetIntensity(List<Image<Gray, Byte>> input)
    {
      List<double> res = new List<double>();
      foreach (var val in input)
        res.Add(CvInvoke.Sum(val).V0);

      return res;
    }

    public List<double> WindowAVG(List<double> input, int window)
    {

      List<double> res = new List<double>();
      double cap = 0;
      for (int i = 1; i <= window; i++)
      {
        cap = 0;
        for (int j = 0; j < i; j++) cap += input[j];
        //cap += input[i];
        res.Add( cap / i );
      }

      int k = 1;
      for (int i = window + 1; i < input.Count; i++)
      {
        cap = 0;
        for (int j = k; j < i; j++) cap += input[j];
        k++;
        res.Add(cap / window);
      }

      return res;
      /*
      double dec = 1/(double)window;
      double cap;
      List<double> res = new List<double>();

      for (int i = 0; i < input.Count - window; i++)      
      {
        cap = 0;
        for (int j = i; j < i + window; j++)
          cap += input[j];
        res.Add(cap*dec);
      }

      return res;
      * */
    }

    public void SaveToVideo(List<Image<Gray, Byte>> img)
    {
      string savepath = @"C:\Users\Leica\Documents\Visual Studio 2012\Projects\SimpleCameraSaveFiles\Отчет\Видео\CH65.avi";
      VideoWriter VW = new VideoWriter(savepath, VideoWriter.Fourcc('D', 'I', 'V', '3'), 20, img[0].Mat.Size, false);
      //VideoWriter VW = new VideoWriter(savepath, -1, 20, img[0].Mat.Size, false);
      //foreach (var V in img) VW.Write(V.Mat);
      Mat tmpMat = new Mat();
      for (int i = 0; i < img.Count; i++)
      {
        tmpMat = img[i].Mat;
        VW.Write(tmpMat);
      }
     VW.Dispose();
     tmpMat.Dispose();
    }
    
    public List<double> MinFromZ_Project(List<double> input)
    {
      Image<Gray, Byte> minIMG = new Image<Gray, byte>(@"C:\Users\Админ\Desktop\НИР\EXPERIMENTS\Separated\TEST\Z-Project Gray\Min.png");
      Image<Gray, Byte> Mask = new Image<Gray, byte>(@"C:\Users\Админ\Desktop\НИР\EXPERIMENTS\Separated\TEST\Binary Masks\0.png");
      
      Image<Gray, Byte> tmp = Mask.CopyBlank();
      CvInvoke.cvSetImageROI(tmp, new Rectangle(5, 5, 172, 130));
      minIMG.CopyTo(tmp);
      CvInvoke.cvResetImageROI(tmp);

      //
      tmp = tmp.Copy(Mask);
      //
      List<double> res = new List<double>();
      res.AddRange(input);

      double MIN = CvInvoke.Sum(tmp).V0;
      for (int i = 0; i < res.Count; i++)
        res[i] -= MIN;

      return res;
    }

    public void PlotData(List<double> input, string Title, string filename)
    {
      List<double> X = new List<double>();
      for (int i = 0; i < input.Count; i++) X.Add(i);

      GraphPane pane1 = zedGraphControl.GraphPane;

      pane1.CurveList.Clear();
      pane1.XAxis.Scale.Max = X.Count + 50;
      PointPairList list1 = new PointPairList(X.ToArray(), input.ToArray());
      LineItem myCurve1 = pane1.AddCurve("", list1, Color.Black, SymbolType.None);
      zedGraphControl.AxisChange();
      zedGraphControl.Invalidate();
      pane1.Title.Text = Title;
      Image bp1 = zedGraphControl.GetImage();
      bp1.Save(@"C:\Users\Admin\Desktop\Антон\EXPERIMENTS\M-Movie\Raw results\" + filename + ".png");
    }

    public void LocalMINMAX(List<double> input, int window)
    {
      List<int> MAXS = new List<int>();
      List<int> MINS = new List<int>();

      bool OK_max = true;
      bool OK_min = true;
      int k = 0;
      for (int i = window / 2; i < input.Count - window / 2; i++)
      {
        for (int j = 0 + k; j < i + k; j++)
        {
          try
          {
            if (input[j] > input[i]) OK_max = false;
            if (input[j] < input[i]) OK_min = false;
          }
          catch (Exception ex) { }
        }

        if (OK_max) MAXS.Add(i);
        if (OK_min) MINS.Add(i);
        
        OK_max = true;
        OK_min = true;
        k++;
      }

      k = 5;
      /*
      GraphPane pane1 = zedGraphControl.GraphPane;

      pane1.CurveList.Clear();
      pane1.XAxis.Scale.Max = X.Count + 50;
      PointPairList list1 = new PointPairList(X.ToArray(), Y.ToArray());
      LineItem myCurve1 = pane1.AddCurve("", list1, Color.Black, SymbolType.None);
      zedGraphControl.AxisChange();
      zedGraphControl.Invalidate();
      pane1.Title.Text = "Mins";

      PointPairList list1 = new PointPairList(X.ToArray(), Y.ToArray());
      LineItem myCurve1 = pane1.AddCurve("", list1, Color.Black, SymbolType.None);

      Image bp1 = zedGraphControl.GetImage();
      bp8.Save(@"C:\Users\Админ\Desktop\НИР\EXPERIMENTS\Separated\bp8.png");
      */


    }

    public List<double> WindowDispersion(List<double> input, int window)
    {
      List<double> res = new List<double>();

      double mean = 0;
      double disp = 0;
      List<double> X = new List<double>();
      List<double> XX = new List<double>();

      for (int i = 0; i < input.Count - window; i++)
      {
        
        //X = new List<double>();
        //XX = new List<double>();
        mean = 0; disp = 0;

        for (int j = i; j < i +  window; j++) mean += input[j];
        mean /= window;

        for (int j = i; j < i + window; j++) disp += (input[j] - mean) * (input[j] - mean);
        disp /= window;
        X.Add(Math.Sqrt(disp) );
      }
      for (int i = input.Count - window; i < input.Count; i++) X.Add(input[i]);

      return X;
    }


  }
}
