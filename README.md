# Unity Entity Component System Example

This is a example game done completly in the new Unity Entity Component System. It was done to show what performance gains are possible with the new Unity Entity Component System.

This code implements systems for pathfinding, enemy finite state machine, collisions (with new Unity Physics), input, inventory, weapon animation (by using slerp and lerp).

I removed all assets except for the scripts that were used in this project, 
since the meshes ive originaly used in this project might be protected by copyright. 
The meshes that are shown in the screenshot (weapon and shield) were done by me.

This was originaly a school project.

# Screenshots

## Character Creation

### Pick a class

![ClassSelect](https://user-images.githubusercontent.com/58613850/112959856-23d69080-9144-11eb-87a2-d87e7a6009da.png)

### Spend points on various stats

![ClassPoints](https://user-images.githubusercontent.com/58613850/112959893-2d5ff880-9144-11eb-9151-f1d031b2e6b7.png)

### Movement / Rotation and Jumping / Gravity

https://user-images.githubusercontent.com/58613850/112962217-6e590c80-9146-11eb-9f6c-9c547b1b1a46.mp4

### Pick up items

https://user-images.githubusercontent.com/58613850/112960021-4b2d5d80-9144-11eb-92e1-a85a801d401b.mp4

### Use Weapon and Shield

https://user-images.githubusercontent.com/58613850/112959978-4072c880-9144-11eb-8e20-a612633b1b9f.mp4

# Benchmark

The project was tested with 10'000 pathfinding agents, simultanously searching for a path. On my Ryzen 3950x it still performed with an average of 3.2 ms (update rate).
