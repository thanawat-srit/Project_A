using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;

public class Game : MonoBehaviour
{
    public GameObject chesspiece;
    public GameObject turnBarRed;
    public GameObject turnBarBlue;
    public GameObject barBlue1,barBlue2,barBlue3,barBlue4;
    public GameObject barRed1,barRed2,barRed3,barRed4;
    public GameObject sBarBlue1,sBarBlue2;
    public GameObject sBarRed1,sBarRed2;
    public Button skipBlueButton, skipRedButton;
    public TMP_Text blueHpText, blueAtkText, blueDefText;
    public TMP_Text redHpText, redAtkText, redDefText;
    public TMP_Text winnerText, roundText;
    public Image blueHpIcon, blueAtkIcon, blueDefIcon;
    public Image redHpIcon, redAtkIcon, redDefIcon;
    public Image backPlate;
    public Image blueSkipStatus, redSkipStatus;
    public Image attackStatusImage;
    public AudioSource roundSound, endSound;
    public Sprite tank, knight, mage, archer, healer;

    private bool checkSkipBlue = false;
    private bool checkSkipRed = false;
    private bool combatPhaseCheck = false;
    private bool killInRound = false;
    private bool betweenRounds = false;
    private int barB = 3;
    private int barR = 3;
    private int sBarB = -1;
    private int sBarR = -1;
    private int roundCount = 1;

    private GameObject[,] positions = new GameObject[8, 12];
    private List<GameObject> playerBlue = new List<GameObject>();
    private List<GameObject> playerRed = new List<GameObject>();
    private GameObject[] barBlue = new GameObject[4];
    private GameObject[] barRed = new GameObject[4];
    private GameObject[] sBarBlue = new GameObject[2];
    private GameObject[] sBarRed = new GameObject[2];

    private string currentPlayer = "blue";

    private bool gameOver = false;

    public void Start()
    {

        skipRedButton.interactable = false;

        barBlue = new GameObject[4] {barBlue1,barBlue2,barBlue3,barBlue4};
        barRed = new GameObject[4] {barRed1,barRed2,barRed3,barRed4};
        sBarBlue = new GameObject[2] {sBarBlue1,sBarBlue2};
        sBarRed = new GameObject[2] {sBarRed1,sBarRed2};

        playerRed = new List<GameObject>() { 
            Create("red_tank","tank",       7, 1, 2, 5, 8),
            Create("red_knight","knight",   6, 2, 1, 2, 8),
            Create("red_mage","mage",       5, 2, 0, 1, 10),
            Create("red_archer","archer",   5, 2, 0, 6, 10),
            Create("red_healer","healer",   5, 2, 0, 4, 10), 
            Create("red_bard","bard",       4, 0, 0, 3, 10)};
            
        playerBlue = new List<GameObject>() { 
            Create("blue_tank","tank",      7, 1, 2, 2, 3),
            Create("blue_knight","knight",  6, 2, 1, 5, 3),
            Create("blue_mage","mage",      5, 2, 0, 6, 1),
            Create("blue_archer","archer",  5, 2, 0, 1, 1),
            Create("blue_healer","healer",  5, 2, 0, 3, 1), 
            Create("blue_bard","bard",      4, 0, 0, 4, 1)};


        for (int i = 0; i < playerBlue.Count; i++)
        {
            SetPosition(playerBlue[i]);
            SetPosition(playerRed[i]);
        }
        StartCoroutine(ShowRound(roundText,2f));
    }
    public List<GameObject> GetPlayRedList(){
        return playerRed;
    }
    public List<GameObject> GetPlayBlueList(){
        return playerBlue;
    }

    public void Update()//-----------------------------------------------------------------------------------
    {
        
        CheckEndGame();

        SkipStatus();

        if(!combatPhaseCheck){
            BardBuff();
        }

        if (gameOver == true && Input.GetMouseButtonDown(0))
        {
            gameOver = false;
            SceneManager.LoadScene(0);
        }

        if(!betweenRounds){
            if(currentPlayer == "red" && barB < 0 && barR < 0){
                currentPlayer = "blue";
                betweenRounds = true;
                CombatAndPrepair();
                PlayChangeToBlue();
            }else if(currentPlayer == "blue" && barB < 0 && barR < 0){
                currentPlayer = "red";
                betweenRounds = true;
                CombatAndPrepair();
                PlayChangeToRed();
            }else if(barB < 0 && checkSkipRed){
                currentPlayer = "red";
                betweenRounds = true;
                CombatAndPrepair();
                PlayChangeToRed();
            }else if(checkSkipBlue && barR < 0 ){
                currentPlayer = "blue";
                betweenRounds = true;
                CombatAndPrepair();
                PlayChangeToBlue();
            }else if(checkSkipBlue && checkSkipRed){
                betweenRounds = true;
                CombatAndPrepair();
            }
        }
        
        
    }

    

    public GameObject Create(string name,string role,int hp,int atk,int def, int x, int y)
    {
        GameObject obj = Instantiate(chesspiece, new Vector3(0, 0, -1), Quaternion.identity);
        Chessman cm = obj.GetComponent<Chessman>(); 
        cm.name = name; 
        cm.SetHp(hp);
        cm.SetAtk(atk);
        cm.SetDef(def);
        cm.SetMaxHp(hp);
        cm.SetMaxDef(def);
        cm.SetRole(role);
        cm.SetXBoard(x);
        cm.SetYBoard(y);
        cm.Activate(); 
        return obj;
    }

    public void SetPosition(GameObject obj)
    {
        Chessman cm = obj.GetComponent<Chessman>();

        positions[cm.GetXBoard(), cm.GetYBoard()] = obj;
    }

    public void SetPositionEmpty(int x, int y)
    {
        positions[x, y] = null;
    }

    public GameObject GetPosition(int x, int y)
    {
        return positions[x, y];
    }

    public bool PositionOnBoard(int x, int y)
    {
        if (x < 0 || y < 0 || x >= positions.GetLength(0) || y >= positions.GetLength(1)) return false;
        return true;
    }

    public string GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public bool IsGameOver()
    {
        return gameOver;
    }

    public void NextTurn()
    {
        if (currentPlayer == "red" && barR >= 0)
        {
            if(!checkSkipRed && sBarR < 0){
                barRed[barR].SetActive(false);
                barR--;
                if(!checkSkipBlue && barB>=0){
                    FullSwitchToBlue();
                }
            }
            else if(!checkSkipRed)
            {
                sBarRed[sBarR].SetActive(false);
                sBarR--;
                if(!checkSkipBlue && barB>=0){
                    FullSwitchToBlue();
                }
            } 
        }else if(currentPlayer == "red" && barR < 0){
            FullSwitchToBlue();
        }
        else if(currentPlayer == "blue" && barB >= 0)
        {
            if(!checkSkipBlue && sBarB < 0){
                barBlue[barB].SetActive(false);
                barB--;
                if(!checkSkipRed && barR>=0){
                    FullSwitchToRed();
                }
            }
            else if(!checkSkipBlue)
            {
                sBarBlue[sBarB].SetActive(false);
                sBarB--;
                if(!checkSkipRed && barR>=0){
                    FullSwitchToRed();
                }
            }
        }else if(currentPlayer == "blue" && barB < 0){
            FullSwitchToRed();
        }
    }

