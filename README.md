[logo]: https://thumbs.gfycat.com/FrigidFearfulDrake-size_restricted.gif "Final road creation result"

# Simple-optimized-A-Pathfinder

A simple pathfinder that I tryed to optimize the maximum as I can, and share what I've discovered :)

The idea was pretty simple, implement a simple A* Pathfinder and Profile using [Unity Profiler](https://docs.unity3d.com/Manual/Profiler.html) and [my own method](https://github.com/badawe/Simple-optimized-A-Pathfinder/blob/master/Assets/%5BPathfinding%5D/Scripts/Profiler/ProfilerController.cs) to track the average time when running it a 1000 times. 

### Final result:

![alt text][logo]


### Performance Comparison

|Action|GC |GetPath() MS| GC to original | GC to previous | GetPath() to original | GetPathToPrevious | Commit | 
|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|
|Original|40,6 KB| 29,87 ms|  ||||[Commit](https://github.com/badawe/Simple-optimized-A-Pathfinder/commit/4b77634dec8e922f26b224fe5ce90e159ec52c77)|
|Step 1|50,3 KB| 20,14 ms| +23,89% |+23,89%|-32,57%|-32,57%|[Commit](https://github.com/badawe/Simple-optimized-A-Pathfinder/commit/63ef36d72ef2b8b833d8b209f65e8ce607fd3889)|
|Step 2|50,3 KB| 16,95 ms| +23,89% |0%|-43,25%|-15,87%|[Commit](https://github.com/badawe/Simple-optimized-A-Pathfinder/commit/f2c6cb6bc46176e951a5cb66fb85144a8434728a)|
|Step 3|36,2 KB| 16,92 ms| -10,14% |-28,03%|-43,35%|-0,18%|[Commit](https://github.com/badawe/Simple-optimized-A-Pathfinder/commit/98646c06cb17f7748b66bfac7d12291a76b8eee1)|
|Step 4|32 KB| 10,97 ms| -21,18% |-11,60%|-63,27%|-35,17%|[Commit](https://github.com/badawe/Simple-optimized-A-Pathfinder/commit/3c865bf9fe04f170fee3557f2a3a0fda07a07539)|
|Step 5|32 KB| 9,91 ms| -21,18% |0%|-66,82%|-9,66%|[Commit](https://github.com/badawe/Simple-optimized-A-Pathfinder/commit/ec80d08662755292acf5ea1cbd0710c9925082a7)|
|Step 6|32 KB| 10,11 ms| -21,18% |0%|-66,15%|-2,02%|[Commit](https://github.com/badawe/Simple-optimized-A-Pathfinder/commit/7b1b9f4ad7543df6687f9f61a07a2d7694dff8b1)|
|Step 7|22,1 KB| 7,53 ms| -45,57% |-30,94%|-74,79%|-25,52%|[Commit](https://github.com/badawe/Simple-optimized-A-Pathfinder/commit/dfb86e9209cd879eeb7917fb4789414b72f1e617)|
|Step 8|22,1 KB| 7,46 ms| -45,57% |0%|-75,03%|-0,93%|[Commit](https://github.com/badawe/Simple-optimized-A-Pathfinder/commit/d9520b83675de5069259ae4393d11d4b314b3b41)|
|Step 9|12,6 KB| 7,46 ms| -68,97% |-42,99%|-75,03%|0%|[Commit](https://github.com/badawe/Simple-optimized-A-Pathfinder/commit/2c57ba982181c091b1dedb213e58d83f3b9ede48)|
|Step 10|12,6 KB| 7,32 ms| -68,97% |0%|-75,49%|-1,88%|[Commit](https://github.com/badawe/Simple-optimized-A-Pathfinder/commit/78ffb6e8a08db978f8a8e43b6e52f93b17b40d5d)|
|Step 11|4,2 KB| 6,59 ms| -89,66% |-66,67%|-77,94%|-9,97%|[Commit](https://github.com/badawe/Simple-optimized-A-Pathfinder/commit/a2f9ab2965185d75f0c903f5f9ef5aa37be88a97)|
|Step 12|0 KB| 4,82 ms| -100% |-100%|-83,86%|-26,86%|[Commit](https://github.com/badawe/Simple-optimized-A-Pathfinder/commit/a2f9ab2965185d75f0c903f5f9ef5aa37be88a97)|


#### Steps Descriptions
Step 1 - Dictionary used as quick acess to to check if a tile is inside OpenList
Step 2 - Caching index when comparing best tile from open list, and using it to remove it from list
Step 3 - Static neighbours array
Step 4 - Add FastPriorityQueue as OpenList
Step 5 - Replacing Vector2Int to int for X and Y position
Step 6 - Using reversed for instead of foreach on the neighbours array
Step 7 - Removing Dictionary for open list, since FastQueue its doing the same
Step 8 - Change return List to be List<Tile> from Tile<Vector2Int>
Step 9 - Using only F cost when adding to priority queue
Step 10 - Removing the reverse method from the GetPath
Step 11 - Removing closedList and ussing a Toggle inside the Tile itself
Step 12 - Increasing return list size to be 20% of the available tiles on the map
