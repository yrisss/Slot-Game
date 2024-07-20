using UnityEngine;

namespace View
{
    public class ExitButton : MonoBehaviour
    {
        [SerializeField] private GameObject slot;

        public void Exit()
        {
            Destroy(slot);
        }
    }
}
