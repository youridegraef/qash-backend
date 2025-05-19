using Application.Domain;

// ReSharper disable once CheckNamespace
namespace Application.Interfaces;

public interface ITagService
{
    public Tag GetById(int id);
    public List<Tag> GetByUserId(int userId);
    public List<Tag> GetByUserIdPaged(int userId, int page, int pageSize);
    public Tag Add(string name, string colorHexCode, int userId);
    public bool Edit(Tag tag);
    public bool Delete(int tagId);
}