    public void FullSwitchToBlue(){
        turnBarRed.SetActive(false);
        currentPlayer = "blue";
        skipRedButton.interactable = false;
        skipBlueButton.interactable = true;
        turnBarBlue.SetActive(true);
    }
    public void FullSwitchToRed(){
        turnBarBlue.SetActive(false);
        currentPlayer = "red";
        skipBlueButton.interactable = false;
        skipRedButton.interactable = true;
        turnBarRed.SetActive(true);
    }

    
    private void CombatAndPrepair(){
        combatPhaseCheck = true;
        attackStatusImage.enabled = true;
        CombatPhase();//CombatPhase---------------------------------
        
        //ResetStat();//ResetStat ----> Set Def & Set AtkTime-------
        //SaveEnergy();//SaveEnergy ----> Fill Energy---------------
    }

    public void SaveEnergy(){
        Debug.Log(barB);
        Debug.Log(barR);
        //RE_Blue
        for(int i=0;i<barB+1 && i<2 && sBarB<1;i++){
            sBarBlue[i].SetActive(true);
            sBarB++;
        }
        //RE_Red
        for(int i=0;i<barR+1 && i<2 && sBarR<1;i++){
            sBarRed[i].SetActive(true);
            sBarR++;
        }
        for(int i=0;i<4;i++){
            barBlue[i].SetActive(true);
            barRed[i].SetActive(true);
            barB=3;
            barR=3;
        }
        checkSkipBlue = false;
        checkSkipRed = false;
    }
    private void SkipStatus(){
        if(checkSkipBlue){
            blueSkipStatus.enabled = true;
        }else{
            blueSkipStatus.enabled = false;
        }
        if(checkSkipRed){
            redSkipStatus.enabled = true;
        }else{
            redSkipStatus.enabled = false;
        }
    }
    private void CheckEndGame(){
        if(playerRed.Count == 0 && playerBlue.Count == 0){
            Winner("Tie");
        }else if(playerRed.Count == 0){
            Winner("Blue");
        }else if(playerBlue.Count == 0){
            Winner("Red");
        }
    }
    public void Winner(string playerWinner)
    {
        backPlate.enabled = true;
        winnerText.enabled = true;
        if(playerWinner == "Tie"){
            winnerText.SetText("Tie!");
        }else if(playerWinner == "Blue"){
            winnerText.SetText("Blue Win!");
        }else if(playerWinner == "Red"){
            winnerText.SetText("Red Win!");
        }
        endSound.Play();
        gameOver = true;
    }

    private void PlayChangeToBlue(){
        turnBarBlue.SetActive(true);
        turnBarRed.SetActive(false);
        skipBlueButton.interactable = true;
        skipRedButton.interactable = false;
    }
    private void PlayChangeToRed(){
        turnBarBlue.SetActive(false);
        turnBarRed.SetActive(true);
        skipBlueButton.interactable = false;
        skipRedButton.interactable = true;
    }

    public void ClickButtonBlue(){
        Chessman cm = chesspiece.GetComponent<Chessman>();
        cm.DestroyMovePlates();
        checkSkipBlue = true;
        currentPlayer = "red";
        skipBlueButton.interactable = false;
        PlayChangeToRed();
    }
    public void ClickButtonRed(){
        Chessman cm = chesspiece.GetComponent<Chessman>();
        cm.DestroyMovePlates();
        checkSkipRed = true;
        currentPlayer = "blue";
        skipRedButton.interactable = false;
        PlayChangeToBlue();
    }
    public int GetBarB(){
        return this.barB;
    }
    public int GetBarR(){
        return this.barR;
    }
    public bool GetCheckSkipBlue(){
        return this.checkSkipBlue;
    }
    public bool GetCheckSkipRed(){
        return this.checkSkipRed;
    }

    public void DestroyAttackPlates()
    {
        GameObject[] attackPlates = GameObject.FindGameObjectsWithTag("AttackPlate");
        for (int i = 0; i < attackPlates.Length; i++)
        {
            Destroy(attackPlates[i]);
        }
        // BlueHideStatus();
    }
    public void BlueHideStatus(){
        blueHpText.enabled = false;
        blueAtkText.enabled = false;
        blueDefText.enabled = false;
        redHpText.enabled = false;
        redAtkText.enabled = false;
        redDefText.enabled = false;
    }

    public void BlueShowStatus(GameObject gO){
        string hp = gO.GetComponent<Chessman>().GetHp().ToString();
        string atk = gO.GetComponent<Chessman>().GetAtk().ToString();
        string def = gO.GetComponent<Chessman>().GetDef().ToString();
        blueHpText.enabled = true;
        blueAtkText.enabled = true;
        blueDefText.enabled = true;
        blueHpText.SetText(hp);
        blueAtkText.SetText(atk);
        blueDefText.SetText(def);
    }

    public void RedShowStatus(GameObject gO){
        string hp = gO.GetComponent<Chessman>().GetHp().ToString();
        string atk = gO.GetComponent<Chessman>().GetAtk().ToString();
        string def = gO.GetComponent<Chessman>().GetDef().ToString();
        redHpText.enabled = true;
        redAtkText.enabled = true;
        redDefText.enabled = true;
        redHpText.SetText(hp);
        redAtkText.SetText(atk);
        redDefText.SetText(def);
    }

    public void FindBlueTank(){
        foreach(GameObject go in playerBlue){
            if(go.name == "blue_tank"){
                go.GetComponent<Chessman>().SurroundAttackPlate();
                BlueShowStatus(go);
            }
        }
    }
    public void FindBlueKnight(){
        foreach(GameObject go in playerBlue){
            if(go.name == "blue_knight"){
                go.GetComponent<Chessman>().SurroundAttackPlate();
                BlueShowStatus(go);
            }
        }
    }
    public void FindBlueMage(){
        foreach(GameObject go in playerBlue){
            if(go.name == "blue_mage"){
                go.GetComponent<Chessman>().MageAttackPlate();
                BlueShowStatus(go);
            }
        }
    }
    public void FindBlueArcher(){
        foreach(GameObject go in playerBlue){
            if(go.name == "blue_archer"){
                go.GetComponent<Chessman>().ArcherAttackPlate();
                BlueShowStatus(go);
            }
        }
    }
    public void FindBlueHealer(){
        foreach(GameObject go in playerBlue){
            if(go.name == "blue_healer"){
                go.GetComponent<Chessman>().HealerAttackPlate();
                BlueShowStatus(go);
            }
        }
    }
    public void FindBlueBard(){
        foreach(GameObject go in playerBlue){
            if(go.name == "blue_bard"){
                go.GetComponent<Chessman>().BardAttackPlate();
                BlueShowStatus(go);
            }
        }
    }
    public void FindRedTank(){
        foreach(GameObject go in playerRed){
            if(go.name == "red_tank"){
                go.GetComponent<Chessman>().SurroundAttackPlate();
                RedShowStatus(go);
            }
        }
    }
    public void FindRedKnight(){
        foreach(GameObject go in playerRed){
            if(go.name == "red_knight"){
                go.GetComponent<Chessman>().SurroundAttackPlate();
                RedShowStatus(go);
            }
        }
    }
    public void FindRedMage(){
        foreach(GameObject go in playerRed){
            if(go.name == "red_mage"){
                go.GetComponent<Chessman>().MageAttackPlate();
                RedShowStatus(go);
            }
        }
    }
    public void FindRedArcher(){
        foreach(GameObject go in playerRed){
            if(go.name == "red_archer"){
                go.GetComponent<Chessman>().ArcherAttackPlate();
                RedShowStatus(go);
            }
        }
    }
    public void FindRedHealer(){
        foreach(GameObject go in playerRed){
            if(go.name == "red_healer"){
                go.GetComponent<Chessman>().HealerAttackPlate();
                RedShowStatus(go);
            }
        }
    }
    public void FindRedBard(){
        foreach(GameObject go in playerRed){
            if(go.name == "red_bard"){
                go.GetComponent<Chessman>().BardAttackPlate();
                RedShowStatus(go);
            }
        }
    }

