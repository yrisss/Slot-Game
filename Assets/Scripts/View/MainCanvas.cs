using System;
using UnityEngine;

namespace View
{
    public class MainCanvas : MonoBehaviour
    {
        public Action ONExit;
    
        public void Exit()
        {
            ONExit?.Invoke();   
            Destroy(this.gameObject);
        }
    }
}
