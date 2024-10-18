using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class LessonsController : MonoBehaviour
{
    [SerializeField] private DroneAgent agent;
    [SerializeField] private Enemy[] enemies;

    [SerializeField] private int defaultLesson = 0;

    private List<GameObject> obstacles;

    [SerializeField] private Lesson[] lessons;
    private Lesson currentLesson;
    private int currentLessonIndex;

    private Enemy selectedEnemy;

    private void Awake()
    {
        obstacles = new List<GameObject>();
        agent.OnNewEpisode += StartLesson;
    }

    private void Start()
    {
        StartLesson();
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

    private void SelectRandomEnemy()
    {
        if (selectedEnemy != null)
        {
            Destroy(selectedEnemy.gameObject);
        }

        int enemyIndex = Random.Range(0, enemies.Length);
        selectedEnemy = enemies[enemyIndex];
    }

    private void StartLesson()
    {
        SetCurrentLesson();

        SelectRandomEnemy();
        SpawnEnemy();

        SetSpawnPoints();
        ActivateObstacles();
    }

    private void SetCurrentLesson()
    {
        currentLessonIndex = GetLessonIndex();
        currentLesson = lessons[currentLessonIndex];
    }

    private void SpawnEnemy()
    {
        selectedEnemy = Instantiate(selectedEnemy, transform);
        selectedEnemy.DroneAgent = agent;
        selectedEnemy.MoveBetweenWaypoints.waypoints = currentLesson.Waypoints.ToArray();
    }

    private int GetLessonIndex()
    {
        return (int)Academy.Instance.EnvironmentParameters.GetWithDefault("Lesson", defaultLesson);
    }

    private void SetSpawnPoints()
    {
        agent.SetSpawnZoneCollider(currentLesson.GetRandomSpawnPoint(currentLesson.AgentSpawnPoints));
        selectedEnemy.SetSpawnZoneCollider(currentLesson.GetRandomSpawnPoint(currentLesson.EnemySpawnPoints));
    }

    private void ActivateObstacles()
    {
        DeactivateAllObstacles();

        int obstaclesCount = currentLesson.Walls.Length;

        for (int i = 0; i < obstaclesCount; i++)
        {
            GameObject obstacle = currentLesson.Walls[i];
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
