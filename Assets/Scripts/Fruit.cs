using System.Collections;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    [SerializeField] private int _index;
    [SerializeField] private int _scoreValue;
    [HideInInspector] public bool hasCollided = false;

    public bool canCollide = false;

    private const float MIN_COLLISION_VELOCITY = 2.5f;
    private Rigidbody2D _rb;

    private void Start() {
        _rb = GetComponent<Rigidbody2D>();
        StartCoroutine(ColliderEnableTimer());
    }

    private IEnumerator ColliderEnableTimer() {
        yield return new WaitForSeconds(0.25f);
        canCollide = true;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        CollisionProcessing(collision);
        if (_rb.velocity.magnitude > MIN_COLLISION_VELOCITY)
            VibrationController.Vibrate();
    }

    private void OnCollisionStay2D(Collision2D collision) {
        CollisionProcessing(collision);
    }

    private void CollisionProcessing(Collision2D collision) {
        if (collision.gameObject.CompareTag(this.tag)) {
            if (!hasCollided && canCollide && collision.gameObject.GetComponent<Fruit>().canCollide) {
                collision.gameObject.GetComponent<Fruit>().hasCollided = true;
                hasCollided = true;

                AudioController.instance.PlaySound(AudioController.Sound.Merge);
                if (!FruitController.instance._mergedFruitTags.Contains(this.tag)) {
                    FruitController.instance._mergedFruitTags.Add(this.tag);
                    AudioController.instance.PlaySound(AudioController.Sound.Success);
                }

                Vector2 thisPosition = transform.position;
                Vector2 otherPosition = collision.transform.position;

                GameController.instance.Score += _scoreValue;

                FruitController.instance.SpawnNewFruit(thisPosition, otherPosition, _index);
                VibrationController.Vibrate();

                Destroy(collision.gameObject);
                Destroy(gameObject);
            }
        }
    }

    public int GetIndex() => _index;
}
