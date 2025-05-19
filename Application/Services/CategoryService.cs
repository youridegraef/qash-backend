using Application.Domain;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class CategoryService(
    ICategoryRepository categoryRepository,
    ILogger<CategoryService> logger)
    : ICategoryService
{
    public Category GetById(int id)
    {
        try
        {
            Category category = categoryRepository.FindById(id);

            if (category != null!)
            {
                return category;
            }

            throw new CategoryNotFoundException($"No category with id: {id} found.");
        }
        catch (CategoryNotFoundException ex)
        {
            logger.LogError(ex, "No category with id: {CategoryId} found.", id);
            throw;
        }
        catch (DatabaseException ex)
        {
            logger.LogError(ex, "Database error retrieving category with id: {CategoryId}", id);
            throw new Exception($"Database error retrieving category with id: {id}", ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving category with id: {CategoryId}", id);
            throw new Exception($"Error retrieving category with id: {id}", ex);
        }
    }

    public List<Category> GetByUserId(int userId)
    {
        try
        {
            return categoryRepository.FindByUserId(userId);
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogError(ex, "No categories found for user_id: {UserId}", userId);
            throw new Exception($"No categories found for user_id: {userId}", ex);
        }
        catch (DatabaseException ex)
        {
            logger.LogError(ex, "Database error retrieving categories for user_id: {UserId}", userId);
            throw new Exception($"Database error retrieving categories for user_id: {userId}", ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving categories for user_id: {UserId}", userId);
            throw new Exception($"Error retrieving categories for user_id: {userId}", ex);
        }
    }

    public List<Category> GetByUserIdPaged(int userId, int page, int pageSize)
    {
        try
        {
            return categoryRepository.FindByUserIdPaged(userId, page, pageSize);
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogError(ex, "No categories found for user_id: {UserId}", userId);
            throw new Exception($"No categories found for user_id: {userId}", ex);
        }
        catch (DatabaseException ex)
        {
            logger.LogError(ex, "Database error retrieving paged categories for user_id: {UserId}", userId);
            throw new Exception($"Database error retrieving paged categories for user_id: {userId}", ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving paged categories for user_id: {UserId}", userId);
            throw new Exception($"Error retrieving paged categories for user_id: {userId}", ex);
        }
    }

    public Category Add(string name, int userId, string colorHexCode)
    {
        try
        {
            Category category = new Category(name, userId, colorHexCode);
            category.Id = categoryRepository.Add(category);
            return category;
        }
        catch (ArgumentException ex)
        {
            logger.LogError(ex, "Invalid category data: {Message}", ex.Message);
            throw new InvalidDataException("Invalid category data: " + ex.Message, ex);
        }
        catch (DatabaseException ex)
        {
            logger.LogError(ex, "Database error while adding a category.");
            throw new Exception("Database error while adding a category.", ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred while adding a category.");
            throw new Exception("An unexpected error occurred.", ex);
        }
    }

    public bool Edit(Category category)
    {
        try
        {
            return categoryRepository.Edit(category);
        }
        catch (CategoryNotFoundException ex)
        {
            logger.LogError(ex, "Category with ID {CategoryId} was not found for update.", category.Id);
            return false;
        }
        catch (DatabaseException ex)
        {
            logger.LogError(ex, "Database error while updating category with ID {CategoryId}", category.Id);
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating category with ID {CategoryId}", category.Id);
            return false;
        }
    }

    public bool Delete(int id)
    {
        try
        {
            var category = categoryRepository.FindById(id);
            return categoryRepository.Delete(category);
        }
        catch (CategoryNotFoundException ex)
        {
            logger.LogError(ex, "Category with ID {CategoryId} was not found for deletion.", id);
            return false;
        }
        catch (DatabaseException ex)
        {
            logger.LogError(ex, "Database error while deleting category with ID {CategoryId}", id);
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting category with ID {CategoryId}", id);
            return false;
        }
    }
}