using UnityEngine;
using UnityEngine.UI;
public class UiView : MonoBehaviour
{
    [SerializeField] ScenarioController[] scenarios;
    [SerializeField] GameObject selectorPanel;
    [SerializeField] GameObject finalPanel;
    [SerializeField] GameObject wrongObjectPanel;
    [SerializeField] Text infoText;

    void Start()
    {
        OnStart();
    }

    public void StartScenario(int number)
    {
        selectorPanel.SetActive(false);
        scenarios[number].gameObject.SetActive(true);
        scenarios[number].Init(this);
    }
    public void OnScenarioComplete(int MistakesScore, float timeOverall)
    {
        finalPanel.SetActive(true);
        infoText.text = ("Mistakes: "+ MistakesScore+" Time: "+ timeOverall +" sec");
    }
    public void OnWrongObjectTaken(ScenarioModel scenarioModel)
    {
        wrongObjectPanel.SetActive(true);
    }
    public void OnContinue()
    {
        wrongObjectPanel.SetActive(false);
    }
    public void OnRestart()
    {
        foreach (var scenario in scenarios)
        {
            scenario.Restart();
        }
        OnStart();
    }

        public void OnStart()
    {
        foreach(var scenario in scenarios)
        {
            scenario.gameObject.SetActive(false);
        }
        selectorPanel.SetActive(true);
        finalPanel.SetActive(false);
        wrongObjectPanel.SetActive(false);
    }

}
