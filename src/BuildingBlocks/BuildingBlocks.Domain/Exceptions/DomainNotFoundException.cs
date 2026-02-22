namespace BuildingBlocks.Domain.Exceptions
{
    public sealed class DomainNotFoundException : DomainException
    {
        public DomainNotFoundException(string message)
            : base(message) { }

        public DomainNotFoundException(string entity, object key)
            : base($"{entity} with ID '{key}' was not found.") { }

        public DomainNotFoundException(string entity, object key, string parentEntity, object parentKey)
            : base($"{entity} ({key}) was not found in {parentEntity} ({parentKey}).") { }
    }
}
