using System.Collections.Generic;
using UnityEngine;

public class Players
{
    public List<Card> handList;
    public string nickname;
    public int scores;
    public List<Transform> cardObj;
    public Players()
    {
        handList = new List<Card>();
        cardObj = new List<Transform>();
    } 
    public void addCard(Card card)
    {
        handList.Add(card);
    }
    public void removeCardByIndex(int index)
    {
        handList.RemoveAt(index);
        cardObj.RemoveAt(index);
    }
    public void showAllCards()
    {
        foreach (Card card in handList)
        {
            Debug.Log(card.color + " " + card.value);    
        }
    }
    public int getCountCards()
    {
        return handList.Count;
    }
    public Card getCardByIndex(int index)
    {
        return handList[index];
    }
    public Vector3 getVectorObj(int index)
    {
        return cardObj[index].position;
    }
    public void listCardValidUI(Card card)
    {
        List<int> list = listCardValid(card);
        foreach (int i in list)
        {
            ClickCard x = cardObj[i].GetComponent<ClickCard>(); // lấy obj 
            x.listTransform = cardObj;
            Vector3 v = cardObj[i].position;
            v.y = -6f;
            cardObj[i].position = v;
        }
    }
    public List<int> listCardValid(Card card)
    {
        List<int> list = new List<int>();
        int count = 0;
        foreach (Card i in handList)
        {
            if (i.color.Equals(card.color) || i.value.Equals(card.value) || i.value.Equals("COL") || i.value.Equals("PL4") || (card.color.Equals("-1")))
            {
                list.Add(count);
            }
            count++;
        }
        return list;    
    }
}
