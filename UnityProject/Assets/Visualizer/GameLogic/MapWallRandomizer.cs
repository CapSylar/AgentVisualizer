using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

namespace Visualizer.GameLogic
{
    public static class MapWallRandomizer
    {
        private static Random _random;
        private static Map _currentMap;

        public static void Randomize(Map map)
        {
            _currentMap = map;
            // remove all the walls if any first

            // TODO: implement this, khoury this is for you my brother !

            var numWalls = (map.sizeX + map.sizeZ) / 5;
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
            TILE_EDGE currentDirection = TILE_EDGE.UP.GetRandom(_random);

            // start with just 5 iterations for testing
            for (var i = 0; i < 10; ++i)
            {
                // get all 6 or less possible next wall positions
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

                // down and right if there is a tile down right
                if (IsValidMove(currentTile, currentDirection.GetPrevious()))
                {
                    validMoves.Add(new Tuple<Tile, TILE_EDGE>(currentTile, currentDirection.GetPrevious()));
                }

                Tile acrossTile = _currentMap.GetNeighbor(currentTile, currentDirection);

                var opp = currentDirection.GetOpposite(); // from other perspective

                // left and up 
                if (IsValidMove(acrossTile, opp.GetPrevious()))
                    validMoves.Add(new Tuple<Tile, TILE_EDGE>(acrossTile, opp.GetPrevious()));

                // left and down 
                if (IsValidMove(acrossTile, opp.GetNext()))
                    validMoves.Add(new Tuple<Tile, TILE_EDGE>(acrossTile, opp.GetNext()));

                var upTile = _currentMap.GetNeighbor(currentTile, currentDirection.GetNext());
                var downTile = _currentMap.GetNeighbor(currentTile, currentDirection.GetPrevious());

                // forwards
                if (upTile != null && IsValidMove(upTile, currentDirection))
                    validMoves.Add(new Tuple<Tile, TILE_EDGE>(upTile, currentDirection));
                // backwards
                if (downTile != null && IsValidMove(downTile, currentDirection))
                    validMoves.Add(new Tuple<Tile, TILE_EDGE>(downTile, currentDirection));

                // we have all the valid moves, choose a random one
                
                // check that we have at least one
                if (validMoves.Count == 0)
                {
                    break;
                }
                
                var nextMove = validMoves[_random.Next(0, validMoves.Count)];
                // place the wall and update
                _currentMap.SetTileWall(nextMove.Item1, nextMove.Item2, true);
                currentTile = nextMove.Item1;
                currentDirection = nextMove.Item2;
            }
        }

        private static bool IsValidMove(Tile tile, TILE_EDGE moveDirection)
        {
            // move is valid if the tile is not an edge tile in the moveDirection and if it does not have a wall there already
            return !_currentMap.IsTileWallOnEdge(tile, moveDirection) &&
                   !tile.HasWall(moveDirection);
        }
    }
}