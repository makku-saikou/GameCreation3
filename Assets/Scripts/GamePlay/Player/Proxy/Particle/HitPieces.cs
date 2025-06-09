// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_06_09
// Description:
// -------------------------------------------------

using UnityEngine;

namespace GamePlay.Player.Particle
{
    public class HitPieces : PlayerParticleBase
    {
        public override string Name => "HitPieces";
        [SerializeField] private ParticleSystem particle;
        public override void Play()
        {
            throw new System.NotImplementedException("HitPieces particle should not be played directly.");
        }

        public override void Stop()
        {
        }

        public void Play(Vector3 position, Vector3 rotation)
        {
            var theParticle = Instantiate(particle, position, Quaternion.identity);
            theParticle.transform.up = rotation;
            theParticle.Play();
            Destroy(theParticle.gameObject, theParticle.main.duration + 0.5f);
        }
    }
}