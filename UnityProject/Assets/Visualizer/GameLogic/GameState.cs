using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Visualizer.GameLogic
{
    [Serializable()]
    public class GameState
    {
        // contains all the data to be loaded/saved for the Game configuration ( Map ( tiles ) + position of agent ) 

        public MapState mapState;
        public AgentState agentState;
        
        public GameState( Map currentMap , Agent currentAgent )
        {
            // get the save state of the map
            mapState = new MapState(currentMap);
            
            // get the save state of the agent if any
            agentState = currentAgent == null ? new AgentState() : new AgentState(currentAgent) ;
        }
        
        public void Save( string filepath )
        {
            // save the map
            Stream saveFileStream = File.Create(filepath);
            BinaryFormatter serializer = new BinaryFormatter();
            
            serializer.Serialize(saveFileStream, this ); // serialize it
            saveFileStream.Close();
        }
    
        public static void Load( string filePath ,  out MapState loadedMapState , out AgentState loadedAgentState )
        {
            loadedMapState = null;
            loadedAgentState = null;
                
            // load map from file
            if (File.Exists(filePath))
            {
                Stream openFileStream = File.OpenRead(filePath);
                BinaryFormatter deserializer = new BinaryFormatter();
                    
                GameState gameState = ( GameState ) deserializer.Deserialize(openFileStream);
                loadedMapState = gameState.mapState;
                loadedAgentState = gameState.agentState;
                openFileStream.Close();
            }
        }
    }
}