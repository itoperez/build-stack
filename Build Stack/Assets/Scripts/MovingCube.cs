using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MovingCube : MonoBehaviour
{
    public static MovingCube CurrentCube { get; private set;}
    public static MovingCube LastCube { get; private set;}
    public MoveDirection MoveDirection { get; set;}

    public static event Action OnDied = delegate { };

    private float speedModifier = 1f;
    private int scoreMovingCube;
    private bool isAlive = true;

    [SerializeField]
    private float moveSpeed = 1f;    

    private void OnEnable() {
        isAlive = true;

        if (LastCube == null) {
            LastCube = GameObject.Find("StartingCube").GetComponent<MovingCube>();             // Bad practice
        }
        CurrentCube = this;
        GetComponent<Renderer>().material.color = GetRandomColor();

        transform.localScale = new Vector3(LastCube.transform.localScale.x, transform.localScale.y, LastCube.transform.localScale.z);
    }

    private Color GetRandomColor() {
        return new Color(UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f));
    }

    internal void Stop() {
        if (gameObject.name == "StartingCube") {
            return;
        }

        moveSpeed = 0f;

        float hangover = GetHangover();
        float direction = hangover > 0 ? 1f : -1f;
        
        float max = MoveDirection == MoveDirection.Z ? LastCube.transform.localScale.z : LastCube.transform.localScale.x;
        if (Mathf.Abs(hangover) >= max) {
            LastCube = null;
            CurrentCube = null;
            //isAlive = false;
            StartCoroutine(DelayEndScreen());
        }         

        if (MoveDirection == MoveDirection.Z) {
            SplitCubeOnZ(hangover, direction);
        } else {
            SplitCubeOnX(hangover, direction);
        }

        LastCube = this;                                                                
    }

    IEnumerator DelayEndScreen() {
        OnDied();
        yield return new WaitForSeconds(.1f);  
        isAlive = false;              
        // yield return new WaitForSeconds(2f);  
        // SceneManager.LoadScene(0);        
    }  

    private float GetHangover() {
        if (MoveDirection == MoveDirection.Z) {
            return transform.position.z - LastCube.transform.position.z;
        } else {
            return transform.position.x - LastCube.transform.position.x;
        }
    }

    private void SplitCubeOnZ(float hangover, float direction) {
        float newZSize = LastCube.transform.localScale.z - Mathf.Abs(hangover);
        float fallingBlockSize = transform.localScale.z - newZSize;

        float newZPosition = LastCube.transform.position.z + (hangover / 2);
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, newZSize);
        transform.position = new Vector3(transform.position.x, transform.position.y, newZPosition);

        float cubeEdge = transform.position.z + (newZSize / 2f * direction);
        float fallingBlockZPosition = cubeEdge + (fallingBlockSize / 2f * direction);

        SpawnDropCube(fallingBlockZPosition, fallingBlockSize);
    }

    private void SplitCubeOnX(float hangover, float direction) {
        float newXSize = LastCube.transform.localScale.x - Mathf.Abs(hangover);
        float fallingBlockSize = transform.localScale.x - newXSize;

        float newXPosition = LastCube.transform.position.x + (hangover / 2);
        transform.localScale = new Vector3(newXSize, transform.localScale.y, transform.localScale.z);
        transform.position = new Vector3(newXPosition, transform.position.y, transform.position.z);

        float cubeEdge = transform.position.x + (newXSize / 2f * direction);
        float fallingBlockXPosition = cubeEdge + (fallingBlockSize / 2f * direction);
        SpawnDropCube(fallingBlockXPosition, fallingBlockSize);
    } 

    private void SpawnDropCube(float fallingBlockZPosition, float fallingBlockSize) {
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

        if (MoveDirection == MoveDirection.Z) {
            cube.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, fallingBlockSize);
            cube.transform.position = new Vector3(transform.position.x, transform.position.y, fallingBlockZPosition);
        } else {
            cube.transform.localScale = new Vector3(fallingBlockSize, transform.localScale.y, transform.localScale.z);
            cube.transform.position = new Vector3(fallingBlockZPosition, transform.position.y, transform.position.z);
        }
        
        cube.AddComponent<Rigidbody>();
        cube.GetComponent<Renderer>().material.color = GetComponent<Renderer>().material.color;

        Destroy(cube.gameObject, 1f);
    }
    
    private void Update() {
        if(!isAlive) {
            if(Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Space)) {
                SceneManager.LoadScene(0);  
            }
        }
        scoreMovingCube = GameObject.Find("ScoreText").GetComponent<ScoreText>().GetScore();

        if (speedModifier >= 1f || speedModifier < 2f) {
            speedModifier = moveSpeed + ( (float)scoreMovingCube / 40);
        }

        if (MoveDirection == MoveDirection.Z) {
            transform.position += transform.forward * Time.deltaTime * (moveSpeed * speedModifier);
        } else {
            transform.position += transform.right * Time.deltaTime * (moveSpeed * speedModifier);
        }

        Debug.Log("Current Speed: " + (moveSpeed * speedModifier));
        Debug.Log("Current Score: " + scoreMovingCube);
    }

    public bool GetIsAlive() {
        return isAlive;
    }
}
