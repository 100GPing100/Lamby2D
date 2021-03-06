﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lamby2D;
using Lamby2D.Core;
using Lamby2D.Drawing;
using Lamby2D.Input;
using Lamby2D.Physics;

namespace DemoLamby2D
{
    class ImageGameObject : GameObject, IDrawable, IMouseAware, ITickable, IStaticPhysicsObject
    {
        // Properties
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Center { get; set; }
        public Vector2 Scale { get; set; }
        public float Rotation { get; set; }
        public int ZIndex { get; set; }
        public bool IsHitTestVisible { get; set; }
        public CollisionPrimitive Collider { get; set; }
        public bool IsSolid { get; set; }
        public Color Color { get; set; }
        public DrawableKind DrawableKind { get; set; }
        public Sprite Sprite { get; set; }
        public float Width
        {
            get
            {
                return (this.DrawableKind == DrawableKind.Texture
                                ? this.Texture.Width
                                : this.DrawableKind == DrawableKind.Sprite
                                        ? this.Sprite.FrameHeight
                                        : 0);
            }
        }
        public float Height
        {
            get
            {
                return (this.DrawableKind == DrawableKind.Texture
                                ? this.Texture.Height
                                : this.DrawableKind == DrawableKind.Sprite
                                        ? this.Sprite.FrameWidth
                                        : 0);
            }
        }
        public bool MoveWithInput { get; set; }

        // Events
        public event MouseButtonEventHandler MouseDown;
        public event MouseButtonEventHandler MouseUp;
        public event MouseMotionEventHandler MouseEnter;
        public event MouseMotionEventHandler MouseLeave;

        // Public
        public bool MouseHitTest(Point position)
        {
            Vector2 world = Game.Current.Graphics.ScreenToWorld(position);
            if (world.IsNaN() == true) {
                return false;
            }

            float x0, y0;
            float x1, y1;

            if (Scale.X < 0) {
                x1 = this.Position.X - this.Center.X * this.Scale.X * this.Width;
                x0 = x1 + this.Width * this.Scale.X;
            } else {
                x0 = this.Position.X - this.Center.X * this.Scale.X * this.Width;
                x1 = x0 + this.Width * this.Scale.X;
            }

            if (Scale.Y < 0) {
                y1 = this.Position.Y - this.Center.Y * this.Scale.Y * this.Height;
                y0 = y1 + this.Height * this.Scale.Y;
            } else {
                y0 = this.Position.Y - this.Center.Y * this.Scale.Y * this.Height;
                y1 = y0 + this.Height * this.Scale.Y;
            }

            return (world.X >= x0 && world.X <= x1 && world.Y >= y0 && world.Y <= y1);
        }
        public void OnMouseDown(MouseButtonEventArgs e)
        {
            if (this.MouseDown != null) {
                this.MouseDown(this, e);
            }
        }
        public void OnMouseUp(MouseButtonEventArgs e)
        {
            if (this.MouseUp != null) {
                this.MouseUp(this, e);
            }
        }
        public void OnMouseEnter(MouseMotionEventArgs e)
        {
            this.Color = new Color(1.0f, 0.25f);
            if (this.MouseEnter != null) {
                this.MouseEnter(this, e);
            }
        }
        public void OnMouseLeave(MouseMotionEventArgs e)
        {
            this.Color = Colors.White;
            if (this.MouseLeave != null) {
                this.MouseLeave(this, e);
            }
        }
        public virtual void Update(float DeltaTime)
        {
            if (this.MoveWithInput == false) {
                return;
            }

            float speed = 200.0f;

            if (this.Sprite.CurrentAnimation != "Ass" && (Game.Current.Input.IsKeyDown(KeyCode.A) || Game.Current.Input.IsKeyDown(KeyCode.D) || Game.Current.Input.IsKeyDown(KeyCode.W) || Game.Current.Input.IsKeyDown(KeyCode.S)) == false) {
                this.Sprite.PlayAnimation("Ass");
            }

            if (Game.Current.Input.IsKeyDown(KeyCode.A) == true) {
                this.Position -= new Vector2(speed * DeltaTime * 0.5f, 0);
                if (this.Sprite.CurrentAnimation != "Run") {
                    this.Sprite.PlayAnimation("Run");
                }
                if (this.Scale.X != 0.5f) {
                    this.Scale *= new Vector2(-1, 1);
                }
            }
            if (Game.Current.Input.IsKeyDown(KeyCode.D) == true) {
                this.Position += new Vector2(speed * DeltaTime * 0.5f, 0);
                if (this.Sprite.CurrentAnimation != "Run") {
                    this.Sprite.PlayAnimation("Run");
                }
                if (this.Scale.X != -0.5f) {
                    this.Scale *= new Vector2(-1, 1);
                }
            }
            if (Game.Current.Input.IsKeyDown(KeyCode.W) == true) {
                this.Position -= new Vector2(0, speed * DeltaTime * 0.5f);
                if (this.Sprite.CurrentAnimation != "Run") {
                    this.Sprite.PlayAnimation("Run");
                }
            }
            if (Game.Current.Input.IsKeyDown(KeyCode.S) == true) {
                this.Position += new Vector2(0, speed * DeltaTime * 0.5f);
                if (this.Sprite.CurrentAnimation != "Run") {
                    this.Sprite.PlayAnimation("Run");
                }
            }
        }

        // Constructors
        public ImageGameObject()
        {
            this.IsHitTestVisible = true; // same as above
            this.Scale = Vector2.One;
            this.Collider = new CollisionCircle(256);
            this.Color = Colors.White;
            this.MoveWithInput = true;
        }
    }
}
