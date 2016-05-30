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
    public string PATH = @"C:\Users\Leica\Documents\Visual Studio 2012\Projects\SimpleCameraSaveFiles\CH65";
    public string savePATH = @"C:\Users\Leica\Documents\Visual Studio 2012\Projects\SimpleCameraSaveFiles\CH37_Kalman.png";
    public string savePATH2 = @"C:\Users\Leica\Documents\Visual Studio 2012\Projects\SimpleCameraSaveFiles\CH65_Mean20.png";
    public Form1()
    {
      InitializeComponent();
      X = new List<double>();
      Y = new List<double>();
      Images = GetImages(GetFiles(PATH));


      Tools.Denoise.PrepareDenoiseFunctions(Images[0].Width, Images[0].Height);
      SaveToVideo(Images);
      for (int i = 0; i < Images.Count; i++) Images[i] = Tools.Denoise.SigmaReject2(Images[i]);

      //Y = GetIntensity(Images);
      //for (int i = 0; i < Y.Count; i++) X.Add(i);

      //#region ZEDGRAPH 
      ///*
      //GraphPane pane1 = zedGraphControl.GraphPane;
      
      //  pane1.CurveList.Clear();
      //  PointPairList list1 = new PointPairList(X.ToArray(), Y.ToArray());
      //  LineItem myCurve1 = pane1.AddCurve("", list1, Color.Black, SymbolType.None);
      //  zedGraphControl.AxisChange();
      //  zedGraphControl.Invalidate();
      //  Image bp1 = zedGraphControl.GetImage();
      // bp1.Save(savePATH);

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
      double dec = 1/(double)window;
      double cap ;
      List<double> res = new List<double>();

      for (int i = 0; i < input.Count - window; i++)      
      {
        cap = 0;
        for (int j = i; j < i + window; j++)
          cap += input[j];
        res.Add(cap*dec);
      }

      return res;
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
  }
}