    public void CombatPhase(){
        // BardBuff();
        KnightAttack();
        // TankAttack();
        // ArcherAttack();
        // MageAttack();
        // HealerHeal();
        
    }

    public void ResetStat(){
        foreach(GameObject blue in playerBlue){
            Chessman gO = blue.GetComponent<Chessman>();
            int maxDef = gO.GetMaxDef();
            gO.SetDef(maxDef);
            gO.SetAtkTime(1);
        }
        foreach(GameObject red in playerRed){
            Chessman gO = red.GetComponent<Chessman>();
            int maxDef = gO.GetMaxDef();
            gO.SetDef(maxDef);
            gO.SetAtkTime(1);
        }
    }

    public void DealDamage(GameObject enamy,int enamyHp, int enamyDef, int weAtk){
        int currentEnamyHp = enamyHp;
        int currentEnamyDef = enamyDef;
        int damageDeal=0;
        if(weAtk<=enamyDef){
            currentEnamyDef = enamyDef-weAtk;
            damageDeal = 0;
        }else{
            currentEnamyHp = enamyHp-(weAtk-enamyDef);
            currentEnamyDef = 0;
            damageDeal = enamyDef-weAtk;
        }

        enamy.GetComponent<Chessman>().ShowDamageFloating(damageDeal);

        enamy.GetComponent<Chessman>().SetHp(currentEnamyHp);
        enamy.GetComponent<Chessman>().SetDef(currentEnamyDef);

        Debug.Log("Hp = "+enamy.name + currentEnamyHp);
        Debug.Log("Def = "+enamy.name + currentEnamyDef);
        Debug.Log("Damage = "+weAtk);


        if(currentEnamyHp<=0){
            enamy.GetComponent<Chessman>().SetDie(true);
            Debug.Log("kill");
        }
    }

    private void CheckDeadChess(){
        for(int i=0;i<playerBlue.Count;i++){
            GameObject blue = playerBlue[i];
            if(blue.GetComponent<Chessman>().GetDie()==true){
                playerBlue.RemoveAt(i);
                SetPositionEmpty(blue.GetComponent<Chessman>().GetXBoard(),
                blue.GetComponent<Chessman>().GetYBoard());
                blue.GetComponent<Chessman>().enabled = false;
                blue.SetActive(false);
                killInRound = true;
            }
        }
        for(int i=0;i<playerRed.Count;i++){
            GameObject red = playerRed[i];
            if(red.GetComponent<Chessman>().GetDie()==true){
                playerRed.RemoveAt(i);
                SetPositionEmpty(red.GetComponent<Chessman>().GetXBoard(),
                red.GetComponent<Chessman>().GetYBoard());
                red.GetComponent<Chessman>().enabled = false;
                red.SetActive(false);
                killInRound = true;
            }
        }
    }
    public void Healing(GameObject ally,int allyHp, int allyMaxHp, int weAtk){
        int Heal=0;
        if(allyHp + weAtk >= allyMaxHp){
            ally.GetComponent<Chessman>().SetHp(allyMaxHp);
            Heal = allyMaxHp-allyHp;
        }else{
            ally.GetComponent<Chessman>().SetHp(allyHp + weAtk);
            Heal = weAtk;
        }
        ally.GetComponent<Chessman>().ShowHealingFloating(Heal);
        int currentAllyHp = ally.GetComponent<Chessman>().GetHp();

        Debug.Log("Hp = "+ ally.name + currentAllyHp);
        Debug.Log("Heal = "+ weAtk);
    }

    public void DealDamageAOE(GameObject we, int weAtk,int xTarget, int yTarget){
        string team = we.GetComponent<Chessman>().GetPlayerTeam();

        if(team == "blue"){
            foreach(GameObject sideEnamy in playerRed){
                int xSideTarget = sideEnamy.GetComponent<Chessman>().GetXBoard();
                int ySideTarget = sideEnamy.GetComponent<Chessman>().GetYBoard();
                int targetHp = sideEnamy.GetComponent<Chessman>().GetHp();
                int targetDef =sideEnamy.GetComponent<Chessman>().GetDef();

                if(xSideTarget == xTarget && ySideTarget+1 == yTarget ||
                xSideTarget == xTarget+1 && ySideTarget == yTarget ||
                xSideTarget == xTarget && ySideTarget-1 == yTarget ||
                xSideTarget == xTarget-1 && ySideTarget == yTarget ){
                    DealDamage(sideEnamy,targetHp,targetDef,weAtk-1);
                }
            }
        }
        
        if(team == "red"){
            foreach(GameObject sideEnamy in playerBlue){
                int xSideTarget = sideEnamy.GetComponent<Chessman>().GetXBoard();
                int ySideTarget = sideEnamy.GetComponent<Chessman>().GetYBoard();
                int targetHp = sideEnamy.GetComponent<Chessman>().GetHp();
                int targetDef =sideEnamy.GetComponent<Chessman>().GetDef();
                
                if(xSideTarget == xTarget && ySideTarget+1 == yTarget ||
                xSideTarget == xTarget+1 && ySideTarget == yTarget ||
                xSideTarget == xTarget && ySideTarget-1 == yTarget ||
                xSideTarget == xTarget-1 && ySideTarget == yTarget ){
                    DealDamage(sideEnamy,targetHp,targetDef,weAtk-1);
                }
            }
        }
    }

    public void AttackZone(int matrixX, int matrixY,ref List<GameObject> enamies, GameObject we)
    {

        string team = we.GetComponent<Chessman>().GetPlayerTeam();

        if(team == "blue"){
            foreach(GameObject go in playerRed){
                if(go.GetComponent<Chessman>().GetXBoard() == matrixX && 
                    go.GetComponent<Chessman>().GetYBoard() == matrixY)
                {
                    enamies.Add(go);
                }
            }
        }else if(team == "red"){
            foreach(GameObject go in playerBlue){
                if(go.GetComponent<Chessman>().GetXBoard() == matrixX && 
                    go.GetComponent<Chessman>().GetYBoard() == matrixY)
                {
                    enamies.Add(go);
                }
            }
        }
    }

    public void BuffZone(int matrixX, int matrixY,ref List<GameObject> allies, GameObject we)
    {

        string team = we.GetComponent<Chessman>().GetPlayerTeam();

        if(team == "blue"){
            foreach(GameObject go in playerBlue){
                if(go.GetComponent<Chessman>().GetXBoard() == matrixX && 
                    go.GetComponent<Chessman>().GetYBoard() == matrixY)
                {
                    allies.Add(go);
                }
            }
        }else if(team == "red"){
            foreach(GameObject go in playerRed){
                if(go.GetComponent<Chessman>().GetXBoard() == matrixX && 
                    go.GetComponent<Chessman>().GetYBoard() == matrixY)
                {
                    allies.Add(go);
                }
            }
        }
    }

