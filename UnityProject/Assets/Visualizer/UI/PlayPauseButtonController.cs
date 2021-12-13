using System;
using UnityEngine;
using UnityEngine.UI;

namespace Visualizer.UI
{
    public class PlayPauseButtonController : MonoBehaviour
    {
        public static PlayPauseButtonController Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public void ResetSprite()
        {
            onPlay = true;
            imageComponent.sprite = onPlay ? playSprite : pauseSprite;
        }

        public Sprite pauseSprite;
        private Sprite playSprite;
        private Image imageComponent;
        private bool onPlay;
        
        // Start is called before the first frame update
        void Start()
        {
            imageComponent = gameObject.GetComponent<Image>();
            playSprite = imageComponent.sprite;
            ResetSprite();
        }

        public void OnPlayPauseButtonPressed()
        {
            // just swap the sprite
            onPlay = !onPlay;
            imageComponent.sprite = onPlay ? playSprite : pauseSprite;
        }
    }
}
