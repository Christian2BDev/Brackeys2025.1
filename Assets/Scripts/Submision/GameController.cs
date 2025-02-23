using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [Header("Settings")]
    public List<Item> possibleItems; // Original list of items
    public List<GameObject> spawnPoints; // Original list of spawn points
    public GameObject itemPrefab;
    public Item chooseItem;
    
    [Header("Configs")]
    public GameObject uiManagerObject;
    public static int Health = 3;
    public float timerStart;
    public static float Timer;
    public static int PartsCollected;
    public static int PartsNeeded;
    public  int partsNeededConf;
    
    private List<Item> _shuffledItemList; // Shuffled copy for items
    private List<GameObject> _shuffledSpawnPoints; // Shuffled copy for spawn points
    private int _currentItemIndex;
    private int _currentSpawnPointIndex;

    private void Start()
    {
        Timer = timerStart;
        PartsNeeded = partsNeededConf;
        GetSpawnPoints();
        ShuffleItems();
        ShuffleSpawnPoints();
        PlaceItems();
        GetProfItem();
    }

    void Update()
    {
        TimerMethode();
    }

    void TimerMethode()
    {
        if ((Timer >= 1))
        {
            Timer -= Time.deltaTime;
            uiManagerObject.GetComponent<UIManager>().UpdateUI();
        }
        else
        {
            Damage(true);
        }
    }
    private void GetProfItem()
    {

        var it = GetNextItem();
        Debug.Log("Get Item " + it.Name);
        chooseItem = it;

    }

    private void ShuffleItems()
    {
        _shuffledItemList = new List<Item>(possibleItems); // Create a copy
        System.Random rng = new System.Random();
        int n = _shuffledItemList.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (_shuffledItemList[k], _shuffledItemList[n]) = (_shuffledItemList[n], _shuffledItemList[k]); // Swap using deconstruction
        }
    }

    private void ShuffleSpawnPoints()
    {
        _shuffledSpawnPoints = new List<GameObject>(spawnPoints); // Create a copy
        System.Random rng = new System.Random();
        int n = _shuffledSpawnPoints.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (_shuffledSpawnPoints[k], _shuffledSpawnPoints[n]) = (_shuffledSpawnPoints[n], _shuffledSpawnPoints[k]); // Swap using deconstruction
        }
    }

    void PlaceItems()
    {
        foreach (var item in possibleItems)
        {
            var temp = Instantiate(itemPrefab, _shuffledSpawnPoints[_currentSpawnPointIndex].transform);
            if (temp.TryGetComponent<Part>(out Part part))
            {
                part.itemConfig = item;
            }
            _currentSpawnPointIndex++;
        }
    }

  

    private Item GetNextItem()
    {
        if (_currentItemIndex >= _shuffledItemList.Count)
        {
            Debug.Log("All items have been picked.");
            return null;
        }

        return _shuffledItemList[_currentItemIndex++];
    }

    private void GetSpawnPoints()
    {
       spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint").ToList();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out var player))
        {
            player.inSubmitArea = true;
            if (player.submitArea ==null)
            {
                player.submitArea = gameObject;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out var player))
        {
            player.inSubmitArea = false;
        }
    }

    public void SubmitItem(GameObject holdingItem)
    {
        Part p = holdingItem.GetComponent<Part>();
        var i = p.itemConfig;
        if (i == chooseItem)
        {
            Destroy(holdingItem);
            PartsCollected++;
            if (PartsCollected == PartsNeeded)
            {
                Debug.Log("Congratulations!");
                SceneManager.LoadScene("Start");
            }
            else
            {
                GetProfItem();
                Debug.Log("Next Item!");
            }

           
            
        }
        else
        {
            holdingItem.transform.position = p.startPosition;
            Damage(false);
        }
    }

    private void Damage(bool instaKill)
    {
        Debug.Log("You're hit");
        Health--;  
        if (Health <= 0 || instaKill)
        {
            Debug.Log("Well nice knowing you! BOOM");
            SceneManager.LoadScene("GameOver");
        }
        
    }
}
