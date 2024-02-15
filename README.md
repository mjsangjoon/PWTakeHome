# Unity Pathfinding Take-Home Project

Welcome to the Pocket Worlds Unity Pathfinding Take-Home Project! In this repository, we'd like you to demonstrate your engineering skills by creating a small Unity project that implements pathfinding. While Unity provides a built-in NavMesh system for pathfinding, this test will challenge you to create your own custom pathfinding solution for a 2D grid.

This project will serve as the primary jumping off point for our technical interviews.

## Project Description
We've already put together a project that has the foundations of what you need to get started. Please use it as a starting point and build your solution on top of it.

Your task is to build a Unity project that meets the following requirements:

1. **Pathfinding Implementation**: Develop a custom pathfinding system without using Unity's NavMesh system. You are free to choose any pathfinding algorithm you prefer, such as A* or Dijkstra's algorithm.

3. **2D Grid**: Implement the pathfinding on a 2D grid where each cell represents either an impassable obstacle or a clear area. You have the flexibility to determine how this grid data is populated. Options include creating an editor tool to author it, automatically generating it from scene geometry, procedural generation, or any other method of your choice.

4. **Path Smoothing**: Pathfinding on a 2D grid can result in stair-stepped paths that look unnatural when followed directly. Your character should use a method to smooth out the path following to appear more natural, similar to how a human would move.

## Getting Started
To begin the project, follow these steps:

1. Clone this repository to your local machine:

   ```shell
   git clone https://github.com/pocketzworld/unity-tech-test.git

2. Create a Unity project or use an existing one.

3. Build the custom pathfinding system within your Unity project according to the project requirements outlined above.

4. Implement player control, allowing the character to follow the calculated path when the player clicks on the screen.

5. Ensure the path follows a natural-looking trajectory by implementing path smoothing.

6. Test your project thoroughly to ensure it meets the specified requirements.

7. Document your code, providing clear comments and explanations of your implementation choices.

8. Beyond this, feel free to add whatever other bells and whistles to the project you'd like. This is your opportunity to show off what you can do.

## Submission Guidelines
When you have completed the project, please follow these guidelines for submission:

1. Commit and push your code to your GitHub repository.

2. Update this README with any additional instructions, notes, or explanations regarding your implementation, if necessary.

3. Provide clear instructions on how to run and test your project.

4. Share the repository URL with the hiring team or interviewer.

## Additional information about this implementation
Remarks
I took "grid" to be quite literal, and implemented a grid system that can be used by a non-technical user with a little guidance.
I chose to make the objects and character snap to the nodes that were defined in the grid system to make it easier to debug
and work with.
In the inspector for the NavGrid, I added additional fields to choose how many cells per side the user would like.
That in turn will change the size of the grid and number of possible node locations.
I chose to use A* as the pathfinding
algorithm as it is the most widely used algorithm in video games, and I did not see myself being put in situations that would
make A* unoptimal.
The pathsmoothing algorithm I chose combines straight and curved path sections into the path that the 
player character takes. The curved sections are created by imagining a circle with tangent points at turns that are created
by the path given by A*. It performs well when linking multiple curved sections together, but falls short when linking straight
pieces with curved sections or longer routes. This is possibly due to the curved sections not being fully followed through
to the start of the next section of the path.

How to run and test the project
The project has a scene with objects that were generated and placed by code in editor (see below). Open the scene and
press play to begin testing the project. You can click around the plane to move to that location.
To test the project, please follow the directions below to add more obstacles.

Obstacles
- The user can add obstacles to the scene randomly by selecting a number of obstacles to add, and then right clicking the
field and selecting the "Add this many obstacles to grid" option. The grid will avoid adding any obstacles on top of ones
already in the obstacle list, as well as avoiding the grid spot closest to the player when called. It will first add the
locations to the Obstacle Locations array, and then instantiate an obstacle prefab at each location.
- The populating of obstacles unfortunately does not save unless something else in the inspector window is changed.
- To remove obstacles from the map, the user must delete the object from the screen as well as removing the object's location
from the Obstacle Locations array.
- The user can add obstacle locations into the array directly, but it is not advised to do so at this time as there is no 
easy way to add the prefab object to the scene by adding the location into the array (I would love to look into ways to do
this however). It may also break the pathfinding algorithm as the algorithm assumes a constant Y value at the height of the
player's transform.
- The user is advised to remove all obstacles from the scene when resizing the number of cells, as the underlying grid does
not currently take into account any obstacles on the field.



## Additional Information

Feel free to be creative in how you approach this project. Your solution will be evaluated based on code quality, efficiency, and how well it meets the specified requirements.

Good luck, and we look forward to seeing your Unity pathfinding project! If you have any questions or need clarifications, please reach out to us.
