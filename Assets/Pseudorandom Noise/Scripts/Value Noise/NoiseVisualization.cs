using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

using static Noise;

public class NoiseVisualization : Visualization
{
    private static int noiseId = Shader.PropertyToID("_Noise");

    [SerializeField]
    private int seed;

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
        Job<Lattice1D>.ScheduleParallel(positions, noise, seed, domain, resolution, handle).Complete();
        noiseBuffer.SetData(noise.Reinterpret<float>(4 * 4));
    }
}