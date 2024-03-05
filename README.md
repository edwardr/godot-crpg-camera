# CRPG style camera system for Godot C#

This is a camera module to replicate the type of 2D camera you would find in classic isometric CRPGs like Baldur's Gate / Icewind Dale, Planescape Torment, and Fallout.

It allows for a variety of movement options over a large 2D background, including mouse-wheel zooming, screen-edge scrolling, and click-and-drag movement with the middle-mouse button.

Area map background is courtesy of [user Aranthor of the Gibberlings3](https://www.gibberlings3.net/forums/topic/23598-old-and-forgotten-iwd-mod-art-give-away/). 

If you just want to test the camera functionality, download a build under Releases. Hit ESC or click on the rug at the South end of the map to exit the game.

This is built for Godot C# 4.1+, although you could easily adapt it for 3.5 Mono if you update the references to the `MouseButton` enum in the Camera.cs `_UnhandledInput` method. [These references were renamed and overhauled in v4.0.](https://docs.godotengine.org/en/3.5/classes/class_%40globalscope.html#enum-globalscope-buttonlist)

https://github.com/edwardr/godot-csharp-crpg-camera/assets/2935628/68d24460-66da-4616-8ed7-681b0ad9e68b

