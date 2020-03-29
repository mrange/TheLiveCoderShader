using Khronos;
using System;
using System.Windows.Forms;

// Based upon:
//  https://github.com/luca-piccioni/OpenGL.Net/tree/master/Samples/HelloTriangle
namespace TheLiveCodersShader
{
  static class TheLiveCoders
  {
    public static void DebugTrace(string message)
    {
      Console.WriteLine(message);
      System.Diagnostics.Debug.WriteLine(message);
    }

    [STAThread]
    static void Main()
    {
      string envDebug = Environment.GetEnvironmentVariable("DEBUG");
      if (envDebug == "GL")
      {
        KhronosApi.Log += delegate(object sender, KhronosLogEventArgs e)
        {
          DebugTrace(e.ToString());
        };
        KhronosApi.LogEnabled = true;
      }

      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new Main());
    }
  }
}
