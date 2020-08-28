using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RabbitLabirint
{
    public class UIStep : MonoSingleton<UIStep>
    {
        [SerializeField]
        private TextMeshProUGUI textBlock;
        [SerializeField]
        private string textTitle;

        public void SetValue(int value)
        {
            textBlock.text = textTitle + value;
        }
    }
}
