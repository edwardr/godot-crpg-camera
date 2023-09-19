
using System;
//using System.Data;
//using System.Diagnostics.Metrics;
//using System.Numerics;
//using System.Reflection.Metadata;
//using System.Threading;
using Godot;

public partial class Camera : Camera2D
{

    public float MinZoom = 0.75f;
    public float MaxZoom = 1.25f;
    public float ZoomIncrement = 0.1f;
    public float TargetZoom = 1.0f;
    public float EdgeScrollMargin = 50.0f;
    public float BaseEdgeScrollSpeed = 400.0f;
    public Vector2 ViewportSize;
    public bool ProcessZoomEvent = false;
    public bool ProcessEdgeScroll = false;
    public string EdgeScrollEvent;
    public Resource CursorPoint = ResourceLoader.Load("res://assets/cursor-point.png");
    public Resource CursorGrab = ResourceLoader.Load("res://assets/cursor-grab.png");

    public override void _Ready()
    {
        this.SetViewportSize();
        this.SetMapBoundary();

        // Set up the resize signal in case viewport size changes.
        GetViewport().Connect(Viewport.SignalName.SizeChanged, Callable.From(TriggerResizeViewport));
    }

    public void SetViewportSize()
    {
        this.ViewportSize = GetViewport().GetVisibleRect().Size;
    }

    public void SetMapBoundary()
    {
        // Assumes the background top-left starts at 0,0
        var BackgroundLayer = GetParent().GetNode<Sprite2D>("Background");
        var BackgroundRect = BackgroundLayer.Texture.GetSize();
        this.LimitRight = (int)BackgroundRect[0];
        this.LimitBottom = (int)BackgroundRect[1];
        this.LimitLeft = 0;
        this.LimitTop = 0;
    }

    public void TriggerResizeViewport()
    {
        this.SetViewportSize();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (this.ProcessZoomEvent)
        {
            this.Zoom = new Vector2(TargetZoom, TargetZoom);
            this.ProcessZoomEvent = false;
        }

        if (this.ProcessEdgeScroll)
        {
            var EdgeScrollZoomModifier = this.Zoom.Y * 1.5f;
            switch (this.EdgeScrollEvent)
            {
                case "scroll_top_left":
                    this.SetScreenPosition(
                        this.GetClampedScreenValueX((this.Position.X - ((float)delta * BaseEdgeScrollSpeed) * EdgeScrollZoomModifier)),
                        this.GetClampedScreenValueY((this.Position.Y - ((float)delta * BaseEdgeScrollSpeed) * EdgeScrollZoomModifier))
                    );
                    break;
                case "scroll_bottom_left":
                    this.SetScreenPosition(
                        this.GetClampedScreenValueX((this.Position.X - ((float)delta * BaseEdgeScrollSpeed) * EdgeScrollZoomModifier)),
                        this.GetClampedScreenValueY((this.Position.Y + ((float)delta * BaseEdgeScrollSpeed) * EdgeScrollZoomModifier))
                    );
                    break;
                case "scroll_left":
                    this.SetScreenPosition(
                        this.GetClampedScreenValueX((this.Position.X - ((float)delta * BaseEdgeScrollSpeed) * EdgeScrollZoomModifier)),
                        this.Position.Y
                    );
                    break;
                case "scroll_top_right":
                    this.SetScreenPosition(
                        this.GetClampedScreenValueX((this.Position.X + ((float)delta * BaseEdgeScrollSpeed) * EdgeScrollZoomModifier)),
                        this.GetClampedScreenValueY((this.Position.Y - ((float)delta * BaseEdgeScrollSpeed) * EdgeScrollZoomModifier))
                    );
                    break;
                case "scroll_bottom_right":
                    this.SetScreenPosition(
                        this.GetClampedScreenValueX((this.Position.X + ((float)delta * BaseEdgeScrollSpeed) * EdgeScrollZoomModifier)),
                        this.GetClampedScreenValueY((this.Position.Y + ((float)delta * BaseEdgeScrollSpeed) * EdgeScrollZoomModifier))
                    );
                    break;
                case "scroll_right":
                    this.SetScreenPosition(
                        this.GetClampedScreenValueX((this.Position.X + ((float)delta * BaseEdgeScrollSpeed) * EdgeScrollZoomModifier)),
                        this.Position.Y
                    );
                    break;
                case "scroll_bottom":
                    this.SetScreenPosition(
                        this.GetClampedScreenValueX((this.Position.X)),
                        this.GetClampedScreenValueY((this.Position.Y + ((float)delta * BaseEdgeScrollSpeed) * EdgeScrollZoomModifier))
                    );
                    break;
                case "scroll_top":
                    this.SetScreenPosition(
                        this.GetClampedScreenValueX(this.Position.X),
                        this.GetClampedScreenValueY((this.Position.Y - ((float)delta * BaseEdgeScrollSpeed) * EdgeScrollZoomModifier))
                    );
                    break;
            }
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton eventMouseButton)
        {
            if (@event.IsPressed())
            {
                switch (eventMouseButton.ButtonIndex)
                {
                    case MouseButton.WheelDown:
                        ZoomIn();
                        break;
                    case MouseButton.WheelUp:
                        ZoomOut();
                        break;
                    case MouseButton.Middle:
                        Input.SetCustomMouseCursor(CursorGrab);
                        break;
                }
            }
            else
            {
                if (eventMouseButton.ButtonIndex == MouseButton.Middle)
                {
                    Input.SetCustomMouseCursor(CursorPoint);
                }
            }
        }

        if (@event is InputEventMouseMotion eventMouseMotion)
        {
            var MousePosition = GetViewport().GetMousePosition();
            this.DetectEdgeScroll(MousePosition.X, MousePosition.Y);

            if (eventMouseMotion.ButtonMask == MouseButtonMask.Middle)
            {
                var PositionX = this.GetClampedScreenValueX(this.Position.X - eventMouseMotion.Relative[0]);
                var PositionY = this.GetClampedScreenValueY(this.Position.Y - eventMouseMotion.Relative[1]);

                this.SetScreenPosition(PositionX, PositionY);
            }
        }
    }

