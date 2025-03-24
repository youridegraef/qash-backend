using Application.Domain;
using Application.Exceptions;
using Application.Interfaces;

namespace Application.Services;

public class TagService : ITagService
{
    private readonly ITagRepository _tagRepository;

    public TagService(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }


    public List<Tag> GetAll()
    {
        try
        {
            return _tagRepository.FindAll();
        }
        catch (Exception)
        {
            throw new Exception($"No tags found");
        }
    }

    public Tag GetById(int id)
    {
        try
        {
            return _tagRepository.FindById(id);
        }
        catch (KeyNotFoundException)
        {
            throw new Exception($"No tags found");
        }
        catch (Exception)
        {
            throw new Exception($"No tags found");
        }
    }

    public List<Tag> GetByUserId(int userId)
    {
        try
        {
            List<Tag> allTags = _tagRepository.FindAll();
            var filteredTags = allTags
                .Where(t => t.UserId == userId).ToList();

            return filteredTags;
        }
        catch (KeyNotFoundException)
        {
            throw new Exception($"No tags found");
        }
        catch (Exception)
        {
            throw new Exception($"No tags found");
        }
    }

    public List<Tag> GetByName(string name)
    {
        try
        {
            List<Tag> allTags = _tagRepository.FindAll();
            var filteredTags = allTags
                .Where(t => t.Name == name).ToList();

            return filteredTags;
        }
        catch (KeyNotFoundException)
        {
            throw new Exception($"No tags found");
        }
        catch (Exception)
        {
            throw new Exception($"No tags found");
        }
    }

    public List<Tag> GetByNameAndUserId(string name, int userId)
    {
        try
        {
            List<Tag> allTags = _tagRepository.FindAll();
            var filteredTags = allTags
                .Where(t => t.Name == name && t.UserId == userId).ToList();

            return filteredTags;
        }
        catch (KeyNotFoundException)
        {
            throw new Exception($"No tags found");
        }
        catch (Exception)
        {
            throw new Exception($"No tags found");
        }
    }

    public Tag Add(string name, string colorHexCode, int userId)
    {
        try
        {
            Tag tag = new Tag(name, colorHexCode, userId);
            tag.Id = _tagRepository.Add(tag);

            return tag;
        }
        catch (Exception)
        {
            throw new Exception($"No tags found");
        }
    }

    public bool Edit(Tag tag)
    {
        try
        {
            return _tagRepository.Edit(tag);
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool Delete(int tagId)
    {
        try
        {
            Tag tag = _tagRepository.FindById(tagId);
            return _tagRepository.Delete(tag);
        }
        catch (Exception)
        {
            return false;
        }
    }
}