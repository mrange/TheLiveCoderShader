using System;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

using Khronos;
using OpenGL;

namespace TheLiveCodersShader
{
  public partial class Main : Form
  {
    static readonly string[] _vs = {
@"
in vec4 a_position;

out VertexData
{
  vec4 v_position;
} outData;

uniform vec2  iResolution;
uniform float iTime;

void main()
{
  // Some drivers don't like position being written here
  // with the tessellation stages enabled also.
  // Comment next line when Tess.Eval shader is enabled.
  gl_Position = a_position;

  outData.v_position  = a_position;
}
"
    };

    static readonly string[] _fs = {
@"
#version 150 compatibility

in VertexData
{
  vec4 v_position;
} inData;

out vec4 fragColor;

uniform vec2  iResolution;
uniform float iTime;

void mainImage(out vec4 fragColor, in vec2 fragCoord);
void main() {
  vec2 pos = 0.5*(1.0 + inData.v_position.xy)*iResolution;
  mainImage(fragColor, pos);
}
",
@"
#define PI     3.141592654
#define TAU    (2.0*PI)
#define SCA(a) vec2(sin(a), cos(a))
#define TTIME  (iTime*TAU)
#define PERIOD 600.0

const float a1 = PI/2.0;
const float a2 = PI*4.5/6.0;

const vec2 sca1 = SCA(a1);
const vec2 sca2 = SCA(a2);

vec2 hash(vec2 p) {
  p = vec2(dot (p, vec2 (127.1, 311.7)), dot (p, vec2 (269.5, 183.3)));
  return -1. + 2.*fract (sin (p)*43758.5453123);
}

float noise(vec2 p) {
  const float K1 = .366025404;
  const float K2 = .211324865;

  vec2 i = floor (p + (p.x + p.y)*K1);
    
  vec2 a = p - i + (i.x + i.y)*K2;
  vec2 o = step (a.yx, a.xy);    
  vec2 b = a - o + K2;
  vec2 c = a - 1. + 2.*K2;

  vec3 h = max (.5 - vec3 (dot (a, a), dot (b, b), dot (c, c) ), .0);

  vec3 n = h*h*h*h*vec3 (dot (a, hash (i + .0)),dot (b, hash (i + o)), dot (c, hash (i + 1.)));

  return dot (n, vec3 (70.));
}

float fbm(vec2 pos, float tm) {
  vec2 offset = vec2(cos(tm), sin(tm*sqrt(0.5)));
  float aggr = 0.0;
    
  aggr += noise(pos);
  aggr += noise(pos + offset) * 0.5;
  aggr += noise(pos + offset.yx) * 0.25;
  aggr += noise(pos - offset) * 0.125;
  aggr += noise(pos - offset.yx) * 0.0625;
    
  aggr /= 1.0 + 0.5 + 0.25 + 0.125 + 0.0625;
    
  return (aggr * 0.5) + 0.5;    
}

vec3 lightning(vec2 pos, float offset) {
  vec3 col = vec3(0.0);
  vec2 f = 10.0*SCA(PI/2.0 + TTIME/PERIOD);
    
  for (int i = 0; i < 3; i++) {
    float btime = TTIME*85.0/PERIOD + float(i);
    float rtime = TTIME*75.0/PERIOD + float(i) + 10.0;
    float d1 = abs(offset * 0.03 / (0.0 + offset - fbm((pos + f) * 3.0, rtime)));
    float d2 = abs(offset * 0.03 / (0.0 + offset - fbm((pos + f) * 2.0, btime)));
    col += vec3(d1 * vec3(0.1, 0.3, 0.8));
    col += vec3(d2 * vec3(0.7, 0.3, 0.5));
  }
    
  return col;
}

float mod1(inout float p, float size) {
  float halfsize = size*0.5;
  float c = floor((p + halfsize)/size);
  p = mod(p + halfsize, size) - halfsize;
  return c;
}

vec2 toPolar(vec2 p) {
  return vec2(length(p), atan(p.y, p.x));
}

vec2 toRect(vec2 p) {
  return vec2(p.x*cos(p.y), p.x*sin(p.y));
}

float pmin(float a, float b, float k) {
  float h = max(k-abs(a-b), 0.0)/k;
  return min(a, b) - h*h*k*(1.0/4.0);
}

float circle(vec2 p, float r) {
  return length(p) - r;
}

float box(vec2 p, vec2 b, vec4 r) {
  r.xy = (p.x>0.0)?r.xy : r.zw;
  r.x  = (p.y>0.0)?r.x  : r.y;
  vec2 q = abs(p)-b+r.x;
  return min(max(q.x,q.y),0.0) + length(max(q,0.0)) - r.x;
}

float arc(vec2 p, vec2 sca, vec2 scb, float ra, float rb) {
  p *= mat2(sca.x,sca.y,-sca.y,sca.x);
  p.x = abs(p.x);
  float k = (scb.y*p.x>scb.x*p.y) ? dot(p.xy,scb) : length(p.xy);
  return sqrt(dot(p,p) + ra*ra - 2.0*ra*k) - rb;
}

float spokes(vec2 p, float s) {
  vec2 pp = toPolar(p);
  pp.y += TTIME*40.0/PERIOD;
  mod1(pp.y, TAU/10.0);
  pp.y += PI/2.0;
  p = toRect(pp);
  float ds = box(p, s*vec2(0.075, 0.5), s*vec4(0.04));
  return ds;
}

float arcs(vec2 p, float s) {
  
  float d1 = arc(p, sca1, sca2, s*0.275, s*0.025);
  float d2 = arc(p, sca1, sca2, s*0.18, s*0.025);
  
  return min(d1, d2);
}

float meeple(vec2 p, float s) {
  float dh = box(p - s*vec2(0.0, -0.035), s*vec2(0.07, 0.1), s*vec4(0.065));
  float dc = box(p - s*vec2(0.0, -0.22), s*vec2(0.15, 0.04), s*vec4(0.05, 0.02, 0.05, 0.02));
  
  return pmin(dh, dc, s*0.115);
}

float theLiveCoders(vec2 p, float s) {
  float ds = spokes(p, s);
  float dc = circle(p, 0.375*s);
  float da = arcs(p, s);
  float dm = meeple(p, s);
  
  float d = ds;
  d = min(d, dc);
  d = max(d, -da);
  d = max(d, -dm);
  
  return d;
}

float df(vec2 p) {
  float d = theLiveCoders(p, 1.0 - 0.5*cos(TTIME*7.0/PERIOD));
  return d;
}

vec3 postProcess(vec3 col, vec2 q, vec2 p)  {
  col=pow(clamp(col,0.0,1.0),vec3(0.75)); 
  col=col*0.6+0.4*col*col*(3.0-2.0*col);
  col=mix(col, vec3(dot(col, vec3(0.33))), -0.4);
  col*=vec3(1.0 - tanh(pow(length(p/1.5), 5.0)));
  return col;
}

void mainImage(out vec4 fragColor, in vec2 fragCoord) {
  vec2 q = fragCoord/iResolution.xy;
  vec2 p = -1. + 2. * q;
  p.x *= iResolution.x/iResolution.y;
  vec2 op = p;

  p *= 1.0;

  float d = df(p);
  
  const vec3  background   = vec3(0.0)/vec3(255.0);

  vec3 col = background;

  float borderStep = 0.0075;
 
  vec3 baseCol = vec3(1.0);
  vec4 logoCol = vec4(baseCol, 1.0)*smoothstep(-borderStep, 0.0, -d);
  
  if (d >= 0.0) {
    vec2 pp = toPolar(p);
    float funky = 0.7*pow((0.5 - 0.5*cos(TTIME/PERIOD)), 4.0);
    pp.x *= 1./(pow(length(p) + funky, 15.0) + 1.0);
    p = toRect(pp);
    col += lightning(p, (pow(abs(d), 0.25 + 0.125*sin(0.5*iTime + p.x + p.y))));
  }
  col = clamp(col, 0.0, 1.0);

  col *= 1.0 - logoCol.xyz;

  col = postProcess(col, q, op);
  col *= smoothstep(0.0, 16.0, iTime*iTime);
  fragColor = vec4(col, 1.0);
}
"
    };

