namespace Codeabuse
{
    public interface IGroupMessagesHandler : IEditorUpdateCallback
    {
        void UpdateGroupChildren();
        void UpdateSelf();
    }
}