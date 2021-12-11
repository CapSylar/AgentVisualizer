namespace Visualizer.GameLogic.Conditions
{
    // passed on to a game, to tell it when to stop
    public abstract class StoppingCondition
    {
        //TODO: for now all StoppingConditions should have a default constructor
        public abstract bool HasEnded( Game game );
        
        // not very clean separation with UI
        public abstract GraphicalConstructor GetGraphicalConstructor();
    }
}