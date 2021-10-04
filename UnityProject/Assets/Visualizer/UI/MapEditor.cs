using UnityEngine;
using UnityEngine.UIElements;
using Visualizer;
using Visualizer.UI;

public class MapEditor : MonoBehaviour
{
    private Camera _currentCamera;
    private ItemPlacer _currentPlacer;

    void Start()
    {
        _currentCamera = Camera.main;
        _currentPlacer = new WallPlacer();
    }

    private Vector3 _worldPos;

    void Update() // should work everytime we are in editing Mode
    {
        if (isMouseOnMap(out _worldPos))
        {
            _currentPlacer.Update(_worldPos);
        }

        if (Input.GetMouseButtonDown((int) MouseButton.LeftMouse)) // place an item 
        {
            _currentPlacer.PlaceItem(); // place picked item if possible
        }

        if (Input.GetMouseButtonDown((int) MouseButton.RightMouse)) // remove an item
        {
            _currentPlacer.RemoveItem(); // remove picked item if possible or if any
        }
    }

    private bool isMouseOnMap(out Vector3 position)
    {
        Ray inputRay = _currentCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(inputRay, out hit))
        {
            position = transform.InverseTransformPoint(hit.point);
            return true;
        }

        position = Vector3.negativeInfinity;
        return false;
    }
}
