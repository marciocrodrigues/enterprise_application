using EasyNetQ;
using NSE.Core.Messages.Integration;
using Polly;
using RabbitMQ.Client.Exceptions;

namespace NSE.MessageBus
{
    public class MessageBus : IMessageBus
    {
        private IRpc _rpc;
        private IPubSub _pubSub;
        private IBus _bus;
        private readonly string _connectionString;

        public MessageBus(string connectionString)
        {
            _connectionString = connectionString;
            TryConnect();
        }

        public bool IsConnected => AdvancedBus?.IsConnected ?? false;

        public IAdvancedBus AdvancedBus => _bus?.Advanced;

        public async Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request)
            where TRequest : IntegrationEvent
            where TResponse : ResponseMessage
        {
            TryConnect();
            return await _rpc.RequestAsync<TRequest, TResponse>(request);
        }

        public async Task<IDisposable> RespondAsync<TRequest, TResponse>(Func<TRequest, Task<TResponse>> responder)
            where TRequest : IntegrationEvent
            where TResponse : ResponseMessage
        {
            TryConnect();
            return await _rpc.RespondAsync(responder);
        }

        public async Task PublishAsync<T>(T message) where T : IntegrationEvent
        {
            TryConnect();
            await _pubSub.PublishAsync(message);
        }

        public void SubscribeAsync<T>(string subscriptionId, Func<T, Task> onMessage) where T : class
        {
            TryConnect();
            _pubSub.SubscribeAsync(subscriptionId, onMessage);
        }

        public void Publish<T>(T message) where T : IntegrationEvent
        {
            TryConnect();
            _pubSub.Publish(message);
        }

        public void Subscribe<T>(string subscriptionId, Action<T> onMessage) where T : class
        {
            TryConnect();
            _pubSub.Subscribe(subscriptionId, onMessage);
        }

        public TResponse Request<TRequest, TResponse>(TRequest request)
            where TRequest : IntegrationEvent
            where TResponse : ResponseMessage
        {
            return _rpc.Request<TRequest, TResponse>(request);
        }

        public IDisposable Respond<TRequest, TResponse>(Func<TRequest, TResponse> responder)
            where TRequest : IntegrationEvent
            where TResponse : ResponseMessage
        {
            return _rpc.Respond<TRequest, TResponse>(responder);
        }

        private void TryConnect()
        {
            if (IsConnected) return;

            var policy = Policy.Handle<EasyNetQException>()
                .Or<BrokerUnreachableException>()
                .WaitAndRetry(3, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            policy.Execute(() =>
            {
                _bus = RabbitHutch.CreateBus(_connectionString);
                AdvancedBus.Disconnected += onDisconnect;

                if (_bus == null)
                {
                    return;
                }

                _rpc = _bus.Rpc;
                _pubSub = _bus.PubSub;
            });
        }

        private void onDisconnect(object? s, EventArgs e)
        {
            var policy = Policy.Handle<EasyNetQException>()
                .Or<BrokerUnreachableException>()
                .RetryForever();

            policy.Execute(TryConnect);
        }

        public void Dispose()
        {
            _rpc.Dispose();
        }
    }
}
