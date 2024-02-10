using UnityEngine;

public class FruitLineRenderer : MonoBehaviour
{
    [SerializeField] private Transform _generatedFruits;
    [SerializeField] private LineRenderer _line;
    
    private RaycastHit2D _hit;
    private Vector2 _hitPoint;

    public GameObject currentFruit;

    private void FixedUpdate() {
        DrawLine();
    }

    private void DrawLine() {
        currentFruit = GetCurrentFruit();
        _hit = Physics2D.Raycast(currentFruit.transform.position, Vector2.down);
        if (_hit.collider != null) {
            var fruit = _hit.transform.GetComponent<Fruit>();
            if (fruit || _hit.transform.CompareTag("Floor"))
                _hitPoint = _hit.point;

            _line.positionCount = 2;
            _line.SetPosition(0, new Vector3(
                currentFruit.transform.position.x,
                currentFruit.transform.position.y - 0.3f,
                -1)
                );
            _line.SetPosition(1, new Vector3(currentFruit.transform.position.x, _hitPoint.y, -1));

            Color lineColor = currentFruit.GetComponent<FruitSwipeController>().lineColor;
            lineColor = new Color(lineColor.r, lineColor.g, lineColor.b, 1);
            _line.startColor = lineColor;
            _line.endColor = lineColor;
        }
    }

    private GameObject GetCurrentFruit() {
        int childCount = _generatedFruits.childCount;
        var currentFruit = _generatedFruits.transform.GetChild(childCount - 1).gameObject;
        return currentFruit;
    }
}
