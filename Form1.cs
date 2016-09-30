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

    private int _SelectedID = 0;
    public int SelectedID
    {
      get { return _SelectedID; }
      set
      {
        _SelectedID = value;
        DrawNeuronData(value);
        DrawAllSignal();
      }
    }

    private static string import_path_1 = @"C:\Users\Admin\Desktop\Антон\EXPERIMENTS\Processed\Processed\2CH_65 part 3\Neurons Data\Images";
    private static string export_path_1 = @"C:\Users\Admin\Desktop\Антон\EXPERIMENTS\Processed\Processed\2CH_65 part 3\";

    private static string import_path_2 = @"C:\Users\Михаил\YandexDisk\TEST\Neurons Data\Images\";
    private static string export_path_2 = @"C:\Users\Михаил\YandexDisk\TEST\Export\";

    private static string import_path_3 = @"C:\Users\Admin\Desktop\Антон\EXPERIMENTS\Processed\Processed\2CH_64 part 2\Neurons Data\Images";
    private static string export_path_3 = @"C:\Users\Admin\Desktop\Антон\EXPERIMENTS\Processed\Processed\2CH_64 part 2\";
    
    public string Path_toSave = export_path_1;
    public string Path_toLoad = import_path_1;
    public string CurrentFolder = "";
  
    public List<string> NeuronDataFiles;

    public Form1()
    {
      InitializeComponent();



      
      //string path = @"C:\Users\Admin\Desktop\Антон\exp\2CH65.txt";
      //string[] lines = File.ReadAllLines(path);
      //Dictionary<double, double> times = new Dictionary<double, double>();
      //Dictionary<int, string> rr = new Dictionary<int,string>();
      //Dictionary<double, List<string>> rr2 = new Dictionary<double, List<string>>();
      //string name;
      //string tmp;
      //string[] timevals;
      //for (int i = 0; i < lines.Length; i++)
      //{
      //  name = lines[i].Substring(0, lines[i].IndexOf(".Png"));
      //  tmp = lines[i].Replace(name + ".Png # 14.06.2016 ", String.Empty).Replace('%', ':');
      //  timevals = tmp.Split(new char[] { ':' });

      //  times.Add(Convert.ToInt32(name), 1000 * (60 * 60 * Convert.ToInt32(timevals[0]) + 60 * Convert.ToInt32(timevals[1]) + Convert.ToInt32(timevals[2])) + Convert.ToInt32(timevals[3]));
      //  rr.Add(Convert.ToInt32(name), lines[i]);

      //  List<string> tpl = new List<string>();
      //  tpl.Add(name);
      //  if (rr2.ContainsKey(1000 * (60 * 60 * Convert.ToInt32(timevals[0]) + 60 * Convert.ToInt32(timevals[1]) + Convert.ToInt32(timevals[2])) + Convert.ToInt32(timevals[3])))
      //  {
      //    rr2[1000 * (60 * 60 * Convert.ToInt32(timevals[0]) + 60 * Convert.ToInt32(timevals[1]) + Convert.ToInt32(timevals[2])) + Convert.ToInt32(timevals[3])].Add(name);
      //  }
      //  else rr2.Add(1000 * (60 * 60 * Convert.ToInt32(timevals[0]) + 60 * Convert.ToInt32(timevals[1]) + Convert.ToInt32(timevals[2])) + Convert.ToInt32(timevals[3]), tpl);
      //}

      //SortedDictionary<int, string> ss = new SortedDictionary<int,string>(rr);
      //SortedDictionary<double, List<string>> ss2 = new SortedDictionary<double, List<string>>(rr2);

      //SortedDictionary<double, double> sortedTimes = new SortedDictionary<double, double>(times);


      //StreamWriter outF = new StreamWriter(@"C:\Users\Admin\Desktop\Антон\exp\out2.txt");
      //foreach (var I in ss2)
      //{
      //  foreach (var II in I.Value)
        
      //}

      //outF.Close();












      //List<double> keys = sortedTimes.Keys.ToList();
      //List<double> values = sortedTimes.Values.ToList();
      //List<double> diff = new List<double>();
      //double prev = values[0];
      //for (int i = 1; i < values.Count; i++)
      //{
      //  diff.Add(values[i] - prev);
      //  prev = values[i];
      //}

      
      //double Sum = 0;
      //int n = 0;
      //for (int i = 0; i < diff.Count; i++) if (diff[i] > 0) { Sum += diff[i]; n++; }
      //double mean = Sum / n;
      //GraphPane pane = AllSignalZedGraph.GraphPane;
      //pane.CurveList.Clear();

      //PointPairList list = new PointPairList();
      //LineItem curve = pane.AddCurve("", keys.ToArray(), diff.ToArray(), Color.Black, SymbolType.None);


      //pane.XAxis.Scale.Min = keys[0];
      //pane.XAxis.Scale.Max = keys[ keys.Count - 1];
      //pane.YAxis.Scale.Max = 500;
      //pane.YAxis.Scale.Min = -10;
      //zedGraphControl.AxisChange();

      //// Обновляем график
      //zedGraphControl.Invalidate();


      ProcessData();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      //Path_toSave = String.Empty;
      //Path_toLoad = String.Empty;
    }

    public void DrawNeuronActivities(List<List<List<PointD>>> input, string filename)
    {
      #region Variables
      int max_Count = 0;

      List<List<PointD>> normalisedInput = new List<List<PointD>>();
      PointD nullPointD = new PointD(0, 0);
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

    private void DrawNeuronData(int id)
    {
      // Получим панель для рисования
      GraphPane pane = zedGraphControl.GraphPane;

      pane.IsFontsScaled = false;
      pane.Title.IsVisible = false;
      pane.XAxis.Title.IsVisible = false;
      pane.YAxis.Title.IsVisible = false;
      pane.XAxis.Title.Text = "Время (кадр)";
      pane.YAxis.Title.Text = "Номер нейрона";
      // Очистим список кривых на тот случай, если до этого сигналы уже были нарисованы
      pane.CurveList.Clear();
      LineItem myCurve;
      
      pane.XAxis.Scale.Min = 0;
      pane.XAxis.Scale.Max = NeuronDataManager.Neurons[id].IntensityCleanData.Count + 10;

      //чистый сигнал
      double[] raw = NeuronDataManager.Neurons[id].IntensityCleanData.ToArray();
      double[] x = new double[raw.Length];
      //заполним x от 0 до N
      for (int j = 0; j < x.Length; j++) x[j] = j;
        
      myCurve = pane.AddCurve("", x, raw, Color.Black, SymbolType.None);
      myCurve.Line.IsSmooth = true;
      myCurve.Line.SmoothTension = 1;

      // сырой сигнал
      if (drawAverageChB.Checked)
      {
        //raw = NeuronDataManager.Neurons[id].IntensityRawData.ToArray();
        //x = new double[raw.Length];
        //заполним x от 0 до N
        //for (int j = 0; j < x.Length; j++) x[j] = j;
        //myCurve = pane.AddCurve("", x, raw, Color.Blue, SymbolType.None);
       // myCurve.Line.IsSmooth = true;
       // myCurve.Line.SmoothTension = 1;
      }

      if (DrawSigmaChB.Checked)
      {
      }
      if (DrawSelectLevelChB.Checked)
      {
        //Уровень отсечения ввспышек
        raw = (NeuronSelector.SelectedItem as SingleNeuron).SparklesLevel;
        ///x = new double[raw.Length];
        //заполним x от 0 до N
        //for (int j = 0; j < x.Length; j++) x[j] = j;
        myCurve = pane.AddCurve("", x, raw, Color.Red, SymbolType.None);
        myCurve.Line.IsSmooth = true;
        myCurve.Line.SmoothTension = 1;
      }
      if (DrawSparklesChB.Checked)
      {
        raw = new double[2] { 1, 1 };
        for (int i = 0; i < (NeuronSelector.SelectedItem as SingleNeuron).SparkleIndexes.Count; i++)
        {
          x = (NeuronSelector.SelectedItem as SingleNeuron).SparkleIndexes[i];
          myCurve = pane.AddCurve("", x, raw, Color.Blue, SymbolType.None);
          myCurve.Line.IsSmooth = true;
          myCurve.Line.Width = 3;
          myCurve.Line.SmoothTension = 1;
        }
      }
      // Вызываем метод AxisChange (), чтобы обновить данные об осях. 
      // В противном случае на рисунке будет показана только часть графика, 
      // которая умещается в интервалы по осям, установленные по умолчанию
      zedGraphControl.AxisChange();

      // Обновляем график
      zedGraphControl.Invalidate();

      // Отобразим изображения нейрона
      NeuronMask.Image = NeuronDataManager.Neurons[SelectedID].Mask.Clone();
      NeuronBody.Image =  NeuronDataManager.Neurons[SelectedID].Patch.Clone();
    }

    private void DrawAllSignal()
    {
      // Получим панель для рисования
      GraphPane pane = AllSignalZedGraph.GraphPane;
      pane.IsFontsScaled = false;
      pane.Title.IsVisible = false;
      pane.XAxis.Title.IsVisible = false;
      pane.YAxis.Title.IsVisible = false;
      pane.XAxis.Title.Text = "Время (кадр)";
      pane.YAxis.Title.Text = "Номер нейрона";
      // Очистим список кривых на тот случай, если до этого сигналы уже были нарисованы
      pane.CurveList.Clear();

      // Создадим список точек
      PointPairList list = new PointPairList();
      pane.XAxis.Scale.Min = 0;
      pane.XAxis.Scale.Max = NeuronDataManager.Neurons[0].IntensityCleanData.Count + 10;



      // Опорные точки выделяться не будут (SymbolType.None)
      for (int i = 0; i < NeuronDataManager.Neurons.Count; i++)
      {
        double[] raw = NeuronDataManager.Neurons[i].IntensityCleanData.ToArray();
        double[] x = new double[raw.Length];
        //заполним x от 0 до N
        for (int j = 0; j < x.Length; j++) x[j] = j;

        double color_length = 380.0 + i * (780.0 - 380.0) / NeuronDataManager.Neurons.Count;

        LineItem myCurve = pane.AddCurve("", x, raw, Colors.waveToColor(color_length), SymbolType.None);
        myCurve.Line.IsSmooth = true;
        myCurve.Line.SmoothTension = 1;
        if (NeuronDataManager.Neurons[i].ID == SelectedID)
        {
          myCurve.Line.Width = 4;
          myCurve.Line.Color = Color.Black;
        }
      }

      // Вызываем метод AxisChange (), чтобы обновить данные об осях. 
      // В противном случае на рисунке будет показана только часть графика, 
      // которая умещается в интервалы по осям, установленные по умолчанию
      AllSignalZedGraph.AxisChange();

      // Обновляем график
      AllSignalZedGraph.Invalidate();

    }

    private void ProcessData()
    {
      NeuronDataManager.CreateNeuron(Path_toLoad);

      NeuronSelector.Items.Clear();
      foreach (SingleNeuron neuron in NeuronDataManager.Neurons.Values)
      {
        NeuronSelector.Items.Add(neuron);
      }
      NeuronSelector.SelectedIndex = 0;
      DrawAllSignal();
      DrawNeuronData(0);
    }

    //------------------------------------------------------

    private void NeuronSelector_SelectedValueChanged(object sender, EventArgs e)
    {
      this.SelectedID = NeuronSelector.SelectedIndex;
    }

    private void zedGraphControl_Load(object sender, EventArgs e)
    {

    }

    private void drawAverageChB_CheckedChanged(object sender, EventArgs e)
    {
      DrawNeuronData(SelectedID);
    }

    private void DrawSigmaChB_CheckedChanged(object sender, EventArgs e)
    {
      DrawNeuronData(SelectedID);
    }

    private void DrawSelectLevelChB_CheckedChanged(object sender, EventArgs e)
    {
      DrawNeuronData(SelectedID);
    }

    private void DrawSparklesChB_CheckedChanged(object sender, EventArgs e)
    {
      DrawNeuronData(SelectedID);
    }

    private void WindowWindthNUD_ValueChanged(object sender, EventArgs e)
    {
      DrawNeuronData(SelectedID);
    }

    private void NeuronMask_Click(object sender, EventArgs e)
    {

    }

    private void button1_Click(object sender, EventArgs e)
    {

      Form2 window_x = new Form2();
   
      window_x.Show();
     
      Tools.Export.ExportSparkles(Path_toSave + "NeuronsDictionary");
     
    }

    private void NeuronSelector_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    private void BTN_Import_Click(object sender, EventArgs e)
    {
      using (FolderBrowserDialog dialog = new FolderBrowserDialog())
      {
        dialog.Description = "Open a folder which contains the neuron intensities files";
        dialog.ShowNewFolderButton = false;
        //if (Directory.Exists(@"C:\Users\Admin\Desktop\Антон") ) dialog.RootFolder = @"C:\Users\Admin\Desktop\Антон";
        dialog.RootFolder = Environment.SpecialFolder.Desktop; 
        if (dialog.ShowDialog() == DialogResult.OK)
        {
          Path_toLoad = dialog.SelectedPath;
          //ProcessData();
        }
      }
    }

    private void BNT_Destination_Click(object sender, EventArgs e)
    {
      using (FolderBrowserDialog dialog = new FolderBrowserDialog())
      {
        dialog.Description = "Open a folder which contains the neuron intensities files";
        dialog.ShowNewFolderButton = false;
        dialog.RootFolder = Environment.SpecialFolder.Desktop; 
        if (dialog.ShowDialog() == DialogResult.OK)
        {
          Path_toSave = dialog.SelectedPath;
          ProcessData();
        }
      }
    }
  }
}




