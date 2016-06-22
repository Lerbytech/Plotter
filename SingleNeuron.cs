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
    private List<List<PointD>> _Sparkles;
    private Image<Gray, Byte> _Mask;
    private Image<Gray, Byte> _Patch;


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

      Patch = _Patch.Clone();
      Mask = _Patch.ThresholdBinary(new Gray(1), new Gray(255));
      //Sparkles = CurveProcessingTools.GetSparkles(IntensityRawData);
    }

    public SingleNeuron(int _ID, List<double> IntensityData)
    {
      ID = _ID;
      
      IntensityRawData = new List<double>();
      IntensityRawData.AddRange(IntensityData);
      
      Sparkles = new List<List<PointD>>();

      IntensityCleanData = CurveProcessingTools.ProcessCurve(IntensityData);

      Patch = new Image<Gray, byte>(1, 1);
      Mask = new Image<Gray, byte>(1, 1);
      //Sparkles = CurveProcessingTools.GetSparkles(IntensityData);
    }
  }
}