    public void KnightAttack(){
        Debug.Log("KnightAttack");
        attackStatusImage.sprite = knight;
        List<GameObject> redEnamies = new List<GameObject>();
        List<GameObject> blueEnamies = new List<GameObject>();
        
        //Blue Attack--------------------------------------------------------------------------
        foreach(GameObject we in playerBlue){   
            if(we.GetComponent<Chessman>().GetRole() == "knight"){
                Chessman cm = we.GetComponent<Chessman>();
                Debug.Log("Blue Attack");
                int BlueAtkTime = 1;
                int x = cm.GetXBoard();
                int y = cm.GetYBoard();
                AttackZone(x + 0, y + 1,ref redEnamies,we);
                AttackZone(x + 0, y - 1,ref redEnamies,we);
                AttackZone(x - 1, y + 0,ref redEnamies,we);
                AttackZone(x - 1, y - 1,ref redEnamies,we);
                AttackZone(x - 1, y + 1,ref redEnamies,we);
                AttackZone(x + 1, y + 0,ref redEnamies,we);
                AttackZone(x + 1, y - 1,ref redEnamies,we);
                AttackZone(x + 1, y + 1,ref redEnamies,we);
                int cHp=10;
                if(redEnamies != null){
                    foreach(GameObject enamy in redEnamies){
                        int enamyHp = enamy.GetComponent<Chessman>().GetHp();
                        int enamyDef = enamy.GetComponent<Chessman>().GetDef();
                        int weAtk = cm.GetAtk();

                        if(enamy.GetComponent<Chessman>().GetRole() == "tank"){
                            
                            //atk
                            DealDamage(enamy,enamyHp,enamyDef,weAtk);
                            //

                            Debug.Log("Blue Knight attack tank");
                            BlueAtkTime = 0;
                            break;
                        }
                        else
                        {
                            if(enamyHp > 0 && cHp > enamyHp){
                                cHp = enamyHp;
                            }
                        }
                    }
                    if(BlueAtkTime != 0){
                        foreach(GameObject enamy in redEnamies){
                            int enamyHp = enamy.GetComponent<Chessman>().GetHp();
                            int enamyDef = enamy.GetComponent<Chessman>().GetDef();
                            int weAtk = we.GetComponent<Chessman>().GetAtk();
                            if(cHp == enamyHp){
                                //atk
                                DealDamage(enamy,enamyHp,enamyDef,weAtk);
                                //

                                Debug.Log("Blue Knight attack low hp");
                                BlueAtkTime = 0;
                                break;
                            }
                        }
                    }
                }
                
            }
        }

        //Red Attack--------------------------------------------------------------------------
        foreach(GameObject we in playerRed){
            if(we.GetComponent<Chessman>().GetRole() == "knight"){
                Chessman cm = we.GetComponent<Chessman>();
                Debug.Log("Red Attack");
                int RedAtkTime = 1;
                int x = cm.GetXBoard();
                int y = cm.GetYBoard();
                AttackZone(x + 0, y + 1,ref blueEnamies,we);
                AttackZone(x + 0, y - 1,ref blueEnamies,we);
                AttackZone(x - 1, y + 0,ref blueEnamies,we);
                AttackZone(x - 1, y - 1,ref blueEnamies,we);
                AttackZone(x - 1, y + 1,ref blueEnamies,we);
                AttackZone(x + 1, y + 0,ref blueEnamies,we);
                AttackZone(x + 1, y - 1,ref blueEnamies,we);
                AttackZone(x + 1, y + 1,ref blueEnamies,we);

                int cHp=10;
                if(blueEnamies != null){
                    foreach(GameObject enamy in blueEnamies){
                        int enamyHp = enamy.GetComponent<Chessman>().GetHp();
                        int enamyDef = enamy.GetComponent<Chessman>().GetDef();
                        int weAtk = we.GetComponent<Chessman>().GetAtk();
                        
                        if(enamy.GetComponent<Chessman>().GetRole() == "tank"){
                            
                            //atk
                            DealDamage(enamy,enamyHp,enamyDef,weAtk);
                            //


                            Debug.Log("Red Knight attack tank");

                            RedAtkTime = 0;
                            break;
                        }
                        else
                        {
                            if(enamyHp > 0 && cHp > enamyHp){
                                cHp = enamyHp;
                            }
                        }
                    }
                    if(RedAtkTime != 0){
                        foreach(GameObject enamy in blueEnamies){
                            int enamyHp = enamy.GetComponent<Chessman>().GetHp();
                            int enamyDef = enamy.GetComponent<Chessman>().GetDef();
                            int weAtk = we.GetComponent<Chessman>().GetAtk();
                            if(cHp == enamyHp){
                            //atk
                            DealDamage(enamy,enamyHp,enamyDef,weAtk);
                            //

                            Debug.Log("Red Knight attack low hp");

                            RedAtkTime = 0;
                            break;
                            }
                        }
                    }
                }
            }
        }
        StartCoroutine(WaitKnight());
    }

    public void TankAttack(){
        attackStatusImage.sprite = tank;
        Debug.Log("TankAttack");
        List<GameObject> redEnamies = new List<GameObject>();
        List<GameObject> blueEnamies = new List<GameObject>();
        
        //Blue Attack--------------------------------------------------------------------------
        foreach(GameObject we in playerBlue){   
            if(we.GetComponent<Chessman>().GetRole() == "tank"){
                Chessman cm = we.GetComponent<Chessman>();
                Debug.Log("Blue Attack");
                int BlueAtkTime = 1;
                int x = cm.GetXBoard();
                int y = cm.GetYBoard();
                AttackZone(x + 0, y + 1,ref redEnamies,we);
                AttackZone(x + 0, y - 1,ref redEnamies,we);
                AttackZone(x - 1, y + 0,ref redEnamies,we);
                AttackZone(x - 1, y - 1,ref redEnamies,we);
                AttackZone(x - 1, y + 1,ref redEnamies,we);
                AttackZone(x + 1, y + 0,ref redEnamies,we);
                AttackZone(x + 1, y - 1,ref redEnamies,we);
                AttackZone(x + 1, y + 1,ref redEnamies,we);
                int cHp=10;
                if(redEnamies != null){
                    foreach(GameObject enamy in redEnamies){
                        int enamyHp = enamy.GetComponent<Chessman>().GetHp();
                        int enamyDef = enamy.GetComponent<Chessman>().GetDef();
                        int weAtk = cm.GetAtk();

                        if(enamy.GetComponent<Chessman>().GetRole() == "tank"){
                            
                            //atk
                            DealDamage(enamy,enamyHp,enamyDef,weAtk);
                            //

                            Debug.Log("Blue Tank attack tank");
                            BlueAtkTime = 0;
                            break;
                        }
                        else
                        {
                            if(enamyHp > 0 && cHp > enamyHp){
                                cHp = enamyHp;
                            }
                        }
                    }
                    if(BlueAtkTime != 0){
                        foreach(GameObject enamy in redEnamies){
                            int enamyHp = enamy.GetComponent<Chessman>().GetHp();
                            int enamyDef = enamy.GetComponent<Chessman>().GetDef();
                            int weAtk = we.GetComponent<Chessman>().GetAtk();
                            if(cHp == enamyHp){
                            //atk
                            DealDamage(enamy,enamyHp,enamyDef,weAtk);
                            //

                            Debug.Log("Blue Tank attack low hp");
                            BlueAtkTime = 0;
                            break;
                            }
                        }
                    }
                }
                
            }
        }

        //Red Attack--------------------------------------------------------------------------
        foreach(GameObject we in playerRed){
            if(we.GetComponent<Chessman>().GetRole() == "tank"){
                Chessman cm = we.GetComponent<Chessman>();
                Debug.Log("Red Attack");
                int RedAtkTime = 1;
                int x = cm.GetXBoard();
                int y = cm.GetYBoard();
                AttackZone(x + 0, y + 1,ref blueEnamies,we);
                AttackZone(x + 0, y - 1,ref blueEnamies,we);
                AttackZone(x - 1, y + 0,ref blueEnamies,we);
                AttackZone(x - 1, y - 1,ref blueEnamies,we);
                AttackZone(x - 1, y + 1,ref blueEnamies,we);
                AttackZone(x + 1, y + 0,ref blueEnamies,we);
                AttackZone(x + 1, y - 1,ref blueEnamies,we);
                AttackZone(x + 1, y + 1,ref blueEnamies,we);

                int cHp=10;
                if(blueEnamies != null){
                    foreach(GameObject enamy in blueEnamies){
                        int enamyHp = enamy.GetComponent<Chessman>().GetHp();
                        int enamyDef = enamy.GetComponent<Chessman>().GetDef();
                        int weAtk = we.GetComponent<Chessman>().GetAtk();
                        
                        if(enamy.GetComponent<Chessman>().GetRole() == "tank"){
                            
                            //atk
                            DealDamage(enamy,enamyHp,enamyDef,weAtk);
                            //


                            Debug.Log("Red Tank attack tank");

                            RedAtkTime = 0;
                            break;
                        }
                        else
                        {
                            if(enamyHp > 0 && cHp > enamyHp){
                                cHp = enamyHp;
                            }
                        }
                    }
                    if(RedAtkTime != 0){
                        foreach(GameObject enamy in blueEnamies){
                            int enamyHp = enamy.GetComponent<Chessman>().GetHp();
                            int enamyDef = enamy.GetComponent<Chessman>().GetDef();
                            int weAtk = we.GetComponent<Chessman>().GetAtk();
                            if(cHp == enamyHp){
                            //atk
                            DealDamage(enamy,enamyHp,enamyDef,weAtk);
                            //

                            Debug.Log("Red Tank attack low hp");

                            RedAtkTime = 0;
                            break;
                            }
                        }
                    }
                }
                
            }
        }
        StartCoroutine(WaitTank());
    }

