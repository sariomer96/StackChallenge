using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{

    [SerializeField] private GameObject next, retry;
    // Start is called before the first frame update
    public static UIManager instance;
    
    private void Awake()
    {
        instance = this;
    }
    public void WinBtn()
    {
        next.SetActive(true);
    }
    
   
    public void LoseBtn()
    {
        retry.SetActive(true);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(0);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
