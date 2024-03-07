namespace dotnet_training.Models
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
    }
}
