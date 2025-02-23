namespace Codeabuse.UI
{
    public interface ITransitionHandler
    {
        void ToState(UIControlState state);
        void Process();
    }
}