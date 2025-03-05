using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Leap.Forward.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class UIDocumentEvents : MonoBehaviour
    {
        private UIDocument _document;

        [System.Serializable]
        public class EventSubscriptions<T>
        {
            public string _elementName;

            public UnityEvent _subscribers;

            internal EventCallback<T> _callback;

            internal VisualElement _visualElement;
        }

        [SerializeField]
        public EventSubscriptions<ClickEvent>[] _onClick;

        private void Awake()
        {
            _document = GetComponent<UIDocument>();
        }

        private void OnEnable()
        {
            var root = _document?.rootVisualElement;
            if (root == null)
                return;

            Subscribe(root, _onClick);
        }

        private void OnDisable()
        {
            Unsubscribe(_onClick);
        }

        private void Subscribe<TEventType>(VisualElement root, EventSubscriptions<TEventType>[] subscriptions) where TEventType : EventBase<TEventType>, new()
        {
            for (int i = 0; i < subscriptions.Length; i++)
            {
                EventSubscriptions<TEventType> subscriber = subscriptions[i] ?? (subscriptions[i] = new EventSubscriptions<TEventType>());
                subscriber._visualElement = root.Q(subscriber._elementName) as Button;
                if (subscriber._visualElement != null)
                {
                    subscriber._callback = (TEventType evt) => { subscriber._subscribers?.Invoke(); };
                    subscriber._visualElement.RegisterCallback(subscriber._callback);
                }
            }
        }

        private void Unsubscribe<TEventType>(EventSubscriptions<TEventType>[] subscriptions) where TEventType : EventBase<TEventType>, new()
        {
            foreach (var subscriber in subscriptions)
            {
                if (subscriber._visualElement != null && subscriber._callback != null)
                {
                    subscriber._visualElement.UnregisterCallback(subscriber._callback);
                }
            }
        }
    }
}
