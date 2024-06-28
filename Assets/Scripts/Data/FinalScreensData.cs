using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "New Final Screen", menuName = "Final Screen")]
    public class FinalScreensData : ScriptableObject
    {
        [SerializeField]private int[] finalScreen;
        public int[] FinalScreen => finalScreen;
    }
}
