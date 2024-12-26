using Unity.MLAgents.Policies;
using Unity.Sentis;
using UnityEngine;

public class ModelController : MonoBehaviour
{
    private static ModelController instance;
    public static ModelController Instance {  get { return instance; } }

    public static ModelAsset modelAsset;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public static void SetModelAsset(ModelAsset model)
    {
        modelAsset = model;
    }
}
