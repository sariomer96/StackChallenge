using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform character;
    [SerializeField] private List<Material> matList = new List<Material>();
    [SerializeField] public float stackMoveSpeed = 0.3f;
    public static GameManager instance;
    private int index = 0;
    [SerializeField] public Transform previousStack;
    public bool isLeft = false;
    public GameObject stackPrefab;
    private Transform currentStack=null;
    private Coroutine _coroutine=null;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }


    private void Start()
    {
        StartGame();
        
    }

   
    void StartGame()
    {
        currentStack = StackManager.instance.StackSpawn(previousStack,isLeft);
         _coroutine=StartCoroutine(StackManager.instance.MoveStack(currentStack,isLeft));
         currentStack.GetComponentInChildren<MeshRenderer>().material = matList[index];
        if (index==matList.Count-1)
        {
            index = 0;
        }else
            index++;
    
         isLeft = !isLeft;


        StartCoroutine("SpawnRoutine");
  
    }


    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
              
                print("stop");
                   
                
                  StackManager.instance.CutStack(currentStack,previousStack,isLeft);
                  StackManager.instance.SetCurrentStack(currentStack);
                    StopCoroutine(_coroutine);
              
 
                    currentStack= StackManager.instance.StackSpawn(previousStack,isLeft);
              _coroutine=   StartCoroutine(StackManager.instance.MoveStack(currentStack,isLeft));
             currentStack.GetComponentInChildren<MeshRenderer>().material = matList[index];
             
                if (index==matList.Count-1)
                {
                    index = 0;
                }else
                    index++;
            
             
             
               isLeft = !isLeft;
             
         
            }
            yield return null;
        }
    }
    // Update is called once per frame
    
}
