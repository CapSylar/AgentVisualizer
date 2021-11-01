using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Visualizer.GameLogic;
using System;

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

                if (Input.GetMouseButton((int) MouseButton.LeftMouse)) // place an item 
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
            // user pressed the random Walls button

            var map = GameStateManager.Instance.currentMap;
            // remove all the walls if any first
            map.RemoveAllWalls();
            
            // TODO: implement this, khoury this is for you my brother !

            var nbrOfWalls = (map.sizeX + map.sizeZ)/5;
            var r = new System.Random();

            for(int i=0;i < nbrOfWalls;i++){
                int startX = r.Next(0, map.sizeX-1);
                int startY = r.Next(0, map.sizeZ-1);
                buildWall(startX,startY,"any",true);
            }
        }

        public void buildWall(int posX,int posZ, string lastWallLocation, bool firstWall){
            var map = GameStateManager.Instance.currentMap;
            var r = new System.Random();
            if(!firstWall){
                var s = r.Next(0,10);
                if(s < 3){
                    return;
                }
            }

            if(!hasEmptyWall(posX,posZ,lastWallLocation)){
                return;
            }
            
            TILE_EDGE direction;
            Tile neighborTemp;
            TILE_EDGE[] possibleWalls = new TILE_EDGE[2];
            switch (lastWallLocation)
            {
                case "up":
                    possibleWalls = new TILE_EDGE[]{TILE_EDGE.LEFT,TILE_EDGE.RIGHT};
                    break;
                case "down":
                    possibleWalls = new TILE_EDGE[]{TILE_EDGE.LEFT,TILE_EDGE.RIGHT};
                    break;
                case "left":
                    possibleWalls = new TILE_EDGE[]{TILE_EDGE.UP,TILE_EDGE.DOWN};
                    break;
                case "right":
                    possibleWalls = new TILE_EDGE[]{TILE_EDGE.UP,TILE_EDGE.DOWN};
                    break;
                case "upright":
                    possibleWalls = new TILE_EDGE[]{TILE_EDGE.UP,TILE_EDGE.RIGHT};
                    break;
                case "upleft":
                    possibleWalls = new TILE_EDGE[]{TILE_EDGE.UP,TILE_EDGE.LEFT};
                    break;
                case "downright":
                    possibleWalls = new TILE_EDGE[]{TILE_EDGE.DOWN,TILE_EDGE.RIGHT};
                    break;
                case "downleft":
                    possibleWalls = new TILE_EDGE[]{TILE_EDGE.DOWN,TILE_EDGE.LEFT};
                    break;
                case "any":
                    possibleWalls = new TILE_EDGE[]{TILE_EDGE.DOWN,TILE_EDGE.LEFT,TILE_EDGE.RIGHT,TILE_EDGE.UP};
                    break;         
            }
            do{
                var w = r.Next(0,possibleWalls.Length);
                direction = possibleWalls[w];
                Tile tileTemp = map.GetTile(posX, posZ);
                neighborTemp = map.GetNeighbor(tileTemp,direction);
            }while(neighborTemp == null || neighborTemp.HasWall(direction));

            map.PlaceWall(posX, posZ,direction);

            nextTile[] options = new nextTile[3];

            switch (direction)
            {
                case TILE_EDGE.UP:
                    nextTile nextTile1 = new nextTile();
                    nextTile1.positionX = posX-1;
                    nextTile1.positionZ = posZ+1;
                    nextTile1.lastwall = "downright";
                    options[0] = nextTile1;

                    nextTile nextTile2 = new nextTile();
                    nextTile2.positionX = posX;
                    nextTile2.positionZ = posZ+1;
                    nextTile2.lastwall = "down";
                    options[1] = nextTile2;

                    nextTile nextTile3 = new nextTile();
                    nextTile3.positionX = posX+1;
                    nextTile3.positionZ = posZ+1;
                    nextTile3.lastwall = "downleft";
                    options[2] = nextTile3;
                    break;

                case TILE_EDGE.DOWN:
                    nextTile nextTile4 = new nextTile();
                    nextTile4.positionX = posX-1;
                    nextTile4.positionZ = posZ-1;
                    nextTile4.lastwall = "upright";
                    options[0] = nextTile4;

                    nextTile nextTile5 = new nextTile();
                    nextTile5.positionX = posX;
                    nextTile5.positionZ = posZ-1;
                    nextTile5.lastwall = "up";
                    options[1] = nextTile5;

                    nextTile nextTile6 = new nextTile();
                    nextTile6.positionX = posX+1;
                    nextTile6.positionZ = posZ+1;
                    nextTile6.lastwall = "upleft";
                    options[2] = nextTile6;
                    break;

                case TILE_EDGE.LEFT:
                    nextTile nextTile7 = new nextTile();
                    nextTile7.positionX = posX-1;
                    nextTile7.positionZ = posZ-1;
                    nextTile7.lastwall = "upright";
                    options[0] = nextTile7;

                    nextTile nextTile8 = new nextTile();
                    nextTile8.positionX = posX-1;
                    nextTile8.positionZ = posZ;
                    nextTile8.lastwall = "right";
                    options[1] = nextTile8;

                    nextTile nextTile9 = new nextTile();
                    nextTile9.positionX = posX-1;
                    nextTile9.positionZ = posZ+1;
                    nextTile9.lastwall = "downright";
                    options[2] = nextTile9;
                    break;

                case TILE_EDGE.RIGHT:
                    nextTile nextTile10 = new nextTile();
                    nextTile10.positionX = posX+1;
                    nextTile10.positionZ = posZ-1;
                    nextTile10.lastwall = "upleft";
                    options[0] = nextTile10;

                    nextTile nextTile11 = new nextTile();
                    nextTile11.positionX = posX+1;
                    nextTile11.positionZ = posZ;
                    nextTile11.lastwall = "left";
                    options[1] = nextTile11;

                    nextTile nextTile12 = new nextTile();
                    nextTile12.positionX = posX+1;
                    nextTile12.positionZ = posZ+1;
                    nextTile12.lastwall = "downleft";
                    options[2] = nextTile12;
                    break;
            }
            var temp = 0;
            do{
                temp = r.Next(0,3);
            }while(map.GetTile(options[temp].positionX,options[temp].positionZ) == null || !hasEmptyWall(options[temp].positionX, options[temp].positionZ, options[temp].lastwall));

            buildWall(options[temp].positionX,options[temp].positionZ,options[temp].lastwall,false);
            

        }

        public bool hasEmptyWall(int x, int z, string lastwall)
        {
            var map = GameStateManager.Instance.currentMap;
            Tile test = map.GetTile(x, z);
            
            bool hasEmptyWall = false;
            
            switch (lastwall)
            {
                case "up":
                    if(!test.HasWall(TILE_EDGE.LEFT) || !test.HasWall(TILE_EDGE.RIGHT)){
                        hasEmptyWall = true;
                    }
                    break;
                case "down":
                    if(!test.HasWall(TILE_EDGE.LEFT) || !test.HasWall(TILE_EDGE.RIGHT)){
                        hasEmptyWall = true;
                    }
                    break;
                case "left":
                    if(!test.HasWall(TILE_EDGE.UP) || !test.HasWall(TILE_EDGE.DOWN)){
                        hasEmptyWall = true;
                    }
                    break;
                case "right":
                    if(!test.HasWall(TILE_EDGE.UP) || !test.HasWall(TILE_EDGE.DOWN)){
                        hasEmptyWall = true;
                    }
                    break;
                case "upright":
                    if(!test.HasWall(TILE_EDGE.UP) || !test.HasWall(TILE_EDGE.RIGHT)){
                        hasEmptyWall = true;
                    }
                    break;
                case "upleft":
                    if(!test.HasWall(TILE_EDGE.UP) || !test.HasWall(TILE_EDGE.LEFT)){
                        hasEmptyWall = true;
                    }
                    break;
                case "downright":
                    if(!test.HasWall(TILE_EDGE.DOWN) || !test.HasWall(TILE_EDGE.RIGHT)){
                        hasEmptyWall = true;
                    }
                    break;
                case "downleft":
                    if(!test.HasWall(TILE_EDGE.DOWN) || !test.HasWall(TILE_EDGE.LEFT)){
                        hasEmptyWall = true;
                    }
                    break;   
            }
            return hasEmptyWall;
        }

        public void OnRandomDirt()
        {
            // user pressed the random dirt button
            var map = GameStateManager.Instance.currentMap;
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

        public void OnAgentPicked()
        {
            _currentPlacer?.Destroy();
            _currentPlacer = new AgentPlacer(); // create an agent placer
        }

        public void OnGenerate()
        {
            // read the X and Z fields
            // empty spaces are causing problems for some reason, remove them manually
            var stringx = sizeX.text.Replace("\u200B", "");
            var stringz = sizeZ.text.Replace("\u200B", "");

            if (int.TryParse( stringx , out var sizex) && int.TryParse( stringz , out var sizez ))
            {
                GameStateManager.Instance.SetCurrentMap( new Map( sizex , sizez ) );
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
    class nextTile{
                public int positionX {get; set; }
                public int positionZ {get; set; }
                public string lastwall {get; set; }
            };
}
