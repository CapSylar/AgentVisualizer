using System;
using System.Collections.Generic;
using Visualizer.Algorithms;

namespace Visualizer.GameLogic
{
    public static class MapWallRandomizer
    {
        private static int INV_WALL_DENSITY = 20 ;
        private static int FORWARD_MULTIPLIER = 3; // make it 3 times as probable to continue straight
        private static Random _random;
        private static Map _currentMap;

        public static void Randomize(Map map)
        {
            _currentMap = map;
            
            var numWalls = (map.NumOfTiles) / INV_WALL_DENSITY ;
            _random = new Random();

            for (var i = 0; i < numWalls; ++i)
            {
                // TODO: could lead to problems on tiny 2x2 maps
                var startX = _random.Next(1, map.sizeX - 2); // never spawn on an edge tile
                var startY = _random.Next(1, map.sizeZ - 2);
                BuildWall(startX, startY);
            }
        }

        private static void BuildWall(int posX, int posZ)
        {
            Tile currentTile = _currentMap.GetTile(posX, posZ);
            TILE_EDGE currentDirection = TILE_EDGE.UP.GetRandom(_random); // start in a random direction w.r.t tile

            var coolingRate = 0.05;
            var temp = 1.0;
            
            while (_random.NextDouble() < temp) // probability to continue walk becomes smaller with time 
            {
                // get all 3 or less possible next wall positions, left , straight , right 
                // all directions stated wrt the current wall direction
                //
                //      TILE UP LEFT                     TILE UP RIGHT
                //                          UP
                //                          
                //                          ||
                //                          ||
                //                          ||
                //         TILE LEFT        ||              TILE RIGHT
                //                          ||
                //                          ||
                //                          ||
                //
                //
                //                          DOWN
                //      TILE DOWN LEFT                   TILE DOWN RIGHT
                //
                //
                //

                var validMoves = new List<Tuple<Tile, TILE_EDGE>>();

                // up and right if there is a tile up right exists
                if (IsValidMove(currentTile, currentDirection.GetNext()))
                {
                    validMoves.Add(new Tuple<Tile, TILE_EDGE>(currentTile, currentDirection.GetNext()));
                }

                Tile acrossTile = _currentMap.GetNeighbor(currentTile, currentDirection);

                var opp = currentDirection.GetOpposite(); // from other perspective

                // left and up 
                if (IsValidMove(acrossTile, opp.GetPrevious()))
                    validMoves.Add(new Tuple<Tile, TILE_EDGE>(acrossTile, opp.GetPrevious()));

                var upTile = _currentMap.GetNeighbor(currentTile, currentDirection.GetNext());
                // var downTile = _currentMap.GetNeighbor(currentTile, currentDirection.GetPrevious());

                // forwards
                if (upTile != null && IsValidMove(upTile, currentDirection))
                {
                    var upMove = new Tuple<Tile, TILE_EDGE>(upTile, currentDirection);

                    for (int i = 0; i < FORWARD_MULTIPLIER; ++i)
                    {
                        validMoves.Add(upMove);
                    }
                }

                // we have all the valid moves, choose a random one
                var cont = true;
                do
                {
                    // check that we have at least one
                    if (validMoves.Count == 0)
                    {
                        break; // wall can't be expanded any further
                    }
                    
                    var nextMoveIndex = _random.Next(0, validMoves.Count);
                    var nextMove = validMoves[nextMoveIndex];

                    // place the wall and update
                    _currentMap.SetTileWall(nextMove.Item1, nextMove.Item2, true);
                    // check if placing the last wall didn't make some section of the map unreachable which we don' want
                    if (!IsAllReachable())
                    {
                        validMoves.RemoveAt(nextMoveIndex); // isn't a valid move anymore
                        _currentMap.SetTileWall(nextMove.Item1,nextMove.Item2 , false ); // remove it!!
                    }
                    else // pick it!
                    {
                        currentTile = nextMove.Item1;
                        currentDirection = nextMove.Item2;
                        cont = false; // done
                    }
                    
                } while (cont);
                
                // adjust rate
                temp *= (1 - coolingRate);
            }
        }

        private static bool IsAllReachable()
        {
            // do BFS with no goal
            Bfs.DoBfsInReachability( _currentMap , _currentMap.GetTile(0,0) , out var reachableTiles );

            return (reachableTiles.Count == _currentMap.NumOfTiles);
        }

        private static bool IsValidMove(Tile tile, TILE_EDGE moveDirection)
        {
            // move is valid if the tile is not an edge tile in the moveDirection and if it does not have a wall there already
            return !_currentMap.IsTileWallOnEdge(tile, moveDirection) &&
                   !tile.HasWall(moveDirection);
        }
    }
}