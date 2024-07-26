using UnityEngine;

namespace View
{
    public class ExitButton : MonoBehaviour
    {
        [SerializeField] private MainCanvas slot;
        
        public void Exit()
        {
            slot.Exit();
        }
    }
}
