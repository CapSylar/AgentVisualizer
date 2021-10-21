using System;
using TMPro;
using UnityEngine;

namespace Visualizer.UI
{
    public class SpeedSliderController : MonoBehaviour
    {
        public TMP_Text _text;

        public void Start()
        {
            OnSpeedSliderValueChanged(0); // to set the text on Startup
        }

        public void OnSpeedSliderValueChanged( float value )
        {
            //TODO: maybe we want ot change the range? 
            value = value * 9 + 1; // range from 0 to 10 
            _text.text = "X " + (int)value;
        }
    }
}
