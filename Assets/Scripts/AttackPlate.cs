using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPlate : MonoBehaviour
{
    GameObject reference = null;

    int matrixX;
    int matrixY;

    public bool attack = false;

    public void SetCoords(int x, int y)
    {
        matrixX = x;
        matrixY = y;
    }

    public void SetReference(GameObject obj)
    {
        reference = obj;
    }

    public GameObject GetReference()
    {
        return reference;
    }
}
