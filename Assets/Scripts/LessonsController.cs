using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class LessonsController : MonoBehaviour
{
    [SerializeField] private DroneAgent agent;
    [SerializeField] private Enemy enemy;

    [SerializeField] private int defaultLesson = 0;

    private List<GameObject> obstacles;

    [SerializeField] private Lesson[] lessons;
    private Lesson currentLesson;
    private int currentLessonIndex;

    private void Start()
    {
        obstacles = new List<GameObject>();
        agent.OnNewEpisode += StartLesson;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            if (defaultLesson + 1 > lessons.Length)
            {
                defaultLesson = 0;
            }
            defaultLesson = defaultLesson + 1;
            agent.EndEpisode();
        }
    }

    private void OnDestroy()
    {
        agent.OnNewEpisode -= StartLesson;
    }

    private void StartLesson()
    {
        currentLessonIndex = GetLessonIndex();
        currentLesson = lessons[currentLessonIndex];

        SetSpawnPoints();
        ActivateObstacles();
    }

    private int GetLessonIndex()
    {
        return (int)Academy.Instance.EnvironmentParameters.GetWithDefault("Lesson", defaultLesson);
    }

    private void SetSpawnPoints()
    {
        agent.SetSpawnZoneCollider(currentLesson.GetRandomSpawnPoint(currentLesson.AgentSpawnPoints));
        enemy.SetSpawnZoneCollider(currentLesson.GetRandomSpawnPoint(currentLesson.EnemySpawnPoints));
    }

    private void ActivateObstacles()
    {
        DeactivateAllObstacles();

        int obstaclesCount = currentLesson.Obstacles.Length;

        for (int i = 0; i < obstaclesCount; i++)
        {
            GameObject obstacle = currentLesson.Obstacles[i];
            obstacle.SetActive(true);
            obstacles.Add(obstacle);
        }

        // Optionally activate randomized obstacles
        int randomizedObstaclesCount = Random.Range(0, currentLesson.RandmoizedObstacles.Length + 1);

        for (int i = 0; i < randomizedObstaclesCount; i++)
        {
            GameObject randomizedObstacle = currentLesson.GetRandomizedObstacle();
            if (obstacles.Contains(randomizedObstacle))
            {
                continue;
            }
            else if (randomizedObstacle != null)
            {
                randomizedObstacle.SetActive(true);
                obstacles.Add(randomizedObstacle);
            }
        }
    }

    private void DeactivateAllObstacles()
    {
        foreach (GameObject obstacle in obstacles)
        {
            obstacle.SetActive(false);
        }
        obstacles.Clear();
    }
}
