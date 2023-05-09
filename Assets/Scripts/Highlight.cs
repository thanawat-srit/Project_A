using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour
{
    [SerializeField] private GameObject _highlight;
    // Start is called before the first frame update
    void OnMouseEnter()
    {
        _highlight.SetActive(true);
    }

    // Update is called once per frame
    void OnMouseExit()
    {
        _highlight.SetActive(false);
    }
}
