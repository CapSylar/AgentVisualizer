using System;
using System.Collections.Generic;
using System.Linq;
using Visualizer.Algorithms;
using Visualizer.GameLogic;
using Visualizer.UI;

namespace Visualizer.AgentBrains
{
    public class DfsPartialVisibility : BaseBrain
    {                        
        // partially visible map with given radius 
        
        private Agent _actor;
        private Map _currentMap;
        
        // Brain Telemetry
        private List<BrainMessageEntry> _messages = new List<BrainMessageEntry>();
        
        // state
        private HashSet<Tile> _explored = new HashSet<Tile>();
        private List<Tile> _frontier = new List<Tile>();
        private int _currentDepth = 0;

        private int _numOfFrontierTiles;
        public int NumOfFrontierTiles
        {
            get => _numOfFrontierTiles;
            set
            {
                _numOfFrontierTiles = value;
                SendTelemetry(_numOfFrontierTiles);
            }
        }
        
        public DfsPartialVisibility( Map map )
        {
            _currentMap = map;
            
            _messages.Add(new BrainMessageEntry("Frontier Tiles:" , "" ));
        }
        
        private Tile currentTile;
        private void Init()
        {
            // do a first pass to correctly start Evaluate()

            currentTile = _actor.CurrentTile;
            // _explored.Add(currentTile);
            
            //TODO: most probably not needed, check again
            // clean it if needed
            if ( currentTile.IsDirty )
                Commands.Enqueue(new CleanDirtAction(currentTile));
            
            Evaluate();
        }
        
        private void Evaluate() // called at each tile change by the agent
        {
            // update seen tiles
            UpdateExploredTiles( _actor.CurrentTile );
            // update frontier
            UpdateFrontier();

            if (currentTile != _actor.CurrentTile) // he is not where we want him to be
                return;

            // use BFS to clean all tiles in vicinity, use alg from TSP NN

            List<Tile> temp = _explored.ToList().FindAll(tile => tile.IsDirty); // get all dirty tiles

            if (temp.Count > 0) // we have something to clean
            {
                TspNearestNeighborFullVisibility.DoNearestNeighbor(_currentMap,
                    _explored.ToList().FindAll(tile => tile.IsDirty), currentTile, Commands, out var endTile);
                currentTile = endTile; // next expected agent position
            }
            else if ( _frontier.Count > 0 ) // go to closest frontier tile if frontier is still there
            {
                int distance = Int32.MaxValue;
                Tile closestFrontier = null ;
                
                foreach (var tile in _frontier)
                {
                    var currentDistance = 0;
                    if (distance > (currentDistance = _currentMap.BfsDistance(currentTile, tile)))
                    {
                        closestFrontier = tile;
                        distance = currentDistance;
                    }
                }
                
                // remove it from frontier
                Bfs.DoBfs( _currentMap , currentTile , closestFrontier , out var Path );
                Path.RemoveAt(0);

                foreach (var tile in Path)
                {
                    Commands.Enqueue(new GoAction(tile));
                }
                
                // next expected agent position
                currentTile = Path.Last();
            }
        }


        private void UpdateExploredTiles( Tile start )
        {
            // set current visibility region as "seen"
            Bfs.DoBfsInReachabilityWithLimit(_currentMap , start , _currentDepth , out var reachableTiles);
            
            // add the new ones to the explored list
            foreach (var tile in reachableTiles.Where(tile => !_explored.Contains(tile)))
            {
                _explored.Add(tile); 
                tile.SetMark(true); // mark it as seen
            }
        }

        private void UpdateFrontier()
        {
            // get frontier tiles from the explored list
            HashSet<Tile> temp = new HashSet<Tile>();

            foreach (var tile in _explored)
            {
                var neighbors = _currentMap.GetReachableNeighbors(tile);
                foreach (var neighbor in neighbors.Where( neighbor => !_explored.Contains(neighbor)))
                {
                    temp.Add(neighbor);
                }
            }
            
            // this is the new frontier, assign it
            _frontier = temp.ToList();
            NumOfFrontierTiles = _frontier.Count; // send telemetry
        }

        public override void Start(Agent actor)
        {
            _actor = actor;
            // hook callback to agent 
            _actor.HookToEvent(Evaluate);
            
            //Init telemetry
            NumOfFrontierTiles = 0;
            
            // set up PopupWindow and callbacks
            var x = new List<Tuple<string, Func<string, bool>>>();
            x.Add(new Tuple<string, Func<string, bool>>("visibility depth" , s =>
            {
                var value = Double.Parse(s);
                return value >= 1 && value <= 7;
            } ));

            new PopUpHandler(x, Callback);
            
        }
        private void Callback(List<string> results)
        {
            // set depth before init
            _currentDepth = (int) Double.Parse(results[0]);
            // start routing
            Init();
        }
        
        private void SendTelemetry( int distance )
        {
            _messages[0].value = "" + distance;
            GlobalTelemetryHandler.Instance.UpdateBrainTelemetry(_messages);
        }

        public override void Reset()
        {
            // unhook brain from agent 
            _actor.UnHookEvent(Evaluate);
            _explored.Clear();
            _frontier.Clear();
            // remove telemetry fields
            GlobalTelemetryHandler.Instance.DestroyBrainTelemetryFields();
            base.Reset();
        }
    }
}