using System.Collections.Generic;
using UnityEngine;

public class AreaRandomizer : MonoBehaviour
{
    [SerializeField] private DroneAgent agent;

    [SerializeField, Space(10)] GameObject[] objectsToRandomize;
    List<GameObject> activeObjects;

    private void Start()
    {
        activeObjects = new List<GameObject>();

        agent.OnNewEpisode += ActivateRandomObject;
    }

    private void OnDestroy()
    {
        agent.OnNewEpisode -= ActivateRandomObject;
    }

    private void ActivateRandomObject()
    {
        activeObjects = ToggleObjectsVisibility(activeObjects);
        activeObjects.Clear();

        int randomObejctIndex = Random.Range(0, objectsToRandomize.Length);
        activeObjects.Add(objectsToRandomize[randomObejctIndex]);

        activeObjects = ToggleObjectsVisibility(activeObjects);
    }

    private List<GameObject> ToggleObjectsVisibility(List<GameObject> objects)
    {
        foreach (GameObject obj in objects)
        {
            obj.SetActive(!obj.activeSelf);
        }
        return objects;
    }

}
