using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    public GameObject[] fruits;
    public List<FruitData> fruitDatas;
    public CurrentFruitData currentFruitData = new CurrentFruitData();

    public Transform generatedFruits;
    public Transform mergedFruits;

    public static SaveData instance;

    private void Start() {
        instance = this;
    }

    public void LoadData() {
        Time.timeScale = 0f;
        LoadFruitDatas();
        LoadCurrentFruit();
        //Time.timeScale = 1f;
    }

    public void _SaveData() {
        SaveFruitDatas();
        SaveCurrentFruit();
    }

    public void LoadFruitDatas() {
        DestroyAllFruits();

        foreach (var fruitData in fruitDatas) {
            int index = fruitData.index;
            var fruit = Instantiate(fruits[index], mergedFruits);
            fruit.transform.SetLocalPositionAndRotation(
                fruitData.localPosition, 
                fruitData.localRotation
                );

            FruitSwipeController fruitSwipeController = fruit.GetComponent<FruitSwipeController>();
            if (fruitSwipeController)
                Destroy(fruitSwipeController);
        }
        
        void DestroyAllFruits() {
            var fruits = FindAllFruits();
            foreach (var fruit in fruits) {
                Destroy(fruit.gameObject);
            }
        }
    }

    public void SaveFruitDatas() {
        fruitDatas = GetFruitDatas();
    }

    public List<FruitData> GetFruitDatas() {
        var fruits = FindAllFruits();
        List<FruitData> fruitDatas = new List<FruitData>(fruits.Count);

        foreach (var fruit in fruits) {
            FruitData fruitData = new FruitData {
                localPosition = fruit.transform.localPosition,
                localRotation = fruit.transform.localRotation,
                index = fruit.GetIndex()
            };

            fruitDatas.Add(fruitData);
        }

        return fruitDatas;
    }

    public void LoadCurrentFruit() {
        DestroyCurrentFruit();
        int index = currentFruitData.index;
        var currentFruit = Instantiate(fruits[index], generatedFruits);
        currentFruit.transform.SetLocalPositionAndRotation(
            currentFruitData.localPosition,
            Quaternion.Euler(currentFruitData.localRotation)
            );
        currentFruit.GetComponent<Rigidbody2D>().simulated = false;

        void DestroyCurrentFruit() {
            var currentFruit = FindCurrentFruit();
            Destroy(currentFruit.gameObject);
        }
    }

    public void SaveCurrentFruit() {
        var currentFruit = FindCurrentFruit();

        currentFruitData.index = currentFruit.GetIndex();
    }

    public List<Fruit> FindAllFruits() {
        List<Fruit> fruits = new List<Fruit>(GameObject.FindObjectsOfType<Fruit>());
        //fruits.Remove(FindCurrentFruit());

        return fruits;
    }

    public Fruit FindCurrentFruit() {
        var currentFruit = GameObject.FindObjectOfType<FruitSwipeController>();
        return currentFruit.GetComponent<Fruit>();
    }
}
