using System;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using Visualizer.AgentBrains;
using Visualizer;
using Vector3 = UnityEngine.Vector3;

public class retardedBrain : BaseBrain
{
    private Queue<Vector3> _path;
    public retardedBrain( Map map )
    {
        _path = new Queue<Vector3>();
        
        for ( int i = 0 ; i < map._grid.GetLength(0) ; ++i )
            for (int j = 0; j < map._grid.GetLength(1); ++j)
            {
                _path.Enqueue(map._grid[i, j].gameObject.transform.position);
            }
    }
    
    public override Vector3 GetNextDest()
    {
        if ( _path.Count > 0 )
            return _path.Dequeue();

        return Vector3.zero;
    }
}
