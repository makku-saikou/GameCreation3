// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_06_09
// Description:
// -------------------------------------------------

using UnityEngine;

namespace GamePlay.Player.Particle
{
    public class HangTail : PlayerParticleBase
    {
        public override string Name => "HangTail";
        [SerializeField] private ParticleSystem particle;
        private ParticleSystem _particleBuffer;
        
        public override void Play()
        {
            if (!_particleBuffer)
            {
                _particleBuffer = Instantiate(particle, particle.transform.position, Quaternion.identity, ParticleProxy.transform);
                _particleBuffer.Play();
            }
        }

        public override void Stop()
        {
            if(_particleBuffer)
            {
                _particleBuffer.Stop(); 
                Destroy(_particleBuffer.gameObject, 3);
            }
            _particleBuffer = null;
        }
    }
}