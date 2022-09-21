using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float characterMoveSpeed = 0.2f;
    public float camFollowSpeed = 0.15f;
    public float stackMoveSpeed = 0.3f;  // cube move speed  between left - right  
    public List<Stack> stackList = new List<Stack>(); // stacklist holds cubes
    public float toleranceRate = 2.675f;  // toleranceRate is a "perfect state" tolerance
    public float stackPosRangeX = 2.5f;
    public bool isMoveCharacter = false;
    public bool win = false;
    public bool isLeftCutSide = false;
    public Vector3 targetStack;
    public  Character character;
    public static GameManager instance;
    public Stack previousStack;
    public bool isLeft = false;
    public Stack stackPrefab;
   
  
    [SerializeField] float stackSpeedRate = 0.05f;  //  when perfect,  increase to stackspeed
    [SerializeField] List<Material> matList = new List<Material>(); // matList holds all cube materials
    public string state = "IdleRoutine";
    private Animator characterAnim;
    private float baseStackSpeed;
    private bool isWinAnim = false;
    private Vector3 fallTargetPos;
    private AudioSource _audioSource;
    private Vector3 _offset;
    private Stack currentStack=null;
    private float camPosY;
    private int materialIndex = 0; 
    private CameraFollow _cameraFollow;
    [SerializeField] private Transform finishLine;
    private Quaternion camRotation;
    private float stackPosY=-0.5f; // stack  position.y always starts -0.5f 
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
        characterAnim = character.transform.GetComponentInChildren<Animator>();
         baseStackSpeed = stackMoveSpeed;
        _audioSource = transform.GetComponent<AudioSource>();
        previousStack = Instantiate(stackPrefab,new Vector3(character.transform.position.x,stackPosY,character.transform.position.z),Quaternion.identity);
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
        currentStack.GetComponentInChildren<MeshRenderer>().material = matList[materialIndex];
        
        SetMaterialIndex(matList.Count);
        isLeft = !isLeft;


        StartCoroutine("SpawnRoutine");
      
        StartCoroutine("DecisionRoutine");
        StartCoroutine(state);
    }
    
    public  IEnumerator DecisionRoutine()
    {
        float distance=0;
        while (true)
        {
            if (character)
            {
                 distance = Vector3.Distance(character.transform.position, fallTargetPos);
            }
            
           if(isMoveCharacter )
             ChangeState("RunRoutine");
           else if(distance<0.1f&&!currentStack)
            {
                ChangeState("DeadRoutine");
                yield break;
            }
           else if(isWinAnim&&!currentStack)
               ChangeState("WinAnimRoutine");
           else
               ChangeState("IdleRoutine");
           
           yield return  null;
        }
    }

    IEnumerator IdleRoutine()
    { 
        
        if (state=="IdleRoutine")
        {
            RegisterAnimation("Idle");
            yield return null;
        }

        StartCoroutine(state);
    }
    
    IEnumerator DeadRoutine()
    { 
        if (state=="DeadRoutine")
        {
            RegisterAnimation("Falling");
            yield return null;
        }
        StartCoroutine(state);
    }

    IEnumerator RunRoutine()
    {
        
        if (state=="RunRoutine")
        {
            RegisterAnimation("Run");
            yield return null;
        }
        StartCoroutine(state);
    }
    string currentAnimation;
    public void RegisterAnimation(string value)
    {
        if (value == currentAnimation)
            return;
        currentAnimation = value;
        characterAnim.Play(currentAnimation);
    }
    IEnumerator WinAnimRoutine()
    {
        if (state=="WinAnimRoutine")
        {
            RegisterAnimation("dance");
            yield return null;
        }
        StartCoroutine(state);
    }
    
    
    public void ChangeState(string value)
    {
        if (state == value)
            return;
        state = value;
    }
    public void OnClickWin()
    {
        stackMoveSpeed = baseStackSpeed; 
        UIManager.instance.WinBtnDeactivate();
       
        StartCoroutine("CamBackToPlayer");
        StartCoroutine("StartNewLevel");
        
    }
    void MoveFinishLine()
    {
        finishLine.transform.GetComponentInChildren<Collider>().enabled = true;
        finishLine.transform.position = 
        new Vector3(finishLine.transform.position.x, finishLine.transform.position.y, 
      finishLine.transform.position.z+finishLine.transform.localScale.z*1.5f +stackPrefab.transform.localScale.z*10f);
    }
    IEnumerator StartNewLevel()
    { 
      Vector3 finishPos = new Vector3(finishLine.transform.position.x, stackPosY,finishLine.transform.position.z);  
      Stack firstStack=  Instantiate(stackPrefab, 
          finishPos +new Vector3(0,0,finishLine.transform.localScale.z * 2),Quaternion.identity);
      
      previousStack = firstStack;
      
      Vector3 target = new Vector3(previousStack.transform.position.x, character.transform.position.y,
          previousStack.transform.position.z);
      
     yield return character.transform.DOMove(target, 0.5f).WaitForCompletion();
     win = false;
     isWinAnim = false;
     currentStack = StackManager.instance.StackSpawn(previousStack,isLeft);
      
      StackManager.instance.MoveStack(currentStack);
      StartCoroutine("SpawnRoutine");
      targetStack = firstStack.transform.position;
      character.StartCoroutine("MoveRoutine");
      yield return new WaitForSeconds(1);
      MoveFinishLine();

    }
   public IEnumerator CamBackToPlayer()  
    {
        StopCoroutine("WinRoutine");
        _cameraFollow.StopAllCoroutines();
        _cameraFollow.transform.DORotate(camRotation.eulerAngles, 1f);
        
       yield return _cameraFollow.transform.DOMove(new Vector3(_offset.x+character.transform.position.x,camPosY,_offset.z+character.transform.position.z), 0.8f).WaitForCompletion();
       _cameraFollow.StartCoroutine("FollowRoutine");
    }
    void StopSpawn()
    {
         character.StopCoroutine("MoveRoutine");
        if (win) 
         StartCoroutine("WinRoutine");
        else
         StartCoroutine("KillCharacter");
    }

    IEnumerator WinRoutine()
    {
        
        yield return  character.transform.DOMove(finishLine.transform.position, 0.5f).WaitForCompletion();
        isWinAnim = true;
        _cameraFollow.StopAllCoroutines();
        yield return   
                _cameraFollow.transform.DOMove(_cameraFollow.transform.position + new Vector3(0, 2f, -2f), 1f)
            .WaitForCompletion();
        UIManager.instance.WinBtnActivate();
        while (true)
        {
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
                   {
                       PlayClip();  // PERFECT!
                       stackMoveSpeed += stackSpeedRate;
                   }
                   StackManager.instance.SetCurrentStack(currentStack);
                   targetStack = currentStack.transform.position;
               
                    currentStack= StackManager.instance.StackSpawn(previousStack,isLeft);
                    if (currentStack==null)
                    { 
                        StopSpawn();
                        yield   break;
                        
                    }
                    
             StackManager.instance.MoveStack(currentStack);
             currentStack.GetComponentInChildren<MeshRenderer>().material = matList[materialIndex];
             SetMaterialIndex(matList.Count);
             
              isLeft = !isLeft;
            }
            yield return null;
        }
    }

    void SetMaterialIndex(int matListCount)
    {
        if (materialIndex==matListCount-1)
            materialIndex = 0;
         else
            materialIndex++;
    }
  
    IEnumerator KillCharacter()
    {
        Vector3 target = new Vector3(previousStack.transform.position.x, 0, character.transform.position.z+previousStack.transform.localScale.z*1.6f);
        fallTargetPos = target;
        while (true)
        {
            yield return new WaitForFixedUpdate();
            float distance = Vector3.Distance(character.transform.position, target);
            if (distance<0.1f)
            {
              isMoveCharacter = false; 
              Rigidbody rb = character.GetComponent<Rigidbody>();
              rb.constraints = RigidbodyConstraints.None;
              rb.AddForce(Vector3.forward*100);
              _cameraFollow.StopAllCoroutines();
         
              Destroy(character.gameObject,5f);
              
              yield return new WaitForSeconds(0.4f);
              UIManager.instance.LoseBtn();

             yield break;
            }

            isMoveCharacter = true;
            character.transform.position=Vector3.MoveTowards(character.transform.position,target,characterMoveSpeed);
        }
      
    }
      
    }
 
    

