using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.Util;
using ZedGraph;

namespace Plotter
{
  public class SingleNeuron
  {
    private int _ID;
    private List<double> _IntensityRawData;
    private List<double> _IntensityCleanData;
    private double[] _Average;
    private double[] _Sigma;
    private double[] _AveragePlusSigma;
    public int windowWidth = 230;
    private List<List<PointD>> _Sparkles;
    private List<double[]> _SparkleIndexes; // = new List<double[]>();
    private Image<Gray, Byte> _Mask;
    private Image<Gray, Byte> _Patch;
    public bool IsActive = false;

    public int ID
    {
      private set { _ID = value; }
      get { return _ID; }
    }

    public List<double> IntensityRawData
    {
      private set { _IntensityRawData = value; }
      get { return _IntensityRawData; }
    }

    public List<double> IntensityCleanData
    {
      private set { _IntensityCleanData = value; }
      get { return _IntensityCleanData; }
    }

    public double[] SparklesLevel
    {
      get
      {
        if (_AveragePlusSigma == null) AnalyseSignal();
        return _AveragePlusSigma;
      }
      private set
      {
      }
    }
    public double[] AverageLevel
    {
      get
      {
        if (_Average == null) AnalyseSignal();
        return _Average;
      }
      private set
      {
      }
    }
    public double[] SigmaLevel
    {
      get
      {
        if (_Sigma == null) AnalyseSignal();
        return _Sigma;
      }
      private set
      {
      }
    }
    public List<double[]> SparkleIndexes
    {
      private set { _SparkleIndexes = value; }
      get { return _SparkleIndexes; }
    }

    public List<List<PointD>> Sparkles
    {
      set { _Sparkles = value; }
      get { return _Sparkles; }
    }

    // black and white
    public Image<Gray, Byte> Mask
    {
      private set { _Mask = value.Clone(); }
      get { return _Mask; }
    }
    // source signal
    public Image<Gray, Byte> Patch
    {
      private set { _Patch = value.Clone(); }
      get { return _Patch; }
    }

    public SingleNeuron(int _ID, List<double> IntensityData, Image<Gray, Byte> _Patch)
    {
      if (_Patch == null) throw new Exception("ERROR: Patch image is Null");
      ID = _ID;

      IntensityRawData = new List<double>();
      IntensityRawData.AddRange(IntensityData);


      IntensityCleanData = CurveProcessingTools.ProcessCurve(IntensityData);
      AnalyseSignal();
      IndexesToSepSparkles();

      Patch = _Patch.Clone();
      Mask = _Patch.ThresholdBinary(new Gray(1), new Gray(255));
    }

    public SingleNeuron(int _ID, List<double> IntensityData)
    { 
      ID = _ID;

      IntensityRawData = new List<double>();
      IntensityRawData.AddRange(IntensityData);

      IntensityCleanData = CurveProcessingTools.ProcessCurve(IntensityData);
      AnalyseSignal();
      IndexesToSepSparkles();


      Patch = new Image<Gray, byte>(1, 1);
      Mask = new Image<Gray, byte>(1, 1);
    }

    public void AnalyseSignal()
    {
      SparkleIndexes = new List<double[]>();
      double[] raw = _IntensityCleanData.ToArray();
      _Average = new double[raw.Length];
      _Sigma = new double[raw.Length];
      _AveragePlusSigma = new double[raw.Length];

      //1. Построим среднее
      _Average = CurveProcessingTools.WindowAVGC(_IntensityCleanData, windowWidth).ToArray();
      //2. Построим сигму
      _Sigma = CurveProcessingTools.WindowDispersion(_Average.ToList(), windowWidth).ToArray();
      //3. Построим среднее + 3.5 * сигма
      for (int i = 0; i < raw.Length; i++)
      {
        _AveragePlusSigma[i] = _Average[i] + 3.0 * _Sigma[i];
      }
      //4. Найдем вспышки
      SparkleIndexes.Clear();
      for (int i = 0; i < raw.Length; )
      {
        //идем по всему сигналу, если среднее + 3.5 сигма меньше основной кривой 
        //начинамем поиск максимума, начальной и конечной точки
        if (_AveragePlusSigma[i] < raw[i])
        {
          int left = i, right = i;
          // идем в право до уровня сигма
          while (right < raw.Length && _AveragePlusSigma[right] < raw[right])
          {
            right++;
          }

          #region Уточнение границ
          // теперь left и right показывают индексные координаты точек на уровне пересечения с кривой уровня неслучайности.
          // найдем истинную левую граицу
          while (left > 5)
          {
            // будем сдвигаться влево
            //пока среднее от 4 точек до текущей не превысит её
            double sum = 0;
            for (int j = left - 5; j < left; j++)
              sum += raw[j];
            sum /= 5;
            if (sum > raw[left])
              break;
            else
              left--;
          }
          // найдем истинную правую границу
          while (right + 6 < raw.Length)
          {
            // будем сдвигаться вправо
            //пока среднее от 4 точек после текущей не превысит её
            List<double> sum = new List<double>(); ;
            for (int j = right + 1; j < right + 5; j++)
              sum.Add(raw[j]);

            if (sum.Average() > raw[right])
              break;
            else
              right++;
          }
          // к этому моменту у нас есть координаты начала и конца события вспышки
          #endregion

          //найдем максимум светимости события вспышки.
          double max = double.MinValue;
          for (int j = left; j < right; j++)
            if (raw[j] > max)
              max = raw[j];
          //если разница между уровнем отсечения в момент первого пересечения и максимумом существенна, добавим событие в список
          if (max > _AveragePlusSigma[i] + SigmaLevel[left])
            //if (max > 250)
              SparkleIndexes.Add(new double[2] { left, right });
          i = right + 1;

          continue;
        }
        i++;
      }
      // finish
    }

    public void IndexesToSepSparkles()
    {

      Sparkles = new List<List<PointD>>();
      List<PointD> tmp;

      for (int i = 0; i < SparkleIndexes.Count; i++)
      {
        tmp = new List<PointD>();
        for (int j = (int)SparkleIndexes[i][0]; j < SparkleIndexes[i][1]; j++)
          tmp.Add(new PointD( j, IntensityCleanData[j]) );
        Sparkles.Add( tmp);
      }

    }


    public override string ToString()
    {
      return "Neuron Area " + ID;
    }
  }
}
