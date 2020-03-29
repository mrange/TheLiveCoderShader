﻿using System;
using OpenGL;

namespace TheLiveCodersShader
{
  /// <summary>
  /// Buffer abstraction.
  /// </summary>
  class Buffer : IDisposable
  {
    public Buffer(float[] buffer)
    {
      if (buffer == null)
        throw new ArgumentNullException(nameof(buffer));

      // Generate a buffer name: buffer does not exists yet
      BufferName = Gl.GenBuffer();
      // First bind create the buffer, determining its type
      Gl.BindBuffer(BufferTarget.ArrayBuffer, BufferName);
      // Set buffer information, 'buffer' is pinned automatically
      Gl.BufferData(BufferTarget.ArrayBuffer, (uint)(4 * buffer.Length), buffer, BufferUsage.StaticDraw);
    }

    public readonly uint BufferName;

    public void Dispose()
    {
      Gl.DeleteBuffers(BufferName);
    }
  }

}
