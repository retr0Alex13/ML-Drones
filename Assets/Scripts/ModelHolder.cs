using System.Collections;
using System.Collections.Generic;
using Unity.Sentis;
using UnityEngine;
using UnityEngine.UI;

public class ModelHolder : MonoBehaviour
{
    public ModelAsset modelAsset;
    private Button btn;

    private void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(() => 
        {
            TransferModelAsset();
        });
    }

    public void TransferModelAsset()
    {
        ModelController.SetModelAsset(modelAsset);
    }
}
