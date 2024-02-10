using UnityEngine;

public class SwipeController : MonoBehaviour
{
    [SerializeField] private FruitController _fruitController;

    private Camera _camera;

    private Vector2 _touchPosition;

    private void Start() {
        _camera = Camera.main;
    }

    private void Update() {
        if (!GameController.IsPlaying) return;

        if (Input.touchCount > 0) {
            _touchPosition = Input.GetTouch(0).position;
            if (Camera.main.ScreenToWorldPoint(_touchPosition).y >= 2) return;

            if (Input.GetTouch(0).phase == TouchPhase.Began ||
                Input.GetTouch(0).phase == TouchPhase.Moved) {
                _touchPosition = _camera.ScreenToWorldPoint(_touchPosition);
                _fruitController.CurrentFruitGameObject.transform.position = _touchPosition;
            } else if (Input.GetTouch(0).phase == TouchPhase.Ended ||
                Input.GetTouch(0).phase == TouchPhase.Canceled) {
                _fruitController.DropFruit();
            }
        }
    }
}
