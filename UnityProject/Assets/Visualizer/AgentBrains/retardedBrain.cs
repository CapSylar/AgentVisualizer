using System.Collections.Generic;
using UnityEngine;
using Visualizer;
using Visualizer.AgentBrains;
using Visualizer.GameLogic;

public class retardedBrain : BaseBrain
{
    // private Queue<Vector3> _path;
    public retardedBrain( Map map )
    {
        // _path = new Queue<Vector3>();
        //
        // for ( int i = 0 ; i < map.Grid.GetLength(0) ; ++i )
        //     for (int j = 0; j < map.Grid.GetLength(1); ++j)
        //     {
        //         _path.Enqueue(map.Grid[i, j].gameObject.transform.position);
        //     }
    }
    
    public override Vector3 GetNextDest()
    {
        // if ( _path.Count > 0 )
        //     return _path.Dequeue();

        return Vector3.zero; // just stays at 0,0 really retarded
    }

    public override void Start()
    {
        throw new System.NotImplementedException();
    }

    public override void Pause()
    {
        throw new System.NotImplementedException();
    }

    public override void Reset()
    {
        throw new System.NotImplementedException();
    }
}
