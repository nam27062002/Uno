using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class JoinRoom : MonoBehaviourPunCallbacks
{
    public InputField nickname;
    private string IdRoom = "1";
    public void Start()
    {
        nickname.Select();
        TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, true);
    }
    public void Join()
    {
        PhotonNetwork.NickName = nickname.text;
        PhotonNetwork.JoinOrCreateRoom(IdRoom, new RoomOptions { MaxPlayers = 4 }, TypedLobby.Default);
    }
    public override void OnJoinedRoom()
    {
        PhotonNetwork.NickName = nickname.text;
        PhotonNetwork.LoadLevel("ListPlayer");
    }

}
