using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FruitController : MonoBehaviour {
    [SerializeField] private GameObject[] _fruits;
    [SerializeField] private UIController _controllerUI;
    [SerializeField] private Transform _generatedFruits;
    [SerializeField] private Transform _mergedFruits;
    [SerializeField] private FruitLineRenderer _fruitLineRenderer;
    [SerializeField] private LineRenderer _line;

    [SerializeField] private GameObject _cleanParticle;
    [SerializeField] private GameObject _destroyParticle;
    [SerializeField] private GameObject _dequeueParticle;

    public List<GameObject> nextFruits = new List<GameObject>();
    private GameObject _currentFruitPrefab;

    public List<string> _mergedFruitTags = new List<string>();

    public static FruitController instance;

    private float _timerToSpawn = 0f;
    private bool _isPlayingDequeueAnimation = false;

    public static int minFruitIndex = 0;
    public static int maxFruitIndex = 4;

    public GameObject CurrentFruitGameObject {
        get {
            int childCount = _generatedFruits.transform.childCount;
            var fruit = _generatedFruits.transform.GetChild(childCount - 1);
            return fruit.gameObject;
        }
    }

    private void Start() {
        instance = this;
        SpawnFirstFruits();
    }

    private void Update() {
        if (_timerToSpawn <= 0.5)
            _timerToSpawn += Time.deltaTime;
    }

    private void SpawnFirstFruits() {
        int randIndex = UnityEngine.Random.Range(0, 2);
        _currentFruitPrefab = _fruits[randIndex];

        for (int i = 0; i < 3; i++) {
            randIndex = UnityEngine.Random.Range(0, 2);
            nextFruits.Add(_fruits[randIndex]);
            _controllerUI.fruitsQueueUI.Add(_controllerUI.fruitSprites[randIndex]);
        }

        SpawnFruit();
        _controllerUI.UpdateUIQueue();
    }

    private void SpawnFruit() {
        var fruit = Instantiate(_currentFruitPrefab, _generatedFruits);
        StartCoroutine(SpawnAnimation(fruit, 3, true));
        fruit.GetComponent<Rigidbody2D>().simulated = false;
        fruit.transform.position = _generatedFruits.transform.position;
    }

    public void DropFruit() {
        if (_timerToSpawn < 0.5f || _isPlayingDequeueAnimation || !GameController.IsPlaying) return;
        _timerToSpawn = 0;

        int childCount = _generatedFruits.transform.childCount;
        var fruit = _generatedFruits.transform.GetChild(childCount - 1);
        fruit.localPosition = new Vector3(fruit.localPosition.x, fruit.localPosition.y - 0.5f, fruit.localPosition.z);
        fruit.GetComponent<Rigidbody2D>().simulated = true;
        StartCoroutine(ChangeLayerAfterDelay(fruit.gameObject, 0));
        Destroy(fruit.GetComponent<FruitSwipeController>());
        AudioController.instance.PlaySound(AudioController.Sound.Drop);

        GenerateNextFruit();
    }

    public void GenerateNextFruit() {
        _currentFruitPrefab = nextFruits[0];
        nextFruits.RemoveAt(0);
        _controllerUI.fruitsQueueUI.RemoveAt(0);

        int randIndex = UnityEngine.Random.Range(minFruitIndex, maxFruitIndex + 1);
        nextFruits.Add(_fruits[randIndex]);
        _controllerUI.fruitsQueueUI.Add(_controllerUI.fruitSprites[randIndex]);

        SpawnFruit();
        _controllerUI.UpdateUIQueue();
    }

    public void SpawnNewFruit(Vector2 pos1, Vector2 pos2, int index) {
        if (index < _fruits.Length - 1) {
            var fruit = Instantiate(_fruits[index + 1], _mergedFruits);
            Destroy(fruit.GetComponent<FruitSwipeController>());
            fruit.transform.position = new Vector2(
                (pos1.x + pos2.x) / 2,
                (pos1.y + pos2.y) / 2
                );
            StartCoroutine(SpawnAnimation(fruit, 5));
            fruit.layer = 0;
            fruit.GetComponent<Fruit>().hasVibrated = true;

            var particle = Instantiate(_destroyParticle);
            particle.transform.position = new Vector2(
                (pos1.x + pos2.x) / 2,
                (pos1.y + pos2.y) / 2
                );
        } else {
            var particle = Instantiate(_dequeueParticle);
            particle.transform.position = new Vector2(
                (pos1.x + pos2.x) / 2,
                (pos1.y + pos2.y) / 2
                );
            particle.transform.localScale = Vector3.one * 2;

            Destroy(particle, 1.5f);
        }
    }

    private IEnumerator SpawnAnimation(GameObject fruit, float speed, bool isFruitMerged = false) {
        Vector3 defaultScale = fruit.transform.localScale;
        if (isFruitMerged) 
            _line.widthMultiplier = 0;

        float t = 0;
        yield return new WaitUntil(() => {
            t += Time.deltaTime * speed;

            fruit.transform.localScale = Vector3.Lerp(Vector3.zero, defaultScale, t);
            if (isFruitMerged)
                _line.widthMultiplier = Mathf.Lerp(0, 0.3f, t);

            return t >= 1f;
        });

        _controllerUI.DequeueButtonInteractable(true);
        _isPlayingDequeueAnimation = false;
    }

    private IEnumerator ChangeLayerAfterDelay(GameObject fruit, int layer) {
        yield return new WaitForSeconds(1);
        if (fruit)
            fruit.layer = layer;
    }

    public void DestroySmallFruits() {
        if (GameController.instance.cleanCount > 0 || GameController.IsFunMode) {
            List<GameObject> fruitsToDelete = new List<GameObject>();

            var berries = GameObject.FindGameObjectsWithTag("Berry");
            var strawberries = GameObject.FindGameObjectsWithTag("Strawberry");
            var kiwies = GameObject.FindGameObjectsWithTag("Kiwi");

            fruitsToDelete.AddRange(berries);
            fruitsToDelete.AddRange(strawberries);
            fruitsToDelete.AddRange(kiwies);

            fruitsToDelete.Remove(CurrentFruitGameObject);

            var sortedFruits = fruitsToDelete.OrderBy(f => f.transform.localPosition.y).Reverse();

            if (fruitsToDelete.Count > 0) {
                StartCoroutine(DestroyAnimation(new List<GameObject>(sortedFruits)));
                GameController.instance.cleanCount--;
                _controllerUI.UpdateCleanButtonText();
            } else {
                _controllerUI.canvasAnimator.SetTrigger("Show Info Text");
            }
        } else {
            _controllerUI.canvasAnimator.SetTrigger("Clean Button Error");
        }
    }

    private IEnumerator DestroyAnimation(List<GameObject> fruits) {
        for (int i = 0; i < fruits.Count && i < 7; i++) {
            if (fruits[i] != null && fruits[i] != _fruitLineRenderer.currentFruit) {
                var particle = Instantiate(_cleanParticle, fruits[i].transform.position, Quaternion.identity);
                Destroy(particle, 1.5f);
                Destroy(fruits[i]);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    public void Dequeue() {
        if (GameController.instance.dequeueCount > 0 || GameController.IsFunMode) {
            _isPlayingDequeueAnimation = true;
            StartCoroutine(DequeueRoutine());
            GameController.instance.dequeueCount--;
            _controllerUI.UpdateDequeueButtonText();
        } else {
            _controllerUI.canvasAnimator.SetTrigger("Dequeue Button Error");
        }
    }

    private IEnumerator DequeueRoutine() {
        _controllerUI.DequeueButtonInteractable(false);
        var currentFruit = CurrentFruitGameObject;
        float timer = 0f;
        
        var particle = Instantiate(_dequeueParticle);
        particle.transform.position = currentFruit.transform.position;
        Destroy(particle, 1.5f);

        yield return new WaitUntil(() => {
            timer += Time.deltaTime * 2f;
            currentFruit.transform.localScale = Vector2.Lerp(
                currentFruit.transform.localScale,
                Vector3.zero,
                timer
                );
            _line.widthMultiplier = Mathf.Lerp(_line.widthMultiplier, 0, timer);

            return timer >= 1f;
        });

        Destroy(currentFruit);
        GenerateNextFruit();
    }

    public IEnumerator DestroyAllFruitsAnimation() {
        float destroyTime = 0.2f;
        float timer = 0f;

        var currentFruit = CurrentFruitGameObject;
        yield return new WaitUntil(() => {
            timer += Time.deltaTime;
            float t = timer / destroyTime;

            currentFruit.transform.localScale = Vector3.Lerp(
                currentFruit.transform.localScale,
                Vector3.zero,
                t
                );
            _line.widthMultiplier = Mathf.Lerp(_line.widthMultiplier, 0, t);

            return currentFruit.transform.localScale == Vector3.zero && _line.widthMultiplier == 0;
        });

        var fruits = GameObject.FindObjectsOfType<Fruit>();
        for (int i = 0; i < fruits.Length; i++) {
            fruits[i].GetComponent<Rigidbody2D>().simulated = false;
            Destroy(fruits[i].GetComponent<Collider2D>());
        }

        destroyTime = 0.1f;
        for (int i = 0; i < fruits.Length; i++) {
            yield return DestroyAnimation(fruits[i].transform);
        }

        IEnumerator DestroyAnimation(Transform fruitTransform) {
            float timer = 0f;
            yield return new WaitUntil(() => {
                timer += Time.deltaTime;
                float t = timer / destroyTime;
                fruitTransform.localScale = Vector3.Lerp(
                    fruitTransform.localScale,
                    Vector3.zero,
                    t
                    );
                return fruitTransform.localScale == Vector3.zero;
            });
            Destroy(fruitTransform.gameObject);
        }
    }
}