    public void ArcherAttack(){
        attackStatusImage.sprite = archer;
        Debug.Log("ArcherAttack");
        List<GameObject> redEnamies = new List<GameObject>();
        List<GameObject> blueEnamies = new List<GameObject>();
        
        //Blue Attack--------------------------------------------------------------------------
        foreach(GameObject we in playerBlue){   
            if(we.GetComponent<Chessman>().GetRole() == "archer"){
                Chessman cm = we.GetComponent<Chessman>();
                Debug.Log("Blue Attack");
                int BlueAtkTime = cm.GetAtkTime();
                int x = cm.GetXBoard();
                int y = cm.GetYBoard();
                AttackZone(x + 0, y + 3,ref redEnamies,we);
                AttackZone(x + 1, y + 2,ref redEnamies,we);
                AttackZone(x + 2, y + 2,ref redEnamies,we);
                AttackZone(x + 2, y + 1,ref redEnamies,we);
                AttackZone(x + 3, y + 0,ref redEnamies,we);
                AttackZone(x + 2, y - 1,ref redEnamies,we);
                AttackZone(x + 2, y - 2,ref redEnamies,we);
                AttackZone(x - 1, y - 2,ref redEnamies,we);
                AttackZone(x + 0, y - 3,ref redEnamies,we);
                AttackZone(x - 1, y - 2,ref redEnamies,we);
                AttackZone(x - 2, y - 2,ref redEnamies,we);
                AttackZone(x - 2, y - 1,ref redEnamies,we);
                AttackZone(x - 3, y + 0,ref redEnamies,we);
                AttackZone(x - 2, y + 1,ref redEnamies,we);
                AttackZone(x - 2, y + 2,ref redEnamies,we);
                AttackZone(x - 1, y + 2,ref redEnamies,we);
                int cHp=10;
                if(redEnamies!=null){
                    foreach(GameObject enamy in redEnamies){
                        int enamyHp = enamy.GetComponent<Chessman>().GetHp();
                        int enamyDef = enamy.GetComponent<Chessman>().GetDef();
                        int weAtk = cm.GetAtk();

                        if(enamy.GetComponent<Chessman>().GetRole() == "tank"){
                            
                            for(int i=0;i<BlueAtkTime;i++){
                                //atk
                                DealDamage(enamy,enamyHp,enamyDef,weAtk);
                                //
                            }

                            Debug.Log("Blue Archer attack tank");
                            BlueAtkTime = 0;
                            break;
                        }
                        else
                        {
                            if(enamyHp > 0 && cHp > enamyHp){
                                cHp = enamyHp;
                            }
                        }
                    }
                    if(BlueAtkTime != 0){
                        foreach(GameObject enamy in redEnamies){
                            int enamyHp = enamy.GetComponent<Chessman>().GetHp();
                            int enamyDef = enamy.GetComponent<Chessman>().GetDef();
                            int weAtk = we.GetComponent<Chessman>().GetAtk();
                            if(cHp == enamyHp){
                            for(int i=0;i<BlueAtkTime;i++){
                                //atk
                                DealDamage(enamy,enamyHp,enamyDef,weAtk);
                                //
                            }
                            Debug.Log("Blue Archer attack low hp");
                            BlueAtkTime = 0;
                            break;
                            }
                        }
                    }
                }
                
            }
        }

        //Red Attack --------------------------------------------------------------------------
        foreach(GameObject we in playerRed){
            if(we.GetComponent<Chessman>().GetRole() == "archer"){
                Chessman cm = we.GetComponent<Chessman>();
                Debug.Log("Red Attack");
                int RedAtkTime = cm.GetAtkTime();
                int x = cm.GetXBoard();
                int y = cm.GetYBoard();
                AttackZone(x + 0, y + 3,ref blueEnamies,we);
                AttackZone(x + 1, y + 2,ref blueEnamies,we);
                AttackZone(x + 2, y + 2,ref blueEnamies,we);
                AttackZone(x + 2, y + 1,ref blueEnamies,we);
                AttackZone(x + 3, y + 0,ref blueEnamies,we);
                AttackZone(x + 2, y - 1,ref blueEnamies,we);
                AttackZone(x + 2, y - 2,ref blueEnamies,we);
                AttackZone(x - 1, y - 2,ref blueEnamies,we);
                AttackZone(x + 0, y - 3,ref blueEnamies,we);
                AttackZone(x - 1, y - 2,ref blueEnamies,we);
                AttackZone(x - 2, y - 2,ref blueEnamies,we);
                AttackZone(x - 2, y - 1,ref blueEnamies,we);
                AttackZone(x - 3, y + 0,ref blueEnamies,we);
                AttackZone(x - 2, y + 1,ref blueEnamies,we);
                AttackZone(x - 2, y + 2,ref blueEnamies,we);
                AttackZone(x - 1, y + 2,ref blueEnamies,we);

                int cHp=10;
                if(blueEnamies != null){
                    foreach(GameObject enamy in blueEnamies){
                        int enamyHp = enamy.GetComponent<Chessman>().GetHp();
                        int enamyDef = enamy.GetComponent<Chessman>().GetDef();
                        int weAtk = we.GetComponent<Chessman>().GetAtk();
                        
                        if(enamy.GetComponent<Chessman>().GetRole() == "tank"){
                            
                            for(int i=0;i<RedAtkTime;i++){
                                //atk
                                DealDamage(enamy,enamyHp,enamyDef,weAtk);
                                //
                            }

                            Debug.Log("Red Archer attack tank");

                            RedAtkTime = 0;
                            break;
                        }
                        else
                        {
                            if(enamyHp > 0 && cHp > enamyHp){
                                cHp = enamyHp;
                            }
                        }
                    }
                    if(RedAtkTime != 0){
                        foreach(GameObject enamy in blueEnamies){
                            int enamyHp = enamy.GetComponent<Chessman>().GetHp();
                            int enamyDef = enamy.GetComponent<Chessman>().GetDef();
                            int weAtk = we.GetComponent<Chessman>().GetAtk();
                            if(cHp == enamyHp){

                            for(int i=0;i<RedAtkTime;i++){
                                //atk
                                DealDamage(enamy,enamyHp,enamyDef,weAtk);
                                //
                            }

                            Debug.Log("Red Archer attack low hp");

                            RedAtkTime = 0;
                            break;
                            }
                        }
                    }
                }
                
            }
        }
        StartCoroutine(WaitArcher());
    }

