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
using System.IO;
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
    public string Path_toLoad = @"C:\Users\Admin\Desktop\Антон\EXPERIMENTS\M-Movie\Raw results\Neurons Data\Images\";
    public string CurrentFolder = "";

    public List<string> NeuronDataFiles;

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
      
      List<string> paths = GetFiles(Path_toLoad);

      List<List<List<PointD>>> DATA_Good = new List<List<List<PointD>>>();
      List<List<List<PointD>>> DATA_Bad = new List<List<List<PointD>>>();
      List<List<PointD>>[] tmp;

      for (int i = 0; i < paths.Count; i++)
      {
        CurrentFolder = paths[i].Replace(Path_toLoad, String.Empty).Replace(".txt", String.Empty) + "\\";
        Directory.CreateDirectory(Path_toSave + CurrentFolder);
        tmp = ProcessFile(paths[i]);
        DATA_Good.Add(tmp[0]);
        DATA_Bad.Add(tmp[1]);
      }

      DrawNeuronActivities(DATA_Good, "Good");
      DrawNeuronActivities(DATA_Bad, "Bad");
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

    public List<List<PointD>>[] GetSparkles(List<double> inputDisp, List<double> inputSmoothDisp, List<double> inputSRC)
    {
     
      // Get all good values as list
      double leftVal = 0;

      int count1 = 0;
      int count2 = 0;
      List<double> Sparkle = new List<double>();
     
      for (int i = 0; i < inputSmoothDisp.Count; i++)
      {
        if (inputDisp[i] >= inputSmoothDisp[i])
        {
          leftVal = inputSmoothDisp[i];

          while (i < inputSmoothDisp.Count && inputDisp[i] >= leftVal) // очень грязный хак
          {
            Sparkle.Add( inputDisp[i] );
            if (i < inputSmoothDisp.Count) i++;
            else break;
            count1++;
          }
          i--;
        }
        else { Sparkle.Add(0); count2++; }
      }


      // Separate good values to different list
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


      // find good and bad lists
      List<List<PointD>> rejected = new List<List<PointD>>();
      List<List<PointD>> good = new List<List<PointD>>();
      for (int i = 0; i < words.Count; i++)
      {
        double max = words[i].Max(val => val.Y);
       
        int indexMax
            = !inputSRC.Any() ? -1 :
               inputSRC.GetRange((int)words[i][0].X, words[i].Count)
              .Select((value, index) => new { Value = value, Index = index })
              .Aggregate((a, b) => (a.Value > b.Value) ? a : b)
              .Index;
        double start_ind = 0, end_int = 0;

        for (int ind = (int)words[i].First().X; ind < (int)words[i].Last().X; ind++)
        {

        }
       // start_ind = inputSRC.Where(z => z > words[i][0].X).First();
        //end_int = inputSRC.Where(z => z < words[i].Last().X).Last();
        //double max2 = inputSRC.Where(z => z >= start_ind && z <= end_int).Max();
        //double tmpDisp = WindowDispersion(inputSRC, indexMax, 50);
        //double tmpRY = 3.5 * words[i][0].Y;
        //if (WindowDispersion(inputDisp, (int)r.X, 50) > 0.5 * words[i][0].Y)
        if (inputSRC[indexMax] >  3.5 * words[i][0].Y)
        //if (inputSRC[indexMax] > 2.5 * words[i][0].Y)
        //if (max >= 3.5 * words[i][0].Y)
          rejected.Add(words[i]);
        else { good.Add(words[i]); }
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
        for (int j = 0; j < rejected[i].Count; j++) buildfor2[(int)rejected[i][j].X] = rejected[i][j].Y;
      }
      PlotData(inputDisp, buildfor2, "Bad Sparkles after filtration", "Bad Sparkles");


      PlotData(Sparkle, inputSRC, "Sparkle and Src", "Sparkle_And_SRC");
      PlotData(good, inputSRC, "Good Sparkle and Src", "GoodSparkle_And_SRC");
      PlotData(rejected, inputSRC, "Bad Sparkle and Src", "BadSparkle_And_SRC");

      List<List<PointD>>[] res = new List<List<PointD>>[2];
      res[0] = good;
      res[1] = rejected;
      return res;
    }

    public List<string> GetNeuronDataFiles(string PathToDir)
    {
      List<string> res = new List<string>();
      string[] files = Directory.GetFiles(PathToDir);
      for (int i = 0; i < files.Length; i++)
        if (files[i].Contains(".txt")) res.Add(files[i]);

      return res;
    }

    public List<List<PointD>>[] ProcessFile(string path)
    {
      List<double> X = new List<double>();
      List<double> Y = new List<double>();


      #region Load / Kill inclination
      string[] lines = System.IO.File.ReadAllLines(path);  //4586
      for (int i = 0; i < lines.Length; i++)
      {
        X.Add(i);
        Y.Add(double.Parse(lines[i]));
      }
      PlotData(Y, "Raw Signal", "Raw Signal");

      ///Clear inclination
      List<double> AVG = WindowAVGC(Y, 175); PlotData(AVG, "AVG", "AVG");
      List<double> diff = new List<double>();
      for (int i = 0; i < AVG.Count; i++) diff.Add(Y[i] - AVG[i]);
      PlotData(diff, "No Inclination", "No_Inclination");

      // Smoothing
      List<double> minDiff = new List<double>(); minDiff.AddRange(diff);
      minDiff = WindowAVGC(minDiff, 15);
      PlotData(minDiff, "Smooth No Inclination", "Smooth_No_Inclination");

      // Remove lower than zero values
      double Min = minDiff.Min();
      for (int i = 0; i < minDiff.Count; i++) minDiff[i] -= Min;
      PlotData(minDiff, "No Inclination - Min", "No Inclination_Min");
      #endregion

      #region Мишин метод

      // Dispersion
      List<double> disp = WindowDispersion(minDiff, 50);
      PlotData(disp, "DISPERSION", "DISPERSION");

      // Average of dispersion
      List<double> dispAVGC = new List<double>();
      dispAVGC = WindowAVGC(disp, 650);
      PlotData(dispAVGC, "DISPERSION + AVGC", "DISPERSION_AvgC");

      //d
      List<double> test = WindowAVGC(dispAVGC, 650);

      for (int i = 0; i < test.Count; i++)
        test[i] = dispAVGC[i] - test[i];

      Min = test.Min();
      /*
      for (int i = 0; i < test.Count; i++)
        test[i] -= Min;
      */

      PlotData(dispAVGC, test, "Comparison dispAVGC[i] - dispAVGC[i].Windows[800]", "Test");
      PlotData(disp, test, "Comparison disp and dispAVGC[i] - dispAVGC[i].Windows[800]", "Test2");
      PlotData(disp, dispAVGC, "Comparison disp and disp+avgc", "Disp_vs_dispavgc");

      List<List<PointD>> []SparkleGroups = new List<List<PointD>>[2];
      SparkleGroups = GetSparkles(disp, dispAVGC, minDiff);


      return SparkleGroups;
      //DrawNeuronActivities(SparkleGroups[0], "Good");
      //DrawNeuronActivities(SparkleGroups[1], "Bad");
      #endregion 
    }

    public void DrawNeuronActivities(List<List<List<PointD>>> input, string filename)
    {
      #region Variables
      int max_Count = 0;

      List<List<PointD>> normalisedInput = new List<List<PointD>>();
      PointD nullPointD = new PointD(0,0);
      List<PointD> tmpList = new List<PointD>();
      
       List<double> X = new List<double>(); for (int i = 1; i <= max_Count; i++) X.Add(i);
      List<double> Y = new List<double>();

      GraphPane pane1 = zedGraphControl.GraphPane;
      PointPairList list1;
      LineItem myCurve1; 
      #endregion


      // сгенерировать список цветов от 380 до 740
      List<Color> Colors = new List<Color>();
      for (int i = 0; i < input.Count; i++)
        Colors.Add(Plotter.Colors.waveToColor( 380 + (int)((740 - 380) / input.Count)));


      for (int N = 0; N < input.Count; N++)
      {
        //Нормализовать список списков
        max_Count = int.MinValue;
        for (int i = 0; i < input[N].Count; i++)
          if (max_Count < input[N][i][input[N][i].Count - 1].X) max_Count = (int)input[N][i][input[i].Count - 1].X;

        for (int i = 0; i < input[N].Count; i++)
        {
          for (int j = 0; j < (int)input[N][i][0].X; j++) tmpList.Add(nullPointD);
          tmpList.AddRange(input[N][i]);
          for (int j = tmpList.Count; j < max_Count; j++) tmpList.Add(nullPointD);

          normalisedInput.Add(tmpList);
          tmpList = new List<PointD>();
        }
        
        //Нарисовать список списков
        pane1.CurveList.Clear();
        pane1.XAxis.Scale.Max = max_Count + 20;
        pane1.Title.Text = filename;

        for (int i = 0; i < input.Count; i++)
        {
          Y = new List<double>();
          for (int j = 0; j < normalisedInput[i].Count; j++)
            Y.Add(normalisedInput[i][j].Y);

          list1 = new PointPairList(X.ToArray(), Y.ToArray());

          myCurve1 = pane1.AddCurve("", list1, Color.Black, SymbolType.None);
          zedGraphControl.AxisChange();
          zedGraphControl.Invalidate();
        }
      }

     

      Image bp1 = zedGraphControl.GetImage();
      bp1.Save(Path_toSave + filename + "_" + CurrentFolder.Replace("\\", String.Empty) + ".png");

      int k = 0;
    }



    #region 
    
    public List<Image<Gray, Byte>> GetImages(List<string> input)
    {
      Image<Gray, Byte> tmp = new Image<Gray, byte>(1, 1);
      List<Image<Gray, Byte>> res = new List<Image<Gray, byte>>();
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
        res.Add(cap / (i + 1));
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
      bp1.Save(Path_toSave + CurrentFolder + filename + ".png");
    }

    public void PlotData(List<double> input1, List<double> input2, string Title, string filename)
    {
      List<double> X = new List<double>();

      for (int i = 0; i < ((input1.Count > input2.Count) ? input1.Count : input2.Count); i++) X.Add(i);

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
      bp1.Save(Path_toSave + CurrentFolder + filename + ".png");
    }

    public void PlotData(List<List<PointD>> modulate, List<double> input2, string Title, string filename)
    {
      List<double> X = new List<double>();

      for (int i = 0; i < ((modulate.Count > input2.Count) ? modulate.Count : input2.Count); i++) X.Add(i);

      GraphPane pane1 = zedGraphControl.GraphPane;

      pane1.CurveList.Clear();
      pane1.XAxis.Scale.Max = X.Count + 50;
      //pane1.XAxis.Scale.Max = 200;

      PointPairList list1 = new PointPairList(X.ToArray(), input2.ToArray());
      LineItem myCurve1 = pane1.AddCurve("", list1, Color.Black, SymbolType.None);
      zedGraphControl.AxisChange();
      zedGraphControl.Invalidate();
      Image bp1 = zedGraphControl.GetImage();
      //bp1.Save(Path_toSave + filename + ".png");

      List<double> tmp = new List<double>();
      for (int i = 0; i < input2.Count; i++) tmp.Add(1);

      for (int i = 0; i < modulate.Count; i++)
      {
        list1 = new PointPairList(X.GetRange((int)modulate[i][0].X, modulate[i].Count).ToArray(), input2.GetRange((int)modulate[i][0].X, modulate[i].Count).ToArray());
        myCurve1 = pane1.AddCurve("", list1, Color.Red, SymbolType.None);
        myCurve1.Line.Width = 2.5f;

        list1 = new PointPairList(X.GetRange((int)modulate[i][0].X, modulate[i].Count).ToArray(), tmp.ToArray());
        myCurve1 = pane1.AddCurve("", list1, Color.Blue, SymbolType.None);
        myCurve1.Line.Width = 5.5f;

        zedGraphControl.AxisChange();
        zedGraphControl.Invalidate();


      }
      pane1.Title.Text = Title;
      bp1 = zedGraphControl.GetImage();
      bp1.Save(Path_toSave + CurrentFolder + filename + ".png");
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

        for (int j = i; j < i + window; j++) mean += input[j];
        mean /= window;

        for (int j = i; j < i + window; j++) disp += (input[j] - mean) * (input[j] - mean);
        disp /= window;
        res.Add(Math.Sqrt(disp));
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

      if (position < (window - 1) / 2)
      {
        BEG = 0;
        END = position + (window - 1) / 2;
        n = END + 1;
      }
      else
      {
        BEG = position - (window - 1) / 2;
        END = (position + (window - 1) / 2 < input.Count) ? position + (window - 1) / 2 : input.Count;
        n = window;
      }


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

      // Left border
      for (int i = 0; i < window / 2; i++)
      {
        cap = 0;
        for (int j = 0; j < i; j++) cap += input[j];
        for (int k = 0; k < window / 2; k++) cap += input[i + k];

        cap /= window / 2 + i;
        res.Add(cap);
      }

      // Center
      for (int i = window / 2; i < input.Count - window / 2; i++)
      {
        cap = 0;
        for (int j = 0; j < window; j++)
          cap += input[i - window / 2 + j];

        cap /= window;
        res.Add(cap);
      }


      // Right border
      for (int i = input.Count - window / 2; i < input.Count; i++)
      {
        cap = 0;
        for (int j = i - window / 2; j < i; j++) cap += input[j];
        for (int j = i - 1; j < input.Count; j++) cap += input[j];

        cap /= window / 2 + input.Count - i;
        res.Add(cap);
      }

      return res;

    }
    #endregion

  }
}

