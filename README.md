Simulair-Core
==============
Introduction
------------

This is a collection of projects (bindings, code generator, examples and more) for writing ROS2
applications for C# specificly targeted at Unity.

Platform support
----------------
This project curretly supports Ubuntu 18.04. It should be possible to get it working on other systems as well but not tested yet.


Running
-------
The ROS2 integration in UnityRos2 requires that some enviroment variables are set. (This has to do with the finding and linking of `.so` and `.dll` binaries to C# code)

A script called `start_editor.py` (requires python3) sets these environment variables and starts the Editor. **Don't** use sudo. This is only way to start the Editor with ROS2 support at the moment. Opening the project through Unity Hub is not supported yet.

There is an example Scene named `RosNavigationExample` in this project for sending navigation goals to a turtlebot3 using the navigation2 stack.

Tutorials
---------
### Getting Started
- **Tutorial 1:** [Install prerequisites, development tools and run the project.](https://www.notion.so/soarrobotics/Setting-up-the-Simulair-Core-Development-Workspace-4517944d1e084c6da3f5db447df8ed8b)
- **Tutorial 2:** [Write a simple subscriber inside unity.](https://www.notion.so/soarrobotics/Create-a-Simple-Listener-Using-Ros2-Unit-61c4d5760b8a419ca0d240811b8de165)