    public void MageAttack(){
        attackStatusImage.sprite = mage;
        Debug.Log("MageAttack");
        List<GameObject> redEnamies = new List<GameObject>();
        List<GameObject> blueEnamies = new List<GameObject>();
        
        //Blue Attack--------------------------------------------------------------------------
        foreach(GameObject we in playerBlue){   
            if(we.GetComponent<Chessman>().GetRole() == "mage"){
                Chessman cm = we.GetComponent<Chessman>();
                Debug.Log("Blue Attack");
                int BlueAtkTime = 1;
                int x = cm.GetXBoard();
                int y = cm.GetYBoard();
                AttackZone(x + 0, y + 2,ref redEnamies,we);
                AttackZone(x + 1, y + 2,ref redEnamies,we);
                AttackZone(x + 2, y + 1,ref redEnamies,we);
                AttackZone(x + 2, y + 0,ref redEnamies,we);
                AttackZone(x + 2, y - 1,ref redEnamies,we);
                AttackZone(x - 1, y - 2,ref redEnamies,we);
                AttackZone(x + 0, y - 2,ref redEnamies,we);
                AttackZone(x - 1, y - 2,ref redEnamies,we);
                AttackZone(x - 2, y - 1,ref redEnamies,we);
                AttackZone(x - 2, y + 0,ref redEnamies,we);
                AttackZone(x - 2, y + 1,ref redEnamies,we);
                AttackZone(x - 1, y + 2,ref redEnamies,we);
                int cHp=10;
                if(redEnamies != null){
                    foreach(GameObject enamy in redEnamies){
                        int enamyHp = enamy.GetComponent<Chessman>().GetHp();
                        int enamyDef = enamy.GetComponent<Chessman>().GetDef();
                        int weAtk = cm.GetAtk();

                        if(enamy.GetComponent<Chessman>().GetRole() == "tank"){
                            int xTarget = enamy.GetComponent<Chessman>().GetXBoard();
                            int yTarget = enamy.GetComponent<Chessman>().GetYBoard();
                            //atk
                            DealDamage(enamy,enamyHp,enamyDef,weAtk);
                            //AOE
                            DealDamageAOE(we,weAtk,xTarget,yTarget);

                            Debug.Log("Blue Mage attack tank");
                            BlueAtkTime = 0;
                            break;
                        }
                        else
                        {
                            if(enamyHp > 0 && cHp > enamyHp){
                                cHp = enamyHp;
                            }
                        }
                    }
                    if(BlueAtkTime != 0){
                        foreach(GameObject enamy in redEnamies){
                            int enamyHp = enamy.GetComponent<Chessman>().GetHp();
                            int enamyDef = enamy.GetComponent<Chessman>().GetDef();
                            int weAtk = we.GetComponent<Chessman>().GetAtk();
                            if(cHp == enamyHp){
                                
                            int xTarget = enamy.GetComponent<Chessman>().GetXBoard();
                            int yTarget = enamy.GetComponent<Chessman>().GetYBoard();
                            //atk
                            DealDamage(enamy,enamyHp,enamyDef,weAtk);
                            //AOE
                            DealDamageAOE(we,weAtk,xTarget,yTarget);

                            Debug.Log("Blue Mage attack low hp");
                            BlueAtkTime = 0;
                            break;
                            }
                        }
                    }
                }
                
            }
        }

        //Red Attack --------------------------------------------------------------------------
        foreach(GameObject we in playerRed){
            if(we.GetComponent<Chessman>().GetRole() == "mage"){
                Chessman cm = we.GetComponent<Chessman>();
                Debug.Log("Red Attack");
                int RedAtkTime = 1;
                int x = cm.GetXBoard();
                int y = cm.GetYBoard();
                AttackZone(x + 0, y + 2,ref blueEnamies,we);
                AttackZone(x + 1, y + 2,ref blueEnamies,we);
                AttackZone(x + 2, y + 1,ref blueEnamies,we);
                AttackZone(x + 2, y + 0,ref blueEnamies,we);
                AttackZone(x + 2, y - 1,ref blueEnamies,we);
                AttackZone(x - 1, y - 2,ref blueEnamies,we);
                AttackZone(x + 0, y - 2,ref blueEnamies,we);
                AttackZone(x - 1, y - 2,ref blueEnamies,we);
                AttackZone(x - 2, y - 1,ref blueEnamies,we);
                AttackZone(x - 2, y + 0,ref blueEnamies,we);
                AttackZone(x - 2, y + 1,ref blueEnamies,we);
                AttackZone(x - 1, y + 2,ref blueEnamies,we);

                int cHp=10;
                if(blueEnamies != null){
                    foreach(GameObject enamy in blueEnamies){
                    int enamyHp = enamy.GetComponent<Chessman>().GetHp();
                    int enamyDef = enamy.GetComponent<Chessman>().GetDef();
                    int weAtk = we.GetComponent<Chessman>().GetAtk();
                    
                    if(enamy.GetComponent<Chessman>().GetRole() == "tank"){
                        
                        int xTarget = enamy.GetComponent<Chessman>().GetXBoard();
                        int yTarget = enamy.GetComponent<Chessman>().GetYBoard();
                        //atk
                        DealDamage(enamy,enamyHp,enamyDef,weAtk);
                        //AOE
                        DealDamageAOE(we,weAtk,xTarget,yTarget);

                        Debug.Log("Red Mage attack tank");

                        RedAtkTime = 0;
                        break;
                    }
                    else
                    {
                        if(enamyHp > 0 && cHp > enamyHp){
                            cHp = enamyHp;
                        }
                    }
                }
                    if(RedAtkTime != 0){
                        foreach(GameObject enamy in blueEnamies){
                            int enamyHp = enamy.GetComponent<Chessman>().GetHp();
                            int enamyDef = enamy.GetComponent<Chessman>().GetDef();
                            int weAtk = we.GetComponent<Chessman>().GetAtk();
                            if(cHp == enamyHp){

                                int xTarget = enamy.GetComponent<Chessman>().GetXBoard();
                                int yTarget = enamy.GetComponent<Chessman>().GetYBoard();
                                //atk
                                DealDamage(enamy,enamyHp,enamyDef,weAtk);
                                //AOE
                                DealDamageAOE(we,weAtk,xTarget,yTarget);

                                Debug.Log("Red Mage attack low hp");

                                RedAtkTime = 0;
                                break;
                            }
                        }
                    }
                }
            }
        }
        StartCoroutine(WaitMage());
        
    }

