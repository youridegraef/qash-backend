namespace Presentation.ViewModels;

public class CategoryViewModel
{
    public int Id;
    public string Name;

    public CategoryViewModel(string name, int id)
    {
        Name = name;
        Id = id;
    }
}