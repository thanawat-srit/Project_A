using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessTableGenerator : MonoBehaviour
{
    public GameObject tilePrefab;
    public int width = 8;
    public int length = 12;
    

    void Start()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < length; y++)
            {
                Vector3 position = new Vector3(x, y, 0);
                
                BoardCreateboard("board",x,y);
            }
        }
    }

    public void BoardCreateboard(string name, int x, int y)
    {
        GameObject obj = Instantiate(tilePrefab, new Vector3(0, 0, -1), Quaternion.identity);
        Ground gr = obj.GetComponent<Ground>(); 
        gr.name = name; 
        gr.BoardSetXBoard(x);
        gr.BoardSetYBoard(y);
        gr.BoardSetCoords();
    }
}
