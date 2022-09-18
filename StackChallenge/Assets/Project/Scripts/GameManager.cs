using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<Material> matList = new List<Material>();
    public static GameManager instance;
    private int index = 0;
    [SerializeField] public Transform previousStack;
    public bool isLeft = true;
    public GameObject prefab;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }
    
  

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

             Transform transform = StackSpawner.instance.StackSpawn(previousStack,isLeft);
             transform.GetComponentInChildren<MeshRenderer>().material = matList[index];
             if (index==matList.Count-1)
             {
                 index = 0;
             }else
                 index++;
             StackSpawner.instance.SetCurrentStack(transform);
             
             
             isLeft = !isLeft;
             
         
        }
    }
}
