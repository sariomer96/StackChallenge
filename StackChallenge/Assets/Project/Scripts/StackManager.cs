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

        Stack newStack = null;
        if (previousStackScale.x < 0.25f)
        {
            return null;
        }

        if (isLeft)
        {
            print("LEFt");
            newStack = Instantiate(GameManager.instance.stackPrefab,
                new Vector3(-previousStackPos.x -2.5f, -0.5f, previousStackPos.z + previousStackScale.z),
                Quaternion.identity);

            newStack.transform.localScale = previousStack.transform.localScale;


        }
        else
        {
            
            print("RIGHRT");
            newStack = Instantiate(GameManager.instance.stackPrefab,
                new Vector3(previousStackPos.x +2.5f, -0.5f, previousStackPos.z + previousStackScale.z),
                Quaternion.identity);

            newStack.transform.localScale = previousStack.transform.localScale;

        }


        return newStack;

    }

    public float GetDistance(Stack previousStack, Stack currentStack)
    {
        float distance = Vector3.Distance(previousStack.transform.position, currentStack.transform.position);
        return distance;
    }

    public bool GetSideForCut(Stack prev,Stack current)
    {
        bool isLeftSide;
        if (prev.transform.position.x-current.transform.position.x>=0)
        {
            isLeftSide= false;
        }else
            isLeftSide = true;

        return isLeftSide;
    }
    public void CutStack(Stack stack, Stack previousStack, bool isLeft)
    {
        print(isLeft);
        
        float distance = Vector2.Distance(previousStack.transform.position, stack.transform.position);
        print("dis" + distance);
        stack.transform.localScale =
            new Vector3(Mathf.Abs(distance - stack.transform.localScale.x), stack.transform.localScale.y, stack.transform.localScale.z);
        Stack cuttedStack = Instantiate(stack);
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

 

    public void MoveStack(Stack stack, bool isLeft)
    {
        Rigidbody rb = stack.GetComponent<Rigidbody>();
        float posX = stack.transform.position.x;
        int direction = 1;

        if (isLeft)
            direction = 1;
        else
            direction = -1;
   
        stack.transform.DOMoveX(-posX,2f).SetLoops (-1, LoopType.Yoyo).SetEase(Ease.Linear).SetSpeedBased(true);

       // yield return new WaitForFixedUpdate();
        /*while (true)
        {
            yield return new WaitForFixedUpdate();
            /*if (isLeft)
                direction = 1;
            else
                direction = -1;#1#

            
            
            stack.position = new Vector3( Mathf.PingPong(Time.time, 5), stack.position.y, stack.position.z);
            /*if (-posX==stack.position.x)
            {
                direction = 1;
                rb.MovePosition(rb.transform.position +
                                Vector3.right * GameManager.instance.stackMoveSpeed * direction);
            }
            else
            {
                {
                    direction = -1;
                    rb.MovePosition(rb.transform.position +
                                    Vector3.right * GameManager.instance.stackMoveSpeed * direction);

                }


            }#1#

        }*/



    }
}
