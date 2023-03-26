using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace Terrain.controls
{
    public class Shader : IDisposable
    {
        private readonly GL Gl;
        public readonly uint Handle;
        public Shader(GL gl, string vertShaderRelativePath, string fragShaderRelativePath)
        {
            Gl = gl;

            Handle = Gl.CreateProgram();
            uint vert = Gl.CreateShader(ShaderType.VertexShader);
            uint frag = Gl.CreateShader(ShaderType.FragmentShader);

            string curPath = Directory.GetCurrentDirectory();
            Gl.ShaderSource(vert, Path.Join(curPath, vertShaderRelativePath));
            Gl.ShaderSource(frag, Path.Join(curPath, fragShaderRelativePath));

            Gl.CompileShader(vert);
            Gl.CompileShader(frag);

            Gl.AttachShader(Handle, vert);
            Gl.AttachShader(Handle, frag);

            Gl.LinkProgram(Handle);
            Gl.GetShader(vert, ShaderParameterName.CompileStatus, out int vertStatus);
            Gl.GetShader(frag, ShaderParameterName.CompileStatus, out int fragStatus);
            Gl.GetProgram(Handle, ProgramPropertyARB.LinkStatus, out int progStatus);
            if (vertStatus + fragStatus + progStatus > 0)
            {
                Console.WriteLine($"Vertex Shader Log:\n" +
                                  $"{Gl.GetShaderInfoLog(vert)}\n\n" +
                                  $"Fragment Shader Log:\n" +
                                  $"{Gl.GetShaderInfoLog(frag)}\n\n" +
                                  $"Program Link Status Log:\n" +
                                  $"{Gl.GetProgramInfoLog(Handle)}\n\n");
            }

            Gl.DetachShader(Handle, vert);
            Gl.DetachShader(Handle, frag);

            Gl.DeleteShader(vert);
            Gl.DeleteShader(frag);
        }
        public void Use() => Gl.UseProgram(Handle);
        public unsafe void setUniform(uint value, string uniformName, bool transpose = false)
        {
            Gl.Uniform1(getUniformLocation(uniformName), value);
        }
        public unsafe void setUniform(Matrix4X4<float> matrix, string uniformName, bool transpose = false)
        {
            Gl.UniformMatrix4(getUniformLocation(uniformName), 1, transpose, (float*)&matrix);
        }
        public int getUniformLocation(string uniformName) => Gl.GetUniformLocation(Handle, uniformName);

        #region
        private bool disposed = false;
        ~Shader()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            if (disposed) return;
            if (!disposed)
            {
                // Good practice
            }
            Gl.DeleteProgram(Handle);
            disposed = true;
        }
        #endregion
    }
}
