namespace BuildingBlocks.Application.Abstractions.Commands
{
    public interface IIdempotentCommand : ICommand
    {
        string IdempotentKey { get; }
    }
}
