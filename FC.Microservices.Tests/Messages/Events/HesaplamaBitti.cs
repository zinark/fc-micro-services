﻿using FCMicroservices.Components.BUS.Events;

namespace FCMicroservices.Tests.Messages.Events;

[Event]
public class HesaplamaBitti
{
    public int ToplamTutar { get; set; }
}