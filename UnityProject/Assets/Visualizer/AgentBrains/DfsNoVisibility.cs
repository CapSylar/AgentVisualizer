using System;
using System.Collections.Generic;
using System.Linq;
using Visualizer.Algorithms;
using Visualizer.GameLogic;
using Visualizer.UI;

namespace Visualizer.AgentBrains
{
    public class DfsNoVisibility : BaseBrain
    {
        // completely blind
        
        private Map _currentMap;
        private Agent _actor;
        
        // Brain Telemetry
        private List<BrainMessageEntry> _messages = new List<BrainMessageEntry>();
        
        // state

        private List<Tile> _frontier = new List<Tile>(); // use list as stack
        private HashSet<Tile> _explored = new HashSet<Tile>();
        
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

        public DfsNoVisibility( Map map )
        {
            _currentMap = map;
            
            _messages.Add(new BrainMessageEntry("Frontier Tiles:" , "" ));
        }

        private Tile currentTile;
        private void Init()
        {
            // do a first pass to correctly start Evaluate()

            currentTile = _actor.CurrentTile;
            _explored.Add(currentTile);
            
            Evaluate();
        }

        private void Evaluate()
        {
            if (currentTile != _actor.CurrentTile) // not on same page
                return;
            
            currentTile.SetMark(true);
            
            if ( currentTile.IsDirty )
                Commands.Enqueue(new CleanDirtAction(currentTile));
            
            // expand frontier
            var neighbors = _currentMap.GetReachableNeighbors(currentTile);
            Shuffle(neighbors);
            foreach (var neighbor in neighbors.Where(neighbor => !_explored.Contains(neighbor)))
            {
                // if was already added to _frontier earlier, update its position inside it
                if (_frontier.Contains(neighbor))
                {
                    _frontier.Remove(neighbor);
                }
                
                _frontier.Insert(0,neighbor); // push on stack
            }
            
            // update telemetry
            NumOfFrontierTiles = _frontier.Count;

            if (_frontier.Count > 0)
            {
                var next = _frontier[0]; // grab next one
                _frontier.RemoveAt(0);
                _explored.Add(next);
                
                // get path to it since the frontier in some cases might not be right next to us
                Bfs.DoBfs(_currentMap , currentTile , next , out var path);
                path.RemoveAt(0); // remove current tile
                
                foreach (var tile in path)
                {
                    Commands.Enqueue(new GoAction(tile));
                }
                
                currentTile = next; // update 
            }
        }
        public override void Start(Agent actor)
        {
            // hook callback to agent 
            actor.HookToEventOnTileChange(Evaluate);
            
            // Init telemetry
            NumOfFrontierTiles = 0;
            _actor = actor;
            
            Init();
        }
        
        private void SendTelemetry( int distance )
        {
            _messages[0].value = "" + distance;
            GlobalTelemetryHandler.Instance.UpdateBrainTelemetry(_messages);
        }

        public override void Reset()
        {
            _frontier.Clear();
            _explored.Clear();
            GlobalTelemetryHandler.Instance.DestroyBrainTelemetryFields();
            _actor.UnHookEventOnTileChange(Evaluate);
        }
        
        // TODO: merge this shuffle method with the one used for Tsp Configurations
        private void Shuffle( List<Tile> list )
        {
            Random rnd = new Random();
            //Fisher-Yates shuffle
            int n = list.Count;
            while (n > 1)
            {
                --n;
                var k = rnd.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]); // swap
            }
        }
    }
}