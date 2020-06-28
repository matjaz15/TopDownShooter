using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathParticlesCollision : MonoBehaviour {

    public ParticleSystem part;
    public List<ParticleCollisionEvent> collisionEvents;
    public Color bloodColor;

    ParticleDecal particleDecal;


    void Start()
    {
        particleDecal = ParticleDecal.instance;
        part = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);

        for (int i = 0; i < collisionEvents.Count; i++)
        {
            particleDecal.ParticleHit(collisionEvents[i], bloodColor);
        }
    }
}
