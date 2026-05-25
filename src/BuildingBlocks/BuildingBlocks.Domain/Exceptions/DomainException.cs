namespace BuildingBlocks.Domain.Exceptions
{
    public class DomainException : Exception
    {
        public IReadOnlyList<DomainError> Errors { get; } = new List<DomainError>();

        public DomainException(List<DomainError> errors)
        {
            Errors = errors;
        }

        public DomainException(DomainError error)
        {
            Errors = new List<DomainError>() { error };
        }
    }

    public record DomainError(string Code, string Message);
}
