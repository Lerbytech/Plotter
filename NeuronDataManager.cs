using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.Util;


using ZedGraph;

namespace Plotter
{
  public static class NeuronDataManager
  {
    public static Dictionary<int, SingleNeuron> Neurons;
    /*
    public NeuronDataManager()
    {
      Neurons = new Dictionary<int, SingleNeuron>();
    }*/
    /*
    public List<double> GetCleanNeuronIntensities(int num)
    {
      if (Neurons.ContainsKey(num))
        return Neurons[num].IntensityCleanData;
      else return null;
    }

    public List<double> GetRawNeuronIntensities(int num)
    {
      if (Neurons.ContainsKey(num))
        return Neurons[num].IntensityRawData;
      else return null;
    }

    public List<List<PointD>> GetSparkleList(int num)
    {
      if (Neurons.ContainsKey(num))
        return Neurons[num].Sparkles;
      else return null;
    }

    public int GetTotalNumOfNeurons()
    {
      return Neurons.Count;
    }
    */
    public static void CreateNeuron(string PathToDir)
    {
     
      if( !Directory.Exists(PathToDir) )
        throw new Exception("ERROR: PathToDir must be path to directory, not to file");
      
      //read files
      Neurons = new Dictionary<int, SingleNeuron>();

      List<string> PS = new List<string>();

      string[] files = Directory.GetFiles(PathToDir);
      files = files.OrderBy(x => x.Length).ToArray();
      
      // filter out non txt files
      for (int i = 0; i < files.Length; i++)
        if (files[i].Contains(".txt")) PS.Add(files[i]);

      string tempImagePath = String.Empty;
      // create neurons
      for (int i = 0; i < files.Length; i++)
      {
        tempImagePath = Directory.GetParent(Path.GetDirectoryName(files[i])).ToString() + @"\Masks\" + "mask_" + i + ".png"; 
        Neurons.Add(i, new SingleNeuron(i, Tools.IO.TxtIO.GetIntensitiesFromFile(files[i]), new Image<Gray, Byte>(tempImagePath)));
      }
    }

    public static void CreateNeuron(List<string> paths)
    {
      Neurons = new Dictionary<int, SingleNeuron>();
      for (int i = 0; i < paths.Count; i++)
        {
          Neurons.Add(i, new SingleNeuron(i, Tools.IO.TxtIO.GetIntensitiesFromFile(paths[i])));
        }
    }
    /*
    public void FindSparkles()
    {
      List<int> Keys = new List<int>(Neurons.Keys);

      for (int i = 0; i < Keys.Count; i++)
      {
        Neurons[Keys[i]].Sparkles = CurveProcessingTools.GetSparkles(Neurons[Keys[i]].IntensityCleanData); 
      }
    }
    */



  }
}
