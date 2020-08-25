using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UICarrot : MonoSingleton<UICarrot>
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
