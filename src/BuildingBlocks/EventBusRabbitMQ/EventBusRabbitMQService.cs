﻿namespace EventBusRabbitMQ;

public class EventBusRabbitMQService : IEventBus//, IDisposable
{
    private const string BROKER_NAME = "eshop_event_bus";

    private static readonly JsonSerializerOptions s_indentedOptions = new() { WriteIndented = true };
    private static readonly JsonSerializerOptions s_caseInsensitiveOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly IRabbitMQPersistentConnection _persistentConnection;
    private readonly ILogger<EventBusRabbitMQService> _logger;

    //private readonly IEventBusSubscriptionsManager _subsManager;
    //private readonly IServiceProvider _serviceProvider;
    private readonly int _retryCount;

    //private IModel _consumerChannel;
    //private string _queueName;

    public EventBusRabbitMQService(IRabbitMQPersistentConnection persistentConnection,
                                   ILogger<EventBusRabbitMQService> logger,
                                   IServiceProvider serviceProvider,
                                   // IEventBusSubscriptionsManager subsManager,
                                   string queueName = null,
                                   int retryCount = 5
        )
    {
        _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        //_subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
        //  _queueName = queueName;
        //_consumerChannel = CreateConsumerChannel();
        //_serviceProvider = serviceProvider;
        _retryCount = retryCount;
        //_subsManager.OnEventRemoved += SubsManager_OnEventRemoved;
    }

    //private void SubsManager_OnEventRemoved(object sender, string eventName)
    //{
    //    if (!_persistentConnection.IsConnected)
    //    {
    //        _persistentConnection.TryConnect();
    //    }

    //    using var channel = _persistentConnection.CreateModel();
    //    channel.QueueUnbind(queue: _queueName,
    //        exchange: BROKER_NAME,
    //        routingKey: eventName);

    //    if (_subsManager.IsEmpty)
    //    {
    //        _queueName = string.Empty;
    //        _consumerChannel.Close();
    //    }
    //}

    public void Publish(IntegrationEvent @event)
    {
        if (!_persistentConnection.IsConnected)
        {
            _persistentConnection.TryConnect();
        }

        var policy = RetryPolicy.Handle<BrokerUnreachableException>()
            .Or<SocketException>()
            .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
            {
                _logger.LogWarning(ex, "Could not publish event: {EventId} after {Timeout}s", @event.Id, $"{time.TotalSeconds:n1}");
            });

        var eventName = @event.GetType().Name;

        _logger.LogTrace("Creating RabbitMQ channel to publish event: {EventId} ({EventName})", @event.Id, eventName);
        using var channel = _persistentConnection.CreateModel();

        _logger.LogTrace("Declaring RabbitMQ exchange to publish event: {EventId}", @event.Id);
        channel.ExchangeDeclare(exchange: BROKER_NAME, type: ExchangeType.Direct);

        var body = JsonSerializer.SerializeToUtf8Bytes(@event, @event.GetType(), s_indentedOptions);

