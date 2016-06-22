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
    private Dictionary<int, SingleNeuron> Neurons;

    public double[] GetNeuronIntensities(int num)
    {
      if (Neurons.ContainsKey(num))
        return Neurons[num].IntensityData;
      else return null;
    }

    public List<PointD> GetSparkleList(int num)
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
        Neurons.Add( i , new SingleNeuron( i, Tools.IO.TxtIO.GetIntensitiesFromFile(files[i]).ToArray() ) );
      }
    }

    public void CreateNeuron(List<string> paths)
    {
      for (int i = 0; i < paths.Count; i++)
        {
          Neurons.Add(i, new SingleNeuron(i, Tools.IO.TxtIO.GetIntensitiesFromFile(paths[i]).ToArray()));
        }
    }

  }
}
