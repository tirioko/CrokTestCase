using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementView : MonoBehaviour
{
    public event Action<ElementView> OnActionComplete;
    [SerializeField] Animator animator;
    private ScenarioModel.actionsSet actionsSet;
    private Vector3 initialPos;   
    private Vector3 initialDragPos;
    private Vector3 initialMousePos;
    private float dragDirDirectionCheck;
    private bool dragInitializedAlready;
    void Start()
    {
        initialPos = transform.position;       
    }

    public void PerformAction(ScenarioModel.actionsSet actionsSetIn, Ray rayFromMouse)
    {
        actionsSet = actionsSetIn;
        dragDirDirectionCheck = 0;
        if (actionsSet.controllerAction == ScenarioModel.ControllerAction.switchOn)
        {
            if(animator!=null)
                animator.SetFloat("speedMult", 1);//play animation forward  
            OnActionComplete(this);
        }
        else if (actionsSet.controllerAction == ScenarioModel.ControllerAction.switchOff)
        {
            if (animator != null)
                animator.SetFloat("speedMult", -1);//for now just play animation back. May be replaced by another animation            
            OnActionComplete(this);
        }
        else if ((actionsSet.controllerAction == ScenarioModel.ControllerAction.dragIn|| actionsSet.controllerAction == ScenarioModel.ControllerAction.dragOut)&&!dragInitializedAlready)
        {
           dragInitializedAlready = true;         
           initialDragPos = transform.position;
           InputController.onMouseDrag+= PerformDrag;
           InputController.onMouseDragEnd += OnDragEnd;
        }
        else if ((actionsSet.controllerAction == ScenarioModel.ControllerAction.dragDirIn || actionsSet.controllerAction == ScenarioModel.ControllerAction.dragDirOut)&& !dragInitializedAlready)
        {
            dragInitializedAlready = true;         
            initialMousePos = rayFromMouse.GetPoint(Vector3.Distance(transform.position, rayFromMouse.origin));
            InputController.onMouseDrag += PerformDragDir;            
            InputController.onMouseDragEnd += OnDragDirEnd;
        }
    }

    private void PerformDrag(Ray rayFromMouse)//Free drag object to target object
    {              
        var point = rayFromMouse.GetPoint(0.5f*Vector3.Distance(initialPos, rayFromMouse.origin));
        transform.position = point;        
    }
    private void OnDragEnd(Ray rayFromMouse)//Check if object have been dragged to target, if not, return object back
    {
        RaycastHit[] hits;
        hits = Physics.RaycastAll(rayFromMouse, 10000);
        bool targetWasHit=false;
        foreach (var hit in hits)        {
             
            if (hit.transform == actionsSet.target.transform)
            {
                targetWasHit = true;               
            }
        }
        if (targetWasHit)
        {
            InputController.onMouseDrag -= PerformDrag;
            InputController.onMouseDragEnd -= OnDragEnd;
            OnActionComplete?.Invoke(this);
            dragInitializedAlready = false;
            StopAllCoroutines();            
            StartCoroutine(FinalizeDrag());           
        }
        else       
            transform.position = initialDragPos;        
    }

    private void PerformDragDir(Ray rayFromMouse)//check direction of the drag, if direction is ok, move object to target position
    {
        var point = rayFromMouse.GetPoint(Vector3.Distance(transform.position, rayFromMouse.origin));
        if(Vector3.Dot(initialMousePos - point, transform.position - actionsSet.positionToSet) > 0.001f)
        {
            transform.position = Vector3.Lerp(transform.position, actionsSet.positionToSet, 0.01f);
            dragDirDirectionCheck += 0.01f;
        }      
    }
    private void OnDragDirEnd(Ray rayFromMouse)
    {
        if (dragDirDirectionCheck > 0.2f)
        {
            InputController.onMouseDrag -= PerformDragDir;
            InputController.onMouseDragEnd -= OnDragDirEnd;
            OnActionComplete?.Invoke(this);
            dragInitializedAlready = false;
            StopAllCoroutines();
            StartCoroutine(FinalizeDrag());            
        }
    }

    IEnumerator FinalizeDrag()
    {
        float t = 0;
        while (t < 1)
        {
            transform.position = Vector3.Lerp(transform.position, actionsSet.positionToSet, t);          
            t += 2*Time.deltaTime;
            yield return null;
        }       
        transform.position = actionsSet.positionToSet;      
       
    }
    public void ResetTransformToInitialState()
    {
        transform.position = initialPos;  
        if(animator!=null)
            animator.ForceStateNormalizedTime(0);
    }
   
   void Update()
    {
        if(animator != null)
        {            
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime>1&& animator.GetFloat("speedMult")>0)
            {
                animator.SetFloat("speedMult", 0);               
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0 && animator.GetFloat("speedMult") < 0)
            {
                animator.SetFloat("speedMult", 0);
            }
        }
    }
}
