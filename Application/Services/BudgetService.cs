using Application.Domain;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class BudgetService(IBudgetRepository budgetRepository, ILogger<BudgetService> logger)
    : IBudgetService
{
    public Budget GetById(int id)
    {
        try
        {
            Budget budget = budgetRepository.FindById(id);

            if (budget != null!)
            {
                return budget;
            }

            throw new BudgetNotFoundException($"No budget with id: {id} found.");
        }
        catch (BudgetNotFoundException ex)
        {
            logger.LogError(ex, "No budget with id: {BudgetId} found.", id);
            throw;
        }
        catch (DatabaseException ex)
        {
            logger.LogError(ex, "Database error retrieving budget with id: {BudgetId}", id);
            throw new Exception($"Database error retrieving budget with id: {id}", ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving budget with id: {BudgetId}", id);
            throw new Exception($"Error retrieving budget with id: {id}", ex);
        }
    }

    public Budget GetByCategoryId(int categoryId)
    {
        try
        {
            Budget budget = budgetRepository.FindByCategoryId(categoryId);

            if (budget != null!)
            {
                return budget;
            }

            throw new BudgetNotFoundException($"No budget with category id: {categoryId} found.");
        }
        catch (BudgetNotFoundException ex)
        {
            logger.LogError(ex, "No budget with category id: {CategoryId} found.", categoryId);
            throw;
        }
        catch (DatabaseException ex)
        {
            logger.LogError(ex, "Database error retrieving budget with category id: {CategoryId}", categoryId);
            throw new Exception($"Database error retrieving budget with category id: {categoryId}", ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving budget with category id: {CategoryId}", categoryId);
            throw new Exception($"Error retrieving budget with category id: {categoryId}", ex);
        }
    }

    public Budget Add(DateOnly startDate, DateOnly endDate, double target, int categoryId)
    {
        try
        {
            Budget budget = new Budget(startDate, endDate, target, categoryId);
            budget.Id = budgetRepository.Add(budget);
            return budget;
        }
        catch (ArgumentException ex)
        {
            logger.LogError(ex, "Invalid budget data: {Message}", ex.Message);
            throw new InvalidDataException("Invalid budget data: " + ex.Message, ex);
        }
        catch (DatabaseException ex)
        {
            logger.LogError(ex, "Database error while adding a budget.");
            throw new Exception("Database error while adding a budget.", ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred while adding a budget.");
            throw new Exception("An unexpected error occurred.", ex);
        }
    }

    public bool Edit(Budget budget)
    {
        try
        {
            return budgetRepository.Edit(budget);
        }
        catch (BudgetNotFoundException ex)
        {
            logger.LogError(ex, "Budget with ID {BudgetId} was not found for update.", budget.Id);
            return false;
        }
        catch (DatabaseException ex)
        {
            logger.LogError(ex, "Database error while updating budget with ID {BudgetId}", budget.Id);
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating budget with ID {BudgetId}", budget.Id);
            return false;
        }
    }

    public bool Delete(int id)
    {
        try
        {
            var budget = budgetRepository.FindById(id);
            return budgetRepository.Delete(budget);
        }
        catch (BudgetNotFoundException ex)
        {
            logger.LogError(ex, "Budget with ID {BudgetId} was not found for deletion.", id);
            return false;
        }
        catch (DatabaseException ex)
        {
            logger.LogError(ex, "Database error while deleting budget with ID {BudgetId}", id);
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting budget with ID {BudgetId}", id);
            return false;
        }
    }
}