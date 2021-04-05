using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioModel 
{
    public enum ControllerAction {switchOn, switchOff, dragIn, dragOut, dragDirIn, dragDirOut };
    [Serializable]
    public struct interactiveElement
    {
        public GameObject gameObject;
        public ControllerAction controllerAction;
    }
    [Serializable]
    public struct actionsSet
    {
        public ControllerAction controllerAction;
        public Vector3 positionToSet;       
        public GameObject target;
    }
    private int currentStep;
    private int mistakesScore;
    private DateTime timeScore;
    private interactiveElement[] interactiveElements;
    private UiView uiView;   

    public void Init(interactiveElement[] interactiveElementsIn, UiView uiViewIn)
    {
        interactiveElements = interactiveElementsIn;
        uiView = uiViewIn;
        currentStep = 0;
        mistakesScore = 0;        
        InputController.onMousePressed += OnMousePressed;
        timeScore = DateTime.Now;
    }
    public void OnMousePressed(Ray rayFromMouse)
    {
        Debug.Log(currentStep);       
        Physics.Raycast(rayFromMouse, out var hit, 10000);       
        if (hit.transform ==null)
            return;
        if (hit.transform != interactiveElements[currentStep].gameObject.transform)
        {
            OnWrongObjectTaken();
            return;
        }
        var ec = interactiveElements[currentStep].gameObject.GetComponent<ElementData>();
        var ev = interactiveElements[currentStep].gameObject.GetComponent<ElementView>();
        if (ec == null|| ev==null)
        {
         Debug.Log("ElementController or ElementView on object " + hit.transform.gameObject.name+ " is null");
         return;
        }      
        foreach(var act in ec.actions)
        {
            if (act.controllerAction == interactiveElements[currentStep].controllerAction)
            {
                Debug.Log(interactiveElements[currentStep].gameObject.name + " " + act.controllerAction);
                ev.OnActionComplete += OnStepComplete;
                ev.PerformAction(act, rayFromMouse);
                InputController.onMousePressed -= OnMousePressed;
                return;
            }
        }        
    }
    private void OnStepComplete(ElementView ev)
    {       
        ev.OnActionComplete -= OnStepComplete;
        if (currentStep >= interactiveElements.Length - 1)
            OnScenarioComplete();
        else
        {
            currentStep += 1;
            InputController.onMousePressed += OnMousePressed;
        }
    }
    private void OnScenarioComplete()
    {
        Debug.Log("Scenario completed");
       
        var diff = DateTime.Now.Subtract(timeScore);
        uiView.OnScenarioComplete(mistakesScore, (float)diff.TotalSeconds);
    }
    private void OnWrongObjectTaken()
    {
        Debug.Log("Wrong object selected");
        mistakesScore += 1;
        uiView.OnWrongObjectTaken(this);
    }
    public void Restart()
    {
        foreach (var ie in interactiveElements)
        {
           var ev = ie.gameObject.GetComponent<ElementView>();
            if (ev != null)           
                ev.ResetTransformToInitialState();           
        }
        InputController.onMousePressed -= OnMousePressed;
    }
}
