using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

//This script inherits from MonoBehaviourPunCallbacks instead of just MonoBehaviour
public class NetworkManager : MonoBehaviourPunCallbacks, IPunObservable
{
    //Max Players in a room
    [SerializeField]
    private byte maxPlayersPerRoom = 20;

    //Variables to connect our Network Manager to the UI elements
    [SerializeField]
    private Text nickname, status, room, players, bluescoretext, redscoretext;
    [SerializeField]
    private Button buttonPlay, buttonLeave, buttonRespawn;
    [SerializeField]
    private InputField playerName;
    [SerializeField]
    private Button blue_SpellClass, blue_WarriorClass, red_SpellClass, red_WarriorClass, redTeam, blueTeam; //Team Buttons
    [SerializeField]
    public int bluescore, redscore; //Used for the Scores

    public int teamPick = 0;

    // Game Object of player
    public GameObject player;
    //Checks if the player is alive
    public bool isAlive;

    //Prefabs for all character
    [SerializeField] private GameObject bp_SC_Prefab;
    [SerializeField] private GameObject bp_WR_Prefab;
    [SerializeField] private GameObject rp_SC_Prefab;
    [SerializeField] private GameObject rp_WR_Prefab;


    // Start is called before the first frame update
    void Start()
    {
        teamPick = 0;
        status.text = "Connecting...";
        buttonPlay.gameObject.SetActive(false);
        buttonLeave.gameObject.SetActive(false);
        playerName.gameObject.SetActive(false);
        buttonRespawn.gameObject.SetActive(false);


        redTeam.gameObject.SetActive(false);
        blueTeam.gameObject.SetActive(false);

        blue_SpellClass.gameObject.SetActive(false);
        blue_WarriorClass.gameObject.SetActive(false);
        red_SpellClass.gameObject.SetActive(false);
        red_WarriorClass.gameObject.SetActive(false);
        //Make sure the blue and red score is 0
        bluescore = 0;
        redscore = 0;
        //If not connected to the Photon Network then connect
        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings();
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.InRoom)
        {
            //Keep updating the score
            bluescoretext.text = "Blue Score: " + bluescore;
            redscoretext.text = "Red Score: " + redscore;
            nickname.text = "Hello, " + PhotonNetwork.NickName;
            room.text = "Room: " + PhotonNetwork.CurrentRoom.Name;
            players.text = "Players: " + PhotonNetwork.CurrentRoom.PlayerCount + " of " + PhotonNetwork.CurrentRoom.MaxPlayers;
            //Lists all the players in the room
            players.text += ":\n";
            Dictionary<int, Player> mydict = PhotonNetwork.CurrentRoom.Players;
            int i = 1;
            //Checks the names of each player
            foreach (var item in mydict)
                players.text += string.Format("{0,2}. {1}\n", (i++), item.Value.NickName);

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
                } else if (teamPick == 2)
                {
                    red_SpellClass.gameObject.SetActive(true);
                    red_WarriorClass.gameObject.SetActive(true);
                    blue_SpellClass.gameObject.SetActive(false);
                    blue_WarriorClass.gameObject.SetActive(false);
                }

            }

        }
        else if (PhotonNetwork.IsConnected)
        {
            nickname.text = "Type your name below and hit PLAY!";
            room.text = "Not yet in a room...";
            players.text = "Players: 0";
        }
        else
            nickname.text = room.text = players.text = "";

    }
    //When it has connected to Photon servers
    public override void OnConnectedToMaster()
    {
        //Once connected to Photon server...
        Debug.Log("OnConnectedToMaster was called by PUN.");
        status.text = "Connected to Photon.";

     
        //Do not show the Play button
        buttonPlay.gameObject.SetActive(false);
        //Show the box where players can write their own names
        playerName.gameObject.SetActive(true);
        //Makes sure the leave button doesnt show
        buttonLeave.gameObject.SetActive(false);
        //Make sure the Respawn button doesnt show
        buttonRespawn.gameObject.SetActive(false);

        //Make sure player can pick a team
        redTeam.gameObject.SetActive(true);
        blueTeam.gameObject.SetActive(true);
        blue_SpellClass.gameObject.SetActive(false);
        blue_WarriorClass.gameObject.SetActive(false);
        red_SpellClass.gameObject.SetActive(false);
        red_WarriorClass.gameObject.SetActive(false);

        //Gets the Players name and sets the text to the player name
        playerName.text = PlayerPrefs.GetString("PlayerName");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Oops, tried to join a room and failed. Calling CreateRoom!");

        // failed to join a random room, so create a new one
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }

    //When you press the Play button
    public void Play()
    {
        //Sets the player name (Player name is set to PlayerPrefs)
        PlayerPrefs.SetString("PlayerName", playerName.text);
        PhotonNetwork.NickName = playerName.text;
        //Joins a random room
        PhotonNetwork.JoinRandomRoom();
    }

    //When you press the Leave button
    public void Leave()
    {
        //Leaves the room you are in
        PhotonNetwork.LeaveRoom();
    }

    //When you have joined a room the following code will run
    public override void OnJoinedRoom()
    {
        Debug.Log("Yep, you managed to join a room!");
        status.text = "Yep, you managed to join a room!";
        //Disable THe Play button
        buttonPlay.gameObject.SetActive(false);
        //Disable The option to type in your name
        playerName.gameObject.SetActive(false);
        //Shows the leave button
        buttonLeave.gameObject.SetActive(true);

        redTeam.gameObject.SetActive(false);
        blueTeam.gameObject.SetActive(false);

        //Disable Class Changing Buttons
        blue_SpellClass.gameObject.SetActive(false);
        blue_WarriorClass.gameObject.SetActive(false);
        red_SpellClass.gameObject.SetActive(false);
        red_WarriorClass.gameObject.SetActive(false);

        //Spawn Players
        PhotonNetwork.Instantiate(player.name,
         new Vector3(Random.Range(-15, 15), 1, Random.Range(-15, 15)),
        Quaternion.Euler(0, Random.Range(-180, 180), 0)
        , 0);

        //Player Will Be Alive
        isAlive = true;

    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        status.text = newPlayer.NickName + " has just entered.";
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        status.text = otherPlayer.NickName + " has just left.";
    }


    public void blue_Team_Pick()
    {
        //Choose Blue Team
        teamPick = 1;
        //Default Pick The Spell Caster Character
        player = bp_SC_Prefab;
        //Debug.Log("I am blue team");
        //Reactivate the play so the player can play the game
        buttonPlay.gameObject.SetActive(true);
        //Blue Team Active Buttons 
        blue_SpellClass.gameObject.SetActive(true);
        blue_WarriorClass.gameObject.SetActive(true);
        red_SpellClass.gameObject.SetActive(false);
        red_WarriorClass.gameObject.SetActive(false);

    }

    public void red_Team_Pick()
    {
        //Choose Red Team
        teamPick = 2;
        //Default Pick The Spell Caster Character
        player = rp_SC_Prefab;
        //Reactivate the play so the player can play the game
        buttonPlay.gameObject.SetActive(true);
        //Debug.Log("I am red team");
        //Red Team Active Buttons 
        red_SpellClass.gameObject.SetActive(true);
        red_WarriorClass.gameObject.SetActive(true);
        blue_SpellClass.gameObject.SetActive(false);
        blue_WarriorClass.gameObject.SetActive(false);
    }

    public void pick_BP_Spell_Class()
    {
        //Change player prefab to spellcaster
        player = bp_SC_Prefab;
       //Debug.Log("Blue Team: Class type changed to Spellcaster");
    }
    public void pick_BP_Warrior_Class()
    {
        //Change player prefab to warrior
        player = bp_WR_Prefab;
        //Debug.Log("Class type changed to warrior");
    }

    public void pick_RP_Spell_Class()
    {
        //Change player prefab to spellcaster
        player = rp_SC_Prefab;
        //Debug.Log("Class type changed to spellcaster");
    }
    public void pick_RP_Warrior_Class()
    {
        //Change player prefab to warrior
        player = rp_WR_Prefab;
        //Debug.Log("Class type changed to warrior");
    }

    public void Respawn() 
    {
        //Spawn Players
        PhotonNetwork.Instantiate(player.name,
         new Vector3(Random.Range(-15, 15), 1, Random.Range(-15, 15)),
        Quaternion.Euler(0, Random.Range(-180, 180), 0)
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
    }

    //This function allows the variables inside to be sent over the network
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //We own this player so send the other computers the data
            stream.SendNext(bluescore);
            stream.SendNext(redscore);
        }
        else
        {
            //Network player that receives the data
            bluescore = (int)stream.ReceiveNext();
            redscore = (int)stream.ReceiveNext();
        }
    }
}
