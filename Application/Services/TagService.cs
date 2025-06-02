using Application.Domain;
using Application.Dtos;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class TagService(ITagRepository tagRepository, ILogger<TagService> logger) : ITagService
{
    public Tag GetById(int id)
    {
        try
        {
            Tag tag = tagRepository.FindById(id);

            if (tag != null!)
            {
                return tag;
            }

            throw new TagNotFoundException($"No tag with id: {id} found.");
        }
        catch (TagNotFoundException ex)
        {
            logger.LogError(ex, "No tag with id: {TagId} found.", id);
            throw;
        }
        catch (DatabaseException ex)
        {
            logger.LogError(ex, "Database error retrieving tag with id: {TagId}", id);
            throw new Exception($"Database error retrieving tag with id: {id}", ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving tag with id: {TagId}", id);
            throw new Exception($"Error retrieving tag with id: {id}", ex);
        }
    }

    public List<Tag> GetByUserId(int userId)
    {
        try
        {
            return tagRepository.FindByUserId(userId);
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogError(ex, "No tags found for user_id: {UserId}", userId);
            throw new Exception($"No tags found for user_id: {userId}", ex);
        }
        catch (DatabaseException ex)
        {
            logger.LogError(ex, "Database error retrieving tags for user_id: {UserId}", userId);
            throw new Exception($"Database error retrieving tags for user_id: {userId}", ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving tags for user_id: {UserId}", userId);
            throw new Exception($"Error retrieving tags for user_id: {userId}", ex);
        }
    }
    
    public List<Tag> GetByTransactionId(int transactionId)
    {
        try
        {
            return tagRepository.FindByTransactionId(transactionId);
        }
        catch (DatabaseException ex)
        {
            logger.LogError(ex, "Database error retrieving tags for transaction_id: {TransactionId}", transactionId);
            throw new Exception($"Database error retrieving tags for transaction_id: {transactionId}", ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving tags for transaction_id: {TransactionId}", transactionId);
            throw new Exception($"Error retrieving tags for transaction_id: {transactionId}", ex);
        }
    }

    public List<Tag> GetByUserIdPaged(int userId, int page, int pageSize)
    {
        try
        {
            return tagRepository.FindByUserIdPaged(userId, page, pageSize);
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogError(ex, "No tags found for user_id: {UserId}", userId);
            throw new Exception($"No tags found for user_id: {userId}", ex);
        }
        catch (DatabaseException ex)
        {
            logger.LogError(ex, "Database error retrieving paged tags for user_id: {UserId}", userId);
            throw new Exception($"Database error retrieving paged tags for user_id: {userId}", ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving paged tags for user_id: {UserId}", userId);
            throw new Exception($"Error retrieving paged tags for user_id: {userId}", ex);
        }
    }

    public Tag Add(string name, string colorHexCode, int userId)
    {
        try
        {
            var newTag = new Tag(name, colorHexCode, userId);
            var addedTag = tagRepository.Add(newTag);

            return addedTag;
        }
        catch (ArgumentException ex)
        {
            logger.LogError(ex, "Invalid tag data: {Message}", ex.Message);
            throw new InvalidDataException("Invalid tag data: " + ex.Message, ex);
        }
        catch (DatabaseException ex)
        {
            logger.LogError(ex, "Database error while adding a tag.");
            throw new Exception("Database error while adding a tag.", ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred while adding a tag.");
            throw new Exception("An unexpected error occurred.", ex);
        }
    }

    public bool Edit(Tag tag)
    {
        try
        {
            return tagRepository.Edit(tag);
        }
        catch (TagNotFoundException ex)
        {
            logger.LogError(ex, "Tag with ID {TagId} was not found for update.", tag.Id);
            return false;
        }
        catch (DatabaseException ex)
        {
            logger.LogError(ex, "Database error while updating tag with ID {TagId}", tag.Id);
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating tag with ID {TagId}", tag.Id);
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
        catch (TagNotFoundException ex)
        {
            logger.LogError(ex, "Tag with ID {TagId} was not found for deletion.", tagId);
            return false;
        }
        catch (DatabaseException ex)
        {
            logger.LogError(ex, "Database error while deleting tag with ID {TagId}", tagId);
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting tag with ID {TagId}", tagId);
            return false;
        }
    }
}
