using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarnsleyFern : MonoBehaviour
{
    public int numPoints = 20000;

    private float nextX,nextY;
    private float x,y;
    private float r;
    public GameObject dot;

    void Start()
    {
        BF();
        // StartCoroutine(BF());
    }
    void BF(){
        for(int i = 0;i<numPoints;i++){
            r = Random.Range(0, 1f);
        if (r < 0.01)
        {
            nextX = 0;
            nextY = 0.16f * y;
        }
        else if (r < 0.86)
        {
            nextX = 0.85f * x + 0.04f * y;
            nextY = -0.04f * x + 0.85f * y + 1.6f;
        }
        else if (r < 0.93)
        {
            nextX = 0.20f * x - 0.26f * y;
            nextY = 0.23f * x + 0.22f * y + 1.6f;
        }
        else
        {
            nextX = -0.15f * x + 0.28f * y;
            nextY = 0.26f * x + 0.24f * y + 0.44f;
        }
        x = nextX;
        y = nextY;
        // yield return new WaitForSeconds(0.01f);
        CreateDot(x,y);
        }
    }
    void CreateDot(float x, float y)
    {
        Instantiate(dot, new Vector3((x/4f), (y/4f)-4.8f, -1), Quaternion.identity);
    }
}

