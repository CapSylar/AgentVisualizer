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
            Random r = new Random();

            for(int i=0;i < nbrOfWalls;i++){
                int startX = r.Next(0, map.sizeX-1);
                int startY = r.Next(0, map.sizeZ-1);
                buildWall(startX,startY,TILE_EDGE.UP,true);
            }
        }

        public void buildWall(int posX,int posZ, string lastWallLocation, boolean firstWall){
            var map = GameStateManager.Instance.currentMap;
            Random r = new Random();
            if(n != 0){
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
            TILE_EDGE[] possibleWalls;
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
            }
            do{
                var w = r.Next(0,possibleWalls.GetLength());
                direction = possibleWalls[w];
                neighborTemp = map.GetNeighbor(posX,posZ,direction);
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
                    nextTile nextTile1 = new nextTile();
                    nextTile1.positionX = posX-1;
                    nextTile1.positionZ = posZ-1;
                    nextTile1.lastwall = "upright";
                    options[0] = nextTile1;

                    nextTile nextTile2 = new nextTile();
                    nextTile2.positionX = posX;
                    nextTile2.positionZ = posZ-1;
                    nextTile2.lastwall = "up";
                    options[1] = nextTile2;

                    nextTile nextTile3 = new nextTile();
                    nextTile3.positionX = posX+1;
                    nextTile3.positionZ = posZ+1;
                    nextTile3.lastwall = "upleft";
                    options[2] = nextTile3;
                    break;

                case TILE_EDGE.LEFT:
                    nextTile nextTile1 = new nextTile();
                    nextTile1.positionX = posX-1;
                    nextTile1.positionZ = posZ-1;
                    nextTile1.lastwall = "upright";
                    options[0] = nextTile1;

                    nextTile nextTile2 = new nextTile();
                    nextTile2.positionX = posX-1;
                    nextTile2.positionZ = posZ;
                    nextTile2.lastwall = "right";
                    options[1] = nextTile2;

                    nextTile nextTile3 = new nextTile();
                    nextTile3.positionX = posX-1;
                    nextTile3.positionZ = posZ+1;
                    nextTile3.lastwall = "downright";
                    options[2] = nextTile3;
                    break;

                case TILE_EDGE.RIGHT:
                    nextTile nextTile1 = new nextTile();
                    nextTile1.positionX = posX+1;
                    nextTile1.positionZ = posZ-1;
                    nextTile1.lastwall = "upleft";
                    options[0] = nextTile1;

                    nextTile nextTile2 = new nextTile();
                    nextTile2.positionX = posX+1;
                    nextTile2.positionZ = posZ;
                    nextTile2.lastwall = "left";
                    options[1] = nextTile2;

                    nextTile nextTile3 = new nextTile();
                    nextTile3.positionX = posX+1;
                    nextTile3.positionZ = posZ+1;
                    nextTile3.lastwall = "downleft";
                    options[2] = nextTile3;
                    break;
            }

            do{
                var temp = r.Next(0,3);
            }while(map.GetTile(options[temp].positionX,options[temp].positionZ) == null || !hasEmptyWall(options[temp].positionX, options[temp].positionZ, options[temp].lastwall));

            buildWall(options[temp].positionX,options[temp].positionZ,options[temp].lastwall,false);

        }

        public boolean hasEmptyWall(int x, int z, string lastwall){
            Tile test = new Tile();
            test.GridX = x;
            test.GridZ = z;
            boolean hasEmptyWall = false;
            
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
