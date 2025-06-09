// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_06_08
// Description:
// -------------------------------------------------

using System;
using System.Collections.Generic;
using PurpleFlowerCore.Utility;
using UnityEngine;

namespace GamePlay.Player
{
    public class PlayerParticleProxy : MonoBehaviour
    {
        [SerializeField] private List<ParticleSystem> particleSystems;
        public List<ParticleSystem> ParticleSystems => particleSystems;

        private void Start()
        {
            foreach (ParticleSystem particleSystem in particleSystems)
            {
                particleSystem.Stop();
            }
        }

        public ParticleSystem Get(string name)
        {
            ParticleSystem particle = particleSystems.Find(ps => ps.name == name);
            if (particle != null)
            {
                return particle;
            }
            else
            {
                Debug.LogWarning($"Particle '{name}' not found.");
                return null;
            }
        }

        public void Play(string particleName, bool rePlayer = false)
        {
            ParticleSystem particle = particleSystems.Find(ps => ps.name == particleName);
            if (particle is not null)
            {
                if(rePlayer || !particle.isPlaying)
                    particle.Play();
            }
            else
            {
                Debug.LogWarning($"Particle '{particleName}' not found.");
            }
        }
        
        public void Stop(string particleName)
        {
            ParticleSystem particle = particleSystems.Find(ps => ps.name == particleName);
            if (particle is not null)
            {
                if (particle.isPlaying)
                    particle.Stop();
            }
            else
            {
                Debug.LogWarning($"Particle '{particleName}' not found.");
            }
        }

        public ParticleSystem PlayerOnce(string particleName, Vector3 position, float duration = -1f, Transform parent = null)
        {
            ParticleSystem particle = particleSystems.Find(ps => ps.name == particleName);
            if (particle is not null)
            {
                ParticleSystem instance = Instantiate(particle, position, Quaternion.identity, parent);
                instance.Play();
                if (duration >= 0)
                {
                    DelayUtility.Delay(duration, () =>
                    {
                        instance.Stop();
                        DelayUtility.Delay(5f, ()=>{Destroy(instance.gameObject);});
                    });
                }
                return instance;
            }

            Debug.LogWarning($"Particle '{particleName}' not found.");
            return null;
        }
    }
}