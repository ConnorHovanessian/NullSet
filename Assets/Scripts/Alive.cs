using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alive : MonoBehaviour
{
    private int MAX_INVENTORY_SIZE = 64;

    public int health;
    public int maxHealth;
    public int guns;
    public int stonks;
    public Vector3Int location;
    public int range;
    public Item[] inventory;

    public Color DefaultColor;
    public Color OutOfRangeColor;
    public Color InRangeColor;

    GameObject Player;
    Map Map;

    private void Awake()
    {
        health = 40;
        maxHealth = 40;
        guns = 10;
        range = 2;
        stonks = 10;
        inventory = new Item[MAX_INVENTORY_SIZE];
        //Item testItem = new Item("Test Item", 12, Map.Instance.TestItem);
        //inventory[0].GetComponent<Item>().SetItemValues("Test Item", 12, Map.Instance.TestItem);
    }

    void Start()
    {
        DefaultColor = gameObject.GetComponent<SpriteRenderer>().color;
        OutOfRangeColor = (DefaultColor + new Color(1, 1, 1, 1f))/2;
        InRangeColor = (DefaultColor + new Color(1, 0, 0, 1f))/2;
        Player = GameObject.Find("Player");
        Map = GameObject.Find("Map").GetComponent<Map>();
    }
    void OnMouseOver()
    {
        if (gameObject.name != "Player")
        {
            if (Map.IsInRange(Player, gameObject))
                gameObject.GetComponent<SpriteRenderer>().color = InRangeColor;
            else
                gameObject.GetComponent<SpriteRenderer>().color = OutOfRangeColor;
            if (Input.GetMouseButtonDown(0))
            {
                Map.PlayerTryAttack(gameObject);
            }
        }
        else
            Debug.Log("Mouseover player");

    }
    private void OnMouseExit()
    {
        gameObject.GetComponent<SpriteRenderer>().color = DefaultColor;
    }
}