    public void DetectEdgeScroll(float x, float y)
    {
        if (x <= EdgeScrollMargin)
        {
            if (y <= EdgeScrollMargin)
            {
                this.ProcessEdgeScroll = true;
                this.EdgeScrollEvent = "scroll_top_left";
            }
            else if (y >= this.ViewportSize.Y - EdgeScrollMargin)
            {
                this.ProcessEdgeScroll = true;
                this.EdgeScrollEvent = "scroll_bottom_left";
            }
            else
            {
                this.ProcessEdgeScroll = true;
                this.EdgeScrollEvent = "scroll_left";
            }
        }
        else if (x >= (this.ViewportSize.X - EdgeScrollMargin))
        {
            if (y <= EdgeScrollMargin)
            {
                this.ProcessEdgeScroll = true;
                this.EdgeScrollEvent = "scroll_top_right";
            }
            else if (y >= this.ViewportSize.Y - EdgeScrollMargin)
            {
                this.ProcessEdgeScroll = true;
                this.EdgeScrollEvent = "scroll_bottom_right";
            }
            else
            {
                this.ProcessEdgeScroll = true;
                this.EdgeScrollEvent = "scroll_right";
            }
        }
        else if (y < (EdgeScrollMargin))
        {
            this.ProcessEdgeScroll = true;
            this.EdgeScrollEvent = "scroll_top";
        }
        else if (y >= (this.ViewportSize.Y - EdgeScrollMargin))
        {
            this.ProcessEdgeScroll = true;
            this.EdgeScrollEvent = "scroll_bottom";
        }
        else
        {
            this.ProcessEdgeScroll = false;
            this.EdgeScrollEvent = "";
        }
    }

    public void SetScreenPosition(float? x, float? y)
    {
        this.Position = new Vector2(x ?? this.Position.X, y ?? this.Position.Y);
    }

    public float GetClampedScreenValueX(float xPos)
    {
        /**
		* Work around a longtime bug with Camera2D 
		* that makes it ignore your edge limits
		* which causes the camera to get stuck on edges.
		* 
		* @see https://github.com/godotengine/godot/issues/62441
		*/

        return Mathf.Clamp(
             xPos,
             (LimitLeft + (int)(this.ViewportSize[0] / 2 / this.Zoom.X)),
             (LimitRight - (int)(this.ViewportSize[0] / 2 / this.Zoom.X))
         );
    }

    public float GetClampedScreenValueY(float yPos)
    {
        /**
		* Work around a longtime bug with Camera2D 
		* that makes it ignore your edge limits
		* which causes the camera to get stuck on edges.
		* 
		* @see https://github.com/godotengine/godot/issues/62441
		*/

        return Mathf.Clamp(
            yPos,
            (LimitTop + (int)(this.ViewportSize[1] / 2 / this.Zoom.X)),
            (LimitBottom - (int)(this.ViewportSize[1] / 2 / this.Zoom.X))
        );
    }

    public void ZoomIn()
    {
        TargetZoom = Mathf.Max(TargetZoom - ZoomIncrement, MinZoom);
        this.ProcessZoomEvent = true;
    }

    public void ZoomOut()
    {
        TargetZoom = Mathf.Min(TargetZoom + ZoomIncrement, MaxZoom);
        this.ProcessZoomEvent = true;
    }
}