using RabbitLabirint;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RabbitLabirint
{
    public class LoadoutState : State
    {
        [SerializeField]
        private Canvas inventoryCanvas;

        [SerializeField]
        private Button playButton;

        [SerializeField]
        private AudioClip menuMusic;

        public override void Enter(State from)
        {
            inventoryCanvas.gameObject.SetActive(true);

            // set menu music (play)

            playButton.interactable = false;
            playButton.GetComponentInChildren<TextMeshProUGUI>().text = "Loading...";
        }

        public override void Exit(State to)
        {
            inventoryCanvas.gameObject.SetActive(false);
        }

        public override string GetName()
        {
            return "Loadout";
        }

        public override void Tick()
        {
            if (!playButton.interactable)
            {
                playButton.interactable = true;
                playButton.GetComponentInChildren<TextMeshProUGUI>().text = "Play!";
            }
        }

        public void StartGame()
        {
            gameManager.SwitchState("Game");
        }
    }
}
