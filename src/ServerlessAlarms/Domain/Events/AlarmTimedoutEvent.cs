﻿namespace ServerlessAlarms.Domain.Events;

using MediatR;

public class AlarmTimedoutEvent : INotification
{

    public Guid AlarmId { get; set; }

}
