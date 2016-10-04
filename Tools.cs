using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using System.Drawing;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.Util;
using ZedGraph;

namespace Plotter
{
  public static class Tools
  {
    public static class IO
    {
      public static class BinaryIO
      {
        public static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
        {
          using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create))
          {
            var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            binaryFormatter.Serialize(stream, objectToWrite);
          }
        }

        public static T ReadFromBinaryFile<T>(string filePath)
        {
          using (Stream stream = File.Open(filePath, FileMode.Open))
          {
            var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            return (T)binaryFormatter.Deserialize(stream);
          }
        }

      }

      public static class TxtIO
      {
        public static List<double> GetIntensitiesFromFile(string pathToFile)
        {
          string[] lines = System.IO.File.ReadAllLines(pathToFile); 
          
          List<double> res = new List<double>();
          double tmp = 0;

          for (int i = 0; i < lines.Length; i++)
          {
            double.TryParse(lines[i], out tmp);
            res.Add ( tmp );
          }

          return res;
        }
      }
    }

    public static class Export
    {
      public static void ExportSparkles(string path)
      {
        List<int> NDM_Keys = NeuronDataManager.Neurons.Keys.ToList<int>();
        /*
        Dictionary<int, List<double>> res =new Dictionary<int, List<double>>();
        
        List<double[]> tmpSparkle = new List<double[]>();
        List<double> tmpVal = new List<double>();
        
 
        
        for (int i = 0; i < NDM_Keys.Count; i++)
        {
           NDM.Neurons[NDM_Keys[i]].AnalyseSync();
          tmpSparkle = NDM.Neurons[NDM_Keys[i]].SparkleIndexes;
          System.IO.StreamWriter file = new System.IO.StreamWriter(path + "_" + i.ToString() + ".txt");

          for (int j = 0; j < tmpSparkle.Count; j++)
          {
            tmpVal = NDM.Neurons[NDM_Keys[i]].IntensityCleanData.GetRange((int)tmpSparkle[j][0], (int)tmpSparkle[j][1] - (int)tmpSparkle[j][0]);
            //res.Add(NDM_Keys[j], tmpVal);
            foreach (var I in tmpVal)
            { file.Write(I.ToString()); file.Write(" "); }
            file.WriteLine();
          }

          file.Close();
          //Tools.IO.BinaryIO<Dictionary<int, List<double>>>(path + "_" + i.ToString() + ".bin", res, false);
        }
        */
        
        var result = Parallel.For(0, NDM_Keys.Count, i =>
        {
          List<double[]> tmpSparkle = new List<double[]>();
          List<double> tmpVal = new List<double>();
          NeuronDataManager.Neurons[NDM_Keys[i]].AnalyseSignal();
          tmpSparkle = NeuronDataManager.Neurons[NDM_Keys[i]].SparkleIndexes;
          System.IO.StreamWriter file = new System.IO.StreamWriter(path + "_" + i.ToString() + ".txt");

          for (int j = 0; j < tmpSparkle.Count; j++)
          {
            tmpVal = NeuronDataManager.Neurons[NDM_Keys[i]].IntensityCleanData.GetRange((int)tmpSparkle[j][0], (int)tmpSparkle[j][1] - (int)tmpSparkle[j][0]);
            //res.Add(NDM_Keys[j], tmpVal);
            foreach (var I in tmpVal)
            { file.Write(I.ToString()); file.Write(" "); }
            file.WriteLine();
          }

          file.Close();
          //Tools.IO.BinaryIO<Dictionary<int, List<double>>>(path + "_" + i.ToString() + ".bin", res, false);
        });
        


      }
    }

    public static class Separation
    {
     
      /// <summary>
      /// Calculates intensity of Median pixel of image
      /// </summary>
      /// <param name="src">Image</param>
      /// <returns>Intensity of Median pixel</returns>
      public static double MedianPixel(Image<Gray, Byte> src)
      {
        //List<byte> L = new List<byte>();
        int[] Hist = new int[256];

        //src = src.ThresholdToZero(new Gray(38));

        int SRC_Width = src.Width;
        int SRC_Height = src.Height;
        byte[, ,] SRC_Data = src.Data;
        byte nullB = (byte)0;
        for (int i = 0; i < SRC_Width; i++)
          for (int j = 0; j < SRC_Height; j++)
            //if (SRC_Data[j, i, 0] != nullB) L.Add(SRC_Data[j, i, 0]);
            if (SRC_Data[j, i, 0] != nullB)
            {
              Hist[SRC_Data[j, i, 0]]++;
              //L.Add(SRC_Data[j, i, 0]);
            }

        int[] Sum = new int[256];
        for (int i = 1; i < 256; i++)
        {
          Sum[i] = Sum[i - 1] + Hist[i];
        }

        //double result = 0;


        if (Sum[255] == 0) return -1;
        int k = Sum[255] / 2;


        // ЗДЕСЬ И НИЖЕ - полная, не заслуживающая доверия херня. проверить в деле ( по формулам выходит, что всегда мажем на 1 единицу яркости как бы ни старались, так как неизвестно как на гистограмме вынуть правильное значениее
        if (Sum[255] % 2 != 0)
        {
          for (int j = 1; j < 255; j++)
            if (Sum[j] > k) return j;
            else continue;
        }
        else
        {
          for (int j = 1; j < 255; j++)
            if (Sum[j] > k) return j;
            else continue;
        }

        return -1;
        // Test
        //L = new List<byte>();
        //L.Add(5);
        //L.Add(8);
        //L.Add(13);  - w/o this. 8 and 11.5
        //L.Add(15);
        //L.Add(17);

        /*
        if (L.Count == 0) return -1;
        L.Sort();
        if (L.Count % 2 != 0) result = L[L.Count / 2];
        else
        {
          int P = (int)(L.Count / 2) - 1;
          int PPP = (int)(L.Count / 2) + 1;
          int LP = L[P];
          int LPP = L[PPP];

          result = L[(L.Count / 2) - 1]; 
          result += L[(L.Count / 2)];
          result /= 2;
        }*/
        //return result;

      }
    }

    public static class Denoise
    {
      private static List<Image<Gray, Byte>> arr;
      private static int WIDTH;
      private static int HEIGHT; 
      private static byte[, ,] arr_Data0;
      private static byte[, ,] arr_Data1;
      private static byte[, ,] arr_Data2;
      private static byte[, ,] arr_Data3;
      
      public static Image<Gray, Byte> SigmaReject2(Image<Gray, Byte> Q)
      {
        arr.RemoveAt(0);
        arr.Add(Q);

        //if (arr.Length == 1) return arr[0];
        // WIDTH = arr[0].Width;
        //HEIGHT = arr[0].Height;

        arr_Data0 = arr[0].Data; // rows cols channels
        arr_Data1 = arr[1].Data;
        arr_Data2 = arr[2].Data;
        arr_Data3 = arr[3].Data;

        double[] tmp_pixels = new double[4];

        double avr = 0;
        double deviation = 0;

        for (int i = 0; i < HEIGHT; i++) //row
          for (int j = 0; j < WIDTH; j++) //col
          {
            // STEP 1
            tmp_pixels[0] = arr_Data0[i, j, 0];
            tmp_pixels[1] = arr_Data1[i, j, 0];
            tmp_pixels[2] = arr_Data2[i, j, 0];
            tmp_pixels[3] = arr_Data3[i, j, 0];

            avr = Average(tmp_pixels);
            deviation = FindDeviation(tmp_pixels, avr);
            RejectPixels(ref tmp_pixels, avr, deviation);
            avr = Average(tmp_pixels);
            //result[i, j] = new Gray(avr);

            //STEP 2
            deviation = FindDeviation(tmp_pixels, avr);
            RejectPixels(ref tmp_pixels, avr, deviation);
            avr = Average(tmp_pixels);
            arr_Data0[i, j, 0] = (byte)avr;
          }

        return arr[0];
      }

      public static void PrepareDenoiseFunctions(int width, int height)
      {
        WIDTH = width;
        HEIGHT = height;
        Image<Gray, Byte> tmp = new Image<Gray, byte>(WIDTH, HEIGHT, new Gray(0));
        arr = new List<Image<Gray, byte>>();
        for (int i = 0; i < 4 ; i++) arr.Add(tmp);
      }


      private static double Average(double[] arr)
      {
        double avr = 0.0;
        double n_avr = 0;
        for (int i = 0; i < arr.Length; i++)
        {
          if (arr[i] < 0) continue;
          avr += arr[i];
          n_avr = n_avr + 1;
        }
        if (avr == 0) return avr;
        else return avr / n_avr;
      }
      private static void RejectPixels(ref double[] arr, double avr, double dev)
      {
        for (int i = 0; i < arr.Length; i++)
        {
          if (arr[i] < 0) continue;
          if (Math.Abs(arr[i] - avr) > dev) arr[i] = -1;
        }
      }
      private static double FindDeviation(double[] arr, double avr)
      {
        double result = 0;
        double n = 0;
        for (int i = 0; i < arr.Length; i++)
        {
          if (arr[i] < 0) continue;
          else
          {
            result = result + (avr - arr[i]) * (avr - arr[i]);
            n = n + 1;
          }
        }
        if (result == 0) return 0;
        return Math.Sqrt(result / n);
      }
    }

    public static class Statistics
    {

      public static double StdDev(List<double> input)
      {
        double avg = input.Sum() / (double)input.Count();
        double stddev = 0;

        for (int i = 0; i < input.Count; i++)
        {
          stddev += (input[i] - avg) * (input[i] - avg);
        }

        stddev = Math.Sqrt(stddev / (((double)input.Count - 1.0)));

        return stddev;
      }

      public static double PearsonCor(List<double> A, List<double> B)
      {
        
        int N = A.Count;
        double stdA = Tools.Statistics.StdDev(A);
        double stdB = Tools.Statistics.StdDev(B);

        double avgA = A.Sum() / A.Count;
        double avgB = B.Sum() / B.Count;

        double T = 0;
        for (int i = 0; i < N; i++)
          T += (A[i] - avgA) * (B[i] - avgB) / (stdA * stdB);

        T = T / (N - 1);

        return T;
      }

      

    }
  }
}
