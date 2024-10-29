using UnityEngine;
using System.Collections.Generic;

public class ParticleCollisionHandler : MonoBehaviour
{
    public GameObject explosionEffectPrefab;
    public string tagToIgnore = "Ground";
    public float damageAmount = 10f;

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag(tagToIgnore))
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            PlayerHealthController playerHealth = other.GetComponent<PlayerHealthController>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }

            ParticleSystem particleSystem = GetComponent<ParticleSystem>();
            List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
            int numCollisionEvents = particleSystem.GetCollisionEvents(other, collisionEvents);
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSystem.particleCount];
            int numParticlesAlive = particleSystem.GetParticles(particles);

            for (int i = 0; i < numCollisionEvents; i++)
            {
                Vector3 collisionPosition = collisionEvents[i].intersection;
                if (explosionEffectPrefab != null)
                {
                    Instantiate(explosionEffectPrefab, collisionPosition, Quaternion.identity);
                }

                for (int j = 0; j < numParticlesAlive; j++)
                {
                    if (Vector3.Distance(particles[j].position, collisionPosition) < 0.1f)
                    {
                        particles[j].remainingLifetime = 0;
                    }
                }
            }

            particleSystem.SetParticles(particles, numParticlesAlive);
        }
    }
}
