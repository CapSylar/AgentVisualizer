using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Visualizer.GameLogic;

namespace Visualizer.UI
{
    public class MapEditor : MonoBehaviour
    {
        public TextMeshProUGUI sizeX;
        public TextMeshProUGUI sizeZ;

        private Camera _currentCamera;
        private ItemPlacer _currentPlacer = null;
        
        // state variables

        private float _dirtRatio = 0;

        void Start()
        {
            _currentCamera = Camera.main;
        }

        private Vector3 _worldPos;

        void Update() // should work everytime we are in editing Mode                                                                                                                           
        {
            if (_currentPlacer != null && !EventSystem.current.IsPointerOverGameObject()) // no picker, don't do anything
            {
                if (isMouseOnMap(out _worldPos))
                {
                    _currentPlacer.Update(_worldPos);
                }

                if (Input.GetMouseButtonDown((int) MouseButton.LeftMouse)) // place an item 
                {
                    _currentPlacer.PlaceItem(); // place picked item if possible
                }

                if (Input.GetMouseButtonDown((int) MouseButton.RightMouse)) // remove an item
                {
                    _currentPlacer.RemoveItem(); // remove picked item if possible or if any
                }
            }
        }

        private bool isMouseOnMap(out Vector3 position)
        {
            position = Vector3.negativeInfinity;
            //
            // if (EventSystem.current.IsPointerOverGameObject())
            //     return false;
        
            Ray inputRay = _currentCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(inputRay, out hit) && hit.transform.gameObject.layer != LayerMask.NameToLayer("UI"))
            {
                position = transform.InverseTransformPoint(hit.point);
                return true;
            }

            return false;
        }
    
        public void OnLoadMap()
        {
            // open a file chooser dialog
            var path = EditorUtility.OpenFilePanel("Select Map" , "Assets/Visualizer/Maps" , "map");

            if (path.Length != 0)
            {
                GameStateManager.Instance.Load(path);
            }
        }

        public void OnSaveMap()
        {
            var path = EditorUtility.SaveFilePanel("Save Map", "Assets/Visualizer/Maps", "Map", "map");

            if (path.Length != 0)
            {
                GameStateManager.Instance.Save(path);
            }
        }

        //TODO: onRandomWalls() and onRandomDirt() should only be pressed when a map is present, enforce this!!!
        public void OnRandomWalls()
        {
            // user pressed the random wall button
            var map = GameStateManager.Instance.CurrentBoard;
            map.RemoveAllWalls(); // remove all walls first
            
            MapWallRandomizer.Randomize(map);
        }
        
        public void OnRandomDirt()
        {
            // user pressed the random dirt button
            var map = GameStateManager.Instance.CurrentBoard;
            map.MopTheFloor(); // clean all dirt first
            MapDirtRandomizer.Randomize(map , _dirtRatio );
        }

        public void OnWallPicked()
        {
            _currentPlacer?.Destroy();
            _currentPlacer = new WallPlacer(); // create a wall placer
        }

        public void OnDirtyTilePicked()
        {
            _currentPlacer?.Destroy();
            _currentPlacer = new DirtPlacer(); // create a dirty tile placer
        }

        // TODO: agentPlacer and agentEnemyPlacer share a lot of code, refactor!!
        public void OnAgentPicked()
        {
            _currentPlacer?.Destroy();
            _currentPlacer = new GoodAgentPlacer(); // create an agent placer
        }

        public void OnEnemyAgentPicked()
        {
            _currentPlacer?.Destroy();
            _currentPlacer = new EvilAgentPlacer(); // create and enemy agent placer
        }

        public void OnGenerate()
        {
            // read the X and Z fields
            // empty spaces are causing problems for some reason, remove them manually
            var stringx = sizeX.text.Replace("\u200B", "");
            var stringz = sizeZ.text.Replace("\u200B", "");

            if (int.TryParse( stringx , out var sizex) && int.TryParse( stringz , out var sizez ))
            {
                GameStateManager.Instance.SetCurrentMap( new GraphicalBoard( sizex , sizez ) );
            }
        
            //TODO: if failed, give a visual feedback
            // else, input is not formatted correctly, just ignore
        }
        
        public void OnRatioSliderValueChanged( float value )
        {
            // value already between 0 and 1, save it
            _dirtRatio = value;
        }

        // called by the main UI to signal that the user is exiting the editor UI
        public void OnExitEditor()
        {
            // destroy any active pickers
            _currentPlacer?.Destroy();
            _currentPlacer = null;
        }
    }
}
