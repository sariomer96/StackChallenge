using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<Stack> stackList = new List<Stack>();
    public bool isLose = false;
    public bool isLeftCutSide = false;
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
        previousStack = Instantiate(stackPrefab,new Vector3(character.transform.position.x,-0.5f,character.transform.position.z),Quaternion.identity);
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


    void StopSpawn()
    {
         print("aa");
         character.StopCoroutine("MoveRoutine");
         StartCoroutine("KillCharacter");
    }
    IEnumerator SpawnRoutine()
    {
        while (true)
        {
                 
            if (Input.GetMouseButtonDown(0)&&currentStack.hit)
            {
                Destroy(previousStack.gameObject,5f);
                currentStack.transform.DOKill();
              
                print("stop");

                   float distance = StackManager.instance.GetDistance(previousStack, currentStack);
                   if (distance>2.675f)
                   {
                       bool side=StackManager.instance.GetSideForCut(previousStack, currentStack);
                       StackManager.instance.CutStack(currentStack,previousStack,side);
                   }
                 
                  StackManager.instance.SetCurrentStack(currentStack);
                  targetStack = currentStack.transform.position;
               
                
              
 
                    currentStack= StackManager.instance.StackSpawn(previousStack,isLeft);
                    if (currentStack==null)
                    {
                        StopSpawn();
                        yield   break;
                    }
            
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


    IEnumerator GameOver()
    {
        
       
        StackManager.instance.StopAllCoroutines(); 
        //character.StopCoroutine("MoveRoutine");
        while (true)
        {
            float distance = Vector3.Distance(character.transform.position, previousStack.transform.position);
         
            if (distance==0)
            {
                character.StopCoroutine("MoveRoutine");
              KillCharacter();
                yield break;
            }

            yield return new WaitForFixedUpdate();
        }
      
        isLose = true;

    }


    IEnumerator KillCharacter()
    {
        Vector3 target = new Vector3(previousStack.transform.position.x, 0, character.transform.position.z+previousStack.transform.localScale.z*1.57f);
        print("kill");
        while (true)
        {
            yield return new WaitForFixedUpdate();
            float distance = Vector3.Distance(character.transform.position, target);
            if (distance<0.1f)
            {
                Rigidbody rb = character.AddComponent<Rigidbody>();
                rb.AddForce(Vector3.forward*100);
                _cameraFollow.StopAllCoroutines();
                Destroy(character.gameObject,5f);
                yield return new WaitForSeconds(0.7f);
                UIManager.instance.LoseBtn();
                yield break;
            }
             character.transform.position=Vector3.MoveTowards(character.transform.position,target,0.1f);
            
        }
      
        
    }

  
    
    // Update is called once per frame
    
}
