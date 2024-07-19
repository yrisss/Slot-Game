using System.Collections;

namespace PopUp
{
    public interface IPopUp
    {
        public void ShowPopUp();
        public IEnumerator HidePopUp();
    }
}