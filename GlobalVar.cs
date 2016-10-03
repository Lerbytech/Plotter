using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plotter
{
  static public class GlobalVar
  {

    private static string import_path_65 = @"C:\Users\Admin\Desktop\Антон\EXPERIMENTS\Processed\Processed\2CH_65 part 3\Neurons Data\Images";
    private static string export_path_65 = @"C:\Users\Admin\Desktop\Антон\EXPERIMENTS\Processed\Processed\2CH_65 part 3\";

    private static string import_path_2 = @"C:\Users\Михаил\YandexDisk\TEST\Neurons Data\Images\";
    private static string export_path_2 = @"C:\Users\Михаил\YandexDisk\TEST\Export\";

    //private static string import_path_64 = @"C:\Users\Admin\Desktop\Антон\EXPERIMENTS\Processed\Processed\2CH_64 part 2\Neurons Data\Images";
    //private static string export_path_64 = @"C:\Users\Admin\Desktop\Антон\EXPERIMENTS\Processed\Processed\2CH_64 part 2\";

    private static string import_path_64 = @"C:\Users\Admin\Desktop\Антон\EXPERIMENTS\2CH64_Processed\Neurons Data\Images";
    private static string export_path_64 = @"C:\Users\Admin\Desktop\Антон\EXPERIMENTS\2CH64_Processed\";

    public static string import_path;
    public static string export_path;

    private static int _channel_id;
    public static int channel_id
    {
      get
      {
        return _channel_id;
      }
      set
      {
        _channel_id = value;

        if (value == 65)
        {
          import_path = import_path_65;
          export_path = export_path_65;
        }
        else 
        if (value == 64)
        {
          import_path = import_path_64;
          export_path = export_path_64;
        }
        else throw new Exception("error - not defined path");

      }
    }
      private static string _cluster_path;

      public static string cluster_path
      {
        private set
        {
          _cluster_path = value;
        }

         get
        {
          return @"C:\Users\Admin\Desktop\Антон\Crop\export_" + channel_id.ToString();
        }

      }



      public static double FPS = 24;

      public static double clustTimer = 25000;
  }
}
