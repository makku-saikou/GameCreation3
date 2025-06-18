// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_06_09
// Description:
// -------------------------------------------------

using UnityEngine;

namespace GamePlay.Player.Particle
{
    public class ChangeColor : PlayerParticleBase
    {
        [SerializeField] private ParticleSystem blue;
        [SerializeField] private ParticleSystem green;
        [SerializeField] private ParticleSystem red;
        
        public override string Name => "ChangeColor";
        
        public override void Play()
        {
            throw new System.NotImplementedException();
        }
        
        public void Play(EPlayerColor color)
        {
            ParticleSystem particle = null;
            switch (color)
            {
                case EPlayerColor.Blue:
                    particle = blue;
                    break;
                case EPlayerColor.Green:
                    particle = green;
                    break;
                case EPlayerColor.Red:
                    particle = red;
                    break;
            }
            if(particle)
            {
                var theParticle = Instantiate(particle, Player.transform.position, Quaternion.identity);
                theParticle.Play();
                Destroy(theParticle, 5f);
            }
        }

        public override void Stop()
        {
        }
    }
}