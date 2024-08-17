using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mira_ativada : MonoBehaviour
{
    public delegate void mira_ativa();
    public static event mira_ativa bancoposicaomira;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable() 
    {
        bancoposicaomira?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
