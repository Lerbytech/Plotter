using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using ZedGraph;

namespace Plotter
{
  public static class CurveProcessingTools
  {

    public static List<double> ProcessCurve(List<double> input)
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

    private static List<double> KillInclination(List<double> input)
    {
      ///Clear inclination
      List<double> AVG = CurveProcessingTools.WindowAVGC(input, 175); 
      //PlotData(AVG, "AVG", "AVG");

      // Find difference
      List<double> diff = new List<double>();
      for (int i = 0; i < AVG.Count; i++) 
        diff.Add(input[i] - AVG[i]);
      //PlotData(diff, "No Inclination", "No_Inclination");

      // Smoothing
      List<double> minDiff = new List<double>(); minDiff.AddRange(diff);
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
        //border = ((i + window) < input.Count) ? i + window : input.Count;
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


      #region old version

      //List<double> res = new List<double>();
      //double cap = 0;
      //for (int i = 1; i <= window; i++)
      //{
      //  cap = 0;
      //  for (int j = 0; j < i; j++)
      //    cap += input[j];
      //  res.Add(cap / i);
      //}

      //int k = 1;
      //for (int i = window + 1; i < input.Count; i++)
      //{
      //  cap = 0;
      //  for (int j = k; j <= i; j++) cap += input[j];
      //  k++;
      //  res.Add(cap / window);
      //}
      #endregion

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

    public static List<List<PointD>> GetSparkles(List<double> input)
    {
      #region PreProcess
      // Dispersion
      List<double> disp = CurveProcessingTools.WindowDispersion(input, 50);
      //PlotData(disp, "DISPERSION", "DISPERSION");

      // Average of dispersion
      List<double> dispAVGC = new List<double>();
      dispAVGC = CurveProcessingTools.WindowAVGC(disp, 650);
      //PlotData(dispAVGC, "DISPERSION + AVGC", "DISPERSION_AvgC");

      List<double> test = CurveProcessingTools.WindowAVGC(dispAVGC, 650);

      for (int i = 0; i < test.Count; i++)
        test[i] = dispAVGC[i] - test[i];

      //PlotData(dispAVGC, test, "Comparison dispAVGC[i] - dispAVGC[i].Windows[800]", "Test");
      //PlotData(disp, test, "Comparison disp and dispAVGC[i] - dispAVGC[i].Windows[800]", "Test2");
      //PlotData(disp, dispAVGC, "Comparison disp and disp+avgc", "Disp_vs_dispavgc");
      #endregion
      
      List<Point>  SeparationPoints = GetSparkleSeparationPoints(disp, dispAVGC, input);
      List<List<PointD>> RawSparkles = GetSeparatedWords(SeparationPoints, input);

      List<List<PointD>> GoodSparkles = new List<List<PointD>>();
      List<List<PointD>> BadSparkles = new List<List<PointD>>();
      FiltrateSparkles(RawSparkles, input, out GoodSparkles, out BadSparkles);

      return GoodSparkles;
    }


    private static List<Point> GetSparkleSeparationPoints(List<double> inputDisp, List<double> inputSmoothDisp, List<double> input)
    {
      double leftVal = -1;
      int left = 0;
      int threshold = 10;
      int count1 = 0;
      int count2 = 0;
      
      List<Point> res = new List<Point>();
      Point tmp = new Point(0, 0);
      double d = 0;



      for (int i = 0; i < inputSmoothDisp.Count; i++)
      {
        if (inputDisp[i] >= inputSmoothDisp[i])
        {
          if (left == -1) left = i;
          else continue;
        }
        else
        {
          if (left == -1) continue;
          //else { if ( i - left > threshold ) res.Add(new Point(left, i)); left = 0; }
          else { res.Add(new Point(left, i)); left = -1; }
        }
      }

      /*
      for (int i = 0; i < inputSmoothDisp.Count; i++)
      {
        if (inputDisp[i] >= inputSmoothDisp[i])
        {
          leftVal = inputSmoothDisp[i];
          tmp.X = i;
          while (i < inputSmoothDisp.Count && inputDisp[i] >= leftVal) // очень грязный хак
          {
            i++;
           // res.Add(inputDisp[i]);
            /*
            if (i < inputSmoothDisp.Count) i++;
            else break;
            count1++;
          }
          i--;
          tmp.Y = i;
          if ( i - tmp.X <= 5 )res.Add(tmp);
        }
        else {
          //res.Add(0); 
          count2++; 
        }
      }
*/

      return res;
    }

    private static List<List<PointD>> GetSeparatedWords(List<Point> inputPoints, List<double> signal )
    {
      List<List<PointD>> res = new List<List<PointD>>();
      List<PointD> tmpList = new List<PointD>();

      for (int i = 0; i < inputPoints.Count; i++)
      {
        tmpList = new List<PointD>();
        for (int j = inputPoints[i].X; j < inputPoints[i].Y; j++)
        {
          tmpList.Add(new PointD(j , signal[j]));
        }

        res.Add(tmpList);
      }


      return res;
    }

    private static void FiltrateSparkles(List<List<PointD>> RawSparkles, List<double> signal, out List<List<PointD>> GoodSparkles, out List<List<PointD>> BadSparkles)
    {
      GoodSparkles = new List<List<PointD>>();
      BadSparkles = new List<List<PointD>>();

      double MaxVal = 0;

      for (int i = 0; i < RawSparkles.Count; i++)
      {
        if (RawSparkles[i].Count == 0) continue;
        MaxVal = signal.GetRange((int)RawSparkles[i][0].X, RawSparkles[i].Count).Max();

        if (MaxVal > 1.075 * RawSparkles[i][0].Y)
          BadSparkles.Add(RawSparkles[i]);
        else GoodSparkles.Add(RawSparkles[i]);
          
      }
    }
  }
}
