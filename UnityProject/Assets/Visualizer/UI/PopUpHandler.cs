using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Visualizer.UI
{
    public class PopUpHandler
    {
        // static field use by all PopUpHandlers and set by Main
        public static GameObject PopUpWindow;
        public static GameObject UserInputSection;
        public static Button DoneButton;
        
        // references to owned UI elements

        private List<GameObject> _uiElements = new List<GameObject>();
        private List<Text> _labels = new List<Text>();
        private List<TMP_InputField> _inputFields = new List<TMP_InputField>();

        // string for label, string for user text input indication, function for validation
        private List<Tuple<string, Func<string, bool>>> _entries;
        
        // reference to passed callback to be called when completed
        private Action<List<string>> callback;

        public PopUpHandler(List<Tuple<string, Func<string, bool>>> entries , Action<List<string>> callback)
        {
            // on creation, opens a popup, gets the parameters and returns them, then dies a slow death
            _entries = entries;
            
            // set callback
            this.callback = callback;

            var size = _entries.Count;

            // create labels and input text fields
            for (int i = 0; i < size; ++i)
            {
                var label = GameObject.Instantiate(PrefabContainer.Instance.labelPrefab, UserInputSection.transform);
                var textField = GameObject.Instantiate(PrefabContainer.Instance.inputTextField , UserInputSection.transform);
                
                _uiElements.Add(label);
                _uiElements.Add(textField);
                
                _labels.Add(label.GetComponent<Text>());
                _inputFields.Add(textField.GetComponent<TMP_InputField>());
            }
            
            // populate the labels
            for (int i = 0; i < _entries.Count; ++i)
            {
                _labels[i].text = _entries[i].Item1;
            }
            
            // hook event to button
            DoneButton.onClick.AddListener( Validate );
            
            // display the actual popup
            PopUpWindow.SetActive(true);
        }

        // button Callback
        private void Validate() 
        {
            // validate all inputs

            for (int i = 0; i < _inputFields.Count; ++i)
            {
                var value = _inputFields[i].text;
                
                if (!_entries[i].Item2(value)) // validate each on the passed in validator
                {
                    return; // TODO: just quits for now, should give user feedback
                }
            }
            
            // all good, return
            // gather all the results in a list and send it back

            var results = _inputFields.Select(inputField => inputField.text).ToList();

            DestroyPopUp();
            callback( results );
        }

        private void DestroyPopUp()
        {
            PopUpWindow.SetActive(false); // hide again
            // unhook button
            DoneButton.onClick.RemoveAllListeners();
            
            // destroy the Popup, all the Labels
            foreach (var gameObject in _uiElements)
            {
                GameObject.Destroy(gameObject);
            }
            
            _uiElements.Clear();
            _labels.Clear();
            _inputFields.Clear();
        }
    }
}