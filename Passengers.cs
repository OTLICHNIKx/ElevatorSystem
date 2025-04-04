﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Passenger
{
    public int TargetFloor { get; set; }
    public int CurrentFloor { get; set; }
    public int Id { get; set; }
    private static int _counter = 0;
    public Passenger(int targetFloor, int currentFloor = 0)
    {
        TargetFloor = targetFloor;
        CurrentFloor = currentFloor;
        Id = ++_counter;
    }
}
