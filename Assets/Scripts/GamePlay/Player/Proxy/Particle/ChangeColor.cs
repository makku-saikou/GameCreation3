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
            switch (color)
            {
                case EPlayerColor.Blue:
                    blue.Play();
                    break;
                case EPlayerColor.Green:
                    green.Play();
                    break;
                case EPlayerColor.Red:
                    red.Play();
                    break;
                default:
                    break;
            }
        }

        public override void Stop()
        {
        }
    }
}