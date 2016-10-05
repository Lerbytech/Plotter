using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using ZedGraph;


namespace Plotter
{
  public partial class Form3 : Form
  {
    public Form3()
    {
      InitializeComponent();

      Subutiles();
    }

    public void Subutiles()
    {
      //Correlate(corr_data);
      InterInterval();
    }

    private void InterInterval()
    {
      #region opto
      SortedDictionary<double, double> OptoIntervalsCap = new SortedDictionary<double, double>();
      //Dictionary<double, double> IntervalsCap = new Dictionary<double, double>();
      double L = 0;
      double R = 0;

      for (int i = 0; i < NeuronDataManager.Neurons.Count; i++)
      {
        List<double[]> indexes = NeuronDataManager.Neurons[i].SparkleIndexes;
        L = R = 0;

        for (int j = 1; j < indexes.Count; j++)
        {
          L = indexes[j - 1][1];
          R = indexes[j][0];
          
          if (OptoIntervalsCap.ContainsKey((R - L) * 1000 / GlobalVar.FPS))
            OptoIntervalsCap[(R - L) * 1000 / GlobalVar.FPS]++;
          else OptoIntervalsCap.Add((R - L) * 1000 / GlobalVar.FPS, 1);
            
        }

      }

      GraphPane pane1 = zedGraphControl1.GraphPane;
      pane1.Title.Text= "Распределение промежутков между оптическими сигналами";
      pane1.XAxis.Title.Text = "Время ";
      //pane1.YAxis.Title.Text = "Номер нейрона";

      pane1.CurveList.Clear();


      PointPairList list1 = new PointPairList(OptoIntervalsCap.Keys.ToArray(), OptoIntervalsCap.Values.ToArray());

      LineItem myCurve1 = pane1.AddCurve("", list1, Color.Black, SymbolType.None);

      pane1.XAxis.Scale.Max = OptoIntervalsCap.Keys.Max();
      pane1.YAxis.Scale.Max = OptoIntervalsCap.Values.Max();

      zedGraphControl1.AxisChange();
      zedGraphControl1.Invalidate();
      pane1.GetImage().Save(@"C:\Users\Admin\Desktop\Антон\EXPERIMENTS\Sparkles_gap_distribution.png");
      #endregion

      #region electro

      SortedDictionary<double, double> ElectroIntervalsCap = new SortedDictionary<double, double>();
      List<List<double>> clusters = GetClustersFromFile(GlobalVar.channel_id);
      List<double> all_cluster = new List<double>();
      for (int i = 0; i < clusters.Count; i++)
        all_cluster.AddRange(clusters[i]);
      all_cluster.Sort();


      double T = GlobalVar.clustTimer / GlobalVar.FPS;
      double V = 0;
      for (int i = 1; i < all_cluster.Count; i++)
      {
        V = Math.Round((all_cluster[i] - all_cluster[i - 1]) / T);
        //V = (all_cluster[i] - all_cluster[i - 1]) / T;
        if (ElectroIntervalsCap.ContainsKey(V))
          ElectroIntervalsCap[V]++;
        else ElectroIntervalsCap.Add(V, 1);
      }


      GraphPane pane2 = zedGraphControl2.GraphPane;
      pane2.XAxis.Title.Text = "Время ";
      pane1.Title.Text = "Распределение промежутков между электрическими сигналами";
      //pane2.YAxis.Title.Text = "Номер нейрона";

      pane2.CurveList.Clear();


      PointPairList list2 = new PointPairList(ElectroIntervalsCap.Keys.ToArray(), ElectroIntervalsCap.Values.ToArray());

      LineItem myCurve2 = pane2.AddCurve("", list2, Color.Black, SymbolType.None);

      pane2.XAxis.Scale.Max = ElectroIntervalsCap.Keys.Max();
      pane2.YAxis.Scale.Max = ElectroIntervalsCap.Values.Max();

      zedGraphControl2.AxisChange();
      zedGraphControl2.Invalidate();
      pane2.GetImage().Save(@"C:\Users\Admin\Desktop\Антон\EXPERIMENTS\Electro_gap_distribution.png");
      #endregion

     

    }



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
  



  }
}
