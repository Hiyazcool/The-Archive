using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using Hiyazcool;

public class Pathfinding
{
    /*
     * Finish Pathfinding and Map struct and apply singleton and attach to MapHandler
     */
    int2 gridSize;
    private Tilemap.Map currentMap;
    private const int MOVE_DIAGONAL_COST = 14;
    private const int MOVE_STRAIGHT_COST = 10;
    public Pathfinding(int2 gridSize)
    {
        this.gridSize = gridSize;
    }
    public void FindPath(int2 startPosition, int2 endPosition)
    {
        NativeArray<PathNode> pathNodeArray = new NativeArray<PathNode>(gridSize.x * gridSize.y, Allocator.Persistent);
        ConvertMapToArray(ref pathNodeArray,currentMap, startPosition,endPosition);
        pathNodeArray.Dispose();
    }
    private int CalculateIndex(int x, int y)
    {
        return x+ y * gridSize.x;
    }    
    private void ConvertMapToArray(ref NativeArray<PathNode> pathNodeArray, Tilemap.Map map, int2 startPosition, int2 endPosition)
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                PathNode pathNode = map.GetTilePathNode(x,y);
                pathNode.SetNode( CalculateIndex(x,y), new int2(x, y), endPosition);
                pathNodeArray[pathNode.index] = pathNode;
            }
        }
    }
    public struct PathNode
    {
        public int index;
        public int x, y;
        public int fCost, gCost, hCost;
        public bool isTraversable;
        public int originNodeIndex;
        private void CalculateFCost()
        {
            fCost = gCost + hCost;
        }
        private void CalculateDistanceCost(int2 postionOne,int2 positionTwo)
        {
            int xDistance = math.abs(postionOne.x - positionTwo.x);
            int yDistance = math.abs(postionOne.y - positionTwo.y);
            int remaining = math.abs(xDistance - yDistance);
            hCost = MOVE_DIAGONAL_COST * math.min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
        }
        public void SetNode(int index, int2 postionOne, int2 positionTwo)
        {
            this.index = index;
            gCost = int.MaxValue;
            CalculateDistanceCost(postionOne, positionTwo);
            CalculateFCost();
            originNodeIndex = -1;
        }
    }
}