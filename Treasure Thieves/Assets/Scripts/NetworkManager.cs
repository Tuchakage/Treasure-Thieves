using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

//This script inherits from MonoBehaviourPunCallbacks instead of just MonoBehaviour
public class NetworkManager : MonoBehaviourPunCallbacks
{
    //Max Players in a room
    [SerializeField]
    private byte maxPlayersPerRoom = 20;

    //Variables to connect our Network Manager to the UI elements
    [SerializeField]
    private Text nickname, status, room, players;
    [SerializeField]
    private Button buttonPlay, buttonLeave, class1, class2;
    [SerializeField]
    private InputField playerName;

    public GameObject player;

    [SerializeField] private GameObject player_class1;
    [SerializeField] private GameObject player_class2;


    // Start is called before the first frame update
    void Start()
    {
        status.text = "Connecting...";
        buttonPlay.gameObject.SetActive(false);
        buttonLeave.gameObject.SetActive(false);
        playerName.gameObject.SetActive(false);

        //If not connected to the Photon Network then connect
        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings();
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.InRoom)
        {
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
        //Show the Play button
        buttonPlay.gameObject.SetActive(true);
        //Show the box where players can write their own names
        playerName.gameObject.SetActive(true);
        //Makes sure the leave button doesnt show
        buttonLeave.gameObject.SetActive(false);

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

        //Spawn Players
        PhotonNetwork.Instantiate(player.name,
         new Vector3(Random.Range(-15, 15), 1, Random.Range(-15, 15)),
        Quaternion.Euler(0, Random.Range(-180, 180), 0)
        , 0);

    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        status.text = newPlayer.NickName + " has just entered.";
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        status.text = otherPlayer.NickName + " has just left.";
    }


    public void changeToClass1()
    {
        player = player_class1;
    }

    public void changeToClass2()
    {
        player = player_class2;
    }

}
