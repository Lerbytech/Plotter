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
    public int ID
    { private set; 
      public get { return ID; } 
    }
    public List<double> IntensityData 
    { private set; 
      public get { return IntensityData; } 
    }

    public List<List<PointD>> Sparkles
    {
      private set;
      public get { return Sparkles; }
    }

    // black and white
    public Image<Gray, Byte> Mask 
    {
      private set;
      public get { return Mask; }
    }
    // source signal
    public Image<Gray, Byte> Patch 
    {
      private set;
      public get { return Patch; }
    }

    public SingleNeuron(int _ID, List<double> _IntensityData, Image<Gray, Byte> _Patch)
    {
      if (_Patch == null) throw new Exception("ERROR: Patch image is Null");
      ID = _ID;
      IntensityData.AddRange(_IntensityData);
      
      Patch = _Patch.Clone();
      Mask = _Patch.ThresholdBinary(new Gray(1), new Gray(255));
      Sparkles = CurveProcessingTools.GetSparkles(IntensityData);
      
    }

    public SingleNeuron(int _ID, double[] _IntensityData)
    {
      ID = _ID;
      IntensityData.AddRange(_IntensityData);

      Patch = new Image<Gray, byte>(0, 0);
      Mask = new Image<Gray, byte>(0, 0);
      Sparkles = CurveProcessingTools.GetSparkles(IntensityData);
    }
  }
}
