namespace dotnet_training.Models
{
    public interface IChanges
    {
        DateTime CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
    }
}
