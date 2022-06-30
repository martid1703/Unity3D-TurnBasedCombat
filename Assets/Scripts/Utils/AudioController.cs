using System;
using UnityEngine;

namespace UnfrozenTestWork
{
    public class AudioController : MonoBehaviour
    {
        [SerializeField]
        private AudioSource _stepAudio;

        [SerializeField]
        private AudioSource _attackAudio;

        [SerializeField]
        private AudioSource _damageAudio;

        public void PlayStep()
        {
            _stepAudio.PlayOneShot(_stepAudio.clip);
        }

        public void PlayAttack()
        {
            _attackAudio.PlayOneShot(_attackAudio.clip);
        }

        public void PlayDamage()
        {
            _damageAudio.PlayOneShot(_damageAudio.clip);
        }
    }
}
