using UnityEngine;

namespace RabbitLabirint
{
    public class OpenURL : MonoBehaviour
    {
        public string websiteAddress;

        public void OpenURLOnClick()
        {
            Application.OpenURL(websiteAddress);
        }
    }
}
