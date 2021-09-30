using UnityEngine;
using Visualizer;

public class TestingGen : MonoBehaviour
{
    // Start is called before the first frame update
    
    public GameObject _planePrefab;
    public int X;
    public int Y;
    void Start()
    {
        // create new map
        var newMap = new Visualizer.Map(_planePrefab, X, Y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
