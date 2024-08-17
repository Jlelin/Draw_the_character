using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollbarLimit : MonoBehaviour
{
    public Scrollbar scrollbar;
    public float valorMaximo = 1.0f; // Valor máximo desejado para o Scrollbar

    void Start()
    {
        // Adiciona um listener para o evento OnValueChanged do Scrollbar
        scrollbar.onValueChanged.AddListener(OnScrollbarValueChanged);
    }

    // Método chamado sempre que o valor do Scrollbar é alterado
    void OnScrollbarValueChanged(float value)
    {
        // Limita o valor do Scrollbar ao valor máximo desejado
        scrollbar.value = Mathf.Clamp(value, 0f, valorMaximo);
    }

    // Método chamado sempre que o GameObject é ativado
    void OnEnable()
    {
        // Redefine o valor do Scrollbar para 0
        scrollbar.value = 0f;
    }
}
