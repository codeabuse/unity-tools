using UnityEngine;

namespace Codeabuse
{
    /// <summary>
    /// Represents scene or multiple scenes in Editor.
    /// </summary>
    public interface ISceneSetup : IEditorUpdateCallback<ISceneSetup>
    {
        string name { get; set; }
        int Id { get; }
        void Load();
        void Save();
        /// <summary>
        /// Get Scene asset or Scene Setup asset.
        /// </summary>
        /// <returns></returns>
        Object GetUnderlyingObject();
    }
}