using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Codeabuse
{
    public delegate void DraggedObjectsHandler(Object[] draggedObjects);
    
    public class DragAndDropObjectsManipulator : IManipulator
    {
        private VisualElement _target;
        private readonly DraggedObjectsHandler _draggedObjectsHandler;

        public DragAndDropObjectsManipulator(DraggedObjectsHandler draggedObjectsHandler)
        {
            _draggedObjectsHandler = draggedObjectsHandler;
        }

        public VisualElement target
        { 
            get => _target; 
            set => SetTarget(value); 
        }

        private void SetTarget(VisualElement newTarget)
        {
            _target = newTarget;
            _target.RegisterCallback<DragPerformEvent>(OnDragPerformed);
        }

        private void OnDragPerformed(DragPerformEvent evt)
        {
            var data = DragAndDrop.objectReferences;
            _draggedObjectsHandler?.Invoke(data);
        }
    }
}