using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Visualizer.GameLogic
{
    [Serializable()]
    public class GameState
    {
        // contains all the data to be loaded/saved for the Game configuration ( Map ( tiles ) + position of agent ) 

        public Board board;
        public Agent agent;
        
        public GameState( Board currentBoard , Agent currentAgent )
        {
            // get the save state of the map
            board = currentBoard;
            
            // get the save state of the agent if any
            agent = currentAgent ?? new Agent() ;
        }
        
        public void Save( string filepath )
        {
            // save the map
            Stream saveFileStream = File.Create(filepath);
            BinaryFormatter serializer = new BinaryFormatter();
            
            serializer.Serialize(saveFileStream, this ); // serialize it
            saveFileStream.Close();
        }
    
        public static void Load( string filePath ,  out Board loadedBoard , out Agent loadedAgent )
        {
            loadedBoard = null;
            loadedAgent = null;
                
            // load map from file
            if (File.Exists(filePath))
            {
                Stream openFileStream = File.OpenRead(filePath);
                BinaryFormatter deserializer = new BinaryFormatter();
                    
                GameState gameState = ( GameState ) deserializer.Deserialize(openFileStream);
                loadedBoard = gameState.board;
                loadedAgent = gameState.agent;
                openFileStream.Close();
            }
        }
    }
}