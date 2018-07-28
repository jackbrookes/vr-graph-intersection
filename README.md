# VR graph intersection game
VR puzzle game where the goal is to create faces from nodes that do not create intersections in a 3D graph.

This project is unfinished, but I am releasing it because I won't finish it (turns out not to be that fun in my tests). Helped me learn a lot about building software systems.

This is a Unity project that works with Oculus Rift or in Desktop mode 

## Video

[Link to video on Vimeo](https://vimeo.com/282106894)

## Features

### Game

* Click and drag between two similarly colored nodes to join them with an edge
* Joining two already joined nodes removes the edge
* Creating a loop of 3 edges creates a face (triangle)
* Edges are only created if they meet these conditions:
    * Doesn't intersect with an existing face
    * Doesn't create a face that would intersect with an existing edge
    * One or more "moves" are available
* Sound and visual feedback for different outcomes 

### Development

* Procedural level generation
    * "Problems" are specified as ScriptableObjects, and a defined as a set of high-level "rules" about what the problem should look like, e.g.
        * Specification of topological structure of shapes, and their associated node type
        * Portion of additional "blocking" faces, which are apprent potential faces that could not exist in a valid solution 
        * Additional "moves" (edge) granted beyond minimum required to solve perfectly
        * Replacements of nodes with special nodes (currently a "blank" node which allows any node type to be connected to it)
    * The Graph Creator system takes problem specifications and batch converts them into a solvable set of node positions in 3D space (a "Level") inside the editor.
    * Levels are also stored as ScriptableObjects, which are accessed at runtime and used to create the required GameObjects
* Easily add new types of nodes as ScritableObjects with special behaviour
* Test in Desktop or VR mode

