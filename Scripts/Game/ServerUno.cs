using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using TMPro;

public class ServerUno : MonoBehaviour
{
    [Header("Card")]
    public List<GameObject> cardYellow; // object thẻ vàng
    public List<GameObject> cardRed; // object thẻ đỏ
    public List<GameObject> cardBlue; // object thẻ xanh nước biển
    public List<GameObject> cardGreen; // object thẻ xanh lá
    public List<GameObject> cardWILD; // thẻ đen
    [Header("Choice Card")]
    public GameObject PL4;
    public GameObject COL;
    public List<GameObject> cardBlackPL4; // 
    public List<GameObject> cardBlackCol; // 
    List<List<GameObject>> allCard; // tất cả object thẻ
    List<Transform> objCard; // tất cả đối tượng thẻ đã được tạo
    List<Players> m_Players; // danh sách người chơi
    PhotonView photonView;
    Dictionary<string, int> indexPlayer; // key: nickname | value : index
    Desk desk;
    int countCard = 0;
    Card lastCard = new Card("-1", "-1");
    int playerOrder = 0;
    int zCard = 0;
    bool isReverse = false;
    bool choiceCardBlack = false;
    bool choiceCardBlack1 = false;
    Players playersClone = new Players();
    string nicknameClone = "";
    int indexCardClone = -1;
    public List<Material> materialColor;
    public GameObject circle;
    [Header("avatar")]
    public List<GameObject> avt;
    public List<TextMeshProUGUI> txtNickname;
    public GameObject arrowNotice;
    public GameObject buttonSkip;
    private void Start()
    {
        photonView= GetComponent<PhotonView>();
        setIndexPlayer(); // lấy cái tọa độ của người chơi
        showAvatarAndNickname();
        getAllCardObj(); // lấy obj bài
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(showAllCard());
        }        
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            checkCard();
            if (choiceCardBlack)
            {
                setChoice();
            }
            else if (choiceCardBlack1)
            {
                setChoice1();
            }
            else if (arrowNotice.active)
            {
                photonView.RPC("moveCard", RpcTarget.All, PhotonNetwork.NickName,false);
                arrowNotice.SetActive(false);
                
            }
        }

    }
    private void showAvatarAndNickname()
    {
        int count = 0;
        foreach(var i in indexPlayer)
        {
            avt[count].SetActive(true);
            if (count != 0)
            {
                txtNickname[count].text = i.Key;
            }          
            count++;
        }
    }
    [PunRPC]
    private void setColorCircle()
    {
        List<string> colors = new List<string>();
        colors.Add("RED");
        colors.Add("YELLOW");
        colors.Add("BLUE");
        colors.Add("GREEN");
        int count = 0;
        Debug.Log("Có chạy vào set color");
        foreach(string i in colors)
        {
            if (i.Equals(lastCard.color))
            {
                circle.GetComponent<Renderer>().material = materialColor[count];
                return;
            }
            count++;
        }
    }
    private void checkCard()
    {
        try
        {
            foreach (Transform i in objCard)
            {
                ClickCard click = i.GetComponent<ClickCard>();
                if (click.indexChoice != -1)
                {
                    photonView.RPC("choiceCard", RpcTarget.All, PhotonNetwork.NickName, click.indexChoice);
                    click.indexChoice = -1;
                }
            }
        }
        catch(Exception e)
        {
            Debug.LogException(e);
        }
        
    }
    [PunRPC]
    private void choiceCard(string nickname,int indexCard)
    {
        foreach(Players i in m_Players)
        {
            if (i.nickname.Equals(nickname))
            {
                lastCard = i.getCardByIndex(indexCard);
                processCard(i,nickname, indexCard);
                return;
            }
        }
    }
    private IEnumerator moveCardCenter(Players i ,string nickname,int indexCard) 
    {
        int index = getIndexPlayer(nickname);
        if (index == 0)
        {
            i.cardObj[indexCard].rotation = Quaternion.Euler(new Vector3(180, 180, UnityEngine.Random.Range(-30, 30)));
        }
        else if (index == 1)
        {
            i.cardObj[indexCard].rotation = Quaternion.Euler(new Vector3(180, 180, UnityEngine.Random.Range(60, 120)));
        }
        else if (index == 2)
        {
            i.cardObj[indexCard].rotation = Quaternion.Euler(new Vector3(0, 0, UnityEngine.Random.Range(-30, 30)));
        }
        else if (index == 3)
        {
            i.cardObj[indexCard].rotation = Quaternion.Euler(new Vector3(0, 0, UnityEngine.Random.Range(60, 120)));
        }
        animationCard x = i.cardObj[indexCard].GetComponent<animationCard>(); // lấy obj 
        x.vectorTaget = new Vector3(0, 1, 8 - zCard * 0.005f); // di chuyển lá bài đến vecto
        x.isMove = true; // mở khóa di chuyển lá bài
        while (x.isMove)
        {
            yield return null;
        }
        discardAndRemoveCard(nickname, indexCard);
    }
    private void discardAndRemoveCard(string nickname,int indexCard)
    {
        foreach (Players i in m_Players)
        {
            if (i.nickname.Equals(nickname))
            {
                i.removeCardByIndex(indexCard);
                checkWin(nickname);
                showAllCard(nickname);
                zCard++;
                if (PhotonNetwork.IsMasterClient)
                {
                    photonView.RPC("FinishDiscard", RpcTarget.MasterClient);
                }
                break;
            }
        }
    }
    
    private void changeArrrowCircle()
    {
        Vector3 pos;
        Vector3 rot;
        if (isReverse)
        {
            pos = new Vector3(-15.9f, 2.3f, 19f);
            rot = new Vector3(-96f,257f,-268f);          
        }
        else
        {
            pos = new Vector3(-16.8f, 0.1f, 19f);
            rot = new Vector3(-264f, 258f, -113f);
        }
        circle.transform.position = pos;
        circle.transform.rotation = Quaternion.Euler(rot);
    }
    private void processCard(Players m_player,string nickname,int indexCard)
    {
        if (lastCard.value.Equals("SKI"))
        {
            Debug.Log("SKIP");
            playerOrderHandle(2);
            StartCoroutine(moveCardCenter(m_player, nickname, indexCard));
            
        }
        else if (lastCard.value.Equals("REV"))
        {
            isReverse = !isReverse;
            changeArrrowCircle();
            playerOrderHandle(1);
            StartCoroutine(moveCardCenter(m_player, nickname, indexCard));

        }
        else if (lastCard.value.Equals("PL2"))
        {
            
            if (PhotonNetwork.IsMasterClient)
            {
                string s = getNicknameNext(nickname);
                photonView.RPC("moveCard", RpcTarget.All, s,true);
                photonView.RPC("moveCard", RpcTarget.All, s, true);
            }
            playerOrderHandle(2);
            StartCoroutine(moveCardCenter(m_player, nickname, indexCard));

        }
        else if (lastCard.value.Equals("PL4")) 
        {
            playersClone = m_player;
            nicknameClone = nickname;
            indexCardClone = indexCard;
            if (PhotonNetwork.NickName.Equals(nickname))
            {
                PL4.SetActive(true);
                choiceCardBlack = true;
                if (PhotonNetwork.IsMasterClient)
                {
                    string s = getNicknameNext(nickname);
                    photonView.RPC("moveCard", RpcTarget.All, s, true);
                    photonView.RPC("moveCard", RpcTarget.All, s, true);
                    photonView.RPC("moveCard", RpcTarget.All, s, true);
                    photonView.RPC("moveCard", RpcTarget.All, s, true);
                }
            }
        }
        else if (lastCard.value.Equals("COL"))
        {
            playersClone = m_player;
            nicknameClone = nickname;
            indexCardClone = indexCard;
            if (PhotonNetwork.NickName.Equals(nickname))
            {
                COL.SetActive(true);
                choiceCardBlack1 = true;

            }
        }
        // truong hop bai den nua
        else
        {
            playerOrderHandle(1);
            StartCoroutine(moveCardCenter(m_player, nickname, indexCard));
        }
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("setColorCircle", RpcTarget.All);
        }  

    }
    private bool checkWin(string nickname)
    {
        foreach(Players i in m_Players)
        {
            if (PhotonNetwork.NickName.Equals(nickname))
            {
                if (i.handList.Count == 0)
                {
                    return true;
                }
            }
                                           
        }
        return false;
    }
    public void skip()
    {
        showAllCard(PhotonNetwork.NickName);
        buttonSkip.SetActive(false);
        photonView.RPC("addPlayerOrder",RpcTarget.All);
    }
    [PunRPC]
    private void addPlayerOrder()
    {
        playerOrderHandle(1);
        FinishDiscard();
    }

    private void setChoice()
    {
        int count = 0;
        List<string> colors = new List<string>();
        colors.Add("RED");
        colors.Add("GREEN");
        colors.Add("YELLOW");
        colors.Add("BLUE");
        foreach (GameObject i in cardBlackPL4)
        {
            ChoiceAddd4 x = i.GetComponent<ChoiceAddd4>();
            if (x.isChoice)
            {
                x.isChoice = false;
                PL4.SetActive(false);
                choiceCardBlack = !choiceCardBlack;
                photonView.RPC("PL4m", RpcTarget.All, colors[count]);
                return;
            }
            count++;
        }
    }
    [PunRPC]
    private void PL4m(string color)
    {
        lastCard.color = color;
        StartCoroutine(moveCardCenter(playersClone, nicknameClone, indexCardClone));
        playerOrderHandle(2);
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("setColorCircle", RpcTarget.All);
        }
        
    }
    [PunRPC]
    private void COLm(string color)
    {
        lastCard.color = color;
        StartCoroutine(moveCardCenter(playersClone, nicknameClone, indexCardClone));
        playerOrderHandle(1);
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("setColorCircle", RpcTarget.All);
        }
    }
    private void setChoice1()
    {
        int count = 0;
        List<string> colors = new List<string>();
        colors.Add("RED");
        colors.Add("GREEN");
        colors.Add("YELLOW");
        colors.Add("BLUE");
        foreach (GameObject i in cardBlackCol)
        {
            ChoiceAddd4 x = i.GetComponent<ChoiceAddd4>();
            if (x.isChoice)
            {
                x.isChoice = false;
                COL.SetActive(false);
                choiceCardBlack1 = !choiceCardBlack1;
                photonView.RPC("COLm", RpcTarget.All, colors[count]);
                return;
            }
            count++;
        }
    }
    private string getNicknameNext(string nickname)
    {
        int index = getIndexPlayer(nickname);
        if (isReverse)
        {
            index -= 1;
        }
        else
        {
            index += 1;
        }
        if (index == -1)
        {
            index = m_Players.Count - 1;
        }
        if (index == m_Players.Count)
        {
            index = 0;
        }
        Debug.Log(index);
        foreach(var i in indexPlayer)
        {
            if (i.Value == index)
            {
                return i.Key;
            }
        }
        return null;
    }
    private void playerOrderHandle(int x)
    {
        if (isReverse)
        {
            playerOrder -= x;
        }
        else
        {
            playerOrder += x;
        }
        if (playerOrder < 0)
        {
            playerOrder += m_Players.Count;
        }
    }
    private IEnumerator MasterDiscard()
    {
        for (int i = 0; i < 7; i++)
        {
            foreach (var item in indexPlayer)
            {
                photonView.RPC("moveCard", RpcTarget.All, item.Key, true);
                yield return new WaitForSeconds(0.2f);
            }
        }
        yield return new WaitForSeconds(1f);
        photonView.RPC("FinishDiscard",RpcTarget.MasterClient);
    }
    [PunRPC]
    void FinishDiscard() 
    {
        photonView.RPC("MasterSendPlayerOrder",RpcTarget.All,m_Players[playerOrder % m_Players.Count].nickname,lastCard.color,lastCard.value);
    }
    [PunRPC]
    private void MasterSendPlayerOrder(string nickname,string color,string value)
    {
        if (PhotonNetwork.NickName.Equals(nickname))
        {
            playCard(color, value,true);           
        }        
    }
    private void playCard(string color,string value, bool check = true)
    {
        Card card = new Card(color,value);
        for (int i = 0; i < m_Players.Count; i++)
        {
            if (m_Players[i].nickname.Equals(PhotonNetwork.NickName))
            {
                if (m_Players[i].listCardValid(card).Count == 0 && check)
                {
                    arrowNotice.SetActive(true);
                    m_Players[i].listCardValidUI(card);
                }
                else if (m_Players[i].listCardValid(card).Count == 0 && !check)
                {
                    buttonSkip.SetActive(true);
                    m_Players[i].listCardValidUI(card);
                }
                else
                {
                    m_Players[i].listCardValidUI(card);
                }               
                return;
            }
        }
        
    }
    private void setIndexPlayer()
    {       
        string nickname = PhotonNetwork.NickName;
        m_Players = new List<Players>();
        indexPlayer = new Dictionary<string, int>();
        int count = 0;
        int index = 0;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (nickname.Equals(player.NickName))
            {
                index = count;
                break;
            }
            count++;
        }
        count = 0;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Players s = new Players();
            s.nickname = player.NickName;
            s.scores = 0;
            m_Players.Add(s);
            if (count < index)
            {
                indexPlayer[player.NickName] = count + index;
            }
            else
            {
                indexPlayer[player.NickName] = count - index;
            }
            count++;       
        }
    }  
    private void getAllCardObj()
    {
        // vàng -> đỏ -> xanh nước biển -> xanh lá
        allCard = new List<List<GameObject>>();
        allCard.Add(cardRed);
        allCard.Add(cardGreen);
        allCard.Add(cardBlue);
        allCard.Add(cardYellow);
        allCard.Add(cardWILD);
    }
    private IEnumerator showAllCard()
    {
        desk = new Desk();
        float count = 0;
        for (int i = desk.cards.Count - 1; i >= 0; i--)
        {
            photonView.RPC("drawCard", RpcTarget.All, desk.cards[i].color, desk.cards[i].value, count);
            count += 0.005f;
            yield return new WaitForSeconds(0.01f);
        }
        Debug.Log("Đã gửi xong showAllCard");

        photonView.RPC("getTransformCardObj", RpcTarget.All);   
        
    }  
    private GameObject convertCardToCardObj(Card card)
    {
        // đây là lá đen
        if (card.color.Equals("WILD"))
        {
            if (card.value.Equals("COL"))
            {
                return allCard[4][1];
            }
            else
            {
                return allCard[4][0];
            }
        }
        // đây là 4 lá màu
        List<string> colors = new List<string>();
        List<string> values = new List<string>();
        colors.AddRange(new string[] { "RED", "GREEN", "BLUE", "YELLOW" });
        values.AddRange(new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "PL2", "SKI", "REV" });
        for (int i = 0; i < colors.Count; i++)
        {
            for (int j = 0; j < values.Count; j++)
            {
                if (card.color.Equals(colors[i]) && card.value.Equals(values[j]))
                {
                    return allCard[i][j];
                }
            }
        }
        return null;
    } 
    [PunRPC]
    private void drawCard(string color, string value, float count)
    {
        GameObject.Instantiate(convertCardToCardObj(new Card(color, value)), new Vector3(8 - count, 1 - count, 8 - count), Quaternion.Euler(new Vector3(180, 0, 0)), transform);
    }
    [PunRPC]
    private void getTransformCardObj()
    {
        objCard = new List<Transform>();
        foreach (Transform i in gameObject.transform)
        {
            i.AddComponent<animationCard>();
            i.AddComponent<BoxCollider>();
            i.AddComponent<ClickCard>();
            objCard.Insert(0, i);
        }
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(MasterDiscard());
        }
    }
    [PunRPC]
    private IEnumerator moveCard(string nickname,bool check = true) // đưa lá bài từ giữa bàn đến nickname
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Card card = desk.disCard();
            photonView.RPC("disCardByNickname", RpcTarget.All, nickname, card.color, card.value);// phát bài cho người chơi
            photonView.RPC("addGameObjByNickname", RpcTarget.All, nickname, countCard); // lấy transform của đối tượng bài trên UI  
        }
        int index = getIndexPlayer(nickname);
        if (index == 0)
        {
            objCard[countCard].rotation = Quaternion.Euler(new Vector3(180, 180, 0));         
        }
        else if (index == 1)
        {
            objCard[countCard].rotation = Quaternion.Euler(new Vector3(180, 0, 90));
        }
        else if (index == 2)
        {
            objCard[countCard].rotation = Quaternion.Euler(new Vector3(180, 0, 180));
        }
        else if (index == 3)
        {
            objCard[countCard].rotation = Quaternion.Euler(new Vector3(180, 0, -90));
        }

        animationCard x = objCard[countCard].GetComponent<animationCard>(); // lấy obj 
        x.vectorTaget = getPosition(nickname); // di chuyển lá bài đến vecto
        countCard++; // tăng số lá bài đã được lấy
        x.isMove = true; // mở khóa di chuyển lá bài
        while (x.isMove)
        {
            yield return null;
        }
        showAllCard(nickname);
        if (!check && nickname.Equals(PhotonNetwork.NickName))
        {
            playCard(lastCard.color, lastCard.value,false);
        }
    }
    private void showAllCard(string nickname)
    {
        List<Transform> cards = getListTransformByNickName(nickname);   
        float count = 0;
        int index = getIndexPlayer(nickname);
        var x = getDistance(cards.Count,index);
        float distance = x.Item1;
        float v = x.Item2;
        for (int i=0;i<cards.Count;i++)
        {
            if (index == 0)
            {
                cards[i].position = new Vector3(v, -7, 8 - count);
            }
            if (index == 2)
            {
                cards[i].position = new Vector3(v, 9, 8 - count);
            }
            if (index == 1)
            {
                cards[i].position = new Vector3(-16f, v + 1, 8 - count);
            }
            if (index == 3)
            {
                cards[i].position = new Vector3(16f, v + 1, 8 - count);
            }
            count += 0.001f;
            v += distance;
        }
    }
    private List<Transform> getListTransformByNickName(string nickname)
    {
        for (int i = 0; i < m_Players.Count; i++)
        {
            if (m_Players[i].nickname.Equals(nickname))
            {   
                return m_Players[i].cardObj;
            }
        }
        return null;
    }   
    [PunRPC]
    private void disCardByNickname(string nickname,string color,string value)
    {
        for(int i=0;i<m_Players.Count;i++)
        {
            if (m_Players[i].nickname.Equals(nickname))
            {
                m_Players[i].addCard(new Card(color,value));
                return;
            }
        }
    }   
    [PunRPC]
    private void addGameObjByNickname(string nickname, int countCard)
    {
        for (int i = 0; i < m_Players.Count; i++)
        {
            if (m_Players[i].nickname.Equals(nickname))
            {
                m_Players[i].cardObj.Add(objCard[countCard]);
                return;
            }
        }
    }   
    private Tuple<float,float> getDistance(int count,int index)
    {
        float distance = 3; 
        float x = 0;
        float v = -9f;
        if (index == 1 || index == 3) 
        {
            v = -3f;
        }
        if (count % 2 == 1)
        {
            x = -((count - 1 ) / 2 ) *distance;
            if (x < v)
            {
                x = v;
                distance = (x) / (-((count - 1) / 2));
            }
        }
        else
        {
            x = -((count / 2) - 0.5f) * distance;
            if (x < v)
            {
                x = v;
                distance = (x) / (-((count / 2) - 0.5f));
            }
        }
        return new Tuple<float,float>(distance, x);
    }
    private Vector3 getPosition(string nickname)
    {
        Players player = getPlayerByNickname(nickname);
        int countCard = player.getCountCards();  
        int index = getIndexPlayer(nickname);
        var x = getDistance(countCard, index);
        if (index == 0)
        {
            return new Vector3(-x.Item2, -7, 8);
        }
        else if (index == 2)
        {
            return new Vector3(-x.Item2, 9, 8);
        }
        else if (index == 1)
        {
            return new Vector3(-16f, x.Item2 + 1, 8);
        }
        else if (index == 3)
        {
            return new Vector3(16f, x.Item2 + 1, 8);
        }
        return new Vector3();
    }
    private Players getPlayerByNickname(string nickname)
    {
        for (int i=0;i<m_Players.Count;i++) 
        { 
            if (nickname.Equals(m_Players[i].nickname))
            {
                return m_Players[i];
            }
        }
        return null;
    }
    private int getIndexPlayer(string nickname)
    {
        return indexPlayer[nickname];
    }
}

