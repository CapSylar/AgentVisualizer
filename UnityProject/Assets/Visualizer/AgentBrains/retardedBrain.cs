using System;
using System.Collections.Generic;
using UnityEngine;
using Visualizer;
using Visualizer.AgentBrains;
using Visualizer.GameLogic;

public class retardedBrain : BaseBrain
{
    private Queue<Tile> _path;
    private Map currentMap;
    private int GridX, GridZ;
    private Tile _currentTile;
    private bool gogogo = false;
    public retardedBrain( Map map , int gridX = 0  , int gridZ = 0 )
    {
        this.GridZ = gridZ;
        this.GridX = gridX;
        
        currentMap = map;
        _currentTile = map.getTile(gridZ, gridX);
        _path = new Queue<Tile>();
        
        for ( int i = 0 ; i < map.Grid.GetLength(0) ; ++i )
            for (int j = 0; j < map.Grid.GetLength(1); ++j)
            {
                _path.Enqueue(map.getTile(i,j));
            }
    }
    
    public override Tile GetNextDest()
    {
        if (gogogo == false)
        {
            return _currentTile;
        }
        
        if ( _path.Count > 0 )
            return _currentTile = _path.Dequeue();

        return currentMap.getTile(0, 0); // go to origin tile
    }

    public override void Start()
    {
        gogogo = true;
    }

    public override void Pause()
    {
        gogogo = false;
    }

    public override void Reset()
    {
        throw new NotImplementedException();
    }
}
