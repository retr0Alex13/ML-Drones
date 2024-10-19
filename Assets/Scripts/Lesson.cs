using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class Lesson
{
    public BoxCollider[] AgentSpawnPoints;
    public BoxCollider[] EnemySpawnPoints;
    public List<Transform> Waypoints;

    public GameObject[] Walls;
    public GameObject[] RandmoizedObstacles;

    public BoxCollider GetRandomSpawnPoint(BoxCollider[] spawnPoint)
    {
        int spawnPointIndex = Random.Range(0, spawnPoint.Length);
        return spawnPoint[spawnPointIndex];
    }

    public GameObject GetRandomObstacle(GameObject[] Obstacles)
    {
        int randomObstacleIndex = Random.Range(0, Obstacles.Length);
        return Obstacles[randomObstacleIndex];
    }

    public GameObject GetRandomizedObstacle()
    {
        if (RandmoizedObstacles.Length == 0)
        {
            return null;
        }

        int randomObstacleIndex = Random.Range(0, RandmoizedObstacles.Length);
        return RandmoizedObstacles[randomObstacleIndex];
    }
}
