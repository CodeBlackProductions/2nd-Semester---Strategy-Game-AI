using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public static Pathfinder pathfinder;

    private List<Vector2> allTiles = new List<Vector2>();
    private List<Vector2> openTiles = new List<Vector2>();
    private List<Vector2> closedTiles = new List<Vector2>();
    private Vector2 currentTile;
    private Vector2 targetTile;
    private List<int> ACost = new List<int>();
    private List<int> BCost = new List<int>();
    private List<int> FCost = new List<int>();

    public void Awake()
    {
        if (pathfinder == null)
        {
            pathfinder = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public List<Vector2> FindPath(List<Vector2> possibleTiles, Vector2 start, Vector2 target, bool reverseSweep)
    {
        allTiles.Clear();
        openTiles.Clear();
        closedTiles.Clear();

        allTiles = possibleTiles;
        allTiles.Remove(start);

        closedTiles.Add(start);
        currentTile = start;
        targetTile = target;

        GetNeighbours(currentTile);

        GetTileCost();

        int maxCycles = 1000;

        while (maxCycles > 0)
        {
            maxCycles--;
            if (currentTile == targetTile)
            {
                if (!reverseSweep)
                {
                    List<Vector2> tempList = new List<Vector2>();
                    tempList.AddRange(closedTiles);
                    closedTiles = FindPath(tempList, target, start, true);
                    DrawPath();
                    return closedTiles;
                }
                else
                {
                    return closedTiles;
                }
            }
            FindCheapestTile();
            GetNeighbours(currentTile);
            GetTileCost();
        }
        return null;
    }

    private void GetNeighbours(Vector2 tile)
    {
        float x = currentTile.x;
        float y = currentTile.y;

        Vector2 temp = new Vector2(x + 1, y);
        CheckNeighbour(temp);

        temp = new Vector2(x - 1, y);
        CheckNeighbour(temp);

        temp = new Vector2(x, y + 1);
        CheckNeighbour(temp);

        temp = new Vector2(x, y - 1);
        CheckNeighbour(temp);
    }

    private void CheckNeighbour(Vector2 neighbourToCheck)
    {
        if (allTiles.Contains(neighbourToCheck))
        {
            if (!closedTiles.Contains(neighbourToCheck) && !openTiles.Contains(neighbourToCheck))
            {
                openTiles.Add(neighbourToCheck);
            }
        }
    }

    private void GetTileCost()
    {
        ACost.Clear();
        BCost.Clear();
        FCost.Clear();

        for (int i = 0; i < openTiles.Count; i++)
        {
            int xA = Mathf.Abs((int)openTiles[i].x - (int)currentTile.x);
            int yA = Mathf.Abs((int)openTiles[i].y - (int)currentTile.y);
            ACost.Add(xA + yA);

            int xB = Mathf.Abs((int)openTiles[i].x - (int)targetTile.x);
            int yB = Mathf.Abs((int)openTiles[i].y - (int)targetTile.y);
            BCost.Add(xB + yB);

            FCost.Add(ACost[i] + BCost[i]);
        }
    }

    private void FindCheapestTile()
    {
        int tempCost = int.MaxValue;

        for (int i = 0; i < openTiles.Count; i++)
        {
            if (openTiles[i] == targetTile)
            {
                currentTile = openTiles[i];
                closedTiles.Add(openTiles[i]);
                openTiles.Remove(openTiles[i]);
                return;
            }

            if (FCost[i] < tempCost)
            {
                tempCost = FCost[i];
                currentTile = openTiles[i];
            }
            else if (FCost[i] == tempCost)
            {
                if (BCost[i] < BCost[openTiles.IndexOf(currentTile)])
                {
                    tempCost = FCost[i];
                    currentTile = openTiles[i];
                }
            }
        }

        openTiles.Remove(currentTile);
        closedTiles.Add(currentTile);
    }

    private void DrawPath()
    {
        Debug.LogWarning("Path Drawing not yet implemented!");
        //Add Draw Event Call Here
    }
}