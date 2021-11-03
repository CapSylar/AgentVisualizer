using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro.EditorUtilities;
using UnityEditor.Experimental;
using Visualizer.GameLogic;
using Visualizer.UI;

namespace Visualizer.AgentBrains
{
    public class DfsPartialVisibility : BaseBrain
    {
        // for now completely blind, expand later with partial visibility
        
        private Map _currentMap;
        private Agent actor;
        
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


        public DfsPartialVisibility( Map map )
        {
            _currentMap = map;
            
            _messages.Add(new BrainMessageEntry("Frontier Tiles:" , "" ));
        }

        private Tile currentTile;
        private void Init()
        {
            // do a first pass to correctly start Evaluate()

            currentTile = actor.CurrentTile;
            _explored.Add(currentTile);
            currentTile.SetSignal(true);
            
            Evaluate();
        }

        private void Evaluate()
        {
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
                var next = _frontier[0];
                _frontier.RemoveAt(0);
                _explored.Add(next);
                next.SetSignal(true);
                Commands.Enqueue(new GoAction(next));
                currentTile = next;
            }
        }
        public override void Start(Agent actor)
        {
            // hook callback to agent 
            actor.HookToEvent(Evaluate);
            
            // Init telemetry
            NumOfFrontierTiles = 0;

            this.actor = actor;
            Init();
        }
        
        private void SendTelemetry( int distance )
        {
            _messages[0].value = "" + distance;
            GlobalTelemetryHandler.Instance.UpdateBrainTelemetry(_messages);
        }

        public override void Reset()
        {
            throw new System.NotImplementedException();
        }
        
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