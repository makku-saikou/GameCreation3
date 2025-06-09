// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_06_09
// Description:
// -------------------------------------------------

using UnityEngine;

namespace GamePlay.Player.Particle
{
    public class HitFeedback : PlayerParticleBase
    {
        public override string Name => "HitFeedback";
        [SerializeField] private ParticleSystem pieces;
        [SerializeField] private ParticleSystem target;
        public override void Play()
        {
            throw new System.NotImplementedException("HitPieces particle should not be played directly.");
        }

        public override void Stop()
        {
        }

        public void Play(Vector3 position, Vector3 rotation)
        {
            var thePieces = Instantiate(pieces, position, Quaternion.identity);
            var theTarget = Instantiate(target, position, Quaternion.identity);
            thePieces.transform.up = rotation;
            thePieces.Play();
            theTarget.Play();
            Destroy(thePieces.gameObject, thePieces.main.duration + 0.5f);
            Destroy(theTarget.gameObject, theTarget.main.duration + 0.5f);
        }
    }
}