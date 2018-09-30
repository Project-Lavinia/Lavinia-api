﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaviniaApi.Models
{
    public class DistrictMetrics
    {
        public string District { get; set; }
        public int ElectionYear { get; set; }
        public int Area { get; set; }
        public int Population { get; set; }
    }
}