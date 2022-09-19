using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float camFollowSpeed = 0.15f;
    public Vector3 targetStack;
    public  Character character;
    private CameraFollow _cameraFollow;
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

        character =  FindObjectOfType<Character>();
        _cameraFollow = FindObjectOfType<CameraFollow>();
        character.StartCoroutine("MoveRoutine");
        targetStack = previousStack.position;
          
        _cameraFollow.StartCoroutine("FollowRoutine");
        currentStack = StackManager.instance.StackSpawn(previousStack,isLeft);
         _coroutine=StartCoroutine(StackManager.instance.MoveStack(currentStack,isLeft));
         currentStack.GetComponentInChildren<MeshRenderer>().material = matList[index];
        
        if (index==matList.Count-1)
        {
            index = 0;
        }else
            index++;
    
        // isLeft = !isLeft;


        StartCoroutine("SpawnRoutine");
  
    }


    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {

               isLeft= StackManager.instance.GetSide(previousStack, currentStack);
                print("stop");
                   
                
                  StackManager.instance.CutStack(currentStack,previousStack,isLeft);
                  StackManager.instance.SetCurrentStack(currentStack);
                  targetStack = currentStack.position;
                    StopCoroutine(_coroutine);
              
 
                    currentStack= StackManager.instance.StackSpawn(previousStack,isLeft);
              _coroutine=   StartCoroutine(StackManager.instance.MoveStack(currentStack,isLeft));
             currentStack.GetComponentInChildren<MeshRenderer>().material = matList[index];
             
                if (index==matList.Count-1)
                {
                    index = 0;
                }else
                    index++;
            
             
             
           //    isLeft = !isLeft;
             
         
            }
            yield return null;
        }
    }
    // Update is called once per frame
    
}
