using System.Collections;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    [SerializeField] private int _index;
    [SerializeField] private int _scoreValue;
    [HideInInspector] public bool hasCollided = false;
    [HideInInspector] public bool hasVibrated = false;

    public bool canCollide = false;

    private const float MAX_VELOCITY_SPEED = 5f;
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

        float velocity = Mathf.Abs(_rb.velocity.y);
        if (!hasVibrated || velocity > MAX_VELOCITY_SPEED) {
            VibrationController.Vibrate();
            hasVibrated = true;
        }
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
                if (!FruitController.instance._mergedFruitTags.Contains(this.tag)
                    || this.CompareTag("Orange") || this.CompareTag("Pineapple")) {
                    AudioController.instance.PlaySound(AudioController.Sound.Success);

                    if (!(this.CompareTag("Orange") || this.CompareTag("Pineapple")))
                        FruitController.instance._mergedFruitTags.Add(this.tag);
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
