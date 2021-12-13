using System;
using UnityEngine;
using UnityEngine.UI;
using Visualizer.GameLogic;

namespace Visualizer.UI
{
    public class ScoreBoardHandler : MonoBehaviour
    {
        public GameObject scoreBoard;
        
        // reference to UI elements
        public Text bestCleanerId;
        public Text cleanerSteps;
        public Text cleanerCleaned;
        public Text cleanerScore;

        public Text bestDirtPlacerId;
        public Text dirtPlacerSteps;
        public Text dirtPlacerPlaced;
        public Text dirtPlacerScore;

        // this is a singleton, only a single instance should exist at any time during the game
        private static ScoreBoardHandler _instance;
        
        public static ScoreBoardHandler Instance => _instance;

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            // score board is first hidden
            CloseScoreBoard();
        }
        
        private void ClearScoreBoard()
        {
            bestCleanerId.text = "-";
            cleanerSteps.text = "-";
            cleanerCleaned.text = "-";
            cleanerScore.text = "-";

            bestDirtPlacerId.text = "-";
            dirtPlacerSteps.text = "-";
            dirtPlacerPlaced.text = "-";
            dirtPlacerScore.text = "-";
        }

        public void ShowResults( Tuple<Tuple<Agent,int>, Tuple<Agent,int>> bestTeamWise )
        {
            ClearScoreBoard();

            var cleaner = bestTeamWise.Item1.Item1;
            var cleanScore = bestTeamWise.Item1.Item2;

            var dirtPlacer = bestTeamWise.Item2.Item1;
            var dirtScore = bestTeamWise.Item2.Item2;

            if (cleaner != null) // we have a best cleaner, display his stats
            {
                bestCleanerId.text = "" + cleaner.Id;
                cleanerSteps.text = "" + cleaner.Steps;
                cleanerCleaned.text = "" + cleaner.Cleaned;
                cleanerScore.text = "" + cleanScore;
            }

            if (dirtPlacer != null) // we have a best dirt placer, display his stats
            {
                bestDirtPlacerId.text = "" + dirtPlacer.Id;
                dirtPlacerSteps.text = "" + dirtPlacer.Steps;
                dirtPlacerPlaced.text = "" + dirtPlacer.Stained;
                dirtPlacerScore.text = "" + dirtScore;
            }
            
            scoreBoard.SetActive(true); // show the score board
        }

        public void CloseScoreBoard()
        {
            scoreBoard.SetActive(false);
        }
    }
}