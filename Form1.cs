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
    public string Path_toLoad = @"C:\Users\Admin\Desktop\Антон\EXPERIMENTS\M-Movie\Raw results\Neurons Data\Images\";
    public string CurrentFolder = "";
    NeuronDataManager NDM = new NeuronDataManager();
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

      Image<Gray, Byte> z_stddev = new Image<Gray, Byte>(@"C:\Users\Admin\Desktop\Антон\EXPERIMENTS\M-Movie\Additional Raw\STD_M-Movie.png");
      Image<Gray, Byte> z_stddev2 = z_stddev.Clone();
      CvInvoke.CLAHE(z_stddev, 40, new Size(8, 8), z_stddev2);

      z_stddev2 = z_stddev2.ThresholdToZero(new Gray(201));
      z_stddev2 =z_stddev2.SmoothMedian(3);
      z_stddev2._EqualizeHist();
      z_stddev2 = z_stddev2.ThresholdToZero(new Gray(87));

      Image<Gray, Byte> z_stddev3 = z_stddev.Clone();

      Image<Bgr, Byte> ColorImg = new Image<Bgr, byte>(z_stddev.Bitmap);
      Image<Gray, float> Im1 = new Image<Gray, float>(z_stddev.Size);
      Image<Gray, int> Im2 = new Image<Gray, int>(z_stddev.Size);
      Image<Gray, Byte> Im3 = new Image<Gray, Byte>(z_stddev.Size);
      
      
      /*
      if (frame.channels()==3) cvtColor(frame,frame,CV_BGR2GRAY);
    /// Establish the number of bins
    int histSize = 256;
    /// Set the ranges ( for B,G,R) )
    float range[] = { 0, 256 } ;
    const float* histRange = { range };
    bool uniform = true; bool accumulate = false;
    /// Compute the histograms:
    calcHist( &frame, 1, 0, Mat(), hist, 1, &histSize, &histRange, uniform, accumulate );
    hist /= frame.total();

    Mat logP;
    cv::log(hist,logP);

    float entropy = -1*sum(hist.mul(logP)).val[0];

    cout << entropy << endl;

      */

      // MyEntropy

      DenseHistogram Histo = new DenseHistogram(256, new RangeF(0,255));
      Image<Gray, Byte> BlankImg = new Image<Gray, byte>(172, 130, new Gray(0));
      Histo.Calculate(new Image<Gray, Byte>[] { z_stddev2 }, false, null);
      float[] HistArr = new float[256];
      Histo.CopyTo(HistArr);
  


      for (int i = 0; i < HistArr.Length; i++)
        HistArr[i] /= z_stddev2.Width * z_stddev2.Height;

      HistArr[0] = 22360;    
      int cnt = 0;
      double entr = 0;
      float total_size = z_stddev2.Width * z_stddev2.Height; //total size of all symbols in an image
      int index = 256;

      try
      {
        for (int i = 0; i < index; i++)
        {
          float sym_occur = HistArr[i]; //the number of times a sybmol has occured
          if (sym_occur > 0) //log of zero goes to infinity
          {
            cnt++;
            entr += (sym_occur / total_size) * (Math.Log(total_size / sym_occur, 2));
          }
        }
      }
      catch (Exception ex) { }
      
      Mat logP = new Mat();
      double entropy;
      Mat mulR =new Mat();
      




      try
      {
        CvInvoke.Log(Histo, logP);
        CvInvoke.Multiply(Histo, logP, mulR);
        entropy = -1 * CvInvoke.Sum(mulR).V0;
      }
      catch (Exception ex) { }


      try
      {
        CvInvoke.DistanceTransform(z_stddev2, Im1, Im2, DistType.L2, 3);
        Im3 = new Image<Gray, Byte>(@"C:\Users\Admin\Desktop\IMG_Mask_label.png");
        //Im2 = Im2.ThresholdToZero(new Gray(1));

        for (int y = 0; y < Im2.Height; y++)
          for (int x = 0; x < Im2.Width; x++)
          {
            if (Im3[y, x].Intensity < 2) Im3[y, x] = new Gray(0);
            else Im3[y, x] = new Gray(150 + 50 * Im3[y, x].Intensity);
            //else if (Im2[y, x].Intensity != 0) Im2[y, x] = new Gray(1);
          }
        //ColorImg[3] = Im3;

        for (int y = 0; y < ColorImg.Height; y++)
          for (int x = 0; x < ColorImg.Width; x++)
          {
            
          }

        //ColorImg.Add(new Image<Bgr, Byte>(z_stddev.Width, z_stddev.Height, new Bgr(0, 200, 200)), Im3);
        //CvInvoke.Watershed(ColorImg, Im2);

      }
      catch (Exception ex) { }

    


      List<string> paths = GetFiles(Path_toLoad);

      List<List<List<PointD>>> DATA_Good = new List<List<List<PointD>>>();
      List<List<List<PointD>>> DATA_Bad = new List<List<List<PointD>>>();
      List<List<PointD>>[] tmp;

     
      NDM.CreateNeuron(paths);
      //NDM.FindSparkles();

      PlotData(NDM.GetSparkleList(1), NDM.GetCleanNeuronIntensities(1), "TEST", "New algo test");

      List<double> newT = new List<double>();
      List<double> newS = CurveProcessingTools.WindowAVGC(NDM.GetCleanNeuronIntensities(1), 5);
      //List<double> newS = NDM.GetCleanNeuronIntensities(1);
      List<double> disp = CurveProcessingTools.WindowAVGC(newS, 650);

      for (int i = 0; i < newS.Count; i++) newT.Add(0);
      int n = 1;
      for (int i = n; i < newT.Count; i+= n)
      {
        /*
        if (newS[i] - newS[i - n] > disp[i]) newT[i] = newS[i] - newS[i - n];
        else newT[i] = newS[i];*/
        if (newS[i] > disp[i]) newT[i] = newS[i] - newS[i - n] + disp[i];
        else newT[i] = disp[i];
      }


      
      PlotData(newS, newT, "Difference = " + n.ToString(), "Difference_" + n.ToString());
      //PlotData(newS, disp, "AVGC", "AVGC");

      
      double Min = newS.Min();
      double Max = newS.Max();

      int H_l = (int)Math.Round(Max) + 1;
      double[] Histogram = new double[H_l];

      List<double> newW = new List<double>();
      for (int i = 0; i < newS.Count; i++)
      {
        double ppp = H_l / newS[i];
        if (newS[i] >= 1)
        {
          Histogram[(int)Math.Round(H_l / newS[i])]++;
          if ((int)Math.Round(H_l / newS[i]) == 4) newW.Add(0);
          else newW.Add(newS[i]);
        }
        else
        {
          Histogram[0]++;
          newW.Add(newS[i]);
        }
      

      }
      
      
      PlotData(newS, newW, "newW", "newW");









      /*

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
      */
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
      List<double> AVG = CurveProcessingTools.WindowAVGC(Y, 175); PlotData(AVG, "AVG", "AVG");
      List<double> diff = new List<double>();
      for (int i = 0; i < AVG.Count; i++) diff.Add(Y[i] - AVG[i]);
      PlotData(diff, "No Inclination", "No_Inclination");

      // Smoothing
      List<double> minDiff = new List<double>(); minDiff.AddRange(diff);
      minDiff = CurveProcessingTools.WindowAVGC(minDiff, 15);
      PlotData(minDiff, "Smooth No Inclination", "Smooth_No_Inclination");

      // Remove lower than zero values
      double Min = minDiff.Min();
      for (int i = 0; i < minDiff.Count; i++) minDiff[i] -= Min;
      PlotData(minDiff, "No Inclination - Min", "No Inclination_Min");
      #endregion

      #region Мишин метод

      // Dispersion
      List<double> disp = CurveProcessingTools.WindowDispersion(minDiff, 50);
      PlotData(disp, "DISPERSION", "DISPERSION");

      // Average of dispersion
      List<double> dispAVGC = new List<double>();
      dispAVGC = CurveProcessingTools.WindowAVGC(disp, 650);
      PlotData(dispAVGC, "DISPERSION + AVGC", "DISPERSION_AvgC");

      //d
      List<double> test = CurveProcessingTools.WindowAVGC(dispAVGC, 650);

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
     
      List<List<PointD>> curInput = new List<List<PointD>>();

      // найти максимум
      max_Count = int.MinValue;
      for (int N = 0; N < input.Count; N++)
      {
        curInput = input[N];
     
        for (int i = 0; i < curInput.Count; i++)
          if (max_Count < curInput[i][curInput[i].Count - 1].X) max_Count = (int)curInput[i][curInput[i].Count - 1].X;
      }

      //Нормализовать список списков
      for (int N = 0; N < input.Count; N++)
      {
        curInput = input[N];
        for (int i = 0; i < curInput.Count; i++)
        {
        //  for (int j = 0; j < (int)curInput[i][0].X; j++) tmpList.Add(nullPointD);
          for (int j = 0; j < curInput[i].Count; j++) tmpList.Add(curInput[i][j]);
        //  for (int j = tmpList.Count; j < max_Count; j++) tmpList.Add(nullPointD);

          normalisedInput.Add(tmpList);
          tmpList = new List<PointD>();
        }
      }


      pane1.XAxis.Scale.Max = max_Count + 20;
      pane1.Title.Text = filename;

      List<Color> Colors = new List<Color>();
      for (int i = 0; i < normalisedInput.Count; i++)
        Colors.Add(Plotter.Colors.waveToColor(380 + i * (int)((740 - 380) / normalisedInput.Count)));

      pane1.CurveList.Clear();
        

      for (int N = 0; N < normalisedInput.Count; N++)
      {
        //Нарисовать список списков
//        pane1.CurveList.Clear();
        
        Y = new List<double>();
        for (int j = 0; j < normalisedInput[N].Count; j++)
            Y.Add(normalisedInput[N][j].Y);

        list1 = new PointPairList(X.ToArray(), Y.ToArray());

        myCurve1 = pane1.AddCurve("", list1, Colors[N], SymbolType.None);
        myCurve1.Line.IsAntiAlias = true;
        myCurve1.Line.IsSmooth = true;
        
      }

      List<int> sum = new List<int>();
      //for (int i = 0; i < normalisedInput.Count; i++)
      //  sum.Add((int)normalisedInput[i][0].Sum());

      Image bp1 = zedGraphControl.GetImage();
      //bp1.Save(Path_toSave + filename + "_" + CurrentFolder.Replace("\\", String.Empty) + ".png");
      bp1.Save(Path_toSave + filename + ".png");

      int k = 0;

      zedGraphControl.AxisChange();
      zedGraphControl.Invalidate();
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
      bp1.Save(Path_toSave + filename + ".png");
    }

    #endregion

  }
}

