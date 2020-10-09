using UnityEngine;

namespace RabbitLabirint
{
    public class AboutPopup : MonoBehaviour
    {
        public void Open()
        {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}
