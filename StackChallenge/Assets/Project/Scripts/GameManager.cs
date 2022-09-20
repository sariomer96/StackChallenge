using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool isLose = false;
    public float camFollowSpeed = 0.15f;
    public Vector3 targetStack;
    public  Character character;
    private CameraFollow _cameraFollow;
    [SerializeField] private List<Material> matList = new List<Material>();
    [SerializeField] public float stackMoveSpeed = 0.3f;
    public static GameManager instance;
    private int index = 0;
    [SerializeField] public Stack previousStack;
    public bool isLeft = false;
    public Stack stackPrefab;
    private Stack currentStack=null;
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

        character =  FindObjectOfType<Character>();
        _cameraFollow = FindObjectOfType<CameraFollow>();
        character.StartCoroutine("MoveRoutine");
        targetStack = previousStack.transform.position;
          
        _cameraFollow.StartCoroutine("FollowRoutine");
        currentStack = StackManager.instance.StackSpawn(previousStack,isLeft);
      //   _coroutine=StartCoroutine(StackManager.instance.MoveStack(currentStack,isLeft));
        StackManager.instance.MoveStack(currentStack, isLeft);
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
                 
            if (Input.GetMouseButtonDown(0)&&currentStack.hit)
            {
                currentStack.transform.DOKill();
              
                print("stop");

                   float distance = StackManager.instance.GetDistance(previousStack, currentStack);
                   if (distance>2.675f)
                   {
                       StackManager.instance.CutStack(currentStack,previousStack,isLeft);
                   }
                 
                  StackManager.instance.SetCurrentStack(currentStack);
                  targetStack = currentStack.transform.position;
               
                  //  StopCoroutine(_coroutine);
              
 
                    currentStack= StackManager.instance.StackSpawn(previousStack,isLeft);
                    if (currentStack==null)
                    {
                        GameOver();
                        yield break;
                    }
              //_coroutine=   StartCoroutine(StackManager.instance.MoveStack(currentStack,isLeft));
              StackManager.instance.MoveStack(currentStack, isLeft);
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


    void GameOver()
    {
        
       
        StackManager.instance.StopAllCoroutines();

        isLose = true;

    }

  
    
    // Update is called once per frame
    
}
