using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Map : MonoBehaviour
{
    //Map Stuff
    //For Matrix: 0 represents no ground, 1 represents open ground, 2 represents occupied ground
    public int[][] Matrix;
    //LivingMatrix
    public GameObject[][] LivingMatrix;
    private int Maxx;
    private int Maxy;
    //Player Stuff
    private int PlayerEnergyMax;
    private int PlayerEnergy;
    private bool InCombat;
    //EnemyStuff

    //GameObjects
    public static Map Instance;
    public GameObject Tile;
    public GameObject Protag;
    public GameObject Villager;
    public GameObject Camera;
    public GameObject Antag;
    public GameObject DamageTextPF;
    public TextMeshProUGUI DialogTMP;
    public Text EnergyText;
    public Text HealthText;

    private GameObject Player;
    private List<GameObject> Friendlies;
    private List<GameObject> Enemies;

    void Start()
    {
        Instance = this;
        Enemies = new List<GameObject>();
        Friendlies = new List<GameObject>();
        MakeMap();
        InCombat = Enemies.Count > 0;
        PlayerEnergyMax = 2;
        PlayerEnergy = PlayerEnergyMax;
        AssignPlayerStats(100, 100, 4, 10);

        EnergyText.text = PlayerEnergy.ToString() + "/" + PlayerEnergyMax.ToString();
        HealthText.text = Player.GetComponent<Alive>().health + "/" + Player.GetComponent<Alive>().maxHealth;
    }

    void Update()
    {
        PlayerMove();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Ending Turn");
            EndTurn();
        }
    }

    void AssignPlayerStats(int maxHealth, int health, int range, int guns)
    {
        Player.GetComponent<Alive>().maxHealth = maxHealth;
        Player.GetComponent<Alive>().health = health;
        Player.GetComponent<Alive>().range = range;
        Player.GetComponent<Alive>().guns = guns;
    }

    void PlayerMove()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            if (PlayerEnergy > 0 && MoveLiving(Player, new Vector3Int(0, 1, 0)))
                SpendEnergy(1);
            else TryTalking(new Vector3Int(0, 1, 0));
        if (Input.GetKeyDown(KeyCode.DownArrow))
            if(PlayerEnergy > 0 && MoveLiving(Player, new Vector3Int(0, -1, 0)))
                SpendEnergy(1);
            else TryTalking(new Vector3Int(0, -1, 0));
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            if (PlayerEnergy > 0 && MoveLiving(Player, new Vector3Int(-1, 0, 0)))
                SpendEnergy(1);
            else TryTalking(new Vector3Int(-1, 0, 0));
        if (Input.GetKeyDown(KeyCode.RightArrow))
            if (PlayerEnergy > 0 && MoveLiving(Player, new Vector3Int(1, 0, 0)))
                SpendEnergy(1);
            else TryTalking(new Vector3Int(1, 0, 0));

        Vector3 cameraLoc = new Vector3(Player.GetComponent<Alive>().location.x, Player.GetComponent<Alive>().location.y, -10);
        Player.transform.position = Player.GetComponent<Alive>().location;
        Camera.transform.position = cameraLoc;
    }

    void TryTalking(Vector3Int movement)
    {
        Vector3Int loc = Player.GetComponent<Alive>().location;

        Vector3Int oldSpot = Player.GetComponent<Alive>().location;
        Vector3Int newSpot = oldSpot + movement;
        if (newSpot.x >= 0 && newSpot.y >= 0 && newSpot.x < Maxx && newSpot.y < Maxy && LivingMatrix[newSpot.x][newSpot.y] == null) return;
        if (newSpot.x >= 0 && newSpot.y >= 0 && newSpot.x < Maxx && newSpot.y < Maxy && LivingMatrix[newSpot.x][newSpot.y].GetComponent<Talkative>() != null)
        {
            Talkative talker = LivingMatrix[newSpot.x][newSpot.y].GetComponent<Talkative>();
            DialogController.Instance.Talk(talker);
        }

    }

    bool SpendEnergy(int n)
    {
        if (!InCombat) return true;
        if (n > PlayerEnergy) return false; //Not enough energy to do action
        else PlayerEnergy -= n;
        EnergyText.text = PlayerEnergy.ToString() + "/" + PlayerEnergyMax.ToString();
        return true;
    }

    void EndTurn()
    {
        if (Enemies.Count <= 0) ExitCombat();
        MoveEnemies();
        PlayerEnergy = PlayerEnergyMax;
        EnergyText.text = PlayerEnergy.ToString() + "/" + PlayerEnergyMax.ToString();
        HealthText.text = Player.GetComponent<Alive>().health + "/" + Player.GetComponent<Alive>().maxHealth;
    }

    void ExitCombat()
    {
        PlayerEnergy = PlayerEnergyMax;
        InCombat = false;
    }
    void MoveEnemies()
    {
        foreach(GameObject enemy in Enemies){
            if (enemy != null)
            {
                MinimizeDistance(enemy, Player);
                TryAttack(enemy, Player);
            }
        }
    }

    //obj1 moves towards obj2
    void MinimizeDistance(GameObject obj1, GameObject obj2)
    {
        Vector3Int loc1 = obj1.GetComponent<Alive>().location;
        Vector3Int loc2 = obj2.GetComponent<Alive>().location;
        bool moved = false;
        //Half the time, move horizontally first, half vertically
        if(Random.Range(0, 2) == 1)
        {
            if (loc1.x < loc2.x && !moved) {
                moved = MoveLiving(obj1, new Vector3Int(1, 0, 0)); }
            if (loc1.x > loc2.x && !moved)
                moved = MoveLiving(obj1, new Vector3Int(-1, 0, 0));
            if (loc1.y < loc2.y && !moved)
                moved = MoveLiving(obj1, new Vector3Int(0, 1, 0));
            if (loc1.y > loc2.y && !moved)
                moved = MoveLiving(obj1, new Vector3Int(0, -1, 0));
        }
        else
        {
            if (loc1.y < loc2.y && !moved)
                moved = MoveLiving(obj1, new Vector3Int(0, 1, 0));
            if (loc1.y > loc2.y && !moved)
                moved = MoveLiving(obj1, new Vector3Int(0, -1, 0));
            if (loc1.x < loc2.x && !moved)
                moved = MoveLiving(obj1, new Vector3Int(1, 0, 0));
            if (loc1.x > loc2.x && !moved)
                moved = MoveLiving(obj1, new Vector3Int(-1, 0, 0));
        }
    }

    //obj1 attacks obj2
    void TryAttack(GameObject attacker, GameObject defender)
    {
        if(IsInRange(attacker, defender))
        {
            DealDamage(defender, attacker.GetComponent<Alive>().guns);
        }
    }

    public void PlayerTryAttack(GameObject defender)
    {
        if (IsInRange(Player, defender))
            if (SpendEnergy(1))
                DealDamage(defender, 10 + Player.GetComponent<Alive>().guns);
    }

    void DealDamage(GameObject target, int damage)
    {
        target.GetComponent<Alive>().health -= damage;
        DamageText.Create(target.GetComponent<Alive>().location, damage, target.name == "Player");
        Debug.Log(target.name + " took " + damage + " damage!");
        if (target.GetComponent<Alive>().health <= 0)
        {
            if (target.gameObject.name != "Player")
                KillLiving(target);
            else
            {
                //Gameover code goes here
                Debug.Log("Game Over!");
            }
        }
    }

    public bool IsInRange(GameObject attacker, GameObject defender)
    {
        if (Vector3Int.Distance(attacker.GetComponent<Alive>().location, defender.GetComponent<Alive>().location) < attacker.GetComponent<Alive>().range) return true;
        else return false;
    }

    void MakeMap()
    {
        Maxx = 8;
        Maxy = 8;
        Matrix = new int[Maxx][];
        LivingMatrix = new GameObject[Maxx][];
        for (int i = 0; i < Maxx; i++)
        {
            Matrix[i] = new int[Maxy];
            LivingMatrix[i] = new GameObject[Maxy];
            for (int j = 0; j<=i; j++)
            {
                Matrix[i][j] = 1;
                Vector3 location = new Vector3(i, j, 0);
                Instantiate(Tile, location, Quaternion.identity);
            }
        }
        Vector3Int PlayerLocation = new Vector3Int(0, 0, 0);
        Vector3Int VillagerLocation = new Vector3Int(1, 1, 0);
        Vector3Int EnemyLocation = new Vector3Int(5, 5, 0);
        Vector3Int EnemyLocation1 = new Vector3Int(4, 4, 0);
        Vector3Int EnemyLocation2 = new Vector3Int(6, 4, 0);

        DialogNode villagerDialog = new DialogNode("Hello! Thanks for saving me from those monsters! I have a lot to say so that we can check how the text object acts when it's given a ton of text! blah blah blah blah blah blah blah blah blah blah blah blah", 
            new string[3] { "Uh Okay then", "Thank you very cool", "..." }, 
            new DialogNode[3] { new DialogNode("Okay then ? ? ? ?"), new DialogNode("No problem my friend"), new DialogNode("..............................") });

        Player = SpawnLiving(Protag, "Player", PlayerLocation);
        Friendlies.Add(SpawnLiving(Villager, "Villager", VillagerLocation, villagerDialog));
        Enemies.Add(SpawnLiving(Antag, "Enemy", EnemyLocation));
        Enemies.Add(SpawnLiving(Antag, "Enemy", EnemyLocation1));
        Enemies.Add(SpawnLiving(Antag, "Enemy", EnemyLocation2));
        PopulateLivingMatrix(Friendlies);
    }

    void PopulateLivingMatrix(List<GameObject> friendlies)
    {
        foreach (GameObject friendly in friendlies)
        {
            if (friendly != null)
            {
                Vector3Int position = Vector3Int.FloorToInt(friendly.transform.position);
                LivingMatrix[position.x][position.y] = friendly;
            }
        }
    }

    GameObject SpawnLiving(GameObject toSpawn, string name, Vector3Int location)
    {
        GameObject spawned = Instantiate(toSpawn, location, Quaternion.identity);
        spawned.name = name;
        spawned.GetComponent<Alive>().location = location;
        Matrix[location.x][location.y] = 2;
        return spawned;
    }

    //Spawn living with a greeting
    GameObject SpawnLiving(GameObject toSpawn, string name, Vector3Int location, DialogNode dialog)
    {
        GameObject spawned = Instantiate(toSpawn, location, Quaternion.identity);
        spawned.name = name;
        spawned.GetComponent<Alive>().location = location;
        spawned.GetComponent<Talkative>().Dialog = dialog;
        Matrix[location.x][location.y] = 2;
        return spawned;
    }

    bool MoveLiving(GameObject toMove, Vector3Int movement)
    {
        Vector3Int oldSpot = toMove.GetComponent<Alive>().location;
        Vector3Int newSpot = oldSpot + movement;

        if (newSpot.x >= 0 && newSpot.y >= 0 && newSpot.x < Maxx && newSpot.y < Maxy && Matrix[newSpot.x][newSpot.y] == 1)
        {
            Matrix[oldSpot.x][oldSpot.y] = 1;
            Matrix[newSpot.x][newSpot.y] = 2;
            toMove.GetComponent<Alive>().location = newSpot;
            toMove.transform.position = newSpot;
            DialogController.Instance.EndDialog();
            return true;
        }
        return false;
    }

    public void KillLiving(GameObject toKill)
    {
        if (Enemies.Contains(toKill)) Enemies.Remove(toKill);
        Vector3Int loc = toKill.GetComponent<Alive>().location;
        Matrix[loc.x][loc.y] = 1;
        Destroy(toKill);
    }


}
