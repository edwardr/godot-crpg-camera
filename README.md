# CRPG style camera system for Godot C#

This is a camera module to replicate the type of 2D camera you would find in classic isometric CRPGs like Baldur's Gate / Icewind Dale, Planescape Torment, and Fallout.

It allows for a variety of movement options over a large 2D background, including mouse-wheel zooming, screen-edge scrolling, and click-and-drag movement with the middle-mouse button.

It is built for Godot C# 4.1+, although you could easily adapt it for 3.5 Mono if you update the references to the `MouseButton` enum in the Camera.cs `_UnhandledInput` method. [These references were renamed and overhauled in v4.0.](https://docs.godotengine.org/en/3.5/classes/class_%40globalscope.html#enum-globalscope-buttonlist)

~~The background image belongs to Bioware, so that cannot be used, but feel free to use this otherwise in any project.~~ Sample background image has been updated. 

There is also a sample demo build in the /builds directory, if you just want to test the camera functionality. Hit ESC or click on the staircase in the lower-right corner of the map to exit the "game."

https://github.com/edwardr/godot-csharp-crpg-camera/assets/2935628/68d24460-66da-4616-8ed7-681b0ad9e68b

