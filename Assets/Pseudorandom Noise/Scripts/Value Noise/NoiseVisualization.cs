using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

using static Noise;

public class NoiseVisualization : Visualization
{
    private static ScheduleDelegate[,] noiseJobs =
    {
        {
            Job<Lattice1D<LatticeNormal, Perlin>>.ScheduleParallel,
            Job<Lattice1D<LatticeTilling, Perlin>>.ScheduleParallel,
            Job<Lattice2D<LatticeNormal, Perlin>>.ScheduleParallel,
            Job<Lattice2D<LatticeTilling, Perlin>>.ScheduleParallel,
            Job<Lattice3D<LatticeNormal, Perlin>>.ScheduleParallel,
            Job<Lattice3D<LatticeTilling, Perlin>>.ScheduleParallel
        },
        {
            Job<Lattice1D<LatticeNormal, Turbulence<Perlin>>>.ScheduleParallel,
            Job<Lattice1D<LatticeTilling, Turbulence<Perlin>>>.ScheduleParallel,
            Job<Lattice2D<LatticeNormal, Turbulence<Perlin>>>.ScheduleParallel,
            Job<Lattice2D<LatticeTilling, Turbulence<Perlin>>>.ScheduleParallel,
            Job<Lattice3D<LatticeNormal, Turbulence<Perlin>>>.ScheduleParallel,
            Job<Lattice3D<LatticeTilling, Turbulence<Perlin>>>.ScheduleParallel
        },
        {
            Job<Lattice1D<LatticeNormal, Value>>.ScheduleParallel,
            Job<Lattice1D<LatticeTilling, Value>>.ScheduleParallel,
            Job<Lattice2D<LatticeNormal, Value>>.ScheduleParallel,
            Job<Lattice2D<LatticeTilling, Value>>.ScheduleParallel,
            Job<Lattice3D<LatticeNormal, Value>>.ScheduleParallel,
            Job<Lattice3D<LatticeTilling, Value>>.ScheduleParallel
        },
        {
            Job<Lattice1D<LatticeNormal, Turbulence<Value>>>.ScheduleParallel,
            Job<Lattice1D<LatticeTilling, Turbulence<Value>>>.ScheduleParallel,
            Job<Lattice2D<LatticeNormal, Turbulence<Value>>>.ScheduleParallel,
            Job<Lattice2D<LatticeTilling, Turbulence<Value>>>.ScheduleParallel,
            Job<Lattice3D<LatticeNormal, Turbulence<Value>>>.ScheduleParallel,
            Job<Lattice3D<LatticeTilling, Turbulence<Value>>>.ScheduleParallel
        }
    };
    
    private static int noiseId = Shader.PropertyToID("_Noise");

    [SerializeField]
    private Settings noiseSettings = Settings.Default;

    public enum NoiseType
    {
        Perlin, PerlinTurbulence, Value, ValueTurbulence
    }

    [SerializeField]
    private NoiseType type;

    [SerializeField, Range(1, 3)]
    private int dimensions = 3;

    [SerializeField]
    private bool tilling;

    [SerializeField] private SpaceTRS domain = new SpaceTRS
    {
        scale = 8f
    };

    private NativeArray<float4> noise;

    private ComputeBuffer noiseBuffer;

    protected override void EnableVisualization(int dataLength, MaterialPropertyBlock propertyBlock)
    {
        noise = new NativeArray<float4>(dataLength, Allocator.Persistent);
        noiseBuffer = new ComputeBuffer(dataLength * 4, 4);

        propertyBlock.SetBuffer(noiseId, noiseBuffer);
    }

    protected override void DisableVisualization()
    {
        noise.Dispose();
        noiseBuffer.Release();
        noiseBuffer = null;
    }

    protected override void UpdateVisualization(NativeArray<float3x4> positions, int resolution, JobHandle handle)
    {
        noiseJobs[(int)type, 2 * dimensions - (tilling ? 1 : 2)](positions, noise, noiseSettings, domain, resolution, handle).Complete();
        noiseBuffer.SetData(noise.Reinterpret<float>(4 * 4));
    }
}