#region Old Spice

/*
    #region Old Paths
    // 27 38 38normal  47 65
    //C:\Users\Leica\Documents\Visual Studio 2012\Projects\SimpleCameraSaveFiles\CH65_Kalman
    //public string PATH = @"C:\Users\Leica\Documents\Visual Studio 2012\Projects\SimpleCameraSaveFiles\CH65";
    //public string PATH = @"C:\Users\Админ\Desktop\НИР\EXPERIMENTS\Separated\M-Movie012\Raw";
    //public string savePATH = @"C:\Users\Leica\Documents\Visual Studio 2012\Projects\SimpleCameraSaveFiles\CH37_Kalman.png";
    //public string savePATH = @"C:\Users\Админ\Desktop\НИР\EXPERIMENTS\Separated\M-Movie012\SigmaReject2";
    //public Image<Gray, Byte> min = new Image<Gray, byte>(@"C:\Users\Админ\Desktop\НИР\EXPERIMENTS\Separated\M-Movie012\Z-Project Gray\MIN_M-Movie0012.tif");
    //public string savePATH2 = @"C:\Users\Leica\Documents\Visual Studio 2012\Projects\SimpleCameraSaveFiles\CH65_Mean20.png";
    #endregion

    public void Plot(List<double> input, string Title, string X_T, string Y_T)
    {
      List<double> X = new List<double>();
      for (int i = 0; i < input.Count; i++)
        X.Add(i);

      GraphPane pane1 = zedGraphControl.GraphPane;
      pane1.CurveList.Clear();
      pane1.XAxis.Scale.Max = X.Count + 50;

      pane1.Title.Text = Title;
      pane1.XAxis.Title.Text = X_T;
      pane1.YAxis.Title.Text = Y_T;

      PointPairList list1 = new PointPairList(X.ToArray(), input.ToArray());
      LineItem myCurve1 = pane1.AddCurve("", list1,Color.Black, SymbolType.None);
      Image IMG = pane1.GetImage();
      IMG.Save(Path_toSave + Title + ".png");

    }

*/
/*
 * 
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

      pane1.Title.Text = Title;
      pane1.XAxis.Title.Text = "Время, с";
      pane1.YAxis.Title.Text = "Суммарная светимость, ед.";


      
      pane1.XAxis.Scale.Max = X.Count + 50;

      PointPairList list1 = new PointPairList(X.ToArray(), input.ToArray());
      LineItem myCurve1 = pane1.AddCurve("", list1, Color.Black, SymbolType.None);
      zedGraphControl.AxisChange();
      zedGraphControl.Invalidate();
      
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


    public  List<double> ProcessCurve(List<double> input)
    {
      List<double> no_incline = new List<double>();
      no_incline = KillInclination(input);

      // Smoothing
      List<double> minDiff = new List<double>();
      minDiff.AddRange(input);
      minDiff = CurveProcessingTools.WindowAVGC(minDiff, 15); // moar beuty
      //PlotData(minDiff, "Smooth No Inclination", "Smooth_No_Inclination");

      // Remove lower than zero values
      double Min = minDiff.Min();
      for (int i = 0; i < minDiff.Count; i++) minDiff[i] -= Min;
      //PlotData(minDiff, "No Inclination - Min", "No Inclination_Min");

      return no_incline;
    }

    private  List<double> KillInclination(List<double> input)
    {
      ///Clear inclination
      List<double> AVG = CurveProcessingTools.WindowAVGC(input, 175);
      Plot(AVG, "AVG", "Время, с", "Суммарная светимость, ед");

      // Find difference
      List<double> diff = new List<double>();
      for (int i = 0; i < AVG.Count; i++)
        diff.Add(input[i] - AVG[i]);
      //PlotData(diff, "No Inclination", "No_Inclination");

      // Smoothing
      List<double> minDiff = new List<double>();
      minDiff.AddRange(diff);
      minDiff = CurveProcessingTools.WindowAVGC(minDiff, 15);
      //PlotData(minDiff, "Smooth No Inclination", "Smooth_No_Inclination");

      // Remove lower than zero values
      double Min = minDiff.Min();
      for (int i = 0; i < minDiff.Count; i++) minDiff[i] -= Min;
      //PlotData(minDiff, "No Inclination - Min", "No Inclination_Min");

      return minDiff;
    }

    public static List<double> WindowAVG(List<double> input, int window)
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
    }

    public static List<double> WindowDispersion(List<double> input, int window)
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

    public static double WindowDispersion(List<double> input, int position, int window)
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

    public static List<double> WindowAVGC(List<double> input, int window)
    {
      if (input.Count <= window) return input;

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
            Sparkle.Add(inputDisp[i]);
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
        if (inputSRC[indexMax] > 3.5 * words[i][0].Y)
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
    */
/*
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
      

  PlotData(dispAVGC, test, "Comparison dispAVGC[i] - dispAVGC[i].Windows[800]", "Test");
  PlotData(disp, test, "Comparison disp and dispAVGC[i] - dispAVGC[i].Windows[800]", "Test2");
  PlotData(disp, dispAVGC, "Comparison disp and disp+avgc", "Disp_vs_dispavgc");

  List<List<PointD>>[] SparkleGroups = new List<List<PointD>>[2];
  SparkleGroups = GetSparkles(disp, dispAVGC, minDiff);


  return SparkleGroups;
  //DrawNeuronActivities(SparkleGroups[0], "Good");
  //DrawNeuronActivities(SparkleGroups[1], "Bad");
  #endregion
}
*/

#endregion
