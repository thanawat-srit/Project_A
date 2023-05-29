using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarnsleyFern : MonoBehaviour
{
    public int numPoints = 100000;

    private float x,y;
    private float nextX,nextY;
    private float r;

    public GameObject dot;

    void Start()
    {
        BFern();
        // StartCoroutine(BFern());
    }
    void BFern(){
        for(int i = 0;i<numPoints;i++){
            r = Random.Range(0, 1f);
            if (r < 0.01)//1
            {
                nextX = 0;                              // |0 0   | * |x| + |0|
                nextY = 0.16f * y;                      // |0 0.16|   |y|   |0|
            }
            else if (r < 0.86)//85
            {
                nextX = 0.85f * x + 0.04f * y;          // | 0.85 0.04| * |x| + | 0 |
                nextY = -0.04f * x + 0.85f * y + 1.6f;  // |-0.04 0.85|   |y|   |1.6|
            }
            else if (r < 0.93)//7
            {
                nextX = 0.20f * x - 0.26f * y;          // |0.2  -0.26| * |x| + | 0 |
                nextY = 0.23f * x + 0.22f * y + 1.6f;   // |0.23  0.22|   |y|   |1.6|
            }
            else if (r <= 1)//7
            {
                nextX = -0.15f * x + 0.28f * y;         // |-0.15 -0.28| * |x| + | 0  |
                nextY = 0.26f * x + 0.24f * y + 0.44f;  // | 0.26  0.24|   |y|   |1.44|
            }
            x = nextX;
            y = nextY;

            // yield return new WaitForSeconds(0.0001f);

            CreateDot(x,y);
        }
    }
    void CreateDot(float x, float y)
    {
        Instantiate(dot, new Vector3((x/4f), (y/4f)-4.8f, -1), Quaternion.identity);
    }
}

