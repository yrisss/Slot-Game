using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "SlotsGameData", menuName = "SlotsGameData")]
    public class SlotsGameData : ScriptableObject
    {
        [SerializeField] private GameObject[] slotsGame;
        public GameObject[] SlotsGame => slotsGame;
    }
}