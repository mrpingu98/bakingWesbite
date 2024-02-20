using baking_website.Models;

public interface ICategoryRepository
{
    IEnumerable<Category> AllCategories { get; }
}
