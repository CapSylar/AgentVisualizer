using System;
using Visualizer.AgentBrains.GoodBrains;

namespace Visualizer.AgentBrains
{
    public class Chromosome
    {
        //City myCity = new City();

        // The list of cities which are the genes of the chromosome

        protected int[] cityList;


        // The cost for the fitness of the chromosome

        protected double cost;

        // crossover point.

        protected int crossoverPoint;

        // The mutation rate at percentage.

        protected double mutationPercent;

        private Random randObj = new Random();

        public Chromosome()

        {
            //

            // TODO: Add constructor logic here

            //
        }


        public Chromosome(City[] cities)

        {
            var taken = new bool[cities.Length];

            cityList = new int[cities.Length];

            cost = 0.0;

            for (var i = 0; i < cityList.Length; i++) taken[i] = false;

            for (var i = 0; i < cityList.Length - 1; i++)

            {
                int icandidate;

                do

                {
                    icandidate = (int) (randObj.NextDouble() * (double) cityList.Length);
                } while (taken[icandidate]);

                cityList[i] = icandidate;

                taken[icandidate] = true;

                if (i == cityList.Length - 2)

                {
                    icandidate = 0;

                    while (taken[icandidate]) icandidate++;

                    cityList[i + 1] = icandidate;
                }
            }

            calculateCost(cities);

            crossoverPoint = 1;
        }


        // fitness calculation

        //void calculateCost(myCity cities)

        public void calculateCost(City[] cities)

        {
            cost = 0;

            for (var i = 0; i < cityList.Length - 1; i++)

            {
                double dist = cities[cityList[i]].proximity(cities[cityList[i + 1]]);

                cost += dist;
            }
        }


        public double getCost()

        {
            return cost;
        }


        public int getCity(int i)

        {
            return cityList[i];
        }


        public void assignCities(int[] list)

        {
            for (var i = 0; i < cityList.Length; i++) cityList[i] = list[i];
        }


        public void assignCity(int index, int value)

        {
            cityList[index] = value;
        }


        public void assignCut(int cut)

        {
            crossoverPoint = cut;
        }


        public void assignMutation(double prob)

        {
            mutationPercent = prob;
        }


        public int mate(Chromosome father, Chromosome offspring1, Chromosome offspring2)

        {
            var crossoverPostion1 = (int) (randObj.NextDouble() * (double) (cityList.Length - crossoverPoint));

            var crossoverPostion2 = crossoverPostion1 + crossoverPoint;

            var offset1 = new int[cityList.Length];

            var offset2 = new int[cityList.Length];

            var taken1 = new bool[cityList.Length];

            var taken2 = new bool[cityList.Length];

            for (var i = 0; i < cityList.Length; i++)

            {
                taken1[i] = false;

                taken2[i] = false;
            }

            for (var i = 0; i < cityList.Length; i++)

                if (i < crossoverPostion1 || i >= crossoverPostion2)

                {
                    offset1[i] = -1;

                    offset2[i] = -1;
                }

                else

                {
                    var imother = cityList[i];

                    int ifather = father.getCity(i);

                    offset1[i] = ifather;

                    offset2[i] = imother;

                    taken1[ifather] = true;

                    taken2[imother] = true;
                }

            for (var i = 0; i < crossoverPostion1; i++)

            {
                if (offset1[i] == -1)

                    for (var j = 0; j < cityList.Length; j++)

                    {
                        var imother = cityList[j];

                        if (!taken1[imother])

                        {
                            offset1[i] = imother;

                            taken1[imother] = true;

                            break;
                        }
                    }

                if (offset2[i] == -1)

                    for (var j = 0; j < cityList.Length; j++)

                    {
                        int ifather = father.getCity(j);

                        if (!taken2[ifather])

                        {
                            offset2[i] = ifather;

                            taken2[ifather] = true;

                            break;
                        }
                    }
            }

            for (var i = cityList.Length - 1; i >= crossoverPostion2; i--)

            {
                if (offset1[i] == -1)

                    for (var j = cityList.Length - 1; j >= 0; j--)

                    {
                        var imother = cityList[j];

                        if (!taken1[imother])

                        {
                            offset1[i] = imother;

                            taken1[imother] = true;

                            break;
                        }
                    }

                if (offset2[i] == -1)

                    for (var j = cityList.Length - 1; j >= 0; j--)

                    {
                        int ifather = father.getCity(j);

                        if (!taken2[ifather])

                        {
                            offset2[i] = ifather;

                            taken2[ifather] = true;

                            break;
                        }
                    }
            }

            offspring1.assignCities(offset1);

            offspring2.assignCities(offset2);

            var mutate = 0;

            var swapPoint1 = 0;

            var swapPoint2 = 0;

            if (randObj.NextDouble() < mutationPercent)

            {
                swapPoint1 = (int) (randObj.NextDouble() * (double) cityList.Length);

                swapPoint2 = (int) (randObj.NextDouble() * (double) cityList.Length);

                var i = offset1[swapPoint1];

                offset1[swapPoint1] = offset1[swapPoint2];

                offset1[swapPoint2] = i;

                mutate++;
            }

            if (randObj.NextDouble() < mutationPercent)

            {
                swapPoint1 = (int) (randObj.NextDouble() * (double) cityList.Length);

                swapPoint2 = (int) (randObj.NextDouble() * (double) cityList.Length);

                var i = offset2[swapPoint1];

                offset2[swapPoint1] = offset2[swapPoint2];

                offset2[swapPoint2] = i;

                mutate++;
            }

            return mutate;
        }

        public String PrintCity(int i, City[] cities)

        {
            return "" + cities[cityList[i]].getx() + "," + cities[cityList[i]].gety();
        }


        // chromosomes -- an array of chromosomes which is sorted

        // num -- the number of the chromosome list

        public static void sortChromosome(Chromosome[] chromosomes, int num)

        {
            var swapped = true;

            Chromosome dummy;

            while (swapped)

            {
                swapped = false;

                for (var i = 0; i < num - 1; i++)

                    if (chromosomes[i].getCost() > chromosomes[i + 1].getCost())

                    {
                        dummy = chromosomes[i];

                        chromosomes[i] = chromosomes[i + 1];

                        chromosomes[i + 1] = dummy;

                        swapped = true;
                    }
            }
        }
    }
}