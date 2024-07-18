using System;
using ReelsLogic;
using UnityEngine;

namespace DefaultNamespace
{
    public class FreeSpinGame : MonoBehaviour
    {
        [SerializeField] private ReelController _reelController;


        private void Start()
        {
            _reelController.FreeSpinStop += _reelController.ScrollStart;
        }
    }
}