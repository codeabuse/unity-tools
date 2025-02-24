using UnityEngine;
using UnityEngine.Events;

namespace Codeabuse.UI
{
    public class QuitButtonHandler : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent _onQuitRequest = new();
        
        public void Quit()
        {
            _onQuitRequest.Invoke();
            Application.Quit();
        }       
    }
}