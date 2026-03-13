namespace BuildingBlocks.Domain.Abstractions
{
    public interface IEntity<T> : IEntity
    {
        public T Id { get; }
    }

    public interface IEntity
    {
        public DateTime? CreatedDate { get; }
        public string? CreatedBy { get; }
        public DateTime? UpdatedDate { get; }
        public string? UpdatedBy { get; }
        public bool IsActive { get; }
        public bool IsDeleted { get; }
        public byte[]? RowVersion { get; }
    }
}