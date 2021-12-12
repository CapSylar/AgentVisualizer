using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Visualizer.Algorithms;
using Visualizer.GameLogic;
using Visualizer.UI;
using System.Threading;
using Visualizer.GameLogic.AgentMoves;

namespace Visualizer.AgentBrains
{
    public class GeneticsAlg : BaseBrain
    {
        private Board currentMap;
        private Agent _actor;
        
        
        protected int cityCount;

        protected int populationSize;

        protected double mutationPercent;

        protected int matingPopulationSize;

        protected int favoredPopulationSize;

        protected int cutLength;

        protected int generation;

        protected Thread worker = null;

        protected bool started = false;

        protected City [] cities;

        protected Chromosomes [] chromosomes;

        private string status = "";
        
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

        public GeneticsAlg( Board map )
        {
            currentMap = map;
            Commands = new Queue<AgentMove>();

            _messages.Add(new BrainMessageEntry( "global path length:" , "" ));
        }

        private IEnumerator GenerateGlobalPath()
        {
            var currentTile = _actor.CurrentTile;
            var dirtyTiles = currentMap.GetAllDirtyTiles();

          //  var finalTiles = new List<Tile>(dirtyTiles.Count);
          var tiles = new List<String>(dirtyTiles.Count);

            GlobalPathLength = 0;
            
            
            
            ////////////////////////////////////////////
         
            Random randObj = new Random();

            try

            {

                cityCount = dirtyTiles.Count;

                populationSize = 1000;

                mutationPercent = 0.05;

            }

            catch(Exception e)

            {

                cityCount = 100;

            }

            matingPopulationSize = populationSize/2;

            favoredPopulationSize = matingPopulationSize/2;

            cutLength = cityCount/5;

            // create a random list of cities

            cities = new City[cityCount];

            for ( int i=0;i<cityCount;i++ )

            {

                cities[i] = new City(

                    (int)(dirtyTiles[i].GridX),(int)(dirtyTiles[i].GridZ));

            }

 

            // create the initial chromosomes

            chromosomes = new Chromosomes[populationSize];

            for ( int i=0;i<populationSize;i++ )

            {

                chromosomes[i] = new Chromosomes(cities);

                chromosomes[i].assignCut(cutLength);

                chromosomes[i].assignMutation(mutationPercent);

            }

            Chromosomes.sortChromosomes(chromosomes,populationSize);

            started = true;

            generation = 0;
            
            ///////////////////////////////////////////////////////
            
            double thisCost = 500.0;

                             double oldCost = 0.0;

                             double dcost = 500.0;

                             int countSame = 0;

                             //Random randObj1 = new Random();

                             while(countSame<120)

                             {

                                      generation++;

                                      int ioffset = matingPopulationSize;

                                      int mutated = 0;

                                      for ( int i=0;i<favoredPopulationSize;i++ )

                                      {

                                                Chromosomes cmother = chromosomes[i];

                                                int father = (int) ( randObj.NextDouble()*(double)matingPopulationSize);

                                                Chromosomes cfather = chromosomes[father];

                                                mutated += cmother.mate(cfather,chromosomes[ioffset],chromosomes[ioffset+1]);

                                                ioffset += 2;

                                      }

                                      for ( int i=0;i<matingPopulationSize;i++ )

                                      {

                                                chromosomes[i] = chromosomes[i+matingPopulationSize];

                                                chromosomes[i].calculateCost(cities);

                                      }

                                      // Now sort the new population

                                      Chromosomes.sortChromosomes(chromosomes,matingPopulationSize);

                                      double cost = chromosomes[0].getCost();

                                      dcost = Math.Abs(cost-thisCost);

                                      thisCost = cost;

                                      double mutationRate = 100.0 * (double) mutated / (double) matingPopulationSize;

                                      System.Console.WriteLine("Generation = "+generation.ToString()+" Cost = "+thisCost.ToString()+" Mutated Rate = "+mutationRate.ToString()+"%");

                                      if ( (int)thisCost == (int)oldCost )

                                      {

                                                countSame++;

                                      }

                                      else

                                      {

                                                countSame = 0;

                                                oldCost = thisCost;

                                                //System.Console.WriteLine("oldCost = " + oldCost.ToString());

                                      }

                             }
                             //dirtyTiles.Clear();
                             for(int i = 0; i < cities.Length; i++)

                             {
                                 String placeHolder = chromosomes[i].PrintCity(i, cities);
                                 int X_coord = Int32.Parse(placeHolder.Substring(0, placeHolder.IndexOf(",")));
                                 int Z_coord = Int32.Parse(placeHolder.Substring(placeHolder.IndexOf(",")+1));
                                 tiles.Add(placeHolder);
								// City c = chromosomes[i].PrintCity(i, cities);
								 //Tile t = new Tile();
				
								// tiles.Add(chromosomes[i].PrintCity(i, cities));
								// dirtyTiles.Add(chromosomes[i].PrintCity(i, cities));
                                 //tiles[0].GridX = X_coord;
                                 //tiles[0].GridZ = Z_coord;
                                 //Tile[,] Grid = new Tile[X_coord,Z_coord];
                                 //  String placeHolder = chromosomes[i].PrintCity(i, cities);
                                 //dirtyTiles[i] = chromosomes[i].PrintCity(i, cities);

                                 //chromosomes[i].PrintCity(i, cities);

                             }




                             /// /////////////////////////////////////////////////////

            while (dirtyTiles.Count > 0)
            {
                int x = tiles.Count;
                GlobalPathLength +=
                    GetPathToNearestNeighbor(currentMap, dirtyTiles, currentTile, Commands, out var closestTile);
                currentTile = closestTile; // start position for next iteration is the current closest Dirt Tile

                //TODO: use index, runs in O(N) now!!!!
                dirtyTiles.Remove(closestTile); // so it won't be picked again
            }

            yield return null;
        }

