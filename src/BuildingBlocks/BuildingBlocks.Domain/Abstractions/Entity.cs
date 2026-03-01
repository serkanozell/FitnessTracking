public abstract class Entity<T> : IEntity
{
    public T Id { get; protected set; }
    public bool IsActive { get; protected set; }
    public bool IsDeleted { get; protected set; }
    public DateTime? CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? UpdatedBy { get; set; }
    public byte[]? RowVersion { get; set; }
}