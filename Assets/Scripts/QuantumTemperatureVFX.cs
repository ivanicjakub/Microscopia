using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(VisualEffect))]
public class QuantumTemperatureVFX : MonoBehaviour
{
    [Header("Compute Settings")]
    [SerializeField] private ComputeShader computeShader;
    [SerializeField] private int particleCount = 100000;

    [Header("CMB Settings")]
    [SerializeField] private Cubemap cmbSkybox;
    [SerializeField] private Color coldColor = new Color(0, 0, 0.8f, 1); // Cold CMB regions
    [SerializeField] private Color hotColor = new Color(1, 0.8f, 0, 1);  // Hot CMB regions

    [Header("Temperature Mapping")]
    [SerializeField, Range(0.1f, 10f)] private float temperatureScale = 1f;
    [SerializeField, Range(0.1f, 10f)] private float energyScale = 1f;
    [SerializeField, Range(0.1f, 10f)] private float quantumFluctuationScale = 1f;

    [Header("Simulation Bounds")]
    [SerializeField] private Vector3 boundsCentre;
    [SerializeField] private float boundsSize = 10f;
    [SerializeField] private float maxLifetime = 5f;

    private VisualEffect vfx;
    private GraphicsBuffer particleBuffer;
    private int kernelIndex;
    private const int THREAD_GROUP_SIZE = 256;

    private void Start()
    {
        vfx = GetComponent<VisualEffect>();
        InitializeCompute();
    }

    private void InitializeCompute()
    {
        // Find kernel and setup compute shader
        kernelIndex = computeShader.FindKernel("CSMain");

        // Create GraphicsBuffer directly instead of ComputeBuffer
        particleBuffer = new GraphicsBuffer(
            GraphicsBuffer.Target.Structured | GraphicsBuffer.Target.Raw,  // Make it usable by both Compute and VFX
            particleCount,
            sizeof(float) * 13  // size of your particle struct
        );

        // Initialize particle data
        var particles = new Particle[particleCount];
        for (int i = 0; i < particleCount; i++)
        {
            particles[i] = new Particle
            {
                position = Random.insideUnitSphere * boundsSize + boundsCentre,
                velocity = Vector3.zero,
                color = Color.white,
                temperature = 2.725f, // Base CMB temperature
                energy = 0,
                lifetime = Random.value * maxLifetime
            };
        }
        particleBuffer.SetData(particles);

        // Set compute shader parameters
        SetComputeParameters();

        // Set VFX Graph parameters
        vfx.SetInt("ParticleCount", particleCount);
        vfx.SetGraphicsBuffer("ParticleBuffer", particleBuffer);
    }

    private void SetComputeParameters()
    {
        computeShader.SetBuffer(kernelIndex, "particles", particleBuffer);
        computeShader.SetTexture(kernelIndex, "cmbSkybox", cmbSkybox);
        computeShader.SetFloat("maxLifetime", maxLifetime);
        computeShader.SetInt("numParticles", particleCount);
        computeShader.SetVector("boundsCentre", boundsCentre);
        computeShader.SetFloat("boundsSize", boundsSize);

        // Temperature mapping parameters
        computeShader.SetVector("coldColor", coldColor);
        computeShader.SetVector("hotColor", hotColor);
        computeShader.SetFloat("temperatureScale", temperatureScale);
        computeShader.SetFloat("energyScale", energyScale);
        computeShader.SetFloat("quantumFluctuationScale", quantumFluctuationScale);
    }

    private void Update()
    {
        computeShader.SetFloat("deltaTime", Time.deltaTime);
        computeShader.SetFloat("time", Time.time);

        int numThreadGroups = Mathf.CeilToInt(particleCount / (float)THREAD_GROUP_SIZE);
        computeShader.Dispatch(kernelIndex, numThreadGroups, 1, 1);
    }

    private void OnDestroy()
    {
        if (particleBuffer != null)
        {
            particleBuffer.Release();
            particleBuffer = null;
        }
    }

    private struct Particle
    {
        public Vector3 position;
        public Vector3 velocity;
        public Color color;
        public float temperature;
        public float energy;
        public float lifetime;
    }
}