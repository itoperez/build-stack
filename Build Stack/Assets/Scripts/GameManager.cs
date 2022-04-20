using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static event Action OnCubeSpawned = delegate { };

    private CubeSpawner[] spawners;
    private int spawnerIndex;
    private CubeSpawner currentSpawner;
    private bool isAlive = true;

    private void MovingCube_OnDied() {
        isAlive = false;
    }    

    private void Awake() {
        spawners = FindObjectsOfType<CubeSpawner>();
    }

    private void Start() {
        MovingCube.OnDied += MovingCube_OnDied;
    }

    private void Update()
    {
        //isAlive = GameObject.Find("MovingCube").GetComponent<MovingCube>().GetIsAlive();
        if ((Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Space)) && isAlive) {             // "Fire1" for ctrl, left click, tap, etc.
            if (MovingCube.CurrentCube != null) {
                MovingCube.CurrentCube.Stop();
            }
            spawnerIndex = spawnerIndex == 0 ? 1 : 0;
            currentSpawner = spawners[spawnerIndex];

            currentSpawner.SpawnCube();
            OnCubeSpawned();
        }
    }
}
