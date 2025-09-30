using System.Collections.Generic;
using Xunit;

namespace Shared.Test.Events;
using VoxelForge.Shared.Events;

public class TestEventA : IGameEvent
{
    public string Message { get; set; }
}

public class TestEventB : IGameEvent
{
    public int Value { get; set; }
}

public class EventBusTest
{
    // Test that a single subscriber receives the event
    [Fact]
    public void SingleSubscriberReceivesEvent()
    {
        var eventBus = new EventBus();
        var receivedEvents = new List<TestEventA>();

        void Handler(TestEventA e) => receivedEvents.Add(e);

        eventBus.Subscribe<TestEventA>(Handler);
        var testEvent = new TestEventA { Message = "Hello, World!" };
        eventBus.Publish(testEvent);

        Assert.Single(receivedEvents);
        Assert.Equal("Hello, World!", receivedEvents[0].Message);
    }

    // Test that multiple subscribers receive the event
    [Fact]
    public void MultipleSubscribersReceiveEvent()
    {
        var eventBus = new EventBus();
        var receivedEvents = new List<TestEventA>();

        void Handler(TestEventA e) => receivedEvents.Add(e);
        void AnotherHandler(TestEventA e) => receivedEvents.Add(e);

        eventBus.Subscribe<TestEventA>(Handler);
        eventBus.Subscribe<TestEventA>(AnotherHandler);
        var testEvent = new TestEventA { Message = "Hello, World!" };
        eventBus.Publish(testEvent);

        Assert.Equal(2, receivedEvents.Count);
        Assert.All(receivedEvents, e => Assert.Equal("Hello, World!", e.Message));
    }
    
    // Test that publishing an event with no subscribers does not throw an error
    [Fact]
    public void NoSubscribersNoError()
    {
        var eventBus = new EventBus();
        var testEvent = new TestEventA { Message = "Hello, World!" };
        
        // Should not throw any exception
        eventBus.Publish(testEvent);
    }

    // Test that different event types are handled separately
    [Fact]
    public void DifferentEventTypesHandledSeparately()
    {
        var eventBus = new EventBus();
        var receivedEventA = new List<TestEventA>();
        var receivedEventB = new List<TestEventB>();

        void HandlerA(TestEventA e) => receivedEventA.Add(e);
        void HandlerB(TestEventB e) => receivedEventB.Add(e);

        eventBus.Subscribe<TestEventA>(HandlerA);
        eventBus.Subscribe<TestEventB>(HandlerB);

        var testEventA = new TestEventA { Message = "Event A" };
        var testEventB = new TestEventB { Value = 42 };

        eventBus.Publish(testEventA);

        Assert.Single(receivedEventA);
        Assert.Equal("Event A", receivedEventA[0].Message);
        Assert.Empty(receivedEventB);

        eventBus.Publish(testEventB);
        Assert.Single(receivedEventB);
        Assert.Equal(42, receivedEventB[0].Value);
        Assert.Single(receivedEventA); // Still only one event A received
    }
}