using Photon.Pun;
using TMPro;

public class EventJoinRoom : MonoBehaviourPunCallbacks
{
    public PhotonView photonView;
    public TextMeshProUGUI[] text;
    void Start()
    {
        photonView.RPC("sendMessageJoin", RpcTarget.All);
    }

    [PunRPC]
    private void sendMessageJoin()
    {
        int length = PhotonNetwork.PlayerList.Length;
        for (int i = 0; i < 4; i++)
        {
            if (i < length)
            {
                text[i].text = "#" + (i + 1).ToString() + " " + PhotonNetwork.PlayerList[i].NickName;
            }
            else
            {
                text[i].text = "";
            }
        }
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        photonView.RPC("sendMessageJoin", RpcTarget.All);
    }
    public void runGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("Uno");
        }
    }
}
