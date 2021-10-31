namespace Visualizer.UI
{
    public class AgentTelemetry
    {
        public int Steps; // steps made from start of run
        public int Turns; // Turns made since start of run

        public AgentTelemetry(int steps, int turns)
        {
            Steps = steps;
            Turns = turns;
        }

        public AgentTelemetry()
        {
            Steps = Turns = 0;
        }
    }
}