    static readonly float[] _triangleVertices = new float[] {
      -1.0f , -1.0f,
      +1.0f , -1.0f,
      +1.0f , +1.0f,
      -1.0f , +1.0f,
    };

    readonly Stopwatch sw = new Stopwatch();

    Program _program;
    VertexArray _vertices;

    public Main()
    {
      InitializeComponent();
      sw.Start();
    }

    void glControl_ContextCreated(object sender, GlControlEventArgs e)
    {
      GlControl glControl = (GlControl)sender;

      // GL Debugging
      if (Gl.CurrentExtensions != null && Gl.CurrentExtensions.DebugOutput_ARB)
      {
        Gl.DebugMessageCallback(GLDebugProc, IntPtr.Zero);
        Gl.DebugMessageControl(DebugSource.DontCare, DebugType.DontCare, DebugSeverity.DontCare, 0, null, true);
      }

      // Allocate resources and/or setup GL states
      switch (Gl.CurrentVersion.Api)
      {
        case KhronosVersion.ApiGl:
          if (Gl.CurrentVersion >= Gl.Version_320)
            RenderControl_CreateGL320();
          else
            RenderControl_CreateGL100();
          break;
        case KhronosVersion.ApiGles2:
          RenderControl_CreateGLES2();
          break;
      }

      /*
      // Uses multisampling, if available
      if (Gl.CurrentVersion != null && Gl.CurrentVersion.Api == KhronosVersion.ApiGl && glControl.MultisampleBits > 0)
      {
        Gl.Enable(EnableCap.Multisample);
      }
      */
    }

    void RenderControl_CreateGLES2()
    {
      throw new NotImplementedException("GLES not supported ATM");
    }

    void RenderControl_CreateGL100()
    {
      throw new NotImplementedException("GL v 1.00 not supported ATM");
    }

    void RenderControl_CreateGL320()
    {
      _program = new Program(_vs, _fs);
      _vertices = new VertexArray(_program, _triangleVertices);
    }

    static void GLDebugProc(DebugSource source, DebugType type, uint id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
    {
      string strMessage;

      unsafe
      {
        strMessage = Encoding.ASCII.GetString((byte*)message.ToPointer(), length);
      }

      TheLiveCoders.DebugTrace($"{source}, {type}, {severity}: {strMessage}");
    }

    void glControl_ContextDestroying(object sender, GlControlEventArgs e)
    {
      DestroyResources();
    }

    void DestroyResources()
    {
      _vertices?.Dispose();
      _program?.Dispose();
      _vertices = null;
      _program = null;
    }

    void glControl_ContextUpdate(object sender, GlControlEventArgs e)
    {

    }

    void glControl_Render(object sender, GlControlEventArgs e)
    {
      var time = sw.ElapsedMilliseconds / 1000.0F;
      var sz = ClientSize;
      var width = (float)sz.Width;
      var height = (float)sz.Height;

      Control senderControl = (Control)sender;

      Gl.Viewport(0, 0, senderControl.ClientSize.Width, senderControl.ClientSize.Height);
      Gl.Clear(ClearBufferMask.ColorBufferBit);

      switch (Gl.CurrentVersion.Api)
      {
        case KhronosVersion.ApiGl:
          if (Gl.CurrentVersion >= Gl.Version_320)
            RenderControl_RenderGL320(time, width, height);
          else if (Gl.CurrentVersion >= Gl.Version_110)
            RenderControl_RenderGL110(time, width, height);
          else
            RenderControl_RenderGL100(time, width, height);
          break;
        case KhronosVersion.ApiGles2:
          RenderControl_RenderGLES2(time, width, height);
          break;
      }
    }

    void RenderControl_RenderGL320(float time, float width, float height)
    {
      Gl.UseProgram(_program.ProgramName);
      Gl.Uniform1(_program.LocationTime, time);
      Gl.Uniform2(_program.LocationResolution, width, height);
      Gl.BindVertexArray(_vertices.ArrayName);
      Gl.DrawArrays(PrimitiveType.Quads, 0, 4);
    }

    void RenderControl_RenderGLES2(float time, float width, float height)
    {
      throw new NotImplementedException("GLES not supported ATM");
    }

    void RenderControl_RenderGL100(float time, float width, float height)
    {
      throw new NotImplementedException("GL v 1.00 not supported ATM");
    }

    void RenderControl_RenderGL110(float time, float width, float height)
    {
      throw new NotImplementedException("GL v 1.10 not supported ATM");
    }

    void Main_FormClosed(object sender, FormClosedEventArgs e)
    {
      DestroyResources();
    }
  }

}
