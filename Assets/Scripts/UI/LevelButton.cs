using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RabbitLabirint
{
    public class LevelButton : MonoBehaviour
    {
        [Header("Level sprites")]
        public Sprite emptySpriteLevel;
        public Sprite starSpriteLevel;

        [Header("Level stars")]
        [SerializeField]
        private Image star1;
        [SerializeField]
        private Image star2;
        [SerializeField]
        private Image star3;

        public void SetLevelStars(int amountStars)
        {
            switch (amountStars)
            {
                case 1:
                    if (star1 != null)
                    {
                        star1.sprite = starSpriteLevel;
                    }
                    break;
                case 2:
                    if (star1 != null && star2 != null)
                    {
                        star1.sprite = starSpriteLevel;
                        star2.sprite = starSpriteLevel;
                    }
                    break;
                case 3:
                    if (star1 != null && star2 != null && star3 != null)
                    {
                        star1.sprite = starSpriteLevel;
                        star2.sprite = starSpriteLevel;
                        star3.sprite = starSpriteLevel;
                    }
                    break;
            }
        }
    }
}
