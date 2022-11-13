using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace BrunoMikoski.Pathfinder
{
    public class SimplePathfinder
    {
        private const int MOVE_COST = 10;
        private const int MAXIMUM_SIMULTANEOUS_QUERY = 30;
        
        private static SimplePathfinder instance;
        private static SimplePathfinder Instance
        {
            get
            {
                if (instance == null)
                    instance = new SimplePathfinder();
                return instance;
            }
        }

        private struct PathQueryData
        {
            public readonly int2 StartPosition;
            public readonly int2 EndPosition;
            public readonly Action<PathResultData> Callback;
            public int QueryID;

            public PathQueryData(int2 startPosition, int2 endPosition, Action<PathResultData> callback)
            {
                StartPosition = startPosition;
                EndPosition = endPosition;
                Callback = callback;
                QueryID = -1;
            }
        }
        
        
        public enum PathQueryResult
        {
            Complete,
            Incomplete,
            Invalid
        }

        public struct PathResultData
        {
            public readonly PathQueryResult PathQueryResult;
            public readonly int2 StartPosition;
            public readonly int2 EndPosition;
            public readonly int2[] PathPositions;

            public PathResultData(PathQueryResult pathQueryResult, int2 startPosition, int2 endPosition, int2[] pathPositions)
            {
                PathQueryResult = pathQueryResult;
                StartPosition = startPosition;
                EndPosition = endPosition;
                PathPositions = pathPositions;
            }
        }
        
        private Queue<PathQueryData> pathQueryQueue;
        private List<PathQueryData> runningQueries;
        private List<JobHandle> runningQueriesJobHandle;
        private List<PathQueryJob> runningJobs;
        
        private NativeArray<int2> NEIGHBOURS_OFFSET;
        private Queue<NativeArray<PathNode>> gridNodesPool;
        private NativeList<int> queryIDs;


        private int2 gridSize = new int2(100, 100);

        private PathfinderBehaviour cachedPathFinderBehaviour;
        private PathfinderBehaviour PathfinderBehaviour
        {
            get
            {
                if (cachedPathFinderBehaviour == null)
                {
                    GameObject pathFinderBehaviourGameObject = new GameObject("PathfinderBehaviour");
                    cachedPathFinderBehaviour = pathFinderBehaviourGameObject.AddComponent<PathfinderBehaviour>();
                }

                return cachedPathFinderBehaviour;
            }
        }


        public static void Setup(int2 gridSize)
        {
            Instance.SetupInternal(gridSize);
        }

        private void SetupInternal(int2 gridSize)
        {
            this.gridSize = gridSize;
            
            pathQueryQueue = new Queue<PathQueryData>();
            
            runningQueries = new List<PathQueryData>(MAXIMUM_SIMULTANEOUS_QUERY);
            
            runningQueriesJobHandle = new List<JobHandle>(MAXIMUM_SIMULTANEOUS_QUERY);
            runningJobs = new List<PathQueryJob>(MAXIMUM_SIMULTANEOUS_QUERY);
            gridNodesPool = new Queue<NativeArray<PathNode>>(MAXIMUM_SIMULTANEOUS_QUERY);
            queryIDs = new NativeList<int>(MAXIMUM_SIMULTANEOUS_QUERY, Allocator.Persistent);
            NEIGHBOURS_OFFSET = new NativeArray<int2>(4, Allocator.Persistent)
            {
                [0] = new int2(-1, 0), 
                [1] = new int2(+1, 0), 
                [2] = new int2(0, +1), 
                [3] = new int2(0, -1)
            };

            for (int i = 0; i < MAXIMUM_SIMULTANEOUS_QUERY; i++)
            {
                queryIDs.Add(i);
            }

            CreateGridPool();

            PathfinderBehaviour.StartCoroutine(CheckForJobCompletionEnumerator());
        }

        private IEnumerator CheckForJobCompletionEnumerator()
        {
            while (true)
            {
                bool tryToResolveQueue = false;
                for (var i = runningQueriesJobHandle.Count - 1; i >= 0; i--)
                {
                    JobHandle jobHandle = runningQueriesJobHandle[i];
                    if (!jobHandle.IsCompleted) 
                        continue;
                    
                    jobHandle.Complete();

                    PathQueryJob pathQueryJob = runningJobs[i];
                    PathQueryData queryData = GetPathQueryById(pathQueryJob.QueryID);
                        
                    //cannot find the query by the path ID
                    if (queryData.QueryID == -1)
                        continue;

                    PathResultData pathResult = new PathResultData(
                        (PathQueryResult) pathQueryJob.PathResult[0],
                        queryData.StartPosition, 
                        queryData.EndPosition, 
                        pathQueryJob.ResultPath.ToArray());

                    pathQueryJob.ResultPath.Dispose();
                    pathQueryJob.PathResult.Dispose();


                    runningQueriesJobHandle.RemoveAtSwapBack(i);
                    runningJobs.RemoveAtSwapBack(i);
                    runningQueries.RemoveAtSwapBack(i);
                    gridNodesPool.Enqueue(pathQueryJob.Grid);
                    queryIDs.Add(pathQueryJob.QueryID);
                    
                    queryData.Callback?.Invoke(pathResult);
                    tryToResolveQueue = true;
                }

                if (tryToResolveQueue)
                    ResolveQueue();
                
                yield return null;
            }
        }

        private PathQueryData GetPathQueryById(int jobID)
        {
            for (var i = 0; i < runningQueries.Count; i++)
            {
                PathQueryData pathQueryData = runningQueries[i];
                if (pathQueryData.QueryID == jobID)
                    return pathQueryData;
            }
            return new PathQueryData();
        }

        private struct PathNode
        {
            public int2 Position;

            public int Index;

            public int GCost;
            public int HCost;
            public int FCost;

            public bool IsWalkable;

            public int CameFromNodeIndex;

            public void CalculateFCost()
            {
                FCost = GCost + HCost;
            }

            public void SetIsWalkable(bool isWalkable)
            {
                this.IsWalkable = isWalkable;
            }
        }

        private void CreateGridPool()
        {
            for (int i = 0; i < MAXIMUM_SIMULTANEOUS_QUERY; i++)
            {
                NativeArray<PathNode> gridNodes = new NativeArray<PathNode>(gridSize.x * gridSize.y,Allocator.Persistent);

                for (int x = 0; x < gridSize.x; x++)
                {
                    for (int y = 0; y < gridSize.y; y++)
                    {
                        PathNode pathNode = new PathNode
                        {
                            Position = new int2(x, y),
                            Index = CalculateIndex(x, y), 
                            IsWalkable = true
                        };
                        gridNodes[pathNode.Index] = pathNode;
                    }
                }

                gridNodesPool.Enqueue(gridNodes);
            }

        }

        private int CalculateIndex(int positionX, int positionY)
        {
            return positionX + positionY * gridSize.x;
        }
        
        public static void GetPath(int2 startPosition, int2 finalPosition, Action<PathResultData> callback)
        {
            Instance.AddToQueue(new PathQueryData(startPosition, finalPosition, callback));
        }

        private void AddToQueue(PathQueryData pathQueryData)
        {
            pathQueryQueue.Enqueue(pathQueryData);
            ResolveQueue();
        }

        private void ResolveQueue()
        {
            if (runningQueries.Count >= MAXIMUM_SIMULTANEOUS_QUERY)
                return;

            PathQueryData query = pathQueryQueue.Dequeue();
            NativeArray<PathNode> grid = gridNodesPool.Dequeue();
            int queryID = queryIDs[0];
            queryIDs.RemoveAtSwapBack(0);

            query.QueryID = queryID;
            PathQueryJob pathQueryJob = new PathQueryJob()
            {
                StartPosition = query.StartPosition,
                EndPosition = query.EndPosition,
                Grid = grid,
                GridSize = gridSize,
                NeighboursOffset = NEIGHBOURS_OFFSET,
                QueryID = queryID
            };

            runningJobs.Add(pathQueryJob);
            runningQueriesJobHandle.Add(pathQueryJob.Schedule());
            runningQueries.Add(query);
        }

        private struct PathQueryJob : IJob
        {
            public int QueryID;

            public int2 StartPosition;
            public int2 EndPosition;
            public NativeArray<PathNode> Grid;
            public int2 GridSize;
            [ReadOnly]
            public NativeArray<int2> NeighboursOffset;

            [WriteOnly]
            public NativeArray<int2> ResultPath;
            [WriteOnly]
            public NativeArray<int> PathResult;

            private PathQueryResult pathQueryResult;

            public void Execute()
            {
                PrepareNodes();
                
                int startPositionIndex = CalculateIndex(StartPosition);
                PathNode startPathNode = Grid[startPositionIndex];

                int endPositionIndex = CalculateIndex(EndPosition);
                
                startPathNode.GCost = 0;
                startPathNode.CalculateFCost();
                Grid[startPositionIndex] = startPathNode;

                NativeList<int> openList = new NativeList<int>(Allocator.Temp);
                NativeList<int> closedList = new NativeList<int>(Allocator.Temp);

                openList.Add(startPositionIndex);

                while (openList.Length > 0)
                {
                    int currentNodeIndex = GetLowestCostFNodeIndex(openList);
                    PathNode currentNode = Grid[currentNodeIndex];

                    if (currentNodeIndex == endPositionIndex)
                        break;

                    for (int i = 0; i < openList.Length; i++)
                    {
                        if (openList[i] == currentNodeIndex)
                        {
                            openList.RemoveAtSwapBack(i);
                            break;
                        }
                    }

                    closedList.Add(currentNodeIndex);

                    for (int i = 0; i < NeighboursOffset.Length; i++)
                    {
                        int2 neighbourOffset = NeighboursOffset[i];
                        int2 neighbourPosition = new int2(currentNode.Position.x + neighbourOffset.x,
                            currentNode.Position.y + neighbourOffset.y);

                        if (!IsPositionInsideGrid(neighbourPosition))
                            continue;

                        int neighbourNodeIndex = CalculateIndex(neighbourPosition);

                        if (closedList.Contains(neighbourNodeIndex))
                            continue;

                        PathNode neighbourNode = Grid[neighbourNodeIndex];
                        if (!neighbourNode.IsWalkable)
                            continue;

                        int tentativeGCost = currentNode.GCost +
                                             CalculateDistanceCost(currentNode.Position, neighbourPosition);
                        if (tentativeGCost < neighbourNode.GCost)
                        {
                            neighbourNode.CameFromNodeIndex = currentNodeIndex;
                            neighbourNode.GCost = tentativeGCost;
                            neighbourNode.CalculateFCost();
                            Grid[neighbourNodeIndex] = neighbourNode;

                            if (!openList.Contains(neighbourNode.Index))
                            {
                                openList.Add(neighbourNode.Index);
                            }
                        }
                    }
                }

                PathNode endNode = Grid[endPositionIndex];
                if (endNode.CameFromNodeIndex == -1)
                {
                    PathResult[0] = (int) PathQueryResult.Invalid;
                }
                else
                {
                    ResultPath = CalculatePath(endNode);
                    PathResult[0] = (int) PathQueryResult.Complete;
                }
                
                openList.Dispose();
                closedList.Dispose();
            }

            private void PrepareNodes()
            {
                for (var i = 0; i < Grid.Length; i++)
                {
                    PathNode pathNode = Grid[i];
                    pathNode.GCost = int.MaxValue;
                    pathNode.HCost = CalculateDistanceCost(pathNode.Position, EndPosition);
                    pathNode.CalculateFCost();

                    pathNode.CameFromNodeIndex = -1;
                    Grid[i] = pathNode;
                }
            }

            private int CalculateIndex(int2 position) 
            {
                return position.x + position.y * GridSize.x;
            }

            private NativeList<int2> CalculatePath(PathNode endNode)
            {
                NativeList<int2> path = new NativeList<int2>(Allocator.Temp)
                {
                    endNode.Position
                };

                PathNode currentNode = endNode;
                while (currentNode.CameFromNodeIndex != -1)
                {
                    PathNode cameFromNode = Grid[currentNode.CameFromNodeIndex];
                    path.Add(cameFromNode.Position);
                    currentNode = cameFromNode;
                }

                return path;
            }

            private bool IsPositionInsideGrid(int2 gridPosition)
            {
                return
                    gridPosition.x >= 0 &&
                    gridPosition.y >= 0 &&
                    gridPosition.x < GridSize.x &&
                    gridPosition.y < GridSize.y;
            }


            private int CalculateDistanceCost(int2 fromPosition, int2 toPosition)
            {
                int xDistance = math.abs(fromPosition.x - toPosition.x);
                int yDistance = math.abs(fromPosition.y - toPosition.y);
                int remaining = math.abs(xDistance - yDistance);
                return MOVE_COST * math.min(xDistance, yDistance) + MOVE_COST * remaining;
            }


            private int GetLowestCostFNodeIndex(NativeList<int> openList)
            {
                PathNode lowestCostPathNode = Grid[openList[0]];
                for (int i = 1; i < openList.Length; i++)
                {
                    PathNode testPathNode = Grid[openList[i]];
                    if (testPathNode.FCost < lowestCostPathNode.FCost)
                    {
                        lowestCostPathNode = testPathNode;
                    }
                }

                return lowestCostPathNode.Index;
            }
        }
    }
}
