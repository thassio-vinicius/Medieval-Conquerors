using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public List<GameObject> characters = new List<GameObject>();
    [HideInInspector]
    public string characterSelected, nickname;

    private void Awake() 
    {
        DontDestroyOnLoad(gameObject);
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
