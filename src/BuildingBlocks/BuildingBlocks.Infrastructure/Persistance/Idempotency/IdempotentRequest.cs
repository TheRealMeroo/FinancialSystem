namespace BuildingBlocks.Infrastructure.Persistance.Idempotency
{
    public class IdempotentRequest
    {
        public Guid Id { get; set; }
        public string Key { get; set; } = default!;
        public string RequestType { get; set; } = default!;
        public string ResponseData { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
    }
}
