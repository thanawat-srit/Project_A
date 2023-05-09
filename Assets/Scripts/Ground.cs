using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    private int boardxBoard = -1;
    private int boardyBoard = -1;
    
    public void BoardSetCoords()
    {
        float x = boardxBoard;
        float y = boardyBoard;

        x *= 1f;
        y *= 1f;

        x += -3.5f;
        y += -5f;

        this.transform.position = new Vector3(x, y, -1.0f);
    }

    public int BoardGetXBoard()
    {
        return boardxBoard;
    }

    public int BoardGetYBoard()
    {
        return boardyBoard;
    }

    public void BoardSetXBoard(int x)
    {
        boardxBoard = x;
    }

    public void BoardSetYBoard(int y)
    {
        boardyBoard = y;
    }
}
