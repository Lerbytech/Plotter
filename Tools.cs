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

    public static class Plotter
    {
      public static ZedGraphControl zedGraphControl;
      public static string PathToSave;

      public static void PlotData(List<double> input, string Title, string filename)
      {
        List<double> X = new List<double>();
        for (int i = 0; i < input.Count; i++) X.Add(i);

        GraphPane pane1 = zedGraphControl.GraphPane;

        pane1.CurveList.Clear();
        pane1.XAxis.Scale.Max = X.Count + 50;

        PointPairList list1 = new PointPairList(X.ToArray(), input.ToArray());
        LineItem myCurve1 = pane1.AddCurve("", list1, Color.Black, SymbolType.None);
        zedGraphControl.AxisChange();
        zedGraphControl.Invalidate();
        pane1.Title.Text = Title;
        Image bp1 = zedGraphControl.GetImage();
        bp1.Save(PathToSave + filename + ".png");
      }

      public static void PlotData(List<List<double>> input, string Title, string filename)
      {
        List<double> X = new List<double>();

        int max = 0;
        for (int i = 0; i < input.Count; i++)
          if (max < input[i].Count) max = input[i].Count;

        for (int i = 0; i < max; i++) X.Add(i);

        GraphPane pane1 = zedGraphControl.GraphPane;

        pane1.CurveList.Clear();
        pane1.XAxis.Scale.Max = X.Count + 50;
        pane1.Title.Text = Title;

        PointPairList list;
        LineItem myCurve;

        for (int i = 0; i < input.Count; i++)
        {

          list = new PointPairList(X.ToArray(), input[i].ToArray());
          myCurve = pane1.AddCurve("", list, Color.Black, SymbolType.None);
        }


        zedGraphControl.AxisChange();
        zedGraphControl.Invalidate();
        
        
        Image bp1 = zedGraphControl.GetImage();
        bp1.Save(PathToSave + filename + ".png");
      }

      public static void PlotData(List<List<PointD>> input, string Title, string filename)
      {
        List<double> X = new List<double>();
        List<double> Y = new List<double>();
        GraphPane pane1 = zedGraphControl.GraphPane;


        pane1.CurveList.Clear();
        pane1.XAxis.Scale.Max = input[input.Count - 1][input[input.Count - 1].Count - 1].Y + 50; //very last index
        pane1.Title.Text = Title;

        PointPairList list;
        LineItem myCurve; 
        zedGraphControl.AxisChange();
        zedGraphControl.Invalidate();

        for (int i = 0; i < input.Count; i++)
        {
          X = new List<double>();
          for (int j = 0; j < input[i].Count; j++)
          {
            X.Add(input[i][j].X);
            Y.Add(input[i][j].Y);
          }

          list = new PointPairList(X.ToArray(), Y.ToArray());
          myCurve = pane1.AddCurve("", list, Color.Black, SymbolType.None);       
        }
        zedGraphControl.AxisChange();
        zedGraphControl.Invalidate();


        Image bp1 = zedGraphControl.GetImage();
        bp1.Save(PathToSave + filename + ".png");
      }
    }


  }
}
