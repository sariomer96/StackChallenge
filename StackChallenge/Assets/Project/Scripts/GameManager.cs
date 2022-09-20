using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float toleranceRate = 2.675f;
    public float characterMoveSpeed = 0.2f;
    public float stackPosRangeX = 2.5f;
    private AudioSource _audioSource;
    [SerializeField] private Transform finishLine;
    private Vector3 _offset;
    private float camPosY;
    private Quaternion camRotation;
    public bool win = false;
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
      
        StartGame();
        
    }

   public void PlayClip()
    {
        _audioSource.Play();
        _audioSource.pitch += 0.2f;
    }
    void StartGame()
    {

        _audioSource = transform.GetComponent<AudioSource>();
        previousStack = Instantiate(stackPrefab,new Vector3(character.transform.position.x,-0.5f,character.transform.position.z),Quaternion.identity);
        character =  FindObjectOfType<Character>();
        _cameraFollow = FindObjectOfType<CameraFollow>();
        
        _offset = _cameraFollow.transform.position-character.transform.position;
        camRotation = _cameraFollow.transform.rotation;
        camPosY = _cameraFollow.transform.position.y;
        character.StartCoroutine("MoveRoutine");
        
        targetStack = previousStack.transform.position;
          
        _cameraFollow.StartCoroutine("FollowRoutine");
        currentStack = StackManager.instance.StackSpawn(previousStack,isLeft);
  
        StackManager.instance.MoveStack(currentStack);
        currentStack.GetComponentInChildren<MeshRenderer>().material = matList[index];
        
        SetMaterialIndex(matList.Count);
        isLeft = !isLeft;


        StartCoroutine("SpawnRoutine");
  
    }

    public void OnClickWin()
    {
         UIManager.instance.WinBtnDeactivate();
       
        StartCoroutine("CamBackToPlayer");
        StartCoroutine("SpawnOnFinishLine");
        
    }

    IEnumerator SpawnOnFinishLine()
    {
      
        Vector3 finishPos = new Vector3(finishLine.transform.position.x, -0.5f,finishLine.transform.position.z);
      Stack firstStack=  Instantiate(stackPrefab, finishPos +new Vector3(0,0,finishLine.transform.localScale.z * 2),Quaternion.identity);
      
      previousStack = firstStack;
      
      Vector3 target = new Vector3(previousStack.transform.position.x, character.transform.position.y,
          previousStack.transform.position.z);
      
     yield return character.transform.DOMove(target, 0.5f).WaitForCompletion();
     win = false;
     currentStack = StackManager.instance.StackSpawn(previousStack,isLeft);
      
      StackManager.instance.MoveStack(currentStack);
      StartCoroutine("SpawnRoutine");
      targetStack = firstStack.transform.position;
      character.StartCoroutine("MoveRoutine");
      yield return new WaitForSeconds(1);
      finishLine.transform.GetComponentInChildren<Collider>().enabled = true;
      finishLine.transform.position = new Vector3(finishLine.transform.position.x, finishLine.transform.position.y,
          finishLine.transform.position.z+finishLine.transform.localScale.z*1.5f +stackPrefab.transform.localScale.z*10f);

    }
   public IEnumerator CamBackToPlayer()
    {
        StopCoroutine("WinRoutine");
        
        _cameraFollow.StopAllCoroutines();
        
        character.transform.GetComponentInChildren<Animator>().CrossFade("Run",0.3f);
        
        _cameraFollow.transform.DORotate(camRotation.eulerAngles, 1f);
        
       yield return _cameraFollow.transform.DOMove(new Vector3(_offset.x+character.transform.position.x,camPosY,_offset.z+character.transform.position.z), 0.8f).WaitForCompletion();
       _cameraFollow.StartCoroutine("FollowRoutine");
       
    }
    void StopSpawn()
    {
         print("aa");
         character.StopCoroutine("MoveRoutine");
         if (win)
         {
             print("YOU WIN");
             StartCoroutine("WinRoutine");
         }else
         StartCoroutine("KillCharacter");
    }

    IEnumerator WinRoutine()
    {
        
        yield return  character.transform.DOMove(finishLine.transform.position, 0.5f).WaitForCompletion();
        character.transform.GetComponentInChildren<Animator>().Play("dance");
        _cameraFollow.StopAllCoroutines();
        print("WIN");
        yield return   
                _cameraFollow.transform.DOMove(_cameraFollow.transform.position + new Vector3(0, 2f, -2f), 1f)
            .WaitForCompletion();
        UIManager.instance.WinBtnActivate();
        while (true)
        {
            print("ROTATE");
            _cameraFollow.transform.RotateAround(character.transform.position,new Vector3(0,1,0),0.5f);
            yield return new WaitForFixedUpdate();
        }
        
        
    }
    IEnumerator SpawnRoutine()
    {
        while (true)
        {
                 
            if (Input.GetMouseButtonDown(0)&&currentStack.hit)
            { 
                currentStack.transform.DOKill();
                
                float distance = StackManager.instance.GetDistance(previousStack, currentStack);
                   if (distance >toleranceRate )
                   {
                       bool side = StackManager.instance.GetSideForCutStack(previousStack, currentStack);
                       StackManager.instance.CutStack(currentStack, previousStack, side);
                       _audioSource.pitch = 1;
                   }
                   else
                       PlayClip();
                 
                  StackManager.instance.SetCurrentStack(currentStack);
                  targetStack = currentStack.transform.position;
               
                    currentStack= StackManager.instance.StackSpawn(previousStack,isLeft);
                    if (currentStack==null)
                    { 
                        StopSpawn();
                        yield   break;
                        
                    }
            
              StackManager.instance.MoveStack(currentStack);
             currentStack.GetComponentInChildren<MeshRenderer>().material = matList[index];
             SetMaterialIndex(matList.Count);
             
              isLeft = !isLeft;
             
            }
            yield return null;
        }
    }

    void SetMaterialIndex(int matListCount)
    {
        if (index==matListCount-1)
         
            index = 0;
         else
            index++;
    }
  


    IEnumerator KillCharacter()
    {
        Vector3 target = new Vector3(previousStack.transform.position.x, 0, character.transform.position.z+previousStack.transform.localScale.z*1.57f);
     
        while (true)
        {
            yield return new WaitForFixedUpdate();
            float distance = Vector3.Distance(character.transform.position, target);
            if (distance<0.1f)
            {
                Rigidbody rb = character.GetComponent<Rigidbody>();
                rb.constraints = RigidbodyConstraints.None;
                rb.AddForce(Vector3.forward*100);
                _cameraFollow.StopAllCoroutines();
                Destroy(character.gameObject,5f);
                character.GetComponentInChildren<Animator>().Play("Falling");
                yield return new WaitForSeconds(0.4f);
                UIManager.instance.LoseBtn();
             
                yield break;
            }
             character.transform.position=Vector3.MoveTowards(character.transform.position,target,characterMoveSpeed);
            
        }
      
        
    }

  
    
    // Update is called once per frame
    
}
