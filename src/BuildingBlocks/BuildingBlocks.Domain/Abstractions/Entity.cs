namespace BuildingBlocks.Domain.Abstractions
{
    public abstract class Entity<T> : IEntity
    {
        public T Id { get; protected set; }
        public bool IsActive { get; protected set; }
        public bool IsDeleted { get; protected set; }
        public DateTime? CreatedDate { get; internal set; }
        public string? CreatedBy { get; internal set; }
        public DateTime? UpdatedDate { get; internal set; }
        public string? UpdatedBy { get; internal set; }
        public byte[]? RowVersion { get; internal set; }
    }
}