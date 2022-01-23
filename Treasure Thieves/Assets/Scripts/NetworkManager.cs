using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;

//This script inherits from MonoBehaviourPunCallbacks instead of just MonoBehaviour
public class NetworkManager : MonoBehaviourPunCallbacks, IPunObservable
{


    //Variables to connect our Network Manager to the UI elements

    [SerializeField] private Text nickname, status, room, players, bluescoretext, redscoretext, bluewinnertext, redwinnertext;

    [SerializeField] private Button buttonLeave, buttonRespawn;

    [SerializeField] private Button blue_SpellClass, blue_WarriorClass, red_SpellClass, red_WarriorClass, redTeam, blueTeam; //Team Buttons
    public int bluescore, redscore, bluePlayerCount, redPlayerCount, maxInTeam;
    public int teamPick = 0;
    public bool win = false; // Used to identify if there is a winner
    bool functionCalledOnce = false; //Makes it so the function is only called once (Used for End game function)

    // Game Object of player
    public GameObject player;
    //Checks if the player is alive
    public bool isAlive;

    Vector3 spawnLocation; //Set the spawn location
    Quaternion spawnRotation; //Set Spawn Rotation
    //Prefabs for all character
    [SerializeField] private GameObject bp_SC_Prefab;
    [SerializeField] private GameObject bp_WR_Prefab;
    [SerializeField] private GameObject rp_SC_Prefab;
    [SerializeField] private GameObject rp_WR_Prefab;

    [SerializeField] private GameObject spellProgressBar1;
    [SerializeField] private GameObject KKProgressBar1;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        functionCalledOnce = false;
        teamPick = 0;
        //Make sure the blue and red score is 0
        bluescore = 0;
        redscore = 0;
        bluePlayerCount = 0;
        redPlayerCount = 0;
        //Make sure the winner Text is empty
        bluewinnertext.text = " ";
        redwinnertext.text = " ";

