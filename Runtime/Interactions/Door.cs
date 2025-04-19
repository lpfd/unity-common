using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Leap.Forward.Interactions
{
    [RequireComponent (typeof(AudioSource))]
    [RequireComponent(typeof(Animation))]
    public class Door : MonoBehaviour, IHoverable, IInteractable
    {
        [SerializeField]
        public string _cursorGlyph = "";

        [SerializeField]
        public string _toolkip = "";

        [SerializeField]
        public bool _isLocked = false;

        [SerializeField]
        public float _openState = 0.0f;

        [SerializeField]
        public AnimationClip _openAnimation;

        [SerializeField]
        public AudioClip _openSound;

        [SerializeField]
        public AnimationClip _closeAnimation;

        [SerializeField]
        public AudioClip _closeSound;

        [SerializeField]
        public AnimationClip _lockedAnimation;

        [SerializeField]
        public AudioClip _lockedSound;

        string IHoverable.CursorGlyph => _cursorGlyph;

        string IHoverable.Tooltip => _toolkip;

        GameObject IUnityComponent.GameObject => gameObject;

        void Start()
        {
            var anim = GetComponent<Animation>();
            if (_openAnimation != null)
            {
                anim.AddClip(_openAnimation, _openAnimation.name);
            }
            if (_closeAnimation != null)
            {
                anim.AddClip(_closeAnimation, _closeAnimation.name);
            }
            if (_lockedAnimation != null)
            {
                anim.AddClip(_lockedAnimation, _lockedAnimation.name);
            }
        }

        public void StartInteraction(GameObject gameObject)
        {
            // Get the Animation
            var anim = GetComponent<Animation>();
            if (anim != null)
            {
                if (anim.isPlaying)
                    return;
            }

            if (_openState == 0.0f)
            {
                if (_isLocked)
                {
                    Play(_lockedAnimation, _lockedSound, 0.0f);
                }
                else
                {
                    Play(_openAnimation, _openSound, 1.0f);
                }
            }
            else
            {
                Play(_closeAnimation, _closeSound, 0.0f);
            }
        }

        private void Play(AnimationClip animationClip, AudioClip audioClip, float targetState)
        {
            // Get the Animation and AudioSource components
            var anim = GetComponent<Animation>();
            var audioSource = GetComponent<AudioSource>();

            // Add the animation clip to the Animation component
            if (animationClip != null)
            {
                anim.Play(animationClip.name);
            }

            // Assign the audio clip to the AudioSource
            if (audioClip != null)
            {
                audioSource.clip = audioClip;
            }

            _openState = targetState;
        }

        public void StopInteraction(GameObject gameObject)
        {
        }

        void IHoverable.OnPointerOut()
        {
        }

        void IHoverable.OnPointerOver()
        {
        }
    }
}