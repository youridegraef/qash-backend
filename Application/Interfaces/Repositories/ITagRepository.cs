using Application.Domain;

namespace Application.Interfaces;

public interface ITagRepository
{
    public List<Tag> FindAll();

    public Tag? FindById(int id);

    public bool Add(Tag tag);

    public bool Edit(Tag tag);

    public bool Delete(Tag tag);
}