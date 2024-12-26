using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Sentis;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private ModelAsset[] modelAssets;
    [SerializeField] private Button modelButton;

    public void StartSimulation()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void CreateListOfModels()
    {
        if (panel.transform.hierarchyCount > 0)
        {
            foreach (GameObject obj in panel.transform)
            {
                Destroy(obj);
            }
        }
        foreach (var model in modelAssets)
        {
            Button btn = Instantiate(modelButton, panel.transform);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = model.name;
            if (btn.TryGetComponent(out ModelHolder modelHolder))
            {
                modelHolder.modelAsset = model;
            }
        }
    }

    public void QuitFromApp()
    {
        Application.Quit();
    }
}
