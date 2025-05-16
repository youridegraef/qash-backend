using Application.Domain;
using Application.Exceptions;
using Application.Interfaces;

namespace Application.Services;

public class TagService(ITagRepository tagRepository) : ITagService
{
    public List<Tag> GetAll()
    {
        try
        {
            return tagRepository.FindAll();
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
            return tagRepository.FindById(id);
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
            List<Tag> allTags = tagRepository.FindAll();
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
            List<Tag> allTags = tagRepository.FindAll();
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
            List<Tag> allTags = tagRepository.FindAll();
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
            tag.Id = tagRepository.Add(tag);

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
            return tagRepository.Edit(tag);
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
            Tag tag = tagRepository.FindById(tagId);
            return tagRepository.Delete(tag);
        }
        catch (Exception)
        {
            return false;
        }
    }
}