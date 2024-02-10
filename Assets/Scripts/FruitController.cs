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

    [SerializeField] private GameObject _cleanParticle;
    [SerializeField] private GameObject _destroyParticle;

    private List<GameObject> _nextFruits = new List<GameObject>();
    private GameObject _currentFruitPrefab;

    public List<string> _mergedFruitTags = new List<string>();

    public static FruitController instance;

    private float _timerToSpawn = 0f;

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
            _nextFruits.Add(_fruits[randIndex]);
            _controllerUI.fruitsQueueUI.Add(_controllerUI.fruitSprites[randIndex]);
        }

        SpawnFruit();
        _controllerUI.UpdateUIQueue();
    }

    private void SpawnFruit() {
        var fruit = Instantiate(_currentFruitPrefab, _generatedFruits);
        StartCoroutine(SpawnAnimation(fruit, 3));
        fruit.GetComponent<Rigidbody2D>().simulated = false;
        fruit.transform.position = _generatedFruits.transform.position;
        _controllerUI.EnableUndoButton();
    }

    public void DropFruit() {
        SaveData.instance._SaveData();
        if (_timerToSpawn < 0.5f) return;
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
        _currentFruitPrefab = _nextFruits[0];
        _nextFruits.RemoveAt(0);
        _controllerUI.fruitsQueueUI.RemoveAt(0);

        int randIndex = UnityEngine.Random.Range(0, 5);
        _nextFruits.Add(_fruits[randIndex]);
        _controllerUI.fruitsQueueUI.Add(_controllerUI.fruitSprites[randIndex]);

        SpawnFruit();
        _controllerUI.UpdateUIQueue();
    }

    public void SpawnNewFruit(Vector2 pos1, Vector2 pos2, int index) {
        if (index < _fruits.Length - 1) {
            var fruit = Instantiate(_fruits[index + 1], _mergedFruits);
            fruit.transform.position = new Vector2(
                (pos1.x + pos2.x) / 2,
                (pos1.y + pos2.y) / 2
                );
            StartCoroutine(SpawnAnimation(fruit, 5));
            if (fruit.layer != 0)
                fruit.layer = 0;

            var particle = Instantiate(_destroyParticle);
            particle.transform.position = new Vector2(
                (pos1.x + pos2.x) / 2,
                (pos1.y + pos2.y) / 2
                );
        }
    }

    private IEnumerator SpawnAnimation(GameObject fruit, float speed) {
        Vector3 defaultScale = fruit.transform.localScale;
        
        float t = 0;
        yield return new WaitUntil(() => {
            t += Time.deltaTime;

            fruit.transform.localScale = Vector3.Lerp(Vector3.zero, defaultScale, t * speed);

            return fruit.transform.localScale == defaultScale;
        });
    }

    private IEnumerator ChangeLayerAfterDelay(GameObject fruit, int layer) {
        yield return new WaitForSeconds(1);
        if (fruit)
            fruit.layer = layer;
    }

    public void DestroySmallFruits() {
        if (GameController.instance.cleanCount > 0) {
            List<GameObject> fruitsToDelete = new List<GameObject>();

            var berries = GameObject.FindGameObjectsWithTag("Berry");
            var strawberries = GameObject.FindGameObjectsWithTag("Strawberry");
            var kiwies = GameObject.FindGameObjectsWithTag("Kiwi");

            fruitsToDelete.AddRange(berries);
            fruitsToDelete.AddRange(strawberries);
            fruitsToDelete.AddRange(kiwies);

            var sortedFruits = fruitsToDelete.OrderBy(f => f.transform.localPosition.y).Reverse();

            if (fruitsToDelete.Count > 1) {
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
}
