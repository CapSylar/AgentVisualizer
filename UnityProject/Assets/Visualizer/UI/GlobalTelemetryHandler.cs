using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Visualizer.GameLogic;

namespace Visualizer.UI
{
    public class GlobalTelemetryHandler
    {
        private static GlobalTelemetryHandler _instance;
        public static GlobalTelemetryHandler Instance => _instance;

        // UI elements controlled
        private static GameObject _telemetryPanel;
        // map
        private static Text _dirtLeftLabel;

        // Textfield lists
        private List<GameObject> _list = new List<GameObject>();
        private List<Text> _textList = new List<Text>(); // just for quicker access
        private List<Text> _valueList = new List<Text>();

        public GlobalTelemetryHandler( Main mainObject )
        {
            _instance = this; // TODO: fix the singleton
            
            // pull references to needed UI elements from mainObject
            _telemetryPanel = mainObject.telemetryDock;
            _dirtLeftLabel = mainObject.dirtLeftLabel;
            
            // hook up to global reset to clean up the brain telemetry section
            GameStateManager.Instance.OnSceneReset += DestroyBrainTelemetryFields;
        }
        
        // for now will telemetry from 3 types of entities, Brains , Agents and Maps

        private bool _isBrainTelemetryInit = false;
        
        // the brain telemetry messages are modular, unlike the Agent messages and Map messages,
        // hence the type used is a collection of BrainMessageEntry(s)
        // BrainMessage => name = Label of the message, is set when calling UpdateBrainTelemetry the first time
        // value = value that has to be updated
        // changing name after the first call has no effect, and UpdateBrainTelemetry assumes that number of messages stays constant
        // during the brain lifetime
        public void UpdateBrainTelemetry( List<BrainMessageEntry> message )
        {
            if (!_isBrainTelemetryInit) // first time called, init 
            {
                foreach (var entry in message)
                {    
                    // create the UI labels
                    // placement is handled automatically
                    var nameLabel = GameObject.Instantiate(PrefabContainer.Instance.labelPrefab, _telemetryPanel.transform);
                    var valueLabel =  GameObject.Instantiate(PrefabContainer.Instance.labelPrefab, _telemetryPanel.transform);
                    
                    _list.Add(nameLabel);
                    _list.Add(valueLabel);

                    var currentText = nameLabel.GetComponent<Text>();
                    _textList.Add(currentText);
                    _valueList.Add(valueLabel.GetComponent<Text>());

                    // fill in the labels on the left
                    currentText.text = entry.name;
                }
                _isBrainTelemetryInit = true;
            }
            
            // update the values on the right

            for (int i = 0; i < message.Count; ++i)
            {
                _valueList[i].text = message[i].value;
            }
        }

        // used to destroy the UI fields, after a Scene reset for example
        public void DestroyBrainTelemetryFields()
        {
            _textList.Clear();
            _valueList.Clear();

            // destroy the label gameObjects
            foreach (var t in _list)
            {
                GameObject.Destroy(t);
            }
            
            _list.Clear(); // lose all references
            _isBrainTelemetryInit = false;
        }

        // map telemetry and agent telemetry are fixed messages, thus no need to initialize it to find out the number of
        // fields needed

        public void UpdateMapTelemetry( MapTelemetry message )
        {
            // display the telemetry on the UI
            _dirtLeftLabel.text = "" + message.DirtyTiles;
        }
    }
}