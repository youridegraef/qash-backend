using Application.Domain;
using Application.Dtos;

// ReSharper disable once CheckNamespace
namespace Application.Interfaces;

public interface ITagRepository
{
    public List<Tag> FindByUserIdPaged(int userId, int page, int pageSize);
    public List<Tag> FindByUserId(int userId);

    public Tag FindById(int id);
    public List<TagDto> FindByTransactionId(int transactionId);

    public int Add(Tag tag);

    public bool Edit(Tag tag);

    public bool Delete(Tag tag);
}