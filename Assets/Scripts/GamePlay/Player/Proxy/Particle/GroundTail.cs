// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_06_09
// Description:
// -------------------------------------------------

using System;
using PurpleFlowerCore.Utility;
using UnityEngine;

namespace GamePlay.Player.Particle
{
    public class GroundTail : PlayerParticleBase
    {
        [SerializeField] private ParticleSystem particle;
        private float _currentGroundTrailInterval;
        public override string Name => "GroundTail";

        private void Update()
        {
            _currentGroundTrailInterval -= Time.deltaTime;
        }

        public override void Play()
        {
            if (_currentGroundTrailInterval > 0) return;
            _currentGroundTrailInterval = Player.Config.groundTrailInterval;
            
            ParticleSystem instance = Instantiate(particle, transform.position, Quaternion.identity, ParticleProxy.transform);
            instance.Play();
            DelayUtility.Delay(Player.Config.groundTrailDuration, () =>
            {
                instance.Stop();
                DelayUtility.Delay(5f, ()=>{Destroy(instance.gameObject);});
            });
            
        }

        public override void Stop()
        {
            
        }
    }
}