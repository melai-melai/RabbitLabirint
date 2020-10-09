using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RabbitLabirint
{
    public class UIStar : MonoSingleton<UIStar>
    {
        [Header("Result stars")]
        public Sprite starSpriteOne;
        public Sprite starSpriteTwo;
        public Sprite starSpriteThree;
        public Sprite emptySpriteOne;
        public Sprite emptySpriteTwo;
        public Sprite emptySpriteThree;
        public Image starImageOne;
        public Image starImageTwo;
        public Image starImageThree;

        /// <summary>
        /// Set the result stars of the level depending on the status of passing the level
        /// </summary>
        public void SetResultStars(int amount)
        {
            switch (amount)
            {
                case 1:
                    {
                        starImageOne.sprite = starSpriteOne;
                        break;
                    }
                case 2:
                    {
                        starImageOne.sprite = starSpriteOne;
                        starImageTwo.sprite = starSpriteTwo;
                        break;
                    }
                case 3:
                    {
                        starImageOne.sprite = starSpriteOne;
                        starImageTwo.sprite = starSpriteTwo;
                        starImageThree.sprite = starSpriteThree;
                        break;
                    }
                default:
                    break;
            }
        }

        /// <summary>
        /// Set empty sprites at the results stars
        /// </summary>
        public void CleanResultStars()
        {
            starImageOne.sprite = emptySpriteOne;
            starImageTwo.sprite = emptySpriteTwo;
            starImageThree.sprite = emptySpriteThree;
        }
    }
}
