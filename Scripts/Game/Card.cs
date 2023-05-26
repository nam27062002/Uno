using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    public string color { get; set; }
    public string value { get; set; }

    public Card(string color, string value)
    {
        this.color = color;
        this.value = value;
    }
}
