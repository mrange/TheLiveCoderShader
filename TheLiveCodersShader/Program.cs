using System;
using System.Text;
using OpenGL;

namespace TheLiveCodersShader
{
  /// <summary>
  /// Program abstraction.
  /// </summary>
  class Program : IDisposable
  {
    public Program(string[] vertexSource, string[] fragmentSource)
    {
      // Create vertex and frament shaders
      // Note: they can be disposed after linking to program; resources are freed when deleting the program
      using (Object vObject = new Object(ShaderType.VertexShader, vertexSource))
      using (Object fObject = new Object(ShaderType.FragmentShader, fragmentSource))
      {
        // Create program
        ProgramName = Gl.CreateProgram();
        // Attach shaders
        Gl.AttachShader(ProgramName, vObject.ShaderName);
        Gl.AttachShader(ProgramName, fObject.ShaderName);
        // Link program
        Gl.LinkProgram(ProgramName);

        // Check linkage status
        int linked;

        Gl.GetProgram(ProgramName, ProgramProperty.LinkStatus, out linked);

        if (linked == 0)
        {
          const int logMaxLength = 1024;

          StringBuilder infolog = new StringBuilder(logMaxLength);
          int infologLength;

          Gl.GetProgramInfoLog(ProgramName, 1024, out infologLength, infolog);

          throw new InvalidOperationException($"unable to link program: {infolog}");
        }

        // Get uniform locations
        if ((LocationResolution = Gl.GetUniformLocation(ProgramName, "iResolution")) < 0)
          throw new InvalidOperationException("no uniform iResolution");

        if ((LocationTime = Gl.GetUniformLocation(ProgramName, "iTime")) < 0)
          throw new InvalidOperationException("no uniform iTime");

        // Get attributes locations
        if ((LocationPosition = Gl.GetAttribLocation(ProgramName, "a_position")) < 0)
          throw new InvalidOperationException("no attribute a_position");
      }
    }

    public readonly uint ProgramName;

    public readonly int LocationResolution;

    public readonly int LocationTime;

    public readonly int LocationPosition;

    public void Dispose()
    {
      Gl.DeleteProgram(ProgramName);
    }
  }

}
