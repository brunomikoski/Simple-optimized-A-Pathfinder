[logo]: https://thumbs.gfycat.com/FrigidFearfulDrake-size_restricted.gif "Final road creation result"

# Simple-optimized-A-Pathfinder

A simple pathfinder that I tryed to optimize the maximum as I can, and share what I've discovered :)

The idea was pretty simple, implement a simple A* Pathfinder and Profile using [Unity Profiler](https://docs.unity3d.com/Manual/Profiler.html) and [my own method](https://github.com/badawe/Simple-optimized-A-Pathfinder/blob/master/Assets/%5BPathfinding%5D/Scripts/Profiler/ProfilerController.cs) to track the average time when running it a 1000 times. 

###Final result:

![alt text][logo]


### Performance Comparison

| Step          | Average Ticks (AT) |  GC   | GetPath() MS | AT to original (%) | AT to previous step (%) | GC to Original | GC to previous step | GetPath MS to original | GetPath() to Previous step | Commit | 
| ------------- |:------------------:|:-----:| :----------: | :----------------: | :---------------------: | :------------: | :-----------------: | :--------------------: | :------------------------: | :-----:|                    
| Original      | 25027              |  40,6 |     29,5     |                    |                         |                |                     |                        |                            |[Used Commit](https://github.com/badawe/Simple-optimized-A-Pathfinder/commit/dde01c8d3a9e751197874476b1f812770574d364)|
| Dictionary as index | 17292|50,3|20|-30,91%|-30,91%|+23,89%|                     |                        |                            |        |