        policy.Execute(() =>
        {
            var properties = channel.CreateBasicProperties();
            properties.DeliveryMode = 2; // persistent

            _logger.LogTrace("Publishing event to RabbitMQ: {EventId}", @event.Id);

            channel.BasicPublish(
                exchange: BROKER_NAME,
                routingKey: eventName,
                mandatory: true,
                basicProperties: properties,
                body: body);
        });
    }

    //public void SubscribeDynamic<TH>(string eventName)
    //    where TH : IDynamicIntegrationEventHandler
    //{
    //    _logger.LogInformation("Subscribing to dynamic event {EventName} with {EventHandler}", eventName, typeof(TH).GetGenericTypeName());

    //    DoInternalSubscription(eventName);
    //    _subsManager.AddDynamicSubscription<TH>(eventName);
    //    StartBasicConsume();
    //}

    //public void Subscribe<T, TH>()
    //    where T : IntegrationEvent
    //    where TH : IIntegrationEventHandler<T>
    //{
    //    var eventName = _subsManager.GetEventKey<T>();
    //    DoInternalSubscription(eventName);

    //    _logger.LogInformation("Subscribing to event {EventName} with {EventHandler}", eventName, typeof(TH).GetGenericTypeName());

    //    _subsManager.AddSubscription<T, TH>();
    //    StartBasicConsume();
    //}

    //private void DoInternalSubscription(string eventName)
    //{
    //    var containsKey = _subsManager.HasSubscriptionsForEvent(eventName);
    //    if (!containsKey)
    //    {
    //        if (!_persistentConnection.IsConnected)
    //        {
    //            _persistentConnection.TryConnect();
    //        }

    //        _consumerChannel.QueueBind(queue: _queueName,
    //                            exchange: BROKER_NAME,
    //                            routingKey: eventName);
    //    }
    //}

    //public void Unsubscribe<T, TH>()
    //    where T : IntegrationEvent
    //    where TH : IIntegrationEventHandler<T>
    //{
    //    var eventName = _subsManager.GetEventKey<T>();

    //    _logger.LogInformation("Unsubscribing from event {EventName}", eventName);

    //    _subsManager.RemoveSubscription<T, TH>();
    //}

    //public void UnsubscribeDynamic<TH>(string eventName)
    //    where TH : IDynamicIntegrationEventHandler
    //{
    //    _subsManager.RemoveDynamicSubscription<TH>(eventName);
    //}

    //public void Dispose()
    //{
    //    if (_consumerChannel != null)
    //    {
    //        _consumerChannel.Dispose();
    //    }

    //    _subsManager.Clear();
    //}

    public void Subscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        throw new NotImplementedException();
    }

    public void Unsubscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        throw new NotImplementedException();
    }

    //private void StartBasicConsume()
    //{
    //    _logger.LogTrace("Starting RabbitMQ basic consume");

    //    if (_consumerChannel != null)
    //    {
    //        var consumer = new AsyncEventingBasicConsumer(_consumerChannel);

    //        consumer.Received += Consumer_Received;

    //        _consumerChannel.BasicConsume(
    //            queue: _queueName,
    //            autoAck: false,
    //            consumer: consumer);
    //    }
    //    else
    //    {
    //        _logger.LogError("StartBasicConsume can't call on _consumerChannel == null");
    //    }
    //}

    //private async Task Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
    //{
    //    var eventName = eventArgs.RoutingKey;
    //    var message = Encoding.UTF8.GetString(eventArgs.Body.Span);

    //    try
    //    {
    //        if (message.ToLowerInvariant().Contains("throw-fake-exception"))
    //        {
    //            throw new InvalidOperationException($"Fake exception requested: \"{message}\"");
    //        }

    //        await ProcessEvent(eventName, message);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogWarning(ex, "Error Processing message \"{Message}\"", message);
    //    }

    //    // Even on exception we take the message off the queue.
    //    // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX).
    //    // For more information see: https://www.rabbitmq.com/dlx.html
    //    _consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
    //}

    //private IModel CreateConsumerChannel()
    //{
    //    if (!_persistentConnection.IsConnected)
    //    {
    //        _persistentConnection.TryConnect();
    //    }

    //    _logger.LogTrace("Creating RabbitMQ consumer channel");

    //    var channel = _persistentConnection.CreateModel();

    //    channel.ExchangeDeclare(exchange: BROKER_NAME,
    //                            type: "direct");

    //    channel.QueueDeclare(queue: _queueName,
    //                            durable: true,
    //                            exclusive: false,
    //                            autoDelete: false,
    //                            arguments: null);

    //    channel.CallbackException += (sender, ea) =>
    //    {
    //        _logger.LogWarning(ea.Exception, "Recreating RabbitMQ consumer channel");

    //        _consumerChannel.Dispose();
    //        _consumerChannel = CreateConsumerChannel();
    //        StartBasicConsume();
    //    };

    //    return channel;
    //}

    //private async Task ProcessEvent(string eventName, string message)
    //{
    //    _logger.LogTrace("Processing RabbitMQ event: {EventName}", eventName);

    //    if (_subsManager.HasSubscriptionsForEvent(eventName))
    //    {
    //        await using var scope = _serviceProvider.CreateAsyncScope();
    //        var subscriptions = _subsManager.GetHandlersForEvent(eventName);
    //        foreach (var subscription in subscriptions)
    //        {
    //            if (subscription.IsDynamic)
    //            {
    //                if (scope.ServiceProvider.GetService(subscription.HandlerType) is not IDynamicIntegrationEventHandler handler) continue;
    //                using dynamic eventData = JsonDocument.Parse(message);
    //                await Task.Yield();
    //                await handler.Handle(eventData);
    //            }
    //            else
    //            {
    //                var handler = scope.ServiceProvider.GetService(subscription.HandlerType);
    //                if (handler == null) continue;
    //                var eventType = _subsManager.GetEventTypeByName(eventName);
    //                var integrationEvent = JsonSerializer.Deserialize(message, eventType, s_caseInsensitiveOptions);
    //                var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

    //                await Task.Yield();
    //                await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
    //            }
    //        }
    //    }
    //    else
    //    {
    //        _logger.LogWarning("No subscription for RabbitMQ event: {EventName}", eventName);
    //    }
    //}
}

public static class GenericTypeExtensions
{
    public static string GetGenericTypeName(this Type type)
    {
        string typeName;

        if (type.IsGenericType)
        {
            var genericTypes = string.Join(",", type.GetGenericArguments().Select(t => t.Name).ToArray());
            typeName = $"{type.Name.Remove(type.Name.IndexOf('`'))}<{genericTypes}>";
        }
        else
        {
            typeName = type.Name;
        }

        return typeName;
    }

    public static string GetGenericTypeName(this object @object)
    {
        return @object.GetType().GetGenericTypeName();
    }
}