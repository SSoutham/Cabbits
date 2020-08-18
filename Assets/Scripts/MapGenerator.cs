using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public enum Cell : byte
{
    WATER = 0,
    DIRT = 1,
    GRASS = 2,
    BUSH = 3,
    ASH = 4,
    BURNING_BUSH = 5,
    GROWING_GRASS = 6,
    GROWING_GRASS_FULL = 7,
    GROWING_BUSH = 8,
    DISAPPEARING_ASH = 9,
    FIRE = 10,
    CARROT = 11,
    GROWING_CARROT = 12,
    ROTTEN_CARROT = 13,
    BURNING_GRASS = 98,
    NONE = 99,
};

// Author: Iaroslav Titov (c)
public class MapGenerator : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject startGameUI;
    [SerializeField] GameObject wholeScreenButton;

    [Header("Settings")]
    [SerializeField] Tilemap waterLevel;
    [SerializeField] Tilemap lowerLevel;
    [SerializeField] Tilemap midLevel;
    [SerializeField] Tilemap highLevel;
    [SerializeField] Tilemap topLevel;
    [SerializeField] List<TileBase> cellTypes;
    public PoolManager poolManager;
    [SerializeField] int lakeNum = 0;
    [SerializeField] int cullingRadius = 20;
    [SerializeField] int platformSize = 0;
    public int worldWidth;
    public int worldHeight;

    [Header("Prefabs")]
    [SerializeField] TileBase ashSplash;
    [SerializeField] GameObject splash;

    // map data
    private Cell[,] cells;
    private float[,] cellTimers;
    private new Camera camera;
    private Vector3Int savedOldCameraPos;
    Vector3Int savedCameraPos;
    private List<Vector2Int> carrots;

    void Start()
    {
        camera = Camera.main;
        CreateStartMap();
        savedCameraPos = Vector3Int.FloorToInt(camera.transform.position);
        savedOldCameraPos = savedCameraPos;
        carrots = new List<Vector2Int>();
    }

    public void UserTapped()
    {
        StartCoroutine(OpenClouds());
    }

    private IEnumerator OpenClouds()
    {
        startGameUI.GetComponent<Animator>().CrossFade("EndGame", 0.001f);
        yield return new WaitForSeconds(1.0f);
        startGameUI.SetActive(false);
        wholeScreenButton.SetActive(false);
        GameObject.FindObjectOfType<ScoreScript>().Activate();
    }

    public bool isGameStarted()
    {
        return !startGameUI.activeSelf;
    }

    public Cell GetCell(int x, int y)
    {
        return cells[x, y];
    }

    private void CreateStartMap()
    {
        cells = new Cell[worldWidth, worldHeight];
        cellTimers = new float[worldWidth, worldHeight];

        // Fill world with dirt
        for (int x = 0; x < worldWidth; x++)
        {
            for (int y = 0; y < worldHeight; y++)
            {
                cells[x, y] = Cell.DIRT;
                cellTimers[x, y] = Random.value * 10;
            }
        }

        // Make lakes
        for (int i = 0; i < lakeNum; i++)
        {
            int width = 2 + Mathf.FloorToInt(Random.value * 4);
            int height = 2 + Mathf.FloorToInt(Random.value * 4);

            int posX = 1 + Mathf.FloorToInt(Random.value * (worldWidth - 1 - width));
            int posY = 1 + Mathf.FloorToInt(Random.value * (worldHeight - 1 - height));

            for (int x = posX; x < posX + width; x++)
            {
                for (int y = posY; y < posY + height; y++)
                {
                    cells[x, y] = Cell.WATER;
                }
            }
        }

        // make platform of dirt in the middle
        for (int x = worldWidth / 2 - platformSize / 2; x < worldWidth / 2 + platformSize / 2; x++)
        {
            for (int y = worldHeight / 2 - platformSize / 2; y < worldHeight / 2 + platformSize / 2; y++)
            {
                cells[x, y] = Cell.DIRT;
            }
        }

        //Make Grass
        for (int x = 1; x < worldWidth - 2; x++)
        {
            for (int y = 1; y < worldHeight - 2; y++)
            {
                if (cells[x, y] == Cell.DIRT && Random.value < 0.5f)
                {
                    cells[x, y] = Cell.GRASS;
                    cellTimers[x, y] = 5.0f * Random.value * 100.0f;
                }
            }
        }

        StartCoroutine(RenderAll());
    }

    public IEnumerator RenderAll()
    {
        savedOldCameraPos = savedCameraPos;
        savedCameraPos = Vector3Int.FloorToInt(camera.transform.position);

        StartCoroutine(RenderMap(waterLevel, savedOldCameraPos, savedCameraPos, true));
        yield return new WaitForEndOfFrame();
        StartCoroutine(RenderMap(lowerLevel, savedOldCameraPos, savedCameraPos, true));
        yield return new WaitForEndOfFrame();
        StartCoroutine(RenderMap(midLevel, savedOldCameraPos, savedCameraPos, true));
        yield return new WaitForEndOfFrame();
        StartCoroutine(RenderMap(highLevel, savedOldCameraPos, savedCameraPos, true));
        yield return new WaitForEndOfFrame();
        StartCoroutine(RenderMap(topLevel, savedOldCameraPos, savedCameraPos, true));
    }

    public IEnumerator RenderMap(Tilemap tilemap, Vector3Int oldCameraPos, Vector3Int cameraPos, bool renderNewOnly = false)
    {
        for (int x = cameraPos.x - cullingRadius; x < cameraPos.x + cullingRadius; x++)
        {
            for (int y = cameraPos.y - cullingRadius; y < cameraPos.y + cullingRadius; y++)
            {
                RenderCell(x, y, tilemap, renderNewOnly);
            }
            yield return new WaitForEndOfFrame();
        }

        for (int x = cameraPos.x - cullingRadius; x < cameraPos.x + cullingRadius; x++)
        {
            for (int y = cameraPos.y - cullingRadius; y < cameraPos.y + cullingRadius; y++)
            {
                RenderCell(x, y, tilemap, renderNewOnly);
            }
            yield return new WaitForEndOfFrame();
        }

        //Remove old stuff
        {
            int startX = oldCameraPos.x - cullingRadius;
            int endX = startX + ((cameraPos.x > oldCameraPos.x) ? cameraPos.x - oldCameraPos.x : cullingRadius * 2 + (cameraPos.x - oldCameraPos.x));
            int startY = oldCameraPos.y - cullingRadius;
            int endY = startY + ((cameraPos.x > oldCameraPos.x) ? cullingRadius * 2 : Mathf.Abs(oldCameraPos.y - cameraPos.y));

            if (startX > endX)
            {
                int temp = startX;
                startX = endX;
                endX = temp;
            }

            if (cameraPos.x < oldCameraPos.x && cameraPos.y < oldCameraPos.y)
            {
                endY = oldCameraPos.y + cullingRadius + 1;
                startY = endY - (oldCameraPos.y - cameraPos.y) - 1;
                startX = oldCameraPos.x - cullingRadius;
                endX = oldCameraPos.x + cullingRadius - (oldCameraPos.x - cameraPos.x) + 1;
            }

            for (int x = startX; x < endX; x++)
            {
                for (int y = startY; y < endY; y++)
                {
                    RenderCell(x, y, tilemap, renderNewOnly);
                    //topLevel.SetTile(new Vector3Int(x, y, 0), cellTypes[(byte)Cell.WATER]);
                }
                yield return new WaitForEndOfFrame();
            }

            //Remove old stuff 2
            int startX2 = endX - 1;
            int endX2 = startX + cullingRadius * 2 + 1;
            int startY2 = oldCameraPos.y - cullingRadius;
            int endY2 = startY2 + ((cameraPos.x < oldCameraPos.x) ? cullingRadius * 2 : Mathf.Abs(oldCameraPos.y - cameraPos.y));

            if (cameraPos.x > oldCameraPos.x && cameraPos.y < oldCameraPos.y)
            {
                endY2 = oldCameraPos.y + cullingRadius + 1;
                startY2 = endY2 - (oldCameraPos.y - cameraPos.y) - 1;
            }

            for (int x = startX2; x < endX2; x++)
            {
                for (int y = startY2; y < endY2; y++)
                {
                    RenderCell(x, y, tilemap, renderNewOnly);
                    //topLevel.SetTile(new Vector3Int(x, y, 0), cellTypes[(byte)Cell.WATER]);
                }
                yield return new WaitForEndOfFrame();
            }
        }
    }

    public void RenderCell(int x, int y, Tilemap tilemap, bool renderNewOnly = false)
    {
        Vector3Int position = new Vector3Int(x, y, 0);

        //Set all out of range to null
        if (Vector2.Distance(camera.transform.position, new Vector2(x, y)) > cullingRadius)
        {
            tilemap.SetTile(position, null);
            return;
        }

        // DOn't render old ones
        if (renderNewOnly && tilemap.GetTile(position) != null)
        {
            return;
        }

        // fill sides wit water
        if (x < 0 || y < 0 || x >= worldWidth || y >= worldHeight)
        {
            if (tilemap == waterLevel) tilemap.SetTile(position, cellTypes[(byte)Cell.WATER]);
            return;
        }
         
        if (tilemap == waterLevel)
        {
            if (cells[x, y] == Cell.WATER)
                tilemap.SetTile(position, cellTypes[(byte)cells[x, y]]);
            else
                tilemap.SetTile(position, null);
        }
        if (tilemap == lowerLevel)
        {
            if (cells[x, y] != Cell.WATER)
                tilemap.SetTile(position, cellTypes[(byte)Cell.DIRT]);
            else
                tilemap.SetTile(position, null);
        }
        if (tilemap == midLevel)
        {
            if (cells[x, y] == Cell.GRASS || cells[x, y] == Cell.ASH || cells[x, y] == Cell.GROWING_GRASS || cells[x, y] == Cell.GROWING_GRASS_FULL || cells[x, y] == Cell.DISAPPEARING_ASH)
                tilemap.SetTile(position, cellTypes[(byte)cells[x, y]]);
            else if (cells[x, y] == Cell.FIRE)
                tilemap.SetTile(position, cellTypes[(byte)Cell.ASH]);
            else if (cells[x, y] == Cell.BUSH || cells[x, y] == Cell.GROWING_BUSH || cells[x, y] == Cell.BURNING_BUSH || cells[x, y] == Cell.BURNING_GRASS || cells[x, y] == Cell.GROWING_CARROT || cells[x, y] == Cell.CARROT || cells[x, y] == Cell.ROTTEN_CARROT)
                tilemap.SetTile(position, cellTypes[(byte)Cell.GRASS]);
            else
            tilemap.SetTile(position, null);
        }
        if (tilemap == highLevel)
        {
            if (cells[x, y] == Cell.BUSH || cells[x, y] == Cell.BURNING_BUSH || cells[x, y] == Cell.GROWING_BUSH || cells[x, y] == Cell.GROWING_CARROT || cells[x, y] == Cell.CARROT || cells[x, y] == Cell.ROTTEN_CARROT)
                tilemap.SetTile(position, cellTypes[(byte)cells[x, y]]);
            else
                tilemap.SetTile(position, null);
        }
        if (tilemap == topLevel)
        {
            if (cells[x, y] == Cell.BURNING_GRASS || cells[x, y] == Cell.BURNING_BUSH || cells[x, y] == Cell.FIRE)
                tilemap.SetTile(position, cellTypes[(byte)Cell.FIRE]);
            else
                tilemap.SetTile(position, null);
        }
        tilemap.SetTileFlags(position, TileFlags.LockAll);
    }

    void Update()
    {
        if (startGameUI.activeSelf) return;

        for (int x = 1; x < cells.GetUpperBound(0)-1; x++)
        {
            for (int y = 1; y < cells.GetUpperBound(1)-1; y++)
            {
                if (cellTimers[x, y] > 0)
                    cellTimers[x, y] -= Time.deltaTime;
                else
                {
                    if (cells[x, y] == Cell.DIRT)
                    {
                        if (SidesHave(x,y,Cell.DIRT))
                        {
                            cells[x, y] = Cell.GROWING_GRASS;
                            cellTimers[x, y] = 0.5f;
                            RenderCell(x, y, midLevel);
                        }
                        else
                        {
                            cells[x, y] = Cell.GROWING_GRASS_FULL;
                            cellTimers[x, y] = 0.5f;
                            RenderCell(x, y, midLevel);
                        }
                        continue;
                    }

                    //Grass Grow
                    if (cells[x, y] == Cell.GROWING_GRASS || cells[x, y] == Cell.GROWING_GRASS_FULL)
                    {
                        cells[x, y] = Cell.GRASS;
                        cellTimers[x, y] = 5.0f * Random.value * 100.0f;
                        RenderCell(x, y, midLevel);
                        continue;
                    }

                    //Generate Bush or Carrot
                    if (cells[x, y] == Cell.GRASS && y != 1)
                    {
                        if (Random.Range(0, 10) == 0)
                        {
                            cells[x, y] = Cell.GROWING_CARROT;
                            cellTimers[x, y] = 0.8f;
                            RenderCell(x, y, highLevel);
                        }
                        else
                        {
                            cells[x, y] = Cell.GROWING_BUSH;
                            cellTimers[x, y] = 0.8f;
                            RenderCell(x, y, highLevel);
                        }

                        continue;
                    }

                    //Bush Grow
                    if (cells[x, y] == Cell.GROWING_BUSH)
                    {
                        cells[x, y] = Cell.BUSH;
                        cellTimers[x, y] = float.PositiveInfinity;
                        RenderCell(x, y, highLevel);
                        continue;
                    }

                    //Carrot Grow
                    if (cells[x, y] == Cell.GROWING_CARROT)
                    {
                        cells[x, y] = Cell.CARROT;
                        carrots.Add(new Vector2Int(x, y));
                        cellTimers[x, y] = 10.0f + 20.0f * Random.value; // no less than 10 sec, no more than 30
                        RenderCell(x, y, highLevel);
                        continue;
                    }

                    //Carrot Rot
                    if (cells[x, y] == Cell.CARROT)
                    {
                        cells[x, y] = Cell.ROTTEN_CARROT;
                        cellTimers[x, y] = float.PositiveInfinity; // never despawns
                        RenderCell(x, y, highLevel);
                        continue;
                    }

                    //Ash to disappearing ash
                    if (cells[x, y] == Cell.ASH)
                    {
                        cells[x, y] = Cell.DISAPPEARING_ASH;
                        cellTimers[x, y] = 0.5f;
                        RenderCell(x, y, midLevel);
                        continue;
                    }

                    //Ash to Dirt
                    if (cells[x, y] == Cell.DISAPPEARING_ASH)
                    {
                        cells[x, y] = Cell.DIRT;
                        cellTimers[x, y] = 5.0f * Random.value * 10.0f;
                        RenderCell(x, y, midLevel);
                        continue;
                    }

                    // Spread Fire
                    if (cells[x, y] == Cell.BURNING_GRASS || cells[x, y] == Cell.BURNING_BUSH)
                    {
                        SetOnFire(x - 1, y);
                        SetOnFire(x + 1, y);
                        SetOnFire(x, y + 1);
                        SetOnFire(x, y - 1);

                        cellTimers[x, y] = Random.Range(5.0f, 7.0f);
                        cells[x,y] = Cell.FIRE;

                        continue;
                    }

                    // Fire to ash
                    if (cells[x, y] == Cell.FIRE)
                    {
                        cells[x, y] = Cell.ASH;

                        poolManager.pools["Fire"].Return(new Vector3Int(x,y, 0));

                        if (Vector2.Distance(camera.transform.position, new Vector2(x, y)) < cullingRadius) topLevel.SetTile(new Vector3Int(x, y, 0), ashSplash);

                        cellTimers[x, y] = 5.0f * Random.value * 10.0f;

                        RenderCell(x, y, highLevel);
                        RenderCell(x, y, topLevel);
                        RenderCell(x, y, midLevel);
                    }
                }
            }
        }
    }

    private void OnMouseDown()
    {
        Vector2 clickPos = camera.ScreenToWorldPoint(Input.mousePosition);
        if (clickPos.x > 0 && clickPos.x < worldWidth && clickPos.y > 0 && clickPos.y < worldHeight)
            ClickOnTile(Mathf.FloorToInt(clickPos.x), Mathf.FloorToInt(clickPos.y));
        else
        {
            GameObject go2 = Instantiate(splash, camera.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 10, Quaternion.identity);
            Destroy(go2, go2.GetComponent<Animator>().runtimeAnimatorController.animationClips[0].length);
        }
    }

    public void SetOnFire(int x, int y)
    {
        if (x == 0 || x == worldWidth - 1 || y == 0 || y == worldHeight - 1) return;

        switch (cells[x, y])
        {
            case Cell.GRASS:
            case Cell.GROWING_GRASS:
            case Cell.GROWING_CARROT:
            case Cell.CARROT:
            case Cell.ROTTEN_CARROT:
                cellTimers[x, y] = Random.Range(1.0f, 3.0f);
                if (cells[x, y] == Cell.CARROT || cells[x, y] == Cell.ROTTEN_CARROT) carrots.RemoveAll(c => c.x == x && c.y == y);
                cells[x, y] = Cell.BURNING_GRASS;
                RenderCell(x, y, midLevel);
                RenderCell(x, y, topLevel);
                break;
            case Cell.BUSH:
            case Cell.GROWING_BUSH:
                cellTimers[x, y] = Random.Range(1.0f, 3.0f);
                cells[x, y] = Cell.BURNING_BUSH;
                RenderCell(x, y, midLevel);
                RenderCell(x, y, highLevel);
                RenderCell(x, y, topLevel);
                break;
            case Cell.DIRT:
                cells[x, y] = Cell.ASH;
                cellTimers[x, y] = 5.0f * Random.value * 10.0f;
                RenderCell(x, y, midLevel);
                break;
        }
    }

    public bool NeighborsHave(int x, int y, Cell type)
    {
        if (cells[x - 1, y] == type) return true;
        if (cells[x + 1, y] == type) return true;
        if (cells[x, y + 1] == type) return true;
        if (cells[x, y - 1] == type) return true;
        if (cells[x - 1, y - 1] == type) return true;
        if (cells[x + 1, y + 1] == type) return true;
        if (cells[x - 1, y + 1] == type) return true;
        if (cells[x + 1, y - 1] == type) return true;

        return false;
    }

    public bool SidesHave(int x, int y, Cell type)
    {
        if (cells[x - 1, y] == type) return true;
        if (cells[x + 1, y] == type) return true;
        if (cells[x, y + 1] == type) return true;
        if (cells[x, y - 1] == type) return true;

        return false;
    }

    public void ClickOnTile(int x, int y)
    {
        Debug.Log("I clicked on "+cells[x, y]);
        switch (cells[x,y])
        {
            case Cell.GRASS:
            case Cell.BUSH:
            case Cell.ASH:
            case Cell.CARROT:
            case Cell.ROTTEN_CARROT:
                if (cells[x, y] == Cell.CARROT || cells[x, y] == Cell.ROTTEN_CARROT) carrots.RemoveAll(c => c.x == x && c.y == y);
                cells[x, y] = Cell.DIRT;
                cellTimers[x, y] = 5.0f * Random.value * 10.0f;
                RenderCell(x, y, highLevel);
                RenderCell(x, y, midLevel);
                break;
            case Cell.BURNING_BUSH:
            case Cell.BURNING_GRASS:
            case Cell.FIRE:
                cells[x, y] = Cell.ASH;

                poolManager.pools["Fire"].Return(new Vector3Int(x, y, 0));

                topLevel.SetTile(new Vector3Int(x,y,0), ashSplash);

                cellTimers[x, y] = 5.0f * Random.value * 10.0f;
                RenderCell(x, y, topLevel);
                RenderCell(x, y, highLevel);
                RenderCell(x, y, midLevel);
                break;
            case Cell.DIRT:
                cells[x, y] = Cell.WATER;
                RenderCell(x, y, lowerLevel);
                RenderCell(x, y, waterLevel);
                break;
            case Cell.WATER:
                GameObject go2 = Instantiate(splash, camera.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 10, Quaternion.identity);
                Destroy(go2, go2.GetComponent<Animator>().runtimeAnimatorController.animationClips[0].length);
                break;
        }
    }

    public IEnumerator ReloadGame()
    {
        yield return new WaitForSeconds(3.0f);

        startGameUI.SetActive(true);
        startGameUI.GetComponent<Animator>().CrossFade("StartGame", 0.001f);
        yield return new WaitForSeconds(1.0f);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Game");

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public Vector2Int? FindClosestCarrot(Vector2 origin, float radius)
    {
        Vector2Int? res = null;

        foreach (var carrot in carrots)
        {
            if (Vector2.Distance(origin, carrot) < radius)
            {
                if (res == null || Vector2.Distance(origin, carrot) < Vector2.Distance(origin, (Vector2)res))
                    res = carrot;
            }
        }

        return res;
    }
}
