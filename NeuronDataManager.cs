using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


using ZedGraph;

namespace Plotter
{
  public class NeuronDataManager
  {
    public Dictionary<int, SingleNeuron> Neurons;

    public NeuronDataManager()
    {
      Neurons = new Dictionary<int, SingleNeuron>();
    }

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

    public void CreateNeuron(string PathToDir)
    {
      if (PathToDir[PathToDir.Length - 1] != '\\')
        throw new Exception("ERROR: PathToDir must be path to directory, not to file");

      //read files
      List<string> PS = new List<string>();
      string[] files = Directory.GetFiles(PathToDir);
      for (int i = 0; i < files.Length; i++)
        if (files[i].Contains(".txt")) PS.Add(files[i]);

      // create neurons
      for (int i = 0; i < files.Length; i++)
      {
        Neurons.Add( i , new SingleNeuron( i, Tools.IO.TxtIO.GetIntensitiesFromFile(files[i]) ) );
      }
    }

    public void CreateNeuron(List<string> paths)
    {
      for (int i = 0; i < paths.Count; i++)
        {
          Neurons.Add(i, new SingleNeuron(i, Tools.IO.TxtIO.GetIntensitiesFromFile(paths[i])));
        }
    }

    public void FindSparkles()
    {
      List<int> Keys = new List<int>(Neurons.Keys);

      for (int i = 0; i < Keys.Count; i++)
      {
        Neurons[Keys[i]].Sparkles = CurveProcessingTools.GetSparkles(Neurons[Keys[i]].IntensityCleanData); 
      }
    }

  }
}
