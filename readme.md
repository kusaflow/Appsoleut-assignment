Task 1: Make an Object Follow a Path
Features
Flexible Path of Waypoints: The path is defined by a set of waypoints that the object follows.
Object Orientation: The object automatically adjusts its orientation to face the direction of movement.
Smooth/Curved Path (Optional): Bezier curves for a more natural movement.
Path Visualization (Optional): The path is visualized using a LineRenderer to show the trajectory.’
For simple path you can add gameobjects “points_Simple” or similar object that will have the script called : “Simple_Path.cs” and a LineRenderer.

For curve path, you can add the “CurvePointSet” gameobject to a parent that contains CurvePath.cs or in my scene its called - points_Curve.
CurvePointSet contains 4 points start, end and 2 points for control the curveness.
So we can have many curve gameobject (CurvePointSet) to the Parent. And player will transit from one curve to another.

Task 2: Place Objects in a 2D Grid
In the Mandatory scene : we can spawn gameobjects by entering rows and columns and spacing too.And after that we can press the regenerate button to spawn data of the grid.
In this scene there is no instancing so it takes time to load .

In Optional Part : we optimized render time and also gpu instancing, so in this scene we can load data of the grid faster and also it gives us more frame rate because we are using gpu instancing and batching.



Task 3: Bounce an object Inside a closed shape room (no gravity)

Scene 1 : using rigid body without gravity to simulate infinite bounce.
Scene 2 : only mesh render without collider and rigidbody without gravity to simulate infinite bounce.
Scene 3 : using jelly like to simulate the same.



Task 4: Solar System up to 7 planets
 So in this scene we can visualize the path of planets and moons.
The planet also rotates on its axis like earth at 23 deg. Also it revolves around the sun. there are moons that revolve around the planets too.
In the ui we can select some predefined location from the dropdown to go there.
We can toggle the path to set its visibility.
Also we can select planets so follow them. And when following we can move camera close and far with the slider.









 

