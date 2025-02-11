using System;

namespace Codeabuse
{
    /// <summary>
    /// Used to notify other objects when this object is updated in Edit mode
    /// </summary>
    public interface IEditorUpdateCallback
    {
        event Action OnEditorUpdated;
    }
    
    /// <summary>
    /// Used to notify other objects when this object is updated in Edit mode
    /// </summary>
    /// <typeparam name="T">Preferrably the object that implements this interface</typeparam>
    public interface IEditorUpdateCallback<out T>
    {
        event Action<T> OnEditorUpdated;
    }
}