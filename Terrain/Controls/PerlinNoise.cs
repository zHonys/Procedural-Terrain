using Silk.NET.Maths;

namespace Terrain.Controls
{
    public class PerlinNoise
    {
        Random _random;

        public readonly int Seed;
        int vectorsGenerated = 0;
        public int _gridVectorWidth { get; }
        public int _gridVectorHeight { get; }
        Vector2D<float>[,] _vectors;

        public int _gridUnitWidth { get; }
        public int _gridUnitHeight { get; }
        public PerlinNoise(int seed, int gridSize, int gridVectorSize) : this(seed, gridSize, gridSize, gridVectorSize, gridVectorSize) { }
        public PerlinNoise(int seed, int gridUnitWidth = 64, int gridUnitHeight = 64, int gridVectorWidth = 8, int gridVectorHeight = 8)
        {
            _random = new Random(seed);
            this.Seed = seed;

            _gridVectorWidth = gridVectorWidth;
            _gridVectorHeight = gridVectorHeight;
            _vectors = genVectors();

            _gridUnitWidth = gridUnitWidth;
            _gridUnitHeight = gridUnitHeight;

        }
        private static Vector2D<int> getPoint(int num)
        {
            int frq = (int)MathF.Sqrt(num + 1);
            if (frq == MathF.Sqrt(num + 1)) return new Vector2D<int>(frq - 1, frq - 1);

            int dis = num - (int)MathF.Pow(frq, 2);
            if (dis < frq) return new Vector2D<int>(frq, dis);
            return new Vector2D<int>(dis - frq, frq);
        }
        private static int getNum(Vector2D<int> pos)
        {
            if (pos.X == pos.Y) return (int)MathF.Pow(pos.X + 1, 2) - 1;
            if (pos.X > pos.Y) return (int)pos.Y + (int)MathF.Pow(pos.X, 2);
            return (int)(pos.X + pos.Y + MathF.Pow(pos.Y, 2));
        }

        private Vector2D<float>[,] genVectors()
        {
            int max = (int)MathF.Max(_gridVectorWidth, _gridVectorHeight);
            var vectors = new Vector2D<float>[max * 2, max * 2];

            int vectorsNum = getNum(new Vector2D<int>(_gridVectorWidth - 1, _gridVectorHeight - 1)) + 1;
            vectorsGenerated += vectorsNum;

            float theta; Vector2D<int> pos;
            for (int i = 0; i < vectorsNum; i++)
            {
                pos = getPoint(i);
                theta = _random.NextSingle() * MathF.PI * 2;
                vectors[(int)pos.X, (int)pos.Y] = new Vector2D<float>(MathF.Cos(theta), MathF.Sin(theta));
            }
            return vectors;
        }
        private float fade(float t)
        {
            return ((6 * t - 15) * t + 10) * t * t * t;
        }
        private float lerp(float s, float f, float t)
        {
            return s + t * (f - s);
        }
        private float Noise(int x, int y)
        {
            int X = x % (_gridUnitWidth * _gridVectorWidth) / _gridUnitWidth;
            int Y = y % (_gridUnitHeight * _gridVectorHeight) / _gridUnitHeight;

            float Xf = x % _gridUnitWidth / (float)_gridUnitWidth;
            float Yf = y % _gridUnitHeight / (float)_gridUnitHeight;
            Vector2D<float> point = new(Xf, Yf);

            float u = fade(Xf);
            float v = fade(Yf);
            float[,] dots = new float[2, 2];
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    float dot = Vector2D.Dot(_vectors[(X + i) % _gridVectorWidth, (Y + j) % _gridVectorHeight],
                                            new Vector2D<float>(i, j) - point);
                    dots[i, j] = dot;
                }
            }
            return lerp(lerp(dots[0, 0], dots[0, 1], v),
                        lerp(dots[1, 0], dots[1, 1], v),
                        u);
        }
        public float getPoint(int x, int y, int octaves = 8, float persistence = 0.5f, float frequency = 2)
        {
            float total = 0;
            for (int i = 0; i < octaves; i++)
            {
                total += Noise((int)(x * MathF.Pow(frequency, i)), (int)(y * MathF.Pow(frequency, i))) * MathF.Pow(persistence, i);
            }
            return total;
        }
        public float getPointOctave(int x, int y, int octave = 1, float persistence = 0.5f, float frequency = 2)
        {
            return Noise((int)(x * MathF.Pow(frequency, octave)), (int)(y * MathF.Pow(frequency, octave))) * MathF.Pow(persistence, octave);
        }
    }
}
