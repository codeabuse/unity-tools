using UnityEngine.UIElements;

namespace Codeabuse
{
    public static class VisualElementsExtensions
    {
        public static void ShowMessage(this HelpBox helpBox, string message, HelpBoxMessageType messageType)
        {
            helpBox.text = message;
            helpBox.messageType = messageType;
            helpBox.style.display = DisplayStyle.Flex;
        }

        public static void Show(this VisualElement element)
        {
            element.style.display = DisplayStyle.Flex;
        }

        public static void Hide(this VisualElement element)
        {
            element.style.display = DisplayStyle.None;
        }
    }
}