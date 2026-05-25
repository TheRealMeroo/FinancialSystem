using BuildingBlocks.Application.Abstractions.Commands;
using BuildingBlocks.Application.Abstractions.Persistance;
using MediatR;
using System.Text.Json;

namespace BuildingBlocks.Application.Behaviors
{
    public class IdempotencyBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IIdempotentCommand
    {

        private readonly IIdempotencyRepository _idempotencyRepository;

        public IdempotencyBehavior(IIdempotencyRepository idempotencyRepository)
        {
            _idempotencyRepository = idempotencyRepository;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var key = request.IdempotentKey;
            var requestType = request.GetType().Name;

            var existing = await _idempotencyRepository.GetAsync(key, requestType, cancellationToken);

            if (existing != null)
            {
                var previousResponse = JsonSerializer.Deserialize<TResponse>(existing.ResponseData);
                return previousResponse;
            }

            var response = await next();

            await _idempotencyRepository.SaveAsync(key, requestType, response!, cancellationToken);

            return response;
        }
    }
}
