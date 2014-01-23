﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lamby2D.Drawing;
using Lamby2D.Input;

namespace Lamby2D
{
    public class Game : IDisposable
    {
        // Properties
        public Graphics Graphics { get; private set; }

        // Events
        public event KeyEventHandler KeyDown
        {
            add { if (this.Graphics.GraphicsContext.Window != null) this.Graphics.GraphicsContext.Window.KeyDown += value; }
            remove { if (this.Graphics.GraphicsContext.Window != null) this.Graphics.GraphicsContext.Window.KeyDown += value; }
        }
        public event KeyEventHandler KeyUp
        {
            add { if (this.Graphics.GraphicsContext.Window != null) this.Graphics.GraphicsContext.Window.KeyUp += value; }
            remove { if (this.Graphics.GraphicsContext.Window != null) this.Graphics.GraphicsContext.Window.KeyUp += value; }
        }
        
        // Public
        public void MainLoop()
        {
            DateTime lasttick = DateTime.UtcNow;
            
            while (Graphics.GraphicsContext.Window != null) {
                Graphics.GraphicsContext.Window.PollMessages();

                if (this.Graphics.GraphicsContext.Window == null) {
                    break;
                }

                float dt = (float) (DateTime.UtcNow - lasttick).TotalSeconds;
                lasttick = DateTime.UtcNow;

                this.Update(dt);
                this.Draw();
            }
        }

        // Public
        public virtual void Update(float DeltaTime)
        {
        }
        public virtual void Draw()
        {
            this.Graphics.Clear();
            this.Graphics.Flush();
        }
        public void Dispose()
        {
            this.Graphics.Dispose();
            this.Graphics = null;
            this.Cleanup();
        }

        // Protected
        protected virtual void Cleanup()
        {
        }

        // Constructors
        public Game()
        {
            this.Graphics = new Graphics();
        }
    }
}
