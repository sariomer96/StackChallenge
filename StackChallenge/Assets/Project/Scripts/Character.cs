using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    // Start is called before the first frame update

    IEnumerator MoveRoutine()
    {
        while (true)
        {
       
            transform.position=Vector3.MoveTowards(transform.position,new Vector3(GameManager.instance.targetStack.x,transform.position.y,GameManager.instance.targetStack.z),0.2f);
            
            yield return new WaitForFixedUpdate();
        }
    }

  
}