    public void HealerHeal(){
        attackStatusImage.sprite = healer;
        Debug.Log("HealerHeal");
        List<GameObject> blueAllies = new List<GameObject>();
        List<GameObject> redAllies = new List<GameObject>();
        
        //Blue Heal--------------------------------------------------------------------------
        foreach(GameObject we in playerBlue){   
            if(we.GetComponent<Chessman>().GetRole() == "healer"){
                Chessman cm = we.GetComponent<Chessman>();
                Debug.Log("Blue Heal");
                int BlueAtkTime = cm.GetAtkTime();
                int x = cm.GetXBoard();
                int y = cm.GetYBoard();
                //HealRange
                    BuffZone(x + 0, y + 1,ref blueAllies,we);
                    BuffZone(x + 0, y - 1,ref blueAllies,we);
                    BuffZone(x - 1, y + 0,ref blueAllies,we);
                    BuffZone(x - 1, y - 1,ref blueAllies,we);
                    BuffZone(x - 1, y + 1,ref blueAllies,we);
                    BuffZone(x + 1, y + 0,ref blueAllies,we);
                    BuffZone(x + 1, y - 1,ref blueAllies,we);
                    BuffZone(x + 1, y + 1,ref blueAllies,we);
                    BuffZone(x + 0, y + 2,ref blueAllies,we);
                    BuffZone(x + 1, y + 2,ref blueAllies,we);
                    BuffZone(x + 2, y + 2,ref blueAllies,we);
                    BuffZone(x + 2, y + 1,ref blueAllies,we);
                    BuffZone(x + 2, y + 0,ref blueAllies,we);
                    BuffZone(x + 2, y - 1,ref blueAllies,we);
                    BuffZone(x + 2, y - 2,ref blueAllies,we);
                    BuffZone(x - 1, y - 2,ref blueAllies,we);
                    BuffZone(x + 0, y - 2,ref blueAllies,we);
                    BuffZone(x - 1, y - 2,ref blueAllies,we);
                    BuffZone(x - 2, y - 2,ref blueAllies,we);
                    BuffZone(x - 2, y - 1,ref blueAllies,we);
                    BuffZone(x - 2, y + 0,ref blueAllies,we);
                    BuffZone(x - 2, y + 1,ref blueAllies,we);
                    BuffZone(x - 2, y + 2,ref blueAllies,we);
                    BuffZone(x - 1, y + 2,ref blueAllies,we);
                if(blueAllies != null){
                    for(int i=0;i<BlueAtkTime;i++){
                        int saveLostHp = 0;
                        foreach(GameObject ally in blueAllies){//Check allies complete
                            int allyHp = ally.GetComponent<Chessman>().GetHp();
                            int allyMaxHp = ally.GetComponent<Chessman>().GetMaxHp();
                            int lostHp = allyMaxHp - allyHp;
                            
                            if(lostHp > 0 && saveLostHp < lostHp){
                                saveLostHp = lostHp;
                            }
                        }
                        foreach(GameObject ally in blueAllies){
                            int allyHp = ally.GetComponent<Chessman>().GetHp();
                            int allyMaxHp = ally.GetComponent<Chessman>().GetMaxHp();
                            int weAtk = cm.GetAtk();
                            if(saveLostHp == allyMaxHp - allyHp && saveLostHp != 0){
                                
                                //Heal
                                Healing(ally,allyHp,allyMaxHp,weAtk);
                                //
                            
                            Debug.Log("Blue Healer Healing low hp");
                            break;
                            }
                        }
                    }
                }
                
            }
        }

        //Red Heal --------------------------------------------------------------------------
        foreach(GameObject we in playerRed){   
            if(we.GetComponent<Chessman>().GetRole() == "healer"){
                Chessman cm = we.GetComponent<Chessman>();
                Debug.Log("Red Heal");
                int RedAtkTime = cm.GetAtkTime();
                int x = cm.GetXBoard();
                int y = cm.GetYBoard();
                //HealRange
                    BuffZone(x + 0, y + 1,ref redAllies,we);
                    BuffZone(x + 0, y - 1,ref redAllies,we);
                    BuffZone(x - 1, y + 0,ref redAllies,we);
                    BuffZone(x - 1, y - 1,ref redAllies,we);
                    BuffZone(x - 1, y + 1,ref redAllies,we);
                    BuffZone(x + 1, y + 0,ref redAllies,we);
                    BuffZone(x + 1, y - 1,ref redAllies,we);
                    BuffZone(x + 1, y + 1,ref redAllies,we);
                    BuffZone(x + 0, y + 2,ref redAllies,we);
                    BuffZone(x + 1, y + 2,ref redAllies,we);
                    BuffZone(x + 2, y + 2,ref redAllies,we);
                    BuffZone(x + 2, y + 1,ref redAllies,we);
                    BuffZone(x + 2, y + 0,ref redAllies,we);
                    BuffZone(x + 2, y - 1,ref redAllies,we);
                    BuffZone(x + 2, y - 2,ref redAllies,we);
                    BuffZone(x - 1, y - 2,ref redAllies,we);
                    BuffZone(x + 0, y - 2,ref redAllies,we);
                    BuffZone(x - 1, y - 2,ref redAllies,we);
                    BuffZone(x - 2, y - 2,ref redAllies,we);
                    BuffZone(x - 2, y - 1,ref redAllies,we);
                    BuffZone(x - 2, y + 0,ref redAllies,we);
                    BuffZone(x - 2, y + 1,ref redAllies,we);
                    BuffZone(x - 2, y + 2,ref redAllies,we);
                    BuffZone(x - 1, y + 2,ref redAllies,we);

                if(redAllies != null){
                    for(int i=0;i<RedAtkTime;i++){
                        int saveLostHp = 0;
                        foreach(GameObject ally in redAllies){//Check allies complete
                            int allyHp = ally.GetComponent<Chessman>().GetHp();
                            int allyMaxHp = ally.GetComponent<Chessman>().GetMaxHp();
                            int lostHp = allyMaxHp - allyHp;

                            if(lostHp > 0 && saveLostHp < lostHp){
                                saveLostHp = lostHp;
                            }
                        }
                        foreach(GameObject ally in redAllies){
                            int allyHp = ally.GetComponent<Chessman>().GetHp();
                            int allyMaxHp = ally.GetComponent<Chessman>().GetMaxHp();
                            int weAtk = cm.GetAtk();
                            if(saveLostHp == allyMaxHp - allyHp && saveLostHp != 0){
                                
                                //Heal
                                Healing(ally,allyHp,allyMaxHp,weAtk);
                                //
                            
                            Debug.Log("Red Healer Healing low hp");
                            break;
                            }
                        }
                    }
                }
            }
        }
        StartCoroutine(WaitHealer());
    }

