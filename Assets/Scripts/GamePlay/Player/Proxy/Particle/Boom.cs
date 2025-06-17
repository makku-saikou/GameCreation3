// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_06_17
// Description:
// -------------------------------------------------

using UnityEngine;

namespace GamePlay.Player.Particle
{
    public class Boom : PlayerParticleBase
    {
        [SerializeField] private ParticleSystem particle;
        public override string Name => "Boom";
        public override void Play()
        {
            particle.Play();
        }

        public override void Stop()
        {
            particle.Stop();
        }
    }
}