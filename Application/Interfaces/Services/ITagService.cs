using Application.Domain;
using Application.Dtos;

// ReSharper disable once CheckNamespace
namespace Application.Interfaces;

public interface ITagService {
    public Tag GetById(int id);
    public List<Tag> GetByUserId(int userId);
    public List<Tag> GetByUserIdPaged(int userId, int page, int pageSize);
    public List<Tag> GetByTransactionId(int transactionId);
    public Tag Add(string name, int userId);
    public bool Edit(Tag tag);
    public bool Delete(int tagId);
}