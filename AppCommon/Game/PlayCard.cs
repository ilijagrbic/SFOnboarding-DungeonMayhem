﻿using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCommon.Game
{
    public class PlayCard
    {
        public ActorId? PlayerId{ get; set; }
        public int CardIndex { get; set; }
        public bool TargetLeft {  get; set; }
    }
}