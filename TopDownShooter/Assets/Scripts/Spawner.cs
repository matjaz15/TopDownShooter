using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public bool developerMode;
    public int maxPawns = 30;

    public ItemSpawnGroup[] itemsSpawnGroup;
    public Wave[] waves;
    public Enemy enemy;

    Entity playerEntity;
    Transform playerT;

    Wave currentWave;
    int currentWaveNumber;

    int enemiesRemainingToSpawn;
    int enemiesRemainingAlive;
    int enemiesInLevel;
    float nextSpawnTime;

    MapGenerator map;

    float timeBetweenCampingChecks = 2;
    float campThresholdDistance = 1.5f;
    float nextCampCheckTime;
    Vector3 campPositionOld;
    bool isCamping;
    Color initialColor_Enemy;
    Color initialColor_Item;

    List<Item> spawnedItems;

    bool isDisabled;

    public float respawnerTimer { get; private set; }

    public event System.Action<int> OnNewWave;

    private void Start()
    {
        enemiesInLevel = 0;
        spawnedItems = new List<Item>();
        map = FindObjectOfType<MapGenerator>();
        playerEntity = FindObjectOfType<Player>();
        playerT = playerEntity.transform;       
        

        nextCampCheckTime = timeBetweenCampingChecks + Time.time;
        campPositionOld = playerT.position;

        playerEntity.OnDeath += PlayerEntity_OnDeath;

        //Wave one
        NextWave();

        initialColor_Enemy = map.GetRandomOpenTile().GetComponent<Renderer>().material.color;
        initialColor_Item = initialColor_Enemy;
    }

    private void Update()
    {
        if (!isDisabled) {
            if (Time.time > nextCampCheckTime)
            {
                nextCampCheckTime = Time.time + timeBetweenCampingChecks;

                isCamping = (Vector3.Distance(playerT.position, campPositionOld) < campThresholdDistance);
                campPositionOld = playerT.position;
            }

            if ((enemiesRemainingToSpawn > 0 || currentWave.infinite) && Time.time > nextSpawnTime && enemiesInLevel < maxPawns)
            {
                enemiesInLevel++;
                enemiesRemainingToSpawn--;
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

                StartCoroutine("SpawnEnemy");

            }            
        }

        if (developerMode)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                StopCoroutine("SpawnEnemy");
                foreach (Enemy enemy in FindObjectsOfType<Enemy>())
                {
                    GameObject.Destroy(enemy.gameObject);
                }
                NextWave();
            }
        }
    }

    void SpawnItems(int itemGroupIndex, bool clear = true)
    {
        if (clear) {
            foreach (Item item in spawnedItems)
            {
                if (item != null)
                    Destroy(item.gameObject);
            }
            spawnedItems.Clear();
        }
        


        if (itemsSpawnGroup.Length > 0) {
            ItemSpawn[] itemsInGroup = itemsSpawnGroup[itemGroupIndex].itemSpawn;
            for (int i = 0; i < itemsInGroup.Length; i++)
            {
                for (int j = 0; j < itemsInGroup[i].initialSpawnAmount; j++)
                {
                    Transform freeTile = map.GetRandomOpenTile();
                    Item spawnedItem = Instantiate(itemsInGroup[i].item, freeTile.transform.position, itemsInGroup[i].item.transform.rotation) as Item;
                    spawnedItems.Add(spawnedItem);
                }               
            }
        }


    }

    IEnumerator SpawnEnemy() {
        float spawnDelay = 1;
        float tileFlashSpeed = 4;

        Transform spawnTile = map.GetRandomOpenTile();
        if (isCamping) {
            spawnTile = map.GetTileFromPosition(playerT.position);
        }
        Material tileMat = spawnTile.GetComponent<Renderer>().material;
        Color flashColor = Color.red;
        float spawnTimer = 0;

        while (spawnTimer < spawnDelay) {

            tileMat.color = Color.Lerp(initialColor_Enemy, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));

            spawnTimer += Time.deltaTime;
            yield return null;
        }

        Enemy spawnedEnemy = Instantiate(enemy, spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
        spawnedEnemy.OnDeath += SpawnedEnemy_OnDeath;

        tileMat.color = initialColor_Enemy;

        float randomSpeed = Random.Range(currentWave.moveSpeedMin,currentWave.moveSpeedMax);
        spawnedEnemy.SetCharacteristics(randomSpeed, currentWave.hitsToKillPlayer, currentWave.enemyHealth, currentWave.skinColor);
    }

    IEnumerator ItemRespawner()
    {        
        while (true) {
            respawnerTimer = currentWave.itemRespawnTime;
            float t = 0;
            while (t < currentWave.itemRespawnTime)
            {
                t += Time.deltaTime;
                respawnerTimer = currentWave.itemRespawnTime - t;
                yield return null;
            }
            SpawnItems(currentWave.itemRespawnGroupIndex, false);
            yield return null;
        }       

    }

    void PlayerEntity_OnDeath() {
        isDisabled = true;
    }

    private void SpawnedEnemy_OnDeath()
    {
        enemiesInLevel--;
        enemiesRemainingAlive--;

        if (enemiesRemainingAlive == 0) {
            NextWave();
        }
    }

    void ResetPlayerPosition() {
        playerT.position = map.GetTileFromPosition(Vector3.zero).position + Vector3.up * 3;
    }

    void NextWave() {
        //if (currentWaveNumber > 0) {
        //   AudioManager.instance.PlaySound2D("LevelComplete");
        //}
        currentWaveNumber++;
        enemiesInLevel = 0;
        if (currentWaveNumber - 1 < waves.Length) {

            currentWave = waves[currentWaveNumber - 1];
            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesRemainingAlive = enemiesRemainingToSpawn;

            if (OnNewWave != null) OnNewWave(currentWaveNumber);

            ResetPlayerPosition();
            SpawnItems(currentWave.itemSpawnGroupIndex);

            StopCoroutine("ItemRespawner");
            StartCoroutine("ItemRespawner");
        }
        
    }


    [System.Serializable]
    public class ItemSpawn
    {
        public Item item;
        public int initialSpawnAmount = 5;
    }

    [System.Serializable]
    public class ItemSpawnGroup {
        public string groupName;
        public ItemSpawn[] itemSpawn;
    }

    [System.Serializable]
    public class Wave {

        public bool infinite;
        public int enemyCount;
        public float timeBetweenSpawns;

        public float moveSpeedMin;
        public float moveSpeedMax;
        public int hitsToKillPlayer;
        public float enemyHealth;
        public Color skinColor;

        public int itemSpawnGroupIndex;
        public int itemRespawnGroupIndex;
        public int itemRespawnTime;
    }


}
