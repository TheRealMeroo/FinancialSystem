using MediatR;

namespace BuildingBlocks.Application.Behaviors
{
    public abstract class PipelineBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            await BeforeHandle(request, cancellationToken);

            var response = await next();

            await AfterHandle(request, response, cancellationToken);

            return response;
        }

        protected virtual Task BeforeHandle(
            TRequest request,
            CancellationToken cancellationToken)
        { return Task.CompletedTask; }

        protected virtual Task AfterHandle(
            TRequest request,
            TResponse response,
            CancellationToken cancellationToken)
        { return Task.CompletedTask; }
    }
}
