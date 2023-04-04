using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System.Numerics;
using Terrain.Controls;
using Terrain.DataTypes;

namespace Terrain.Generators
{
    public class Terrain3D : IDisposable
    {
        readonly GL _GL;
        uint VAO, VBO, VBE;
        uint indiciesLength = 0;

        public PerlinTexture _perlinTexture;
        public float perlinScalar = 1;

        Matrix4X4<float> _model = Matrix4X4<float>.Identity;
        public Terrain3D(GL gl, PerlinTexture perlinTexture)
        {
            _GL = gl;
            _perlinTexture = perlinTexture;
            SetUp();
        }
        public unsafe void Draw(Controls.Shader shader)
        {
            _GL.BindVertexArray(VAO);

            _perlinTexture.Texture.setTexture(shader);
            shader.SetUniform(perlinScalar, "PerlinScalar");
            shader.SetUniform(_model, "Model");

            _GL.DrawElements(GLEnum.Triangles, indiciesLength, DrawElementsType.UnsignedInt, null);
            _GL.BindVertexArray(0);
        }
        private unsafe void SetUp()
        {
            Vector2D<int> planeSize = (Vector2D<int>)new Vector2D<uint>(_perlinTexture.width, _perlinTexture.height);
            var verticies = VerticiesFactory(planeSize);
            var indicies = IndiciesFactory(planeSize);

            var verticiesLength = planeSize.X * planeSize.Y;
            indiciesLength = (uint)indicies.Length;

            VAO = _GL.GenVertexArray();
            _GL.BindVertexArray(VAO);

            VBO = _GL.GenBuffer();
            _GL.BindBuffer(GLEnum.ArrayBuffer, VBO);

            fixed (void* pointer = &verticies[0])
            {
                _GL.BufferData(GLEnum.ArrayBuffer, (nuint)(sizeof(Vector3D<float>) * verticies.Length), pointer, GLEnum.StaticDraw);
            }

            _GL.VertexAttribPointer(0, 3, GLEnum.Float, false, sizeof(float)*3, null);
            _GL.EnableVertexAttribArray(0);

            VBE = _GL.GenBuffer();
            _GL.BindBuffer(GLEnum.ElementArrayBuffer, VBE);

            fixed (void* pointer = &indicies[0])
            {
                _GL.BufferData(GLEnum.ElementArrayBuffer, (nuint)(sizeof(uint) * indicies.Length), pointer, GLEnum.StaticDraw);
            }
            _GL.BindVertexArray(0);

        }
        private static Vector3D<float>[] VerticiesFactory(Vector2D<int> planeSize)
        {
            Vector3D<float>[] verticies = new Vector3D<float>[planeSize.X * planeSize.Y];
            for (int row = 0; row < planeSize.X; row++)
            {
                for (int col = 0; col < planeSize.Y; col++)
                {
                    verticies[planeSize.X * row + col] = new Vector3D<float>(row, 0, col);
                }
            }
            return verticies;
        }
        private static uint[] IndiciesFactory(Vector2D<int> planeSize)
        {
            uint[] indicies = new uint[(planeSize.X - 1) * (planeSize.Y - 1) * 2 * 3];

            uint index = 0;
            for (int row = 0; row < planeSize.X - 1; row++)
            {
                for (int col = 0; col < planeSize.Y - 1; col++)
                {
                    indicies[index++] = (uint)(planeSize.X * row + col);
                    indicies[index++] = (uint)(planeSize.X * row + col + planeSize.X + 1);
                    indicies[index++] = (uint)(planeSize.X * row + col + planeSize.X);

                    indicies[index++] = (uint)(planeSize.X * row + col);
                    indicies[index++] = (uint)(planeSize.X * row + col + 1);
                    indicies[index++] = (uint)(planeSize.X * row + col + planeSize.X + 1);
                }
            }

            return indicies;
        }
        #region Dispose
        bool disposed = false;
        ~Terrain3D()
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
            if (disposing)
            {
                _perlinTexture.Dispose();
            }

            _GL.DeleteVertexArray(VAO);
            _GL.DeleteBuffer(VBO);
            _GL.DeleteBuffer(VBE);

            disposed = true;
        }
        #endregion
    }
}