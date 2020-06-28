using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDecal : MonoBehaviour {

    public class ParticleDecalData{
        public Vector3 position;
        public float size;
        public Vector3 rotation;
        public Color color;
    }


    public int maxDecals = 100;
    public float decalSizeMin = .5f;
    public float decalSizeMax = 1.5f;
    public float offset = -0.1f;

    private ParticleSystem decalParticleSystem;
    private int particleDecalDataIndex;
    private ParticleDecalData[] particleData;
    private ParticleSystem.Particle[] particles;

    private Spawner spawner;
    private Vector3 invisibilePosition;

    #region Singleton
    public static ParticleDecal instance;

    private void Awake()
    {
        if (instance == null)
        instance = this;
    }
    #endregion

    private void Start()
    {
        invisibilePosition = new Vector3(0, 50, 0);
        spawner = FindObjectOfType<Spawner>();
        spawner.OnNewWave += OnNewWave;

        decalParticleSystem = GetComponent<ParticleSystem>();
        particleData = new ParticleDecalData[maxDecals];
        particles = new ParticleSystem.Particle[maxDecals];
        for (int i = 0; i < particleData.Length; i++)
        {
            particleData[i] = new ParticleDecalData();
        }
    }

    private void OnNewWave(int obj)
    {
        foreach (ParticleDecalData data in particleData)
        {
            data.position = invisibilePosition;
        }

        DisplayParticles();
        print("cleared " + decalParticleSystem.particleCount);
    }

    public void ParticleHit(ParticleCollisionEvent particleCollisionEvent, Color color) {
        SetParticleData(particleCollisionEvent,color);
        DisplayParticles();
    }

    void SetParticleData(ParticleCollisionEvent particleCollisionEvent, Color color) {
        if (particleDecalDataIndex >= maxDecals) {
            particleDecalDataIndex = 0;
        }

        particleData[particleDecalDataIndex].position = particleCollisionEvent.intersection + new Vector3(0, offset,0);
        Vector3 particleRotationEuler = -Quaternion.LookRotation(particleCollisionEvent.normal).eulerAngles;
        particleRotationEuler.z = Random.Range(0,360);
        particleData[particleDecalDataIndex].rotation = particleRotationEuler;
        particleData[particleDecalDataIndex].size = Random.Range(decalSizeMin,decalSizeMax);

        float tintFactor = Random.Range(0,1f);
        Color tint = new Color(color.r* tintFactor, color.g* tintFactor, color.b* tintFactor, color.a);
        particleData[particleDecalDataIndex].color = tint;

        particleDecalDataIndex++;
    }

    void DisplayParticles() {
        

        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].position = particleData[i].position;
            particles[i].rotation3D = particleData[i].rotation;
            particles[i].startSize = particleData[i].size;
            particles[i].startColor = particleData[i].color;
        }        

        decalParticleSystem.SetParticles(particles, particles.Length);
    }
}
