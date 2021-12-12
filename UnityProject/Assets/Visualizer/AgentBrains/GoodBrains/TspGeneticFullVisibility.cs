using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Visualizer.Algorithms;
using Visualizer.GameLogic;
using Visualizer.GameLogic.AgentMoves;
using Visualizer.UI;

namespace Visualizer.AgentBrains.GoodBrains
{
    public class TspGeneticFullVisibility : BaseBrain
    {
        private Agent _actor;
        private int _globalPathLength;

        // for Brain Telemetry
        private readonly List<BrainMessageEntry> _messages = new List<BrainMessageEntry>();
        
        protected Chromosome[] _chromosomes;

        protected City[] cities;

        protected int cityCount;
        private readonly Board currentMap;

        protected int cutLength;

        protected int favoredPopulationSize;

        protected int generation;

        protected int matingPopulationSize;

        protected double mutationPercent;

        protected int populationSize;

        protected bool started;

        private string status = "";

        protected Thread worker = null;

        public TspGeneticFullVisibility(Board map)
        {
            currentMap = map;
            Commands = new Queue<AgentMove>();

            _messages.Add(new BrainMessageEntry("global path length:", ""));
        }

        public int GlobalPathLength
        {
            get => _globalPathLength;
            private set
            {
                _globalPathLength = value;
                SendTelemetry(_globalPathLength);
            }
        }

        private IEnumerator GenerateGlobalPath()
        {
            var currentTile = _actor.CurrentTile;
            var dirtyTiles = currentMap.GetAllDirtyTiles();

            //  var finalTiles = new List<Tile>(dirtyTiles.Count);
            var tiles = new List<Tile>(dirtyTiles.Count);

            GlobalPathLength = 0;


            ////////////////////////////////////////////

            var randObj = new Random();

            try

            {
                cityCount = dirtyTiles.Count;

                populationSize = 1000;

                mutationPercent = 0.05;
            }

            catch (Exception e)

            {
                cityCount = 100;
            }

            matingPopulationSize = populationSize / 2;

            favoredPopulationSize = matingPopulationSize / 2;

            cutLength = cityCount / 5;

            // create a random list of cities

            cities = new City[cityCount];

            for (var i = 0; i < cityCount; i++)
                cities[i] = new City(
                    dirtyTiles[i].GridX, dirtyTiles[i].GridZ);


            // create the initial Chromosome

            _chromosomes = new Chromosome[populationSize];

            for (var i = 0; i < populationSize; i++)

            {
                _chromosomes[i] = new Chromosome(cities);

                _chromosomes[i].assignCut(cutLength);

                _chromosomes[i].assignMutation(mutationPercent);
            }

            Chromosome.sortChromosome(_chromosomes, populationSize);

            started = true;

            generation = 0;

            ///////////////////////////////////////////////////////

            var thisCost = 500.0;

            var oldCost = 0.0;

            var dcost = 500.0;

            var countSame = 0;

            //Random randObj1 = new Random();

            while (countSame < 120)

            {
                generation++;

                var ioffset = matingPopulationSize;

                var mutated = 0;

                for (var i = 0; i < favoredPopulationSize; i++)

                {
                    Chromosome cmother = _chromosomes[i];

                    var father = (int) (randObj.NextDouble() * matingPopulationSize);

                    Chromosome cfather = _chromosomes[father];

                    mutated += cmother.mate(cfather, _chromosomes[ioffset], _chromosomes[ioffset + 1]);

                    ioffset += 2;
                }

                for (var i = 0; i < matingPopulationSize; i++)

                {
                    _chromosomes[i] = _chromosomes[i + matingPopulationSize];

                    _chromosomes[i].calculateCost(cities);
                }

                // Now sort the new population

                Chromosome.sortChromosome(_chromosomes, matingPopulationSize);

                double cost = _chromosomes[0].getCost();

                dcost = Math.Abs(cost - thisCost);

                thisCost = cost;

                var mutationRate = 100.0 * mutated / matingPopulationSize;

                Console.WriteLine("Generation = " + generation + " Cost = " + thisCost + " Mutated Rate = " +
                                  mutationRate + "%");

                if ((int) thisCost == (int) oldCost)

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
            for (var i = 0; i < cities.Length; i++)

            {
                string placeHolder = _chromosomes[i].PrintCity(i, cities);
                var X_coord = int.Parse(placeHolder.Substring(0, placeHolder.IndexOf(",")));
                var Z_coord = int.Parse(placeHolder.Substring(placeHolder.IndexOf(",") + 1));
                var t = currentMap.GetTile(X_coord, Z_coord);
                tiles.Add(t);
            }


            /// /////////////////////////////////////////////////////

            while (tiles.Count > 0)
            {
                var x = tiles.Count;
                GlobalPathLength +=
                    GetPathToNearestNeighbor(currentMap, tiles, currentTile, Commands, out var closestTile);
                currentTile = closestTile; // start position for next iteration is the current closest Dirt Tile

                //TODO: use index, runs in O(N) now!!!!
                tiles.Remove(closestTile); // so it won't be picked again
            }

            yield return null;
        }

        public static int GetPathToNearestNeighbor(Board map, List<Tile> dirtyTiles, Tile start,
            Queue<AgentMove> commands, out Tile closestTile)
        {
            // find closest tile to currentTile
            closestTile = GetNearestDirty(map, dirtyTiles, start);

            // found the closest tile
            // get the path to it

            Bfs.DoBfs(map, start, closestTile, out var path);

            PathToMoveCommands(path, commands);

            commands.Enqueue(new CleanDirtMove(closestTile));

            return path.Count;
        }

        public static Tile GetNearestDirty(Board map, List<Tile> dirtyTiles, Tile startTile)
        {
            // assumes the list if not empty
            // find closest tile to currentTile
            var minIndex = 0;
            var minDistance = int.MaxValue;

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
            _actor = actor;

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

        private void SendTelemetry(int value)
        {
            _messages[0].value = "" + value;
            GlobalTelemetryHandler.Instance.UpdateBrainTelemetry(_messages);
        }
    }


    public class City
    {
        // The city's x coordinate.

        private readonly int xcoord;

        // The city's y coordinate.

        private readonly int ycoord;

        public City()
        {
            //

            // TODO: Add constructor logic here

            //
        }

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
            return proximity(cother.getx(), cother.gety());
        }


        // x -- the x coordinate

        // y -- the y coordinate

        // return The distance.

        public int proximity(int x, int y)
        {
            var xdiff = xcoord - x;
            var ydiff = ycoord - y;

            return (int) Math.Sqrt(xdiff * xdiff + ydiff * ydiff);
        }
    }
}