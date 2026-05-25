using BuildingBlocks.Domain.Exceptions;

namespace BuildingBlocks.Domain.Gaurds
{
    public static class Check
    {
        public static void NotEmpty(Guid value, string name)
        {
            if (value == Guid.Empty)
                throw new DomainException(new DomainError(BuildingBlocksErrorCodes.EmptyGuid, name));
        }

        public static void NotEmpty(string value, string name)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException(new DomainError(BuildingBlocksErrorCodes.EmptyString, name));
        }
    }
}
