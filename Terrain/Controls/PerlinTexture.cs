using Silk.NET.OpenGL;
using System.Runtime.CompilerServices;

namespace Terrain.Controls
{
    public class PerlinTexture : IDisposable
    {
        readonly GL _GL;

        public Texture2D Texture { get { return texture; } }
        Texture2D texture;

        PerlinNoise _noise;
        public readonly uint width, height;
        byte[,] bytes;
        public byte[,] Bytes { get { return bytes; } }

        public PerlinTexture(GL GL, int seed, uint gridSize, uint gridVectorSize)
                         : this(GL, new(seed, (int)gridSize, (int)gridVectorSize)) { }
        public PerlinTexture(GL GL, PerlinNoise perlinNoise)
        {
            _GL = GL;
            _noise = perlinNoise;

            width  = (uint)(perlinNoise._gridVectorWidth  * perlinNoise._gridUnitWidth );
            height = (uint)(perlinNoise._gridVectorHeight * perlinNoise._gridUnitHeight);
            SetUpTexture();
        }
        private unsafe void SetUpTexture()
        {
            uint texHandle = _GL.GenTexture();
            _GL.BindTexture(GLEnum.Texture2D, texHandle);

            bytes = GetBytes();
            fixed (void* pointer = &bytes[0, 0])
            {
                _GL.TexImage2D(GLEnum.Texture2D, 0, (int)GLEnum.Red, width, height,
                               0, GLEnum.Red, GLEnum.UnsignedByte, pointer);
            }

            _GL.GenerateMipmap(TextureTarget.Texture2D);
            texture = new(_GL, texHandle, "Noise");

            _GL.TexParameter(GLEnum.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.ClampToEdge);
            _GL.TexParameter(GLEnum.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.ClampToEdge);

            _GL.TexParameter(GLEnum.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Nearest);
            _GL.TexParameter(GLEnum.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Nearest);

            _GL.TexParameter(GLEnum.Texture2D, TextureParameterName.TextureSwizzleG, (int)GLEnum.Red);
            _GL.TexParameter(GLEnum.Texture2D, TextureParameterName.TextureSwizzleB, (int)GLEnum.Red);
            _GL.TexParameter(GLEnum.Texture2D, TextureParameterName.TextureSwizzleA, (int)GLEnum.One);

            _GL.BindTexture(GLEnum.Texture2D, 0);
        }
        private byte[,] GetBytes()
        {
            byte[,] data = new byte[width, height];

            for (int u = 0; u < width; u++)
            {
                for (int v = 0; v < height; v++)
                {
                    data[u, v] = (byte)((_noise.getPoint(u, v) + 1) * 127.5f);
                }
            }
            return data;
        }
        public void ReSetPerlin(int? seed=null)
        {
            int Seed = seed is null ? new Random().Next() : (int)seed;
            _noise = new PerlinNoise(Seed, _noise._gridUnitWidth, _noise._gridUnitHeight,
                                          _noise._gridVectorWidth, _noise._gridVectorHeight);
            ReSetPerlin(_noise);
        }
        public unsafe void ReSetPerlin(PerlinNoise noise)
        {
            _GL.BindTexture(GLEnum.Texture2D, texture.Handle);
            bytes = GetBytes();
            fixed (void* pointer = &bytes[0, 0])
            {
                _GL.TexImage2D(GLEnum.Texture2D, 0, (int)GLEnum.Red, width, height,
                               0, GLEnum.Red, GLEnum.UnsignedByte, pointer);
            }
            _GL.BindTexture(GLEnum.Texture2D, 0);
        }
        #region Dispose
        bool disposed = false;
        ~PerlinTexture()
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
                texture.Dispose();
            }
            disposed = true;
        }
        #endregion
    }
}