        public static int GetPathToNearestNeighbor( Board map , List<Tile> dirtyTiles , Tile start , Queue<AgentMove> commands , out Tile closestTile )
        {
            // find closest tile to currentTile
            closestTile = GetNearestDirty(map, dirtyTiles, start);
                
            // found the closest tile
            // get the path to it

            Bfs.DoBfs( map , start , closestTile , out var path );

            PathToMoveCommands( path , commands );
            
            commands.Enqueue(new CleanDirtMove(closestTile));

            return path.Count;
        }

        public static Tile GetNearestDirty(  Board map , List<Tile> dirtyTiles  , Tile startTile )
        {
            // assumes the list if not empty
            // find closest tile to currentTile
            var minIndex = 0;
            var minDistance = Int32.MaxValue;
                
            for (var i = 0; i < dirtyTiles.Count; ++i)
            {
                var temp = map.BfsDistance(startTile, dirtyTiles[i]);
             //   if (minDistance > temp )
              //  {
                    minDistance = temp;
                    minIndex = i;
              //  }
            }

            return dirtyTiles[minIndex];
        }
        
        public override void Start(Agent actor)
        {
            // Init telemetry
            GlobalPathLength = 0; // sends telemetry
            this._actor = actor;

            // start path generation
            
            //TODO: find a solution to this
            PrefabContainer.Instance.StartCoroutine(GenerateGlobalPath());
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



public class City

{

    public City()

    {

        //

        // TODO: Add constructor logic here

        //

    }

    // The city's x coordinate.

    private int xcoord;

    // The city's y coordinate.

    private int ycoord;

    // x -- the city's x coordinate

    // y -- the city's y coordinate

    public City(int x, int y)

    {

        xcoord = x;

        ycoord = y;

    }

 

    public int getx()

    {

        return xcoord;

    }

 

    public int gety()

    {

        return ycoord;

    }

 

    // Returns the distance from the city to another city.

    public int proximity(City cother)

    {

        return proximity(cother.getx(),cother.gety());

    }

 

    // x -- the x coordinate

    // y -- the y coordinate

    // return The distance.

    public int proximity(int x, int y)

    {

        int xdiff = xcoord - x;

        int ydiff = ycoord - y;

        return(int)Math.Sqrt( xdiff*xdiff + ydiff*ydiff );

    }

}

}
