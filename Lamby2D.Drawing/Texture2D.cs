﻿using Lamby2D.Native.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Lamby2D.Drawing
{
    public class Texture2D : IDisposable
    {
        // Static operators
        public static bool operator ==(Texture2D a, Texture2D b)
        {
            if ((object) a == null && (object) b == null) {
                return true;
            } else if ((object) a == null || (object) b == null) {
                return false;
            }

            return (a.ID == b.ID);
        }
        public static bool operator !=(Texture2D a, Texture2D b)
        {
            if ((object) a == null && (object) b == null) {
                return false;
            } else if ((object) a == null || (object) b == null) {
                return true;
            }

            return (a.ID != b.ID);
        }


        // Properties
        public uint ID { get; private set; }
        public int Width { get; internal set; }
        public int Height { get; internal set; }

        // Public
        public override bool Equals(object obj)
        {
            return (obj is Texture2D && (Texture2D) obj == this);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public void Dispose()
        {
            if (this.ID != 0) {
                uint[] textures = new uint[1] { this.ID };
                OpenGL.glDeleteTextures(1, Marshal.UnsafeAddrOfPinnedArrayElement(textures, 0));
                this.ID = 0;
            }
        }

        // Constructors
        internal Texture2D(uint id)
        {
            this.ID = id;
        }
    }
}
