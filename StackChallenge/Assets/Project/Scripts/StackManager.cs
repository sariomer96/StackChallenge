using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class StackManager : MonoBehaviour
{
    // Start is called before the first frame update

    public static StackManager instance;


    private void Awake()
    {
        instance = this;
    }
  
    public Stack StackSpawn(Stack previousStack, bool isLeft)
    {
        Vector3 previousStackPos = previousStack.transform.position;
        Vector3 previousStackScale = previousStack.transform.localScale;
        float posX;
        Stack newStack;
        if (previousStackScale.x < 0.25f||GameManager.instance.win)
        { 
            return null;
        }

        if (isLeft)
            posX = -previousStackPos.x-GameManager.instance.stackPosRangeX;
        else
            posX = previousStackPos.x+GameManager.instance.stackPosRangeX;
           
       
           
            
        newStack = Instantiate(GameManager.instance.stackPrefab,
            new Vector3(posX, -0.5f, previousStackPos.z + previousStackScale.z),
            Quaternion.identity);

        newStack.transform.localScale = previousStack.transform.localScale;

        return newStack;

    }

    public float GetDistance(Stack previousStack, Stack currentStack)
    {
        return Vector3.Distance(previousStack.transform.position, currentStack.transform.position); 
    }

    public bool GetSideForCutStack(Stack prev,Stack current)
    {
        bool isLeftSide;
        if (prev.transform.position.x-current.transform.position.x>=0)
            isLeftSide= false;
        else
            isLeftSide = true;

        return isLeftSide;
    }
    
    public void CutStack(Stack stack, Stack previousStack, bool isLeft)
    { 
        float distance =Vector2.Distance(previousStack.transform.position, stack.transform.position);
      
        float stackPosX,stackPosXForCuttedStack;
        stack.transform.localScale =
            new Vector3(Mathf.Abs(distance - stack.transform.localScale.x), stack.transform.localScale.y, stack.transform.localScale.z);
        
        
        Stack cuttedStack = Instantiate(stack);
        cuttedStack.transform.GetComponentInChildren<Collider>().enabled = false;
        
        Destroy(cuttedStack.gameObject,3); 
        
        cuttedStack.transform.localScale = new Vector3(distance, cuttedStack.transform.localScale.y, cuttedStack.transform.localScale.z);
       
        if (!isLeft)
        {
            stack.transform.position = new Vector3(( stack.transform.position.x + (distance / 2)),  stack.transform.position.y,  stack.transform.position.z);
            cuttedStack.transform.position = new Vector3(( stack.transform.position.x - (distance / 2) -  stack.transform.localScale.x / 2), 
            cuttedStack.transform.position.y, cuttedStack.transform.position.z);

        }
        else
        {
            stack.transform.position = new Vector3(( stack.transform.position.x - (distance / 2)),  stack.transform.position.y,  stack.transform.position.z);
            cuttedStack.transform.position = new Vector3(-(- stack.transform.position.x - (distance / 2) -  stack.transform.localScale.x / 2),
                cuttedStack.transform.position.y, cuttedStack.transform.position.z);
        }
        cuttedStack.GetComponent<Rigidbody>().useGravity = true;

    }


    public void SetCurrentStack(Stack currentStack)
    {
        GameManager.instance.previousStack = currentStack;

    }

 

    public void MoveStack(Stack stack)
    { 
        float posX = stack.transform.position.x;
        stack.transform.DOMoveX(-posX,GameManager.instance.stackMoveSpeed).SetLoops (-1, LoopType.Yoyo).SetEase(Ease.Linear).SetSpeedBased(true);
    }
}
