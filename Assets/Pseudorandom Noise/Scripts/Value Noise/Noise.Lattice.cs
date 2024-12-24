using Unity.Mathematics;

using static Unity.Mathematics.math;

public static partial class Noise
{
    public struct Lattice1D : INoise
    {
        public float4 GetNoise4(float4x3 positions, SmallXXHash4 hash)
        {
            return 0f;
        }
    }
}