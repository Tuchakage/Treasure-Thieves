using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class MainMenuManager : MonoBehaviourPunCallbacks, IPunObservable
{
    //Max Players in a room
    [SerializeField]
    private byte maxPlayersPerRoom = 4;

    [SerializeField]
    private InputField playerName;

    [SerializeField]
    private Button joinButton;

    [SerializeField]
    private Text players, timertxt;

    [SerializeField]
    private float timer = 60;
    // Start is called before the first frame update
    void Start()
    {
        //If not connected to the Photon Network then connect
        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings();
        //Make sure Cursor is visible
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        //If we are in a room
        if (PhotonNetwork.InRoom) 
        {
            players.text = "Players: " + PhotonNetwork.CurrentRoom.PlayerCount + " of " + PhotonNetwork.CurrentRoom.MaxPlayers;

            //Lists all the players in the room
            players.text += ":\n";
            Dictionary<int, Player> mydict = PhotonNetwork.CurrentRoom.Players;
            int i = 1;
            //Checks the names of each player
            foreach (var item in mydict)
                players.text += string.Format("{0,2}. {1}\n", (i++), item.Value.NickName);

            DisplayTime(timer);

            //If the timer variable is more than 0 
            if (timer > 0)
            {
                // Then Timer will start counting down
                timer -= Time.deltaTime;
            }
            else
            {
                //When the timer reaches 0 the game will start
                StartGame();
            }

        }
            
    }

    public override void OnConnectedToMaster()
    {
        //Once connected to Photon server...
        Debug.Log("OnConnectedToMaster was called by PUN.");
        //Allows us to create and join rooms
        PhotonNetwork.JoinLobby();

        //Gets the Players name and sets the text to the player name
        //playerName.text = PlayerPrefs.GetString("PlayerName");
    }

    public override void OnJoinedLobby()
    {
        //When we have joined a lobby 


    }

    //When you press the JoinRoom button
    public void JoinRoom()
    {

        //Sets the player name (Player name is set to PlayerPrefs)
        PlayerPrefs.SetString("PlayerName", playerName.text);
        PhotonNetwork.NickName = playerName.text;
        //Joins a random room
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        //Disable Join Button
        joinButton.gameObject.SetActive(false);
        //Make it so you can no longer input a name
        playerName.gameObject.SetActive(false);
        //Show the list of Players in the room
        players.gameObject.SetActive(true);
        //Show the Timer
        timertxt.gameObject.SetActive(true);
    }

    //Everytime A Player joins the room
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        //Check if the room is full 
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            //If room is full then make the timer to be 5 seconds
            timer = 5;
        }
    }

    //Everytime a player leaves the room
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        //Check if the room is not full 
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers - 1)
        {
            //If room is not full then reset the timer
            timer = 60;
        }
    }

    public void StartGame() 
    {
        //Go To The Actual Game
        PhotonNetwork.LoadLevel("Battle");

        //Make sure no one can join whilst game is going on
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.CurrentRoom.IsOpen = false;
    }

    void DisplayTime(float time)
    {
        //This calculates the minutes. Mathf.FloorToInt rounds them down to the largest integer value
        float minutes = Mathf.FloorToInt(time / 60);

        //This calculates the seconds e.g 62 % 60 would return the value 2 aka 1 minute and 2 seconds. 
        float seconds = Mathf.FloorToInt(time % 60);

        //string.Format allows us to place variables inside of a formatted string
        timertxt.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Oops, tried to join a room and failed. Calling CreateRoom!");

        // failed to join a random room, so create a new one
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }

    //This function allows the variables inside to be sent over the network
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //We own this player so send the other computers the data
            stream.SendNext(timer);

        }
        else
        {
            //Network player that receives the data
            timer = (float)stream.ReceiveNext();

        }
    }
}