    public void BardBuff(){
        //Debug.Log("BardBuff");
        List<GameObject> blueAllies = new List<GameObject>();
        List<GameObject> redAllies = new List<GameObject>();
        
        //Blue Heal--------------------------------------------------------------------------
        foreach(GameObject we in playerBlue){   
            if(we.GetComponent<Chessman>().GetRole() == "bard"){
                Chessman cm = we.GetComponent<Chessman>();
                bool CheckBuffTank = false;
                bool CheckBuffKnight = false;
                bool CheckBuffMage = false;
                bool CheckBuffArcher = false;
                bool CheckBuffHealer = false;
                //Debug.Log("Blue bard buff");
                int x = cm.GetXBoard();
                int y = cm.GetYBoard();
                //BuffRange
                    BuffZone(x + 0, y + 1,ref blueAllies,we);
                    BuffZone(x + 0, y - 1,ref blueAllies,we);
                    BuffZone(x - 1, y + 0,ref blueAllies,we);
                    BuffZone(x - 1, y - 1,ref blueAllies,we);
                    BuffZone(x - 1, y + 1,ref blueAllies,we);
                    BuffZone(x + 1, y + 0,ref blueAllies,we);
                    BuffZone(x + 1, y - 1,ref blueAllies,we);
                    BuffZone(x + 1, y + 1,ref blueAllies,we);
                    BuffZone(x + 0, y + 2,ref blueAllies,we);
                    BuffZone(x + 1, y + 2,ref blueAllies,we);
                    BuffZone(x + 2, y + 1,ref blueAllies,we);
                    BuffZone(x + 2, y + 0,ref blueAllies,we);
                    BuffZone(x + 2, y - 1,ref blueAllies,we);
                    BuffZone(x - 1, y - 2,ref blueAllies,we);
                    BuffZone(x + 0, y - 2,ref blueAllies,we);
                    BuffZone(x - 1, y - 2,ref blueAllies,we);
                    BuffZone(x - 2, y - 1,ref blueAllies,we);
                    BuffZone(x - 2, y + 0,ref blueAllies,we);
                    BuffZone(x - 2, y + 1,ref blueAllies,we);
                    BuffZone(x - 1, y + 2,ref blueAllies,we);
                if(blueAllies != null){
                    foreach(GameObject ally in blueAllies){
                        Chessman aL = ally.GetComponent<Chessman>();
                        if(aL.GetRole() == "tank"){
                            aL.SetDef(3);
                            CheckBuffTank = true;
                        }
                        else if(aL.GetRole() == "knight"){
                            aL.SetAtk(3);
                            CheckBuffKnight = true;
                        }
                        else if(aL.GetRole() == "mage"){
                            aL.SetAtk(3);
                            CheckBuffMage = true;
                        }
                        else if(aL.GetRole() == "archer"){
                            aL.SetAtk(3);
                            CheckBuffArcher = true;
                        }
                        else if(aL.GetRole() == "healer"){
                        aL.SetAtk(3);
                            CheckBuffHealer = true;
                        }
                    }
                    foreach(GameObject outArea in playerBlue){
                        Chessman oA = outArea.GetComponent<Chessman>();
                        if(!CheckBuffTank && oA.GetRole() == "tank"){
                            oA.SetDef(2);
                        }
                        else if(!CheckBuffKnight && oA.GetRole() == "knight" ||
                        !CheckBuffMage && oA.GetRole() == "mage" ||
                        !CheckBuffArcher && oA.GetRole() == "archer" ||
                        !CheckBuffHealer && oA.GetRole() == "healer" )
                        {
                            oA.SetAtk(2);
                        }
                    }
                }
                
                

                //Debug.Log("Blue Bard Buff All Allies");
            }
        }

        //Red Heal --------------------------------------------------------------------------
        foreach(GameObject we in playerRed){   
            if(we.GetComponent<Chessman>().GetRole() == "bard"){
                Chessman cm = we.GetComponent<Chessman>();
                bool CheckBuffTank = false;
                bool CheckBuffKnight = false;
                bool CheckBuffMage = false;
                bool CheckBuffArcher = false;
                bool CheckBuffHealer = false;
                //Debug.Log("Red bard buff");
                int x = cm.GetXBoard();
                int y = cm.GetYBoard();
                //BuffRange
                    BuffZone(x + 0, y + 1,ref redAllies,we);
                    BuffZone(x + 0, y - 1,ref redAllies,we);
                    BuffZone(x - 1, y + 0,ref redAllies,we);
                    BuffZone(x - 1, y - 1,ref redAllies,we);
                    BuffZone(x - 1, y + 1,ref redAllies,we);
                    BuffZone(x + 1, y + 0,ref redAllies,we);
                    BuffZone(x + 1, y - 1,ref redAllies,we);
                    BuffZone(x + 1, y + 1,ref redAllies,we);
                    BuffZone(x + 0, y + 2,ref redAllies,we);
                    BuffZone(x + 1, y + 2,ref redAllies,we);
                    BuffZone(x + 2, y + 1,ref redAllies,we);
                    BuffZone(x + 2, y + 0,ref redAllies,we);
                    BuffZone(x + 2, y - 1,ref redAllies,we);
                    BuffZone(x - 1, y - 2,ref redAllies,we);
                    BuffZone(x + 0, y - 2,ref redAllies,we);
                    BuffZone(x - 1, y - 2,ref redAllies,we);
                    BuffZone(x - 2, y - 1,ref redAllies,we);
                    BuffZone(x - 2, y + 0,ref redAllies,we);
                    BuffZone(x - 2, y + 1,ref redAllies,we);
                    BuffZone(x - 1, y + 2,ref redAllies,we);
                if(redAllies != null){
                    foreach(GameObject ally in redAllies){
                        Chessman aL = ally.GetComponent<Chessman>();
                        if(aL.GetRole() == "tank"){
                            aL.SetDef(3);
                            CheckBuffTank = true;
                        }
                        else if(aL.GetRole() == "knight"){
                            aL.SetAtk(3);
                            CheckBuffKnight = true;
                        }
                        else if(aL.GetRole() == "mage"){
                            aL.SetAtk(3);
                            CheckBuffMage = true;
                        }
                        else if(aL.GetRole() == "archer"){
                            aL.SetAtk(3);
                            CheckBuffArcher = true;
                        }
                        else if(aL.GetRole() == "healer"){
                        aL.SetAtk(3);
                            CheckBuffHealer = true;
                        }
                    }
                    foreach(GameObject outArea in playerRed){
                        Chessman oA = outArea.GetComponent<Chessman>();
                        if(!CheckBuffTank && oA.GetRole() == "tank"){
                            oA.SetDef(2);
                        }
                        else if(!CheckBuffKnight && oA.GetRole() == "knight" ||
                        !CheckBuffMage && oA.GetRole() == "mage" ||
                        !CheckBuffArcher && oA.GetRole() == "archer" ||
                        !CheckBuffHealer && oA.GetRole() == "healer" )
                        {
                            oA.SetAtk(2);
                        }
                    }
                }
                
                //Debug.Log("Red Bard Buff All Allies");
            }
        }
    }

    IEnumerator ShowRound(TMP_Text textObj, float delay)
    {
        textObj.enabled = true;
        textObj.SetText("Round {0}",roundCount);
        yield return new WaitForSeconds(delay);
        textObj.enabled = false;
        roundCount++;
    }

    IEnumerator WaitSecord(float time)
    {
        yield return new WaitForSeconds(time);
        
    }
    IEnumerator WaitKnight()
    {
        yield return new WaitForSeconds(2);
        CheckDeadChess();
        if(killInRound){
            endSound.Play();
            killInRound = false;
        }
        TankAttack();
    }
    IEnumerator WaitTank()
    {
        yield return new WaitForSeconds(2);
        CheckDeadChess();
        if(killInRound){
            endSound.Play();
            killInRound = false;
        }
        ArcherAttack();
    }
    IEnumerator WaitArcher()
    {
        yield return new WaitForSeconds(2);
        CheckDeadChess();
        if(killInRound){
            endSound.Play();
            killInRound = false;
        }
        MageAttack();
        
    }
    IEnumerator WaitMage()
    {
        yield return new WaitForSeconds(2);
        CheckDeadChess();
        if(killInRound){
            endSound.Play();
            killInRound = false;
        }
        HealerHeal();
    }
    IEnumerator WaitHealer()
    {
        yield return new WaitForSeconds(2);
        combatPhaseCheck = false;
        if(!gameOver){
            StartCoroutine(ShowRound(roundText,2f));
            roundSound.Play();
        }
        ResetStat();//ResetStat ----> Set Def & Set AtkTime---------
        SaveEnergy();//SaveEnergy ----> Fill Energy-----------------
        attackStatusImage.enabled = false;
        betweenRounds = false;
    }
}
