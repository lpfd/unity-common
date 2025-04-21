using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

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

        [Header("Open")]

        [SerializeField]
        public AnimationClip _openAnimation;

        [SerializeField]
        public AudioClip _openSound;

        [Header("AltOpen")]

        //[SerializeField]
        //public Vector3 _openDirection = Vector3.forward;

        [SerializeField]
        public AnimationClip _altOpenAnimation;

        [SerializeField]
        public AudioClip _altOpenSound;

        [Header("Close")]

        [SerializeField]
        public AudioClip _closeSound;

        [Header("Locked")]

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
            if (_altOpenAnimation != null)
            {
                anim.AddClip(_altOpenAnimation, _altOpenAnimation.name);
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

            if (IsClosed)
            {
                if (_altOpenAnimation != null)
                {
                    var _openDirection = Vector3.forward;
                    if (Vector3.Dot(_openDirection, this.gameObject.transform.InverseTransformPoint(gameObject.transform.position)) >= 0)
                        AltOpenDoor();
                    else
                        OpenDoor();
                }
                else
                {
                    OpenDoor();
                }
            }
            else
            {
                CloseDoor();
            }
        }

        private void Play(AnimationClip animationClip, AudioClip audioClip, float targetState, bool instant)
        {
            if (instant)
            {
                _openState = targetState;

                if (animationClip != null)
                {
                    if (targetState == 0.0f)
                    {
                        animationClip.SampleAnimation(gameObject, 0.0f);
                    }
                    else
                    {
                        animationClip.SampleAnimation(gameObject, animationClip.length);
                    }
                }
                return;
            }
            // Get the Animation and AudioSource components
            var anim = GetComponent<Animation>();
            var audioSource = GetComponent<AudioSource>();

            // Add the animation clip to the Animation component
            if (animationClip != null)
            {
                if (targetState == 0.0f)
                {
                    anim[animationClip.name].speed = -1f;
                    anim[animationClip.name].time = anim[animationClip.name].length;
                }
                else
                {
                    anim[animationClip.name].speed = 1f;
                    anim[animationClip.name].time = 0.0f;
                }
                anim.Play(animationClip.name, PlayMode.StopAll);
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

        private static Vector3[] _arrowPoints = new Vector3[] { 
            new Vector3(0f, 0f, 0f),
            new Vector3(0.5f, 0f, -0.5f),
            new Vector3(0.25f, 0f, -0.5f),
            new Vector3(0.25f, 0f, -1.0f),
            new Vector3(-0.25f, 0f, -1.0f),
            new Vector3(-0.25f, 0f, -0.5f),
            new Vector3(-0.5f, 0f, -0.5f),
        };

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < _arrowPoints.Length; i++)
            {
                var next = (i + 1) % _arrowPoints.Length;

                Vector3 a = transform.position + transform.TransformDirection(_arrowPoints[i]);
                Vector3 b = transform.position + transform.TransformDirection(_arrowPoints[next]);
                Gizmos.DrawLine(a, b);
            }
        }

        #region Open
        public bool IsOpen
        {
            get
            {
                return !IsClosed;
            }
        }

        public bool CanOpen()
        {
            return (_openAnimation != null && IsClosed && !_isLocked);
        }

        public void OpenDoor(bool instant = false)
        {
            if (_isLocked)
            {
                Play(_lockedAnimation, _lockedSound, 0.0f, false);
            }
            else
            {
                Play(_openAnimation, _openSound, 1.0f, instant);
            }
        }
        #endregion

        #region AltOpen
        public bool CanAltOpen()
        {
            return (_altOpenAnimation != null && IsClosed && !_isLocked);
        }

        public void AltOpenDoor(bool instant = false)
        {
            if (_isLocked)
            {
                Play(_lockedAnimation, _lockedSound, 0.0f, false);
            }
            else
            {
                Play(_altOpenAnimation, _altOpenSound, -1.0f, instant);
            }
        }
        #endregion

        #region Close
        public bool IsClosed
        {
            get
            {
                return MathF.Abs(_openState) <= float.Epsilon;
            }
        }

        public bool CanClose()
        {
            return !IsClosed;
        }

        public void CloseDoor(bool instant = false)
        {
            if (_openState > 0)
            {
                Play(_openAnimation, _closeSound ?? _openSound, 0.0f, instant);
            }
            else
            {
                Play(_altOpenAnimation, _closeSound ?? _altOpenSound, 0.0f, instant);
            }
        }
        #endregion
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Door))]
    public class DoorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Door door = (Door)target;

            EditorGUI.BeginDisabledGroup(!door.CanOpen());
            if (GUILayout.Button("Open"))
            {
                door.OpenDoor(true);
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(!door.CanAltOpen());
            if (GUILayout.Button("Alt Open"))
            {
                door.AltOpenDoor(true);
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(!door.CanClose());
            if (GUILayout.Button("Close"))
            {
                door.CloseDoor(true);
            }
            EditorGUI.EndDisabledGroup();
        }
    }
#endif
}