using System;
using System.Collections;
using System.Collections.Generic;
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

    public Transform StackSpawn(Transform previousStack,bool isLeft)
    {
        Vector3 previousStackPos = previousStack.position;
        Vector3 previousStackScale = previousStack.localScale;

        Transform newStack=null;
        
      
        if (isLeft)
        {
          newStack=Instantiate(GameManager.instance.stackPrefab.transform,
                new Vector3(-previousStackPos.x-3f, -0.5f, previousStackPos.z + previousStackScale.z),
                Quaternion.identity);

          newStack.localScale = previousStack.localScale;


        }
        else
        {
            newStack= Instantiate(GameManager.instance.stackPrefab.transform,
                new Vector3(previousStackPos.x+3f, -0.5f, previousStackPos.z + previousStackScale.z),
                Quaternion.identity);
            
            newStack.localScale = previousStack.localScale;
       
        }


        return newStack;

    }

    public void CutStack(Transform stack,Transform previousStack,bool isLeft)
    {
        print(isLeft);
        isLeft = !isLeft;
        float distance =Vector2.Distance(previousStack.position, stack.position);
        print("dis"+distance);
        stack.localScale = new Vector3(Mathf.Abs(distance-stack.localScale.x),stack.localScale.y,stack.localScale.z);
        Transform cuttedStack=  Instantiate(stack);
         cuttedStack.localScale = new Vector3(distance, cuttedStack.localScale.y, cuttedStack.localScale.z);
        if (isLeft)
        {
            stack.position = new Vector3((stack.position.x+(distance / 2)), stack.position.y, stack.position.z);
          cuttedStack.position = new Vector3((stack.position.x-(distance/2)-stack.localScale.x/2), cuttedStack.position.y, cuttedStack.position.z);
            
        }
        else
        {
            stack.position = new Vector3((stack.position.x-(distance / 2)), stack.position.y, stack.position.z);
          cuttedStack.position = new Vector3(-(-stack.position.x-(distance/2)-stack.localScale.x/2), cuttedStack.position.y, cuttedStack.position.z);
        }
        print(isLeft);
     


      
        
   

         cuttedStack.GetComponent<Rigidbody>().useGravity = true;

    }

  
    public void SetCurrentStack(Transform currentStack)
    {
       GameManager.instance.previousStack=currentStack;
    
    }
   

    public IEnumerator MoveStack(Transform stack,bool isLeft)
    {
        Rigidbody rb = stack.GetComponent<Rigidbody>();
        int direction = 1;
        while (true)
        {
            if (isLeft)
                direction = 1;
            else
                direction = -1;
            
            rb.MovePosition(rb.transform.position+Vector3.right*GameManager.instance.stackMoveSpeed*direction);
            yield return new WaitForFixedUpdate();
        }
  
    }
    
}
