using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terrain.DataTypes
{
    public struct Texel
    {
        public Vector3D<float> Position;
        public Vector2D<float> UV;
        public Texel(Vector3D<float> position, Vector2D<float> texUV)
        {
            Position = position;
            UV = texUV;
        }
    }
}
