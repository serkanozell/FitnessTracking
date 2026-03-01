public interface IEntity<T> : IEntity
{
    public T Id { get; }
}

public interface IEntity
{
    public DateTime? CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsActive { get; }
    public bool IsDeleted { get; }
    public byte[]? RowVersion { get; set; }
}