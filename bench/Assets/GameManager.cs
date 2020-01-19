using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        }
        else {
            _instance = this;
        }
    }

    static public int score = 1000;
    [SerializeField] GameObject cubePrefab; 
    private void Start() {
        
    }


    public  void SpawnPigeon() {
        Instantiate(cubePrefab, Vector3.zero, Quaternion.identity, transform);
    }
}