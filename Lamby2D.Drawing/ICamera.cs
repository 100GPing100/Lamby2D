﻿using Lamby2D.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lamby2D.Drawing
{
    public interface ICamera
    {
        // Properties
        Vector2 Position { get; set; }
    }
}