        //So the Respawn button doesnt show up when the game starts
        isAlive = true;

    }

    // Update is called once per frame
    void Update()
    {
        //Keep updating the score
        bluescoretext.text = "Blue Score: " + bluescore;
        redscoretext.text = "Red Score: " + redscore;

        //If the player health is 0
        if (!isAlive)
        {
            buttonRespawn.gameObject.SetActive(true);

            //Respawn Option to change classes
            if (teamPick == 1)
            {
                blue_SpellClass.gameObject.SetActive(true);
                blue_WarriorClass.gameObject.SetActive(true);
                red_SpellClass.gameObject.SetActive(false);
                red_WarriorClass.gameObject.SetActive(false);
            }
            else if (teamPick == 2)
            {
                red_SpellClass.gameObject.SetActive(true);
                red_WarriorClass.gameObject.SetActive(true);
                blue_SpellClass.gameObject.SetActive(false);
                blue_WarriorClass.gameObject.SetActive(false);
            }

        }

        //Keep checking the score
        CheckScore();
        //If there is a winner then End the game
        if (win && !functionCalledOnce)
        {
            EndGame();
            functionCalledOnce = true;
        }

        
    }

    //When you press the Leave button
    public void Leave()
    {
        //Leaves the room you are in
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        //Restart the original scene
        SceneManager.LoadScene(0);
    }
    private void EndGame()
    {
        //Disable The Room
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.DestroyAll();
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.AutomaticallySyncScene = false;
        }
        //Wait X seconds and then return to the main menu
        StartCoroutine(End(3f));
    }

    private IEnumerator End(float p_wait)
    {
        yield return new WaitForSeconds(p_wait);
        //Disconnect
        
        PhotonNetwork.LeaveRoom();
    }

    //Checks to see who the winner is
    void CheckScore()
    {
        if (bluescore == 3)
        {
            win = true;
            bluewinnertext.gameObject.SetActive(true);
            bluewinnertext.text = "BLUE TEAM WINS";
            buttonLeave.gameObject.SetActive(false);
        }
        else if (redscore == 3)
        {
            win = true;
            redwinnertext.gameObject.SetActive(true);
            redwinnertext.text = "RED TEAM WINS";
            buttonLeave.gameObject.SetActive(false);
        }
    }

    public void blue_Team_Pick()
    {
        //Dont do anything if the team is full
        if (bluePlayerCount == maxInTeam)
            return;
        //Choose Blue Team
        teamPick = 1;
        //Increase the amount of players in the team
        photonView.RPC("IncreaseBluePlayerCount", RpcTarget.All);
        //Default Pick The Spell Caster Character
        player = bp_SC_Prefab;
        //Debug.Log("I am blue team");
        //Blue Team Active Buttons 
        blue_SpellClass.gameObject.SetActive(true);
        blue_WarriorClass.gameObject.SetActive(true);
        red_SpellClass.gameObject.SetActive(false);
        red_WarriorClass.gameObject.SetActive(false);
        //Disable Team Picking
        redTeam.gameObject.SetActive(false);
        blueTeam.gameObject.SetActive(false);
        //Spawn Location For Blue Team
        spawnLocation = new Vector3(33.0f, 2.7f, 30.0f);
        //Spawn Rotation For Blue Team
        spawnRotation = Quaternion.Euler(0, -90, 0);
        //Show The Score Of The Game
        bluescoretext.gameObject.SetActive(true);
        redscoretext.gameObject.SetActive(true);

    }

    public void red_Team_Pick()
    {
        //Dont do anything if the team is full
        if (redPlayerCount == maxInTeam)
            return;

        //Choose Red Team
        teamPick = 2;
        //Increase the amount of players in the team
        photonView.RPC("IncreaseRedPlayerCount", RpcTarget.All);
        //Default Pick The Spell Caster Character
        player = rp_SC_Prefab;
        //Debug.Log("I am red team");
        //Red Team Active Buttons 
        red_SpellClass.gameObject.SetActive(true);
        red_WarriorClass.gameObject.SetActive(true);
        blue_SpellClass.gameObject.SetActive(false);
        blue_WarriorClass.gameObject.SetActive(false);
        //Disable Team Picking
        redTeam.gameObject.SetActive(false);
        blueTeam.gameObject.SetActive(false);
        //Spawn Location For Red Team
        spawnLocation = new Vector3(-33.0f, 2.7f, -30.0f);
        //Spawn Rotation For Red Team
        spawnRotation = Quaternion.Euler(0, 90, 0);
        //Show The Score Of The Game
        bluescoretext.gameObject.SetActive(true);
        redscoretext.gameObject.SetActive(true);
    }

    public void pick_BP_Spell_Class()
    {
        //Change player prefab to spellcaster
        player = bp_SC_Prefab;
        spellProgressBar1.SetActive(true);
        ProgressBar spellBar = spellProgressBar1.gameObject.GetComponent<ProgressBar>();
        //Spawns In Player and assigns the Progress bar to them
        spellBar.spell = Respawn().GetComponent<Spellcaster>();
        KKProgressBar1.SetActive(false);
        //Debug.Log("Blue Team: Class type changed to Spellcaster");
    }
    public void pick_BP_Warrior_Class()
    {
        //Change player prefab to warrior
        player = bp_WR_Prefab;
 
        KKProgressBar1.SetActive(true);
        KKProgressBar karateBar = KKProgressBar1.gameObject.GetComponent<KKProgressBar>();
        //Spawns In Player and assigns the Progress bar to them
        karateBar.basicattack = Respawn().GetComponent<KarateKid>();
        spellProgressBar1.SetActive(false);
        //Debug.Log("Class type changed to warrior");
    }

    public void pick_RP_Spell_Class()
    {
        //Change player prefab to spellcaster
        player = rp_SC_Prefab;
        spellProgressBar1.SetActive(true);
        ProgressBar spellBar = spellProgressBar1.gameObject.GetComponent<ProgressBar>();
        //Spawns In Player and assigns the Progress bar to them
        spellBar.spell = Respawn().GetComponent<Spellcaster>();
        KKProgressBar1.SetActive(false);
        //Debug.Log("Class type changed to spellcaster");
    }
    public void pick_RP_Warrior_Class()
    {
        //Change player prefab to warrior
        player = rp_WR_Prefab;
        KKProgressBar1.SetActive(true);
        KKProgressBar karateBar = KKProgressBar1.gameObject.GetComponent<KKProgressBar>();
        //Spawns In Player and assigns the Progress bar to them
        karateBar.basicattack = Respawn().GetComponent<KarateKid>();
        spellProgressBar1.SetActive(false);
        //Debug.Log("Class type changed to warrior");
    }

    public GameObject Respawn()
    {
        //Spawn Players
        GameObject nP = PhotonNetwork.Instantiate(player.name,
         new Vector3(spawnLocation.x, spawnLocation.y, spawnLocation.z),
        Quaternion.Euler(0, spawnRotation.y, 0)
        , 0);
        //Player Will Be Alive
        isAlive = true;

        //Disable The Respawn Button
        buttonRespawn.gameObject.SetActive(false);

        //Disable Classes
        blue_SpellClass.gameObject.SetActive(false);
        blue_WarriorClass.gameObject.SetActive(false);
        red_SpellClass.gameObject.SetActive(false);
        red_WarriorClass.gameObject.SetActive(false);
        return nP;
    }

    [PunRPC]
    void IncreaseBluePlayerCount() 
    {
        bluePlayerCount++;
    }

    [PunRPC]
    void IncreaseRedPlayerCount()
    {
        redPlayerCount++;
    }

    //This function allows the variables inside to be sent over the network
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //We own this player so send the other computers the data
            stream.SendNext(bluescore);
            stream.SendNext(redscore);
            stream.SendNext(win);
        }
        else
        {
            //Network player that receives the data
            bluescore = (int)stream.ReceiveNext();
            redscore = (int)stream.ReceiveNext();
            win = (bool)stream.ReceiveNext();
        }
    }
}
