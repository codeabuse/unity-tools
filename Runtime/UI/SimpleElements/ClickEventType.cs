namespace Codeabuse.UI
{
    public enum ClickEventType
    {
        /// <summary>
        /// When pointer is Down over the object
        /// </summary>
        PointerDown,
        /// <summary>
        /// When pointer is Up over the object, regardless where it has emit the Down event
        /// </summary>
        PointerUp,
        /// <summary>
        /// When the pointer is up over the object considering previous Up event was over the same object
        /// </summary>
        PointerClick
    }
}