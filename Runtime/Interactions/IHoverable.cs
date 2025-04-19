namespace Leap.Forward.Interactions
{
    public interface IHoverable: IUnityComponent
    {
        /// <summary>
        /// The OnPointerOver callback is executed at an Element when a pointing device (such as a mouse or trackpad) 
        /// is used to move the cursor onto the element or one of its child elements.
        /// Similar to https://www.w3schools.com/jsref/event_onmouseover.asp
        /// </summary>
        void OnPointerOver();

        /// <summary>
        /// The OnPointerOut callback is executed at an Element when a pointing device (such as a mouse or trackpad) 
        /// is used to move the cursor so that it is no longer contained within the element or one of its children.
        /// Similar to https://www.w3schools.com/jsref/event_onmouseout.asp
        /// </summary>
        void OnPointerOut();

        /// <summary>
        /// Cursor glyph.
        /// </summary>
        string CursorGlyph { get; }

        /// <summary>
        /// Element tooltip that should appear when pointer is hovering over element.
        /// </summary>
        string Tooltip { get; }
    }
}