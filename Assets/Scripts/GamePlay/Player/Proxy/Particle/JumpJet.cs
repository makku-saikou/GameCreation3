// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_06_09
// Description:
// -------------------------------------------------

using UnityEngine;

namespace GamePlay.Player.Particle
{
    public class JumpJet : PlayerParticleBase
    {
        [SerializeField] private ParticleSystem particle0; // center
        [SerializeField] private ParticleSystem particle1; // left
        [SerializeField] private ParticleSystem particle2; // right
        
        public override string Name => "JumpJet";
        
        public override void Play()
        {
            throw new System.NotImplementedException();
        }
        
        public void Play(float direction)
        {

            if (direction < -0.9f)
            {
                particle1.Play();
            }
            else if (direction > 0.9f)
            {
                particle2.Play();
            }
            else
            {
                particle0.Play();
            }
        }

        public override void Stop()
        {
        }
    }
}