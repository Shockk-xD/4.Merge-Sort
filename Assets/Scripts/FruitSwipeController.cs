using UnityEngine;

public class FruitSwipeController : MonoBehaviour
{
    [SerializeField] private float _minX;
    [SerializeField] private float _maxX;

    private Rigidbody2D _rb;

    public Color lineColor;

    private void Start() {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        if (!_rb.simulated) {
            Vector2 currentPosition = transform.localPosition;

            currentPosition.x = Mathf.Clamp(currentPosition.x, _minX, _maxX);
            currentPosition.y = 0;

            transform.localPosition = currentPosition;
        }
    }
}
