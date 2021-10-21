using UnityEngine;
using UnityEngine.UI;

namespace Visualizer.UI
{
    public class PlayPauseButtonController : MonoBehaviour
    {
        public Sprite pauseSprite;
        private Sprite playSprite;
        private Image imageComponent;
        private bool onPlay = true;
        
        // Start is called before the first frame update
        void Start()
        {
            imageComponent = gameObject.GetComponent<Image>();
            playSprite = imageComponent.sprite;
        }

        // Update is called once per frame
        void Update()
        {
            
        }


        public void OnPlayPauseButtonPressed()
        {
            // just swap the sprite
            onPlay = !onPlay;
            imageComponent.sprite = onPlay ? playSprite : pauseSprite;
        }
    }
}
