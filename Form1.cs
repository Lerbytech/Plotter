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
    public List<Image<Gray, Byte>> Images;


    public string Path_toSave = @"C:\Users\Admin\Desktop\Антон\EXPERIMENTS\M-Movie\Raw results\";
    public string Path_toLoad = @"C:\Users\Admin\Desktop\Антон\EXPERIMENTS\M-Movie\Raw results\Neurons Data\Images\Neuron_1.txt";

    // 27 38 38normal  47 65
    //C:\Users\Leica\Documents\Visual Studio 2012\Projects\SimpleCameraSaveFiles\CH65_Kalman
    //public string PATH = @"C:\Users\Leica\Documents\Visual Studio 2012\Projects\SimpleCameraSaveFiles\CH65";
    //public string PATH = @"C:\Users\Админ\Desktop\НИР\EXPERIMENTS\Separated\M-Movie012\Raw";
    //public string savePATH = @"C:\Users\Leica\Documents\Visual Studio 2012\Projects\SimpleCameraSaveFiles\CH37_Kalman.png";
    //public string savePATH = @"C:\Users\Админ\Desktop\НИР\EXPERIMENTS\Separated\M-Movie012\SigmaReject2";
    //public Image<Gray, Byte> min = new Image<Gray, byte>(@"C:\Users\Админ\Desktop\НИР\EXPERIMENTS\Separated\M-Movie012\Z-Project Gray\MIN_M-Movie0012.tif");
    //public string savePATH2 = @"C:\Users\Leica\Documents\Visual Studio 2012\Projects\SimpleCameraSaveFiles\CH65_Mean20.png";
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


      #region Load / Kill inclination
      string[] lines = System.IO.File.ReadAllLines(Path_toLoad);  //4586
      for (int i = 0; i < lines.Length; i++)
      { 
        X.Add(i);
        Y.Add(double.Parse(lines[i]));
      }
      PlotData(Y, "Raw Signal", "Raw Signal");

      //Clear inclination
      List<double> AVG = WindowAVG(Y, 350); //4585
      PlotData(AVG, "AVG", "AVG");
      
      List<double> diff = new List<double>(); //4585
      for (int i = 0; i < AVG.Count; i++) diff.Add( Y[i] - AVG[i]);
      PlotData(diff, "No Inclination", "No_Inclination");

      // No min
      List<double> minDiff = new List<double>(); //4585
      minDiff.AddRange(diff);
      double Min = minDiff.Min();
      for (int i = 0; i < minDiff.Count; i++) minDiff[i] -= Min;
      PlotData(minDiff, "No Inclination - Min", "No Inclination_Min");

      //Smoothe no inclin      
      minDiff = WindowAVG(minDiff, 15); //4584
      PlotData(minDiff, "Smooth No Inclination", "Smooth_No_Inclination");
      #endregion 

      #region Мишин метод
      List<double> disp = WindowDispersion(minDiff, 50); //4584
      PlotData(disp, "DISPERSION", "DISPERSION");

      List<double> dispAVGC = new List<double>(); //4583
      dispAVGC = WindowAVG(disp, 175);
      PlotData(dispAVGC, "DISPERSION + AVGC", "DISPERSION_AvgC");

      List<double> dispAVGC_Disp = WindowDispersion(dispAVGC, 50); //4583
      PlotData(dispAVGC_Disp, "DISPERSIONS + AVG + DISPERSIONS", "DISPERSION_AvgC_Disp");

      PlotData(disp, dispAVGC, "Comparison disp and disp+avgc", "Disp_vs_dispavgc");      
      
      List<double> Sparkle = new List<double>();
      double leftVal = 0;

      List<double> chunk_disp = WindowDispersion(dispAVGC, 50);

      for (int i = 0; i < dispAVGC.Count; i++)
      {
        if (disp[i] >= dispAVGC[i])
        {
          //leftVal = WindowDispersion(dispAVGC, i, 50);
          //leftVal = chunk_disp[i];
          //double ppp = disp[i];
         // for ( ; i < dispAVGC.Count && (disp[i] >= leftVal); i++)
            //Sparkle.Add(disp[i]);

          while (i < dispAVGC.Count)
          {
            if (disp[i] >= leftVal)
            {
              Sparkle.Add(disp[i]);
            }
            //else Sparkle.Add(-10);
            i++;
          }
        }
        else Sparkle.Add(0);
      }

      PlotData(disp, Sparkle, "Lights", "Sparkles2");
      List<List<PointD>> SS = new List<List<PointD>>();
      SS = GetSparkles(disp, dispAVGC, minDiff);

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

      for (int i = 0; i < window; i++)
      {
        cap += input[i];
        res.Add(cap / ( i + 1));
      }

      int border;
      for (int i = window; i < input.Count; i++)
      {
        cap = 0;
        //border = ((i + window) < input.Count) ? i + window : input.Count;
        if ((i + window) < input.Count)
        {
          border = i + window;
        }
        else
        {
          border = input.Count;
          window = input.Count - i;
        }
        for (int j = i; j < border; j++)
           cap += input[j];  
        res.Add(cap / window);
      }


      return res;


      #region old version

      //List<double> res = new List<double>();
      //double cap = 0;
      //for (int i = 1; i <= window; i++)
      //{
      //  cap = 0;
      //  for (int j = 0; j < i; j++)
      //    cap += input[j];
      //  res.Add(cap / i);
      //}

      //int k = 1;
      //for (int i = window + 1; i < input.Count; i++)
      //{
      //  cap = 0;
      //  for (int j = k; j <= i; j++) cap += input[j];
      //  k++;
      //  res.Add(cap / window);
      //}
      #endregion

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
      //pane1.XAxis.Scale.Max = 200;
      PointPairList list1 = new PointPairList(X.ToArray(), input.ToArray());
      LineItem myCurve1 = pane1.AddCurve("", list1, Color.Black, SymbolType.None);
      zedGraphControl.AxisChange();
      zedGraphControl.Invalidate();
      pane1.Title.Text = Title;
      Image bp1 = zedGraphControl.GetImage();
      bp1.Save(Path_toSave + filename + ".png");
    }

    public void PlotData(List<double> input1, List<double> input2, string Title, string filename)
    {
      List<double> X = new List<double>();

      for (int i = 0; i < ((input1.Count > input2.Count) ? input1.Count : input2.Count ); i++) X.Add(i);

      GraphPane pane1 = zedGraphControl.GraphPane;

      pane1.CurveList.Clear();
      pane1.XAxis.Scale.Max = X.Count + 50;
      //pane1.XAxis.Scale.Max = 200;

      PointPairList list1 = new PointPairList(X.ToArray(), input1.ToArray());
      LineItem myCurve1 = pane1.AddCurve("", list1, Color.Black, SymbolType.None);
      zedGraphControl.AxisChange();
      zedGraphControl.Invalidate();

      list1 = new PointPairList(X.ToArray(), input2.ToArray());
      myCurve1 = pane1.AddCurve("", list1, Color.Red, SymbolType.None);
      myCurve1.Line.Width = 3.0f;
      zedGraphControl.AxisChange();
      zedGraphControl.Invalidate();

      pane1.Title.Text = Title;
      Image bp1 = zedGraphControl.GetImage();
      bp1.Save(Path_toSave + filename + ".png");
    }
    
    public List<double> WindowDispersion(List<double> input, int window)
    {
      double mean = 0;
      double disp = 0;
      List<double> res = new List<double>();

      int i;
      for (i = 0; i < input.Count - window; i++)
      {
        mean = 0; disp = 0;

        for (int j = i; j < i +  window; j++) mean += input[j];
        mean /= window;

        for (int j = i; j < i + window; j++) disp += (input[j] - mean) * (input[j] - mean);
        disp /= window;
        res.Add(Math.Sqrt(disp) );
      }

      int n = 0;
      for (; i < input.Count; i++)
      {
        mean = 0; disp = 0;

        for (int j = i; j < input.Count; j++) mean += input[j];
        mean /= window - n;

        for (int j = i; j < input.Count; j++) disp += (input[j] - mean) * (input[j] - mean);
        disp /= window - n;
        res.Add(Math.Sqrt(disp));
        n++;
      }

   //   for (int i = input.Count - window; i < input.Count; i++) X.Add(input[i]);

      return res;
    }

    public double WindowDispersion(List<double> input, int position, int window)
    {
      double mean = 0;
      double disp = 0;
      double n = 0;
      int BEG;
      int END;

      if (position < ( window - 1) / 2)
      {
        BEG = 0;
        END = position + ( window - 1 )/ 2;
        n = END + 1;
      }
      else
      {
        BEG = position - (window - 1) / 2 ;
        END = (position + (window - 1) / 2 < input.Count) ? position + (window - 1) / 2 : input.Count;
        n = window;
      }

      /*
      for (int i = BEG; i < END; i++) mean += input[i];
      for (int i = BEG; i < END; i++) disp += (input[i] - mean) * (input[i] - mean);
      */
      double res1 = 0;
      double res2 = 0;
      for (int i = BEG; i < END; i++) 
      {
        res1 += input[i] * input[i]; 
        res2 += input[i];
      }

      disp = res1 / n - (res2 / n) * (res2 / n);
      disp = Math.Sqrt(disp);
      return disp;
      disp /= n;
      disp = Math.Sqrt(disp);
      return disp;
    }

    public List<double> WindowAVGC(List<double> input, int window)
    {
      List<double> res = new List<double>();
      double cap = 0;
      for (int i = 0; i < window / 2; i++)
      {
        cap = 0;
        for (int j = 0; j < i; j++) cap += input[j];
        for (int k = 0; k < window / 2; k++) cap += input[i + k];

        cap /= window / 2 + i;
        res.Add( cap );
      }


      for (int i = window / 2; i < input.Count - window / 2; i++)
      {
        cap = 0;
        for (int j = 0; j < window; j++)
          cap += input[i - window / 2 + j];

        cap /= window;
        res.Add(cap);
      }
     
      return res;
      
    }

    public List<List<PointD>> GetSparkles(List<double> inputDisp, List<double> inputSmoothDisp, List<double> inputSRC)
    {
     
      // Get all good values as list
      double leftVal = 0;

      List<double> Sparkle = new List<double>();
      for (int i = 0; i < inputSmoothDisp.Count; i++)
      {
        if (inputDisp[i] >= inputSmoothDisp[i])
        {
          leftVal = inputSmoothDisp[i];

          for (; i < inputSmoothDisp.Count && inputDisp[i] > leftVal; i++)
            Sparkle.Add(inputDisp[i]);
        }
        else Sparkle.Add(0);
      }


      PlotData(inputDisp, Sparkle, "Sparkles", "Sparkles of Sparkles");
      List<PointD> tmp = new List<PointD>();
      List<List<PointD>> words = new List<List<PointD>>();

      for (int i = 0; i < Sparkle.Count; i++)
      {
        if (Sparkle[i] == 0)
        {
          if (tmp.Count == 0) continue;
          else
          {
            words.Add(tmp);
            tmp = new List<PointD>();
          }
        }
        else tmp.Add(new PointD(i, Sparkle[i]));
      }

      List<List<PointD>> rejected = new List<List<PointD>>();
      List<List<PointD>> good = new List<List<PointD>>();
      for (int i = 0; i < words.Count; i++)
      {
        //max = words[i].Max(val => val.Y);
        
        //var r = words[i].Aggregate((agg, next) => next.Y > agg.Y ? next : agg);
        //var r = inputSRC.GetRange((int)words[i][0].X, words[i].Count).Aggregate((agg, next) => next.Y > agg.Y ? next : agg);
        int indexMax
            = !inputSRC.Any() ? -1 :
               inputSRC.GetRange((int)words[i][0].X, words[i].Count)
              .Select((value, index) => new { Value = value, Index = index })
              .Aggregate((a, b) => (a.Value > b.Value) ? a : b)
              .Index;

        double tmpDisp = WindowDispersion(inputSRC, indexMax, 50);
        double tmpRY = 3.5 * words[i][0].Y;
        //if (WindowDispersion(inputDisp, (int)r.X, 50) > 0.5 * words[i][0].Y)
        if (WindowDispersion(inputSRC, indexMax, 50) > 2.5 * words[i][0].Y)
          good.Add(words[i]);
        else { rejected.Add(words[i]); }//words.RemoveAt(i); }
      }

      // for plotting purposes
      List<double> buildfor = new List<double>();
      List<double> buildfor2 = new List<double>();
      for (int i = 0; i < Sparkle.Count; i++) { buildfor.Add(0); buildfor2.Add(0); }


      for (int i = 0; i < good.Count; i++)
      {
        for (int j = 0; j < good[i].Count; j++) buildfor[(int)good[i][j].X] = good[i][j].Y;
      }
      PlotData(inputDisp, buildfor, "Good Sparkles after filtration ", "Good Sparkles");

      for (int i = 0; i < rejected.Count; i++)
      {
        for (int j = 0; j < rejected[i].Count; j++) buildfor2.Insert((int)rejected[i][j].X, rejected[i][j].Y);
      }
      PlotData(inputDisp, buildfor2, "Bad Sparkles after filtration", "Bad Sparkles");

      return words;


    }
  }
}
