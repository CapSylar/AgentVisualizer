using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.WebSockets;
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
            // Do nearest neighbor and sends telemetry
            GlobalPathLength = DoNearestNeighbor( currentMap , currentMap.GetAllDirtyTiles(),
                actor.CurrentTile , Commands , out _ );
            
            yield return null;
        }

        public static int DoNearestNeighbor( Map map , List<Tile> dirtyTiles ,  Tile startTile , Queue<AgentAction> queue , out Tile endTile )
        {
            int totalPathLength = 0;

            var currentTile = startTile;

            while (dirtyTiles.Count > 0)
            {
                // find closest tile to currentTile
                var minIndex = 0;
                var minDistance = Int32.MaxValue;
                
                for (int i = 0; i < dirtyTiles.Count; ++i)
                {
                    var temp = map.BfsDistance(currentTile, dirtyTiles[i]);
                    if (minDistance > temp )
                    {
                        minDistance = temp;
                        minIndex = i;
                    }
                }
                
                // found the closest tile
                // get the path to it

                var closestTile = dirtyTiles[minIndex];
                Bfs.DoBfs( map , currentTile , dirtyTiles[minIndex] , out var path );
                
                dirtyTiles.RemoveAt(minIndex); // so it won't be picked again
                
                path.RemoveAt(0); // agent would be on this tile already,
                totalPathLength += path.Count;
                // Bfs returns it for correctness
                // add all to command list
                foreach (var tile in path)
                {
                    queue.Enqueue(new GoAction(tile));
                }
            
                queue.Enqueue(new CleanDirtAction(closestTile));

                currentTile = closestTile; // start position for next iteration is the current closest Dirt Tile
            }

            endTile = currentTile; // the tile where the total path will end

            return totalPathLength;
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