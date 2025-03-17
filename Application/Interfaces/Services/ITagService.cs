
using Application.Domain;

// ReSharper disable once CheckNamespace
namespace Application.Interfaces;

public interface ITagService
{
    public List<Tag> GetAll();
    public Tag GetById(int id);
    public List<Tag> GetByUserId(int userId);
    public List<Tag> GetByName(string name); 
    public List<Tag> GetByNameAndUserId(string name, int userId); 
    public Tag Add(string name, string colorHexCode, int userId);
    public bool Edit(Tag tag);
    public void Delete(int tagId);
    public bool Assign(int tagId, int transactionId);
    public bool UnAssign(int tagId, int transactionId);
}