﻿using SPMeta2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPMeta2.Events
{
    public class ModelNodeProcessingEventArgs : EventArgs
    {
        public ModelNode ModelNode { get; set; }
        public object ModelHost { get; set; }
    }
}
