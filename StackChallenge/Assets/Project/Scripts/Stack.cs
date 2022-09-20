using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stack : MonoBehaviour
{
    public bool hit = false;
    // Start is called before the first frame update

    private void OnCollisionEnter(Collision other)
    {
   
       
        print((other.transform.gameObject.layer+"aaaa "+LayerMask.GetMask("finish")));
        if (other.transform.gameObject.layer==LayerMask.NameToLayer("finish"))
        {
            GameManager.instance.win = true;
            other.transform.GetComponentInChildren<Collider>().enabled = false;
        }else
            hit = true;
    }
    private void OnCollisionExit(Collision other)
    {
        print(other.gameObject.name);
        hit = false;
    }

  
}
