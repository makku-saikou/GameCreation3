// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_06_09
// Description:
// -------------------------------------------------

using Common.Manager;
using UnityEngine;

namespace GamePlay.Player.Particle
{
    public abstract class PlayerParticleBase : MonoBehaviour
    {
        protected PlayerController Player => GameManager.Instance.Player;
        protected PlayerParticleProxy ParticleProxy => Player.PlayerParticle;
        public abstract string Name { get; }

        public abstract void Play();
        
        public abstract void Stop();
    }
}