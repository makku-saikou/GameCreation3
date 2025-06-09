// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_06_08
// Description:
// -------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Player.Particle
{
    public class PlayerParticleProxy : MonoBehaviour
    {
        [SerializeField] private List<PlayerParticleBase> particleSystems;
        public List<PlayerParticleBase> ParticleSystems => particleSystems;

        private void Start()
        {
            foreach (var particle in particleSystems)
            {
                particle.Stop();
            }
        }

        public PlayerParticleBase Get(string name)
        {
            PlayerParticleBase particle = particleSystems.Find(ps => ps.Name == name);
            if (particle is not null)
            {
                return particle;
            }

            throw new System.Exception($"No particle found with name {name}");
        }
        
        public PlayerParticleBase Get(Type type)
        {
            PlayerParticleBase particle = particleSystems.Find(ps => ps.GetType() == type);
            if (particle is not null)
            {
                return particle;
            }
            
            throw new System.Exception($"No particle found with type {type}");
        }
        
        public T Get<T>() where T : PlayerParticleBase
        {
            PlayerParticleBase particle = particleSystems.Find(ps => ps is T);
            if (particle is not null)
            {
                return particle as T;
            }
            
            throw new System.Exception($"No particle found with type {typeof(T)}");
        }
        
        public PlayerParticleBase this[string name] => Get(name);

        public PlayerParticleBase this[Type type] => Get(type);

        public void Play(string particleName)
        {
            PlayerParticleBase particle = Get(particleName);
            particle.Play();
        }
        
        public void Play<T>() where T : PlayerParticleBase
        {
            PlayerParticleBase particle = Get<T>();
            particle.Play();
        }
        
        public void Stop(string particleName)
        {
            PlayerParticleBase particle = Get(particleName);
            particle.Stop();
        }
        
        public void Stop<T>() where T : PlayerParticleBase
        {
            PlayerParticleBase particle = Get<T>();
            particle.Stop();
        }
        
        public void StopAll()
        {
            foreach (var particle in particleSystems)
            {
                particle.Stop();
            }
        }

        // public ParticleSystem PlayerOnce(string particleName, Vector3 position = default, float duration = -1f, Transform parent = null)
        // {
        //     ParticleSystem particle = particleSystems.Find(ps => ps.name == particleName);
        //     if (particle is not null)
        //     {
        //         Vector3 finalPosition = position == default ? particle.transform.position : position;
        //         ParticleSystem instance = Instantiate(particle, finalPosition, Quaternion.identity, parent);
        //         instance.Play();
        //         if (duration >= 0)
        //         {
        //             DelayUtility.Delay(duration, () =>
        //             {
        //                 instance.Stop();
        //                 DelayUtility.Delay(5f, ()=>{Destroy(instance.gameObject);});
        //             });
        //         }
        //         return instance;
        //     }
        //
        //     Debug.LogWarning($"Particle '{particleName}' not found.");
        //     return null;
        // }
    }
}