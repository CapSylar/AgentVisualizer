using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine.Rendering;
using UnityEngine.Timeline;
using Visualizer.Algorithms;
using Visualizer.GameLogic;
using Visualizer.UI;

namespace Visualizer.AgentBrains
{
    public class TspNearestNeighborFullVisibility : BaseBrain
    {
        private Map currentMap;
        private Agent actor;
        
        // for Brain Telemetry
        private List<BrainMessageEntry> _messages = new List<BrainMessageEntry>();
        private int _globalPathLength = 0;

        public int GlobalPathLength
        {
            get => _globalPathLength;
            private set
            {
                _globalPathLength = value;
                SendTelemetry(_globalPathLength);
            }
        }

        public TspNearestNeighborFullVisibility( Map map )
        {
            currentMap = map;
            Commands = new Queue<AgentAction>();

            _messages.Add(new BrainMessageEntry( "global path length:" , "" ));
        }

        private IEnumerator GenerateGlobalPath()
        {
            var dirtyTiles = currentMap.GetAllDirtyTiles();

            var currentTile = actor.CurrentTile;

            while (dirtyTiles.Count > 0)
            {
                // find closest tile to currentTile
                var minIndex = 0;
                var minDistance = Int32.MaxValue;
                
                for (int i = 0; i < dirtyTiles.Count; ++i)
                {
                    var temp = currentMap.BfsDistance(currentTile, dirtyTiles[i]);
                    if (minDistance > temp )
                    {
                        minDistance = temp;
                        minIndex = i;
                    }
                }
                
                // found the closest tile
                // get the path to it

                var closestTile = dirtyTiles[minIndex];
                Bfs.DoBfs(currentMap , currentTile , dirtyTiles[minIndex] , out var path );
                
                dirtyTiles.RemoveAt(minIndex); // so it won't be picked again
                
                path.RemoveAt(0); // agent would be on this tile already,
                // Bfs returns it for correctness
                GlobalPathLength += path.Count; // sends telemetry
                // add all to command list
                foreach (var tile in path)
                {
                    Commands.Enqueue(new GoAction(tile));
                }
            
                Commands.Enqueue(new CleanDirtAction(closestTile));

                currentTile = closestTile; // start position for next iteration is the current closest Dirt Tile
                yield return null; // skip till next frame
            }

            // IsReady = true;
        }
        
        public override void Start(Agent actor)
        {
            // Init telemetry
            GlobalPathLength = 0; // sends telemetry
            this.actor = actor;

            // start path generation
            actor.StartCoroutine(GenerateGlobalPath());
        }

        public new void Reset()
        {
            base.Reset();
            // reset telemetry
            GlobalTelemetryHandler.Instance.DestroyBrainTelemetryFields();
        }
        
        private void SendTelemetry( int value )
        {
            _messages[0].value = "" + value;
            GlobalTelemetryHandler.Instance.UpdateBrainTelemetry(_messages);
        }
    }
}