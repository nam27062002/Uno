using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Desk
{
    public List<Card> cards;
    public List<Card> cards_disc;
    public Desk()
    {
        cards = new List<Card>();
        cards_disc = new List<Card>();
        build();
        shuffle();
    }
    public void showAllCard() // test show bài
    {
        foreach (Card card in cards)
        {
            Debug.Log(card.color + " " + card.value);
        }
    }
    private void build()
    {
        List<string> colors = new List<string>();
        List<string> func = new List<string>();
        List<string> spe = new List<string>();
        colors.AddRange(new string[] { "RED", "GREEN", "BLUE", "YELLOW" });
        func.AddRange(new string[] { "SKI", "REV", "PL2" });
        spe.AddRange(new string[] { "COL", "PL4" });
        foreach (string color in colors)
        {
            cards.Add(new Card(color, 0.ToString()));
            for (int i = 1; i < 10; i++)
            {
                cards.Add(new Card(color, i.ToString()));
                cards.Add(new Card(color, i.ToString()));
            }
            foreach (string i in func)
            {
                cards.Add(new Card(color, i));
                cards.Add(new Card(color, i));
            }
            foreach (string i in spe)
            {
                cards.Add(new Card("WILD", i));
            }
        }

    }// xây dựng bộ bài
    private void shuffle() // xáo bài
    {
        RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
        int n = cards.Count;
        while (n > 1)
        {
            byte[] box = new byte[1];
            do provider.GetBytes(box);
            while (!(box[0] < n * (Byte.MaxValue / n)));
            int k = (box[0] % n);
            n--;
            Card value = cards[k];
            cards[k] = cards[n];
            cards[n] = value;
        }
    }
    public Card disCard()
    {
        Card card = this.cards[0];
        this.cards.RemoveAt(0); // xóa phần tử đầu tiên ở cards
        cards_disc.Add(card); // thêm cái vừa xóa vào cards disc
        return card;
    }
}
