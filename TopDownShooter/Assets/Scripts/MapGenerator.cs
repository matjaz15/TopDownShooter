using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    public Map[] maps;
    public int mapIndex;

    public Transform tilePrefab;
    public Vector2 maxMapSize;
    public Transform obstaclePrefab;
    public Transform navMeshFloor;
    public Transform navMeshMaskPrefab;
    public Transform mapFloor;
    public Transform beam1;
    public Transform beam2;
    public Transform bottomPrefab;
    public float borderHeight = 20;

    public float navmeshOffset = 4f;

    [Range(0,1)]
    public float outlinePercent;

    public float tileSize;

    List<Coord> allTileCoords;
    Queue<Coord> shuffledTileCoords;
    Queue<Coord> shuffledOpenTileCoords;
    Transform[,] tilemap;

    Map currentMap;

    private void Start(){
        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
    }

    void OnNewWave(int waveNumber) {
        mapIndex = waveNumber - 1;
        GenerateMap();
    }

    public void GenerateMap() {
        currentMap = maps[mapIndex];
        tilemap = new Transform[currentMap.mapSize.x, currentMap.mapSize.y];
        System.Random prng = new System.Random(currentMap.seed);

        //Generating coords
        allTileCoords = new List<Coord>();
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                allTileCoords.Add(new Coord(x,y));
            }
        }

        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), currentMap.seed));

        //Create map holder object
        string holderName = "Generated Map";
        if (transform.Find(holderName)) {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        //Spawning tiles
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                Vector3 tilePosition = CoordToPosition(x,y);
                Transform newTile = Instantiate(tilePrefab, tilePosition,Quaternion.Euler(Vector3.right*90)) as Transform;
                newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
                newTile.parent = mapHolder;
                tilemap[x, y] = newTile;
            }
        }

        //Spawning obstacles
        bool[,] obstacleMap = new bool[(int)currentMap.mapSize.x, (int)currentMap.mapSize.y];

        int obstacleCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obstaclePercent);
        int currentObstacleCount = 0;
        List<Coord> allOpenCoords = new List<Coord>(allTileCoords);

        for (int i = 0; i < obstacleCount; i++) {
            Coord randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount++;
            if (randomCoord != currentMap.mapCenter && MapIsFullyAccesibile(obstacleMap, currentObstacleCount))
            {
                float obstacleHeight = Mathf.Lerp(currentMap.minimumObstacleHeight,currentMap.maximumObstacleHeight,(float)prng.NextDouble());
                Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);

                Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * obstacleHeight/2 * tileSize, Quaternion.identity) as Transform;
                newObstacle.parent = mapHolder;
                newObstacle.localScale = new Vector3(1 - outlinePercent, obstacleHeight, 1 - outlinePercent) * tileSize;

                //Colors
                Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
                Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
                float colorPercent = randomCoord.y / (float)currentMap.mapSize.y;
                obstacleMaterial.color = Color.Lerp(currentMap.foregroundColor,currentMap.backgroundColor,colorPercent);
                obstacleRenderer.sharedMaterial = obstacleMaterial;

                allOpenCoords.Remove(randomCoord);
            }
            else {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--;
            }          
        }

        shuffledOpenTileCoords = new Queue<Coord>(Utility.ShuffleArray(allOpenCoords.ToArray(), currentMap.seed));


        //Creating edge beams
        float yOffset = (-beam1.transform.localScale.y / 2 - 0.03f) ;

        beam1.position = new Vector3(mapHolder.transform.position.x, yOffset, mapHolder.transform.position.z);
        beam1.localScale = new Vector3((maxMapSize.x + currentMap.mapSize.x) / 2f, 1, 1) * tileSize;

        beam2.position = new Vector3(mapHolder.transform.position.x, yOffset, mapHolder.transform.position.z);
        beam2.localScale = new Vector3((maxMapSize.y + currentMap.mapSize.y) / 2f, 1, 1) * tileSize;
        beam2.rotation = Quaternion.Euler(0,90,0);


        //Creating borders 
        Vector2 mMax = maxMapSize * tileSize;
        float positionOffsetX = mMax.x - (mMax.x - ((mMax.x + beam1.localScale.x) / 2) / 2);
        float positionOffsetY = mMax.y - (mMax.y - ((mMax.y + beam2.localScale.x) / 2) / 2);
        float yScale = borderHeight;

        Transform maskRight = Instantiate(obstaclePrefab, Vector3.right * positionOffsetX + new Vector3(0,  -yScale/4, 0), Quaternion.identity, mapHolder) as Transform;
        maskRight.localScale = new Vector3((maskRight.transform.position.x - beam1.localScale.x/2)*2, yScale, beam2.localScale.x);

        Transform maskLeft = Instantiate(obstaclePrefab, Vector3.left * positionOffsetX + new Vector3(0, -yScale / 4, 0), Quaternion.identity, mapHolder) as Transform;
        maskLeft.localScale = new Vector3((maskLeft.transform.position.x + beam1.localScale.x/2)*2, yScale, beam2.localScale.x);

        Transform maskTop = Instantiate(obstaclePrefab, Vector3.forward * positionOffsetY + new Vector3(0, -yScale / 4, 0), Quaternion.identity, mapHolder) as Transform;
        maskTop.localScale = new Vector3(mMax.y, yScale, (maskTop.transform.position.z - beam2.localScale.x / 2) * 2);

        Transform maskBottom = Instantiate(obstaclePrefab, Vector3.back * positionOffsetY + new Vector3(0, -yScale / 4, 0), Quaternion.identity, mapHolder) as Transform;
        maskBottom.localScale = new Vector3(mMax.y, yScale, (maskBottom.transform.position.z + beam2.localScale.x / 2) * 2);

        //Create bottom
        Transform bottom = Instantiate(bottomPrefab, new Vector3(0, -maskRight.localScale.y / 2 - bottomPrefab.localScale.y / 2, 0), Quaternion.identity, mapHolder);
        bottom.localScale = new Vector3(mMax.x - maskRight.transform.localScale.x * 2, 1, mMax.y - maskTop.transform.localScale.z * 2);

        //NavMesh
        navMeshFloor.localScale = mMax;
        mapFloor.localScale = new Vector3(currentMap.mapSize.x * tileSize, currentMap.mapSize.y * tileSize, 1);

        //Creating navmesh mask         
        Vector3 correctedMapFloorScale = new Vector3(mapFloor.localScale.x, mapFloor.localScale.z, mapFloor.localScale.y);
        Vector3 offset1 = new Vector3(correctedMapFloorScale.x / 2 + (bottom.localScale.x / 2 - correctedMapFloorScale.x / 2) / 2, 0, beam1.localScale.z / 2 + (bottom.localScale.z / 2 - beam1.localScale.z / 2) / 2);
        Vector3 offset2 = new Vector3(beam2.localScale.z / 2 + (correctedMapFloorScale.x / 2 - beam2.localScale.z / 2) / 2, 0, correctedMapFloorScale.z / 2 + (bottom.localScale.z / 2 - correctedMapFloorScale.z / 2) / 2);

        Transform maskRightBeamUp = Instantiate(navMeshMaskPrefab, offset1, Quaternion.identity, mapHolder) as Transform;
        maskRightBeamUp.localScale = new Vector3((offset1.x - correctedMapFloorScale.x / 2) * 2, 1, (offset1.z-bottom.localScale.z/2)*2);

        Transform maskRightBeamDown = Instantiate(navMeshMaskPrefab, new Vector3(offset1.x,offset1.y,-offset1.z), Quaternion.identity, mapHolder) as Transform;
        maskRightBeamDown.localScale = new Vector3((offset1.x - correctedMapFloorScale.x / 2) * 2, 1, (-offset1.z + bottom.localScale.z / 2) * 2);

        Transform maskLeftBeamDown = Instantiate(navMeshMaskPrefab, -offset1, Quaternion.identity, mapHolder) as Transform;
        maskLeftBeamDown.localScale = new Vector3((-offset1.x + correctedMapFloorScale.x / 2) * 2, 1, (-offset1.z + bottom.localScale.z / 2) * 2);

        Transform maskLeftBeamUp = Instantiate(navMeshMaskPrefab, new Vector3(-offset1.x,offset1.y,offset1.z), Quaternion.identity, mapHolder) as Transform;
        maskLeftBeamUp.localScale = new Vector3((-offset1.x + correctedMapFloorScale.x / 2) * 2, 1, (offset1.z - bottom.localScale.z / 2) * 2);

        Transform maskBottomBeamUp = Instantiate(navMeshMaskPrefab, offset2, Quaternion.identity, mapHolder) as Transform;
        maskBottomBeamUp.localScale = new Vector3((offset2.x - correctedMapFloorScale.x / 2) * 2, 1, (offset2.z - bottom.localScale.z / 2) * 2);

        Transform maskBottomBeamDown = Instantiate(navMeshMaskPrefab, new Vector3(offset2.x, offset2.y, -offset2.z), Quaternion.identity, mapHolder) as Transform;
        maskBottomBeamDown.localScale = new Vector3((offset2.x - correctedMapFloorScale.x / 2) * 2, 1, (-offset2.z + bottom.localScale.z / 2) * 2);

        Transform maskTopBeamDown = Instantiate(navMeshMaskPrefab, -offset2, Quaternion.identity, mapHolder) as Transform;
        maskTopBeamDown.localScale = new Vector3((-offset2.x + correctedMapFloorScale.x / 2) * 2, 1, (-offset2.z + bottom.localScale.z / 2) * 2);

        Transform maskTopBeamUp = Instantiate(navMeshMaskPrefab, new Vector3(-offset2.x, offset2.y, offset2.z), Quaternion.identity, mapHolder) as Transform;
        maskTopBeamUp.localScale = new Vector3((-offset2.x + correctedMapFloorScale.x / 2) * 2, 1, (offset2.z - bottom.localScale.z / 2) * 2);



    }

    bool MapIsFullyAccesibile(bool[,] obstacleMap,int currentObstacleCount) {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(currentMap.mapCenter);
        mapFlags[currentMap.mapCenter.x, currentMap.mapCenter.y] = true;

        int accesibileTileCount = 1;

        while (queue.Count > 0) {
            Coord tile = queue.Dequeue();
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int neighbourX = tile.x + x;
                    int neighoburY = tile.y + y;
                    if (x == 0 || y == 0) {
                        if (neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighoburY >= 0 && neighoburY < obstacleMap.GetLength(1)) {
                            if (!mapFlags[neighbourX, neighoburY] && !obstacleMap[neighbourX, neighoburY]) {
                                mapFlags[neighbourX, neighoburY] = true; //checked!
                                queue.Enqueue(new Coord(neighbourX,neighoburY));
                                accesibileTileCount++;
                            }
                        }
                    }
                }
            }
        }

        int targetAccessibleTileCount = (int)currentMap.mapSize.x * (int)currentMap.mapSize.y - currentObstacleCount;
        return targetAccessibleTileCount == accesibileTileCount;
    }

    Vector3 CoordToPosition(int x, int y) {
        return new Vector3(-currentMap.mapSize.x / 2f + 0.5f + x, 0, -currentMap.mapSize.y / 2f + 0.5f + y) * tileSize;
    }

    public Coord GetRandomCoord() {
        Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }

    public Transform GetRandomOpenTile() {
        Coord randomCoord = shuffledOpenTileCoords.Dequeue();
        shuffledOpenTileCoords.Enqueue(randomCoord);
        return tilemap[randomCoord.x, randomCoord.y];
    }

    public Transform GetTileFromPosition(Vector3 position) {
        int x = Mathf.RoundToInt( position.x / tileSize + (currentMap.mapSize.x - 1) / 2f);
        int y = Mathf.RoundToInt(position.z / tileSize + (currentMap.mapSize.y - 1) / 2f);
        x = Mathf.Clamp(x, 0, tilemap.GetLength(0)-1);
        y = Mathf.Clamp(y, 0, tilemap.GetLength(1));
        return tilemap[x, y];
    }

    [System.Serializable]
    public struct Coord
    {
        public int x;
        public int y;

        public Coord(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public static bool operator ==(Coord c1, Coord c2) {
            return c1.x == c2.x && c1.y == c2.y;
        }

        public static bool operator !=(Coord c1, Coord c2)
        {
            return !(c1 == c2);
        }
    }

    [System.Serializable]
    public class Map {

        public Coord mapSize;
        [Range(0,1)]
        public float obstaclePercent;
        public int seed;
        public float minimumObstacleHeight;
        public float maximumObstacleHeight;
        public Color foregroundColor;
        public Color backgroundColor;

        public Coord mapCenter{
            get {
                return new Coord(mapSize.x / 2, mapSize.y / 2);
            }
        }
    }


}
