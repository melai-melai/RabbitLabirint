using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RabbitLabirint
{
    public class DataDeleteConfirmation : MonoBehaviour
    {
        protected LoadoutState loadoutState;

        public void Open(LoadoutState owner)
        {
            gameObject.SetActive(true);
            loadoutState = owner;
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        public void Confirm()
        {
            PlayerData.CreateNewSave();
            loadoutState.Refresh();
            Close();
        }

        public void Deny()
        {
            Close();
        }

        
    }
}
