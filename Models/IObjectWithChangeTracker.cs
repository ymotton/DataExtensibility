namespace Models
{
    public interface IObjectWithChangeTracker
    {
        ChangeTracker ChangeTracker { get; }
    }
}
