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
        private bool? _isConnected;
        private readonly string _connectionString;

        public MessageBus(string connectionString)
        {
            _connectionString = connectionString;
            TryConnect();
        }

        public bool IsConnected => _isConnected ?? false;

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
                var createBus = RabbitHutch.CreateBus(_connectionString);

                if (createBus == null)
                {
                    _isConnected = false;
                    return;
                }

                _isConnected = true;
                _rpc = createBus.Rpc;
                _pubSub = createBus.PubSub;
            });
        }

        public void Dispose()
        {
            _rpc.Dispose();
        }
    }
}
