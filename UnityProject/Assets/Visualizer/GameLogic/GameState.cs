using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Visualizer.GameLogic
{
    [Serializable()]
    public class GameState
    {
        // contains all the data to be loaded/saved for the Game configuration ( Map ( tiles ) + position of agent ) 

        public TileState[,] stategrid;
        public AgentState agentState;
        
        public GameState( Map currentMap )
        {
            var grid = currentMap.Grid;
            stategrid = new TileState[grid.GetLength(0), grid.GetLength(1)];
            
            // get all the tile states
            for ( int i = 0 ; i < grid.GetLength(0) ; ++i )
            for (int j = 0; j < grid.GetLength(1); ++j)
            {
                stategrid[i, j] = grid[i, j].getTileState();
            }
            
            // get the position of the agent
            agentState = currentMap.agent == null ? new AgentState() : new AgentState(currentMap.agent) ;
        }
        
        public void Save( string filepath )
        {
            // save the map
            Stream saveFileStream = File.Create(filepath);
            BinaryFormatter serializer = new BinaryFormatter();
            
            serializer.Serialize(saveFileStream, this ); // serialize it
            saveFileStream.Close();
        }
    
        public static void Load( string filePath ,  out TileState[,] loadedMapState , out AgentState loadedAgentState )
        {
            loadedMapState = null;
            loadedAgentState = null;
                
            // load map from file
            if (File.Exists(filePath))
            {
                Stream openFileStream = File.OpenRead(filePath);
                BinaryFormatter deserializer = new BinaryFormatter();
                    
                GameState gameState = ( GameState ) deserializer.Deserialize(openFileStream);
                loadedMapState = gameState.stategrid;
                loadedAgentState = gameState.agentState;
                openFileStream.Close();
            }
        }
    }
}