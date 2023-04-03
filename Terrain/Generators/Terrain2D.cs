using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Terrain.Controls;
using Terrain.DataTypes;

namespace Terrain.Generators
{
    public class Terrain2D : IDisposable
    {
        readonly GL _GL;
        public PerlinTexture Noise { get; }
        uint VAO, VBO, VBE;

        public List<HeightColor> heightMap = new()
            {
                new HeightColor("Deep Water", 0f, new Vector3D<float>(4, 10, 112) / 255),
                new HeightColor("Shalow Water", 0.270f, new Vector3D<float>(18, 90, 199) / 255),
                new HeightColor("Sand", 0.370f, new Vector3D<float>(235, 218, 110) / 255),
                new HeightColor("Land", 0.440f, new Vector3D<float>(66, 184, 37) / 255),
                new HeightColor("High Land", 0.570f, new Vector3D<float>(32, 125, 9) / 255),
                new HeightColor("Mountain", 0.710f, new Vector3D<float>(38, 30, 16) / 255),
                new HeightColor("Ice", 0.830f, new Vector3D<float>(207, 198, 184) / 255)
            };
        Matrix4X4<float> Model;
        public Terrain2D(GL gl, PerlinTexture perlinTexture)
        {
            _GL = gl;
            Noise = perlinTexture;

            SetUp();
            Vector2D<float> scale = new Vector2D<float>(Noise.width, Noise.height)/(64*4);
            Model = Matrix4X4.CreateScale(scale.X, scale.Y, 1);

        }
        public unsafe void Draw(Controls.Shader shader)
        {
            _GL.BindVertexArray(VAO);

            Noise.Texture.setTexture(shader);
            shader.setUniform(Model, "Model");
            SetColors(shader);

            _GL.DrawElements(GLEnum.Triangles, (uint)indices.Length, DrawElementsType.UnsignedInt, null);

            _GL.BindVertexArray(0);
        }
        private unsafe void SetUp()
        {
            VAO = _GL.GenVertexArray();
            _GL.BindVertexArray(VAO);

            VBO = _GL.GenBuffer();
            _GL.BindBuffer(GLEnum.ArrayBuffer, VBO);

            fixed (void* pointer = &texels[0])
            {
                _GL.BufferData(GLEnum.ArrayBuffer, (nuint)(sizeof(Texel) * texels.Length), pointer, GLEnum.StreamDraw);
            }

            _GL.VertexAttribPointer(0, 3, GLEnum.Float, false, (uint)sizeof(Texel), null);
            _GL.EnableVertexAttribArray(0);

            _GL.VertexAttribPointer(1, 2, GLEnum.Float, false, (uint)sizeof(Texel), (void*)sizeof(Vector3D<float>));
            _GL.EnableVertexAttribArray(1);

            VBE = _GL.GenBuffer();
            _GL.BindBuffer(GLEnum.ElementArrayBuffer, VBE);

            fixed (void* pointer = &indices[0])
            {
                _GL.BufferData(GLEnum.ElementArrayBuffer, (nuint)(sizeof(uint) * indices.Length), pointer, GLEnum.StreamDraw);
            }
            _GL.BindVertexArray(0);
        }
        private unsafe void SetColors(Controls.Shader shader)
        {
            for (int i = 0; i < heightMap.Count; i++)
            {
                shader.setUniform(heightMap[i].Height, $"heightColors[{i}].height");
                shader.setUniform(heightMap[i].Color , $"heightColors[{i}].color");
            }
        }
        #region Basic Square
        readonly static private Texel[] texels =
        {
            new(new(1, 1, 1), new(1, 1)), new(new(1, 0, 1), new(1, 0)),
            new(new(0, 0, 1), new(0, 0)), new(new(0, 1, 1), new(0, 1))
        };
        readonly static private uint[] indices =
        {
            0, 1, 3,
            1, 2, 3
        };
        #endregion
        #region Dispose
        bool disposed = false;
        ~Terrain2D()
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
                Noise.Dispose();
            }

            _GL.DeleteVertexArray(VAO);
            _GL.DeleteBuffer(VBO);
            _GL.DeleteBuffer(VBE);

            disposed = true;
        }
        #endregion
    }
    public struct HeightColor
    {
        public string Name;
        public float Height;
        public Vector3D<float> Color;
        public HeightColor(string name, float hight, Vector3D<float> color)
        {
            Name = name;
            Height = hight;
            Color = color;
        }
    }
}
