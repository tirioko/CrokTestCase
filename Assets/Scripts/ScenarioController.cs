using UnityEngine;
using System;
public class ScenarioController : MonoBehaviour
{
    [SerializeField] ScenarioModel.interactiveElement[] interactiveElements;  

    private ScenarioModel scenarioModel;
    public void Init(UiView uiView)
    {
        if (scenarioModel == null)  
            scenarioModel = new ScenarioModel();
        scenarioModel.Init(interactiveElements, uiView);
    }
    public void Restart()
    {
        if (scenarioModel != null)
            scenarioModel.Restart();
    }
}
