using BuildingBlocks.Application.Results;

namespace BuildingBlocks.Application.Exceptions
{
    public class ValidationException : Exception
    {
        private readonly List<ApplicationError> _errors = new List<ApplicationError>();

        public ValidationException(List<ApplicationError> errors)
        {
            _errors = errors;
        }

        public ValidationException(ApplicationError error)
        {
            _errors = new List<ApplicationError>() { error };
        }
    }
}
