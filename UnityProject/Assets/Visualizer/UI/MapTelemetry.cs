using Visualizer.GameLogic;

namespace Visualizer.UI
{
    // this is the message sent by the map to update its info on the UI if any
    public class MapTelemetry
    {
        public int DirtyTiles;

        public MapTelemetry()
        {
            DirtyTiles = 0;
        }
    }
}