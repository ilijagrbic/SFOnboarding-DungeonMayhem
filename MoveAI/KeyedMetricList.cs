﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Fabric.Description;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveAI
{
    internal class KeyedMetricList : KeyedCollection<string, ServiceLoadMetricDescription>
    {
        protected override string GetKeyForItem(ServiceLoadMetricDescription item)
        {
            return item.Name;
        }
    }
}
