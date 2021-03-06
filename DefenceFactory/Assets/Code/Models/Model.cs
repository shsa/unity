﻿using DefenceFactory.Game.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DefenceFactory.Models
{
    public abstract class Model
    {
        public abstract string GetKey(Meta meta);
        public abstract BlockView GetPrefab(Meta meta);
    }
}
