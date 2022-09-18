using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackSpawner : MonoBehaviour
{
    // Start is called before the first frame update

    public static StackSpawner instance;


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
          newStack=Instantiate(GameManager.instance.prefab.transform,
                new Vector3(-previousStackScale.x * 2, -0.5f, previousStackPos.z + previousStackScale.z),
                Quaternion.identity);
        }
        else
        {
            newStack= Instantiate(GameManager.instance.prefab.transform,
                new Vector3(previousStackScale.x * 2, -0.5f, previousStackPos.z + previousStackScale.z),
                Quaternion.identity);
        }


        return newStack;

    }

    public void SetCurrentStack(Transform currentStack)
    {
       GameManager.instance.previousStack=currentStack;
    

    }
    
}
