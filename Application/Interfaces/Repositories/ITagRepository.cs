using Application.Domain;

// ReSharper disable once CheckNamespace
namespace Application.Interfaces;

public interface ITagRepository
{
    public List<Tag> FindAll();

    public Tag FindById(int id);

    public int Add(Tag tag);

    public bool Edit(Tag tag);

    public bool Delete(Tag tag);
}