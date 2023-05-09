using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Chessman : MonoBehaviour
{

    public GameObject controller;
    public GameObject movePlate;
    public GameObject attackPlate;
    public GameObject floatingPrefab;

    private int _hp = 0;
    private int _atk = 0;
    private int _def = 0;
    private int _maxHp = 0;
    private int _maxDef = 0;
    private int _atkTime = 1;
    private string _role = null;
    private bool die = false;

    private int xBoard = -1;
    private int yBoard = -1;

    private string player;
    
    public Sprite blue_tank, blue_knight, blue_mage, blue_archer, blue_healer, blue_bard;
    public Sprite red_tank, red_knight, red_mage, red_archer, red_healer, red_bard;

    public void ShowDamageFloating(int damage){
        GameObject go = Instantiate(floatingPrefab, transform.position,Quaternion.identity,transform);
        go.GetComponent<TMP_Text>().SetText(damage.ToString());
    }
    public void ShowHealingFloating(int heal){
        GameObject go = Instantiate(floatingPrefab, transform.position,Quaternion.identity,transform);
        go.GetComponent<TMP_Text>().SetText(heal.ToString());
        go.GetComponent<TMP_Text>().color = new Color(0,255,0,1);
    }

    public void Activate()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");

        SetCoords();

        switch (this.name)
        {
            case "blue_tank": this.GetComponent<SpriteRenderer>().sprite = blue_tank; player = "blue"; break;
            case "blue_knight": this.GetComponent<SpriteRenderer>().sprite = blue_knight; player = "blue"; break;
            case "blue_mage": this.GetComponent<SpriteRenderer>().sprite = blue_mage; player = "blue"; break;
            case "blue_archer": this.GetComponent<SpriteRenderer>().sprite = blue_archer; player = "blue"; break;
            case "blue_healer": this.GetComponent<SpriteRenderer>().sprite = blue_healer; player = "blue"; break;
            case "blue_bard": this.GetComponent<SpriteRenderer>().sprite = blue_bard; player = "blue"; break;
            case "red_tank": this.GetComponent<SpriteRenderer>().sprite = red_tank; player = "red"; break;
            case "red_knight": this.GetComponent<SpriteRenderer>().sprite = red_knight; player = "red"; break;
            case "red_mage": this.GetComponent<SpriteRenderer>().sprite = red_mage; player = "red"; break;
            case "red_archer": this.GetComponent<SpriteRenderer>().sprite = red_archer; player = "red"; break;
            case "red_healer": this.GetComponent<SpriteRenderer>().sprite = red_healer; player = "red"; break;
            case "red_bard": this.GetComponent<SpriteRenderer>().sprite = red_bard; player = "red"; break;
        }
    }

    public void SetCoords()
    {
        float x = xBoard;
        float y = yBoard;

        x *= 1f;
        y *= 1f;

        x += -3.5f;
        y += -5f;

        this.transform.position = new Vector3(x, y, -1.0f);
    }
    
    public string GetPlayerTeam(){
        return player;
    }

    public int GetXBoard()
    {
        return xBoard;
    }

    public int GetYBoard()
    {
        return yBoard;
    }

    public void SetXBoard(int x)
    {
        xBoard = x;
    }

    public void SetYBoard(int y)
    {
        yBoard = y;
    }

    public int GetAtkTime(){
        return _atkTime;
    }

    public void SetAtkTime(int atkTime){
        _atkTime = atkTime;
    }

    public bool GetDie(){
        return die;
    }

    public void SetDie(bool _die){
        die = _die;
    }    

    private void OnMouseUp(){
        Game gm = controller.GetComponent<Game>();
        if(gm.GetBarB() < 0 && gm.GetBarR() < 0 && gm.GetCheckSkipBlue() && gm.GetCheckSkipRed()||
        gm.GetBarB() < 0 && gm.GetBarR() < 0 ||
        gm.GetBarB() < 0 && gm.GetCheckSkipRed() ||
        gm.GetCheckSkipBlue() && gm.GetBarR() < 0 ||
        gm.GetCheckSkipBlue() && gm.GetCheckSkipRed()){

        }else if (!gm.IsGameOver() && gm.GetCurrentPlayer() == player)
        {
            DestroyMovePlates();

            InitiateMovePlates();
        }
    }

    public void DestroyMovePlates()
    {
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        for (int i = 0; i < movePlates.Length; i++)
        {
            Destroy(movePlates[i]); 
        }
    }

    public void InitiateMovePlates()
    {
        switch (this.name)
        {
            case "blue_tank":
            case "red_tank":
                LineMovePlate(1, 0, 2);
                LineMovePlate(0, 1,2);
                LineMovePlate(-1, 0,2);
                LineMovePlate(0, -1,2);
                break;
            case "blue_knight":
            case "red_knight":
                LineMovePlate(1, 0, 2);
                LineMovePlate(0, 1,2);
                LineMovePlate(-1, 0,2);
                LineMovePlate(0, -1,2);
                LineMovePlate(1, 1,2);
                LineMovePlate(-1, -1,2);
                LineMovePlate(-1, 1,2);
                LineMovePlate(1, -1,2);
                break;
            case "blue_mage":
            case "red_mage":
                LineMovePlate(1, 0, 1);
                LineMovePlate(0, 1,1);
                LineMovePlate(-1, 0,1);
                LineMovePlate(0, -1,1);
                break;
            case "blue_archer":
            case "red_archer":
                LineMovePlate(1, 1,2);
                LineMovePlate(-1, -1,2);
                LineMovePlate(-1, 1,2);
                LineMovePlate(1, -1,2);
                break;
            case "blue_healer":
            case "red_healer":
                LineMovePlate(1, 0,2);
                LineMovePlate(-1, 0,2);
                LineMovePlate(1, 1,1);
                LineMovePlate(-1, -1,1);
                LineMovePlate(-1, 1,1);
                LineMovePlate(1, -1,1);
                break;
            case "blue_bard":
            case "red_bard":
                SurroundMovePlate();
                break;
        }
    }

    public void LineMovePlate(int xIncrement, int yIncrement,int range)
    {
        Game sc = controller.GetComponent<Game>();
        int n = 0;
        int x = xBoard + xIncrement;
        int y = yBoard + yIncrement;

        while (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y) == null && n<range)
        {
            MovePlateSpawn(x, y);
            x += xIncrement;
            y += yIncrement;
            n++;
        }
    }

    public void LMovePlate()
    {
        PointMovePlate(xBoard + 1, yBoard + 2);
        PointMovePlate(xBoard - 1, yBoard + 2);
        PointMovePlate(xBoard + 2, yBoard + 1);
        PointMovePlate(xBoard + 2, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard - 2);
        PointMovePlate(xBoard - 1, yBoard - 2);
        PointMovePlate(xBoard - 2, yBoard + 1);
        PointMovePlate(xBoard - 2, yBoard - 1);
    }

    public void SurroundMovePlate()
    {
        PointMovePlate(xBoard, yBoard + 1);
        PointMovePlate(xBoard, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard + 0);
        PointMovePlate(xBoard - 1, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard + 1);
        PointMovePlate(xBoard + 1, yBoard + 0);
        PointMovePlate(xBoard + 1, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard + 1);
    }

    public void PointMovePlate(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();
        if (sc.PositionOnBoard(x, y))
        {
            GameObject cp = sc.GetPosition(x, y);

            if (cp == null)
            {
                MovePlateSpawn(x, y);
            }
        }
    }
    public GameObject CheckConditionPlate(int x, int y)
    {
        
        Game sc = controller.GetComponent<Game>();
        GameObject caa =sc.GetPosition(x, y);
        if (sc.PositionOnBoard(x, y))
        {
            GameObject cp = sc.GetPosition(x, y);

            if (cp.GetComponent<Chessman>().player != player)
            {
                return cp;
            }
        }
        return caa;
    }

    public void MovePlateSpawn(int matrixX, int matrixY)
    {
        float x = matrixX;
        float y = matrixY;

        x *= 1f;
        y *= 1f;

        x += -3.5f;
        y += -5f;

        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);

        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(matrixX, matrixY);
    }

    public void AttackPlateSpawn(int matrixX, int matrixY)
    {
        float x = matrixX;
        float y = matrixY;

        x *= 1f;
        y *= 1f;

        x += -3.5f;
        y += -5f;

        GameObject mp = Instantiate(attackPlate, new Vector3(x, y, -3.0f), Quaternion.identity);

        AttackPlate apScript = mp.GetComponent<AttackPlate>();
        apScript.attack = true;
        apScript.SetReference(gameObject);
        apScript.SetCoords(matrixX, matrixY);
    }

    public void PointAttackPlate(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();
        if (sc.PositionOnBoard(x, y))
        {
            AttackPlateSpawn(x, y);
        }
    }

    public void SurroundAttackPlate()
    {
        PointAttackPlate(xBoard + 0, yBoard + 1);
        PointAttackPlate(xBoard + 0, yBoard - 1);
        PointAttackPlate(xBoard - 1, yBoard + 0);
        PointAttackPlate(xBoard - 1, yBoard - 1);
        PointAttackPlate(xBoard - 1, yBoard + 1);
        PointAttackPlate(xBoard + 1, yBoard + 0);
        PointAttackPlate(xBoard + 1, yBoard - 1);
        PointAttackPlate(xBoard + 1, yBoard + 1);
    }

    public void MageAttackPlate(){
        PointAttackPlate(xBoard + 0, yBoard + 2);
        PointAttackPlate(xBoard - 1, yBoard + 2);
        PointAttackPlate(xBoard + 1, yBoard + 2);
        PointAttackPlate(xBoard + 2, yBoard + 0);
        PointAttackPlate(xBoard + 2, yBoard + 1);
        PointAttackPlate(xBoard + 2, yBoard - 1);
        PointAttackPlate(xBoard + 0, yBoard - 2);
        PointAttackPlate(xBoard - 1, yBoard - 2);
        PointAttackPlate(xBoard + 1, yBoard - 2);
        PointAttackPlate(xBoard - 2, yBoard + 0);
        PointAttackPlate(xBoard - 2, yBoard + 1);
        PointAttackPlate(xBoard - 2, yBoard - 1);
    }

    public void ArcherAttackPlate(){
        PointAttackPlate(xBoard + 0, yBoard + 3);
        PointAttackPlate(xBoard - 1, yBoard + 2);
        PointAttackPlate(xBoard + 1, yBoard + 2);
        PointAttackPlate(xBoard + 3, yBoard + 0);
        PointAttackPlate(xBoard + 2, yBoard + 1);
        PointAttackPlate(xBoard + 2, yBoard - 1);
        PointAttackPlate(xBoard + 0, yBoard - 3);
        PointAttackPlate(xBoard - 1, yBoard - 2);
        PointAttackPlate(xBoard + 1, yBoard - 2);
        PointAttackPlate(xBoard - 3, yBoard + 0);
        PointAttackPlate(xBoard - 2, yBoard + 1);
        PointAttackPlate(xBoard - 2, yBoard - 1);
        PointAttackPlate(xBoard + 2, yBoard + 2);
        PointAttackPlate(xBoard + 2, yBoard - 2);
        PointAttackPlate(xBoard - 2, yBoard - 2);
        PointAttackPlate(xBoard - 2, yBoard + 2);
    }

    public void HealerAttackPlate(){
        SurroundAttackPlate();
        MageAttackPlate();
        PointAttackPlate(xBoard + 2, yBoard + 2);
        PointAttackPlate(xBoard + 2, yBoard - 2);
        PointAttackPlate(xBoard - 2, yBoard - 2);
        PointAttackPlate(xBoard - 2, yBoard + 2);
    }

    public void BardAttackPlate(){
        SurroundAttackPlate();
        MageAttackPlate();
    }
    public int GetHp(){
        return _hp;
    }
    public int GetAtk(){
        return _atk;
    }
    public int GetDef(){
        return _def;
    }
    public int GetMaxHp(){
        return _maxHp;
    }
    public int GetMaxDef(){
        return _maxDef;
    }
    public void SetHp(int hp){
        this._hp = hp;
    }
    public void SetMaxHp(int hp){
        this._maxHp = hp;
    }
    public void SetAtk(int atk){
        this._atk = atk;
    }
    public void SetDef(int def){
        this._def = def;
    }
    public void SetMaxDef(int def){
        this._maxDef = def;
    }
    public void SetRole(string role){
        this._role = role;
    }
    public string GetRole(){
        return this._role;
    }
}