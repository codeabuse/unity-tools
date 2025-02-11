namespace Codeabuse
{
    public interface IInputReceiver<in TValue>
    {
        void ReceiveInput(TValue value);
    }
}