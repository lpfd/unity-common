using Leap.Forward.Interactions;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Leap.Forward.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class InteractionViewController : MonoBehaviour
    {
        private UIDocument _document;

        private IHoverable _hoverable;

        [SerializeField]
        public string _rootElementName;

        [SerializeField]
        public string _cursorElementName;

        [SerializeField]
        public string _tooltipElementName;

        [SerializeField]
        public string _defaultCursorGlyph = "[.]";

        private void Awake()
        {
            _document = GetComponent<UIDocument>();
            UpdateUI();
        }

        public void OnPointerOver(IHoverable hoverable)
        {
            if (_hoverable != hoverable)
            {
                _hoverable = hoverable;
                UpdateUI();
            }
        }

        public void OnPointerOut(IHoverable hoverable)
        {
            if (_hoverable == hoverable)
            {
                _hoverable = null;
                UpdateUI();
            }
        }

        private void UpdateUI()
        {
            if (!_document)
                return;

            var root = string.IsNullOrEmpty(_rootElementName) ? _document.rootVisualElement : (_document.rootVisualElement.Q(_rootElementName) ?? _document.rootVisualElement);
            if (_hoverable == null)
            {
                root.visible = false;
                return;
            }

            root.visible = true;

            if (!string.IsNullOrEmpty(_cursorElementName))
            {
                var element = _document.rootVisualElement.Q(_cursorElementName);
                var glyph = _hoverable?.CursorGlyph ?? _defaultCursorGlyph ?? "";
                if (!string.IsNullOrEmpty(glyph))
                {
                    SetText(element, glyph);
                }
            }

            if (!string.IsNullOrEmpty(_tooltipElementName))
            {
                var element = _document.rootVisualElement.Q(_tooltipElementName);
                SetText(element, _hoverable?.Tooltip ?? "");
            }
        }

        private void SetText(VisualElement element, string tooltipElementName)
        {
            if (element is Label label)
            {
                label.text = tooltipElementName;
            }
        }
    }
}