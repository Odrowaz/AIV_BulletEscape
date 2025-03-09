using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Turrets

    [SerializeField] GameObject[] turretsPrefab;
    [SerializeField] int numberOfTurrets = 5;
    private List<GameObject> _turrets;

    [SerializeField] [UnityEngine.Range(0.1f, 50f)] private float minDistanceX = 10;
    [SerializeField] [UnityEngine.Range(0.1f, 50f)] private float minDistanceZ = 10;

    [SerializeField] [UnityEngine.Range(0.1f, 100f)] private float deltaX = 30;
    [SerializeField] [UnityEngine.Range(0.1f, 100f)] private float deltaZ = 30;

    [SerializeField] [UnityEngine.Range(0.05f, 5f)] private float minFireRate = 0.5f;
    [SerializeField] [UnityEngine.Range(0.05f, 5f)] private float maxFireRate = 2f;

    [SerializeField] [UnityEngine.Range(1f, 50f)] private float minFireDistance = 10f;
    [SerializeField] [UnityEngine.Range(1f, 100f)] private float maxFireDistance = 30f;

    #endregion

    #region Walls

    private GameObject[] _walls;

    [SerializeField] private GameObject[] wallsPrefab;

    [SerializeField] [UnityEngine.Range(0, 4)] private int extraWallLife = 2;
    
    [SerializeField] [UnityEngine.Range(0.1f, 50f)] private float minDistanceWallX = 1;
    [SerializeField] [UnityEngine.Range(0.1f, 50f)] private float maxDistanceWallZ = 5;

    [SerializeField] [UnityEngine.Range(0.1f, 100f)] private float deltaWallX = 1;
    [SerializeField] [UnityEngine.Range(0.1f, 100f)] private float deltaWallZ = 1;

    #endregion

    [SerializeField] GameObject gameOver;
    [SerializeField] GameObject youWin;

    void Start()
    {
        if (numberOfTurrets == 0)
        {
            Debug.LogWarning("No number of turrets detected");
            return;
        }

        _walls = new GameObject[numberOfTurrets];

        _turrets = new List<GameObject>();

        for (int i = 0; i < numberOfTurrets; i++)
        {
            GameObject turret = Instantiate(turretsPrefab[Random.Range(0, turretsPrefab.Length)]);
            _turrets.Add(turret);
            
            DestroyOnMultipleHit destroyOnMultipleTurret = turret.GetComponent<DestroyOnMultipleHit>();
            destroyOnMultipleTurret.GameManager = this;

            int tries = 5;

            bool intersect = false;

            do
            {
                turret.transform.position = new Vector3(minDistanceX + Random.Range(-1f, 1f) * deltaX, 0,
                    minDistanceZ + Random.Range(-1f, 1f) * deltaZ);

                turret.transform.Rotate(Vector3.up, Random.Range(0, 360f), Space.World);

                foreach (var addedTurrent in _turrets)
                {
                    if (addedTurrent == turret || addedTurrent == null) continue;

                    if (addedTurrent.GetComponent<CapsuleCollider>().bounds.Intersects(turret.GetComponent<CapsuleCollider>().bounds))
                    {
                        intersect = true;
                        break;
                    }
                }

                tries--;
            } while (intersect && tries > 0);

            FireBulletsAtTarget turretScripts = turret.GetComponent<FireBulletsAtTarget>();
            turretScripts.Configure(Random.Range(minFireRate, maxFireRate),
                Random.Range(minFireDistance, maxFireDistance), transform);

            GameObject wall = Instantiate(wallsPrefab[Random.Range(0, wallsPrefab.Length)]);
            _walls[i] = wall;
            
            wall.transform.position = turret.transform.position + new Vector3(
                minDistanceWallX + Random.Range(-1f, 1f) * deltaWallX,
                wall.transform.localScale.y * 0.5f,
                maxDistanceWallZ + Random.Range(-1f, 1f * deltaWallZ));

            wall.transform.RotateAround(turret.transform.position, Vector3.up, Random.Range(0, 360f));

            wall.transform.Rotate(Vector3.up, Random.Range(-45f, -45f), Space.Self);

            DestroyOnMultipleHit destroyOnMultiple = wall.GetComponent<DestroyOnMultipleHit>();
            destroyOnMultiple.GameManager = this;
            destroyOnMultiple.Configure(destroyOnMultipleTurret.MaxHitCount + extraWallLife);
        }
    }

    public void GameOver()
    {
        DestroyAllTurrets();

        Debug.Log($"GameOver: Play Time :{Time.time}");

        gameOver.SetActive(true);
    }

    public void CheckDestroyedElements(GameObject element)
    {
        _turrets.Remove(element);
        Debug.Log($"DidDestroyWall: Turrets Available :{_turrets.Count}");

        if (_turrets.Count <= 0)
        {
            Debug.Log($"GameOver: Play Time: {Time.time}");

            youWin.SetActive(true);
        }
    }

    private void DestroyAllTurrets()
    {
        foreach (var turret in _turrets)
        {
            Destroy(turret);
        }
    }
}
