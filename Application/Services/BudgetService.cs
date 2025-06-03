using Application.Domain;
using Application.Dtos;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class BudgetService(
    IBudgetRepository budgetRepository,
    ICategoryService categoryService,
    ITransactionService transactionService,
    ILogger<BudgetService> logger)
    : IBudgetService {
    public BudgetDto GetById(int id) {
        try {
            var budget = budgetRepository.FindById(id);
            if (budget == null)
                throw new BudgetNotFoundException($"No budget with id: {id} found.");

            var category = categoryService.GetById(budget.CategoryId);
            var spent = transactionService
                .GetByUserId(category.UserId)
                .Where(t => t.CategoryId == category.Id
                            && t.Date >= budget.StartDate
                            && t.Date <= budget.EndDate)
                .Sum(t => t.Amount);

            return new BudgetDto(
                budget.Id,
                category.Name,
                budget.StartDate,
                budget.EndDate,
                spent,
                budget.Target,
                category.Id
            );
        }
        catch (BudgetNotFoundException ex) {
            logger.LogError(ex, "No budget with id: {BudgetId} found.", id);
            throw;
        }
        catch (DatabaseException ex) {
            logger.LogError(ex, "Database error retrieving budget with id: {BudgetId}", id);
            throw new Exception($"Database error retrieving budget with id: {id}", ex);
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error retrieving budget with id: {BudgetId}", id);
            throw new Exception($"Error retrieving budget with id: {id}", ex);
        }
    }

    public List<BudgetDto> GetByUserId(int userId) {
        try {
            var categories = categoryService.GetByUserId(userId);
            if (categories == null || categories.Count == 0) {
                logger.LogInformation("No categories found for user with id: {UserId}", userId);
                return new List<BudgetDto>();
            }

            var transactions = transactionService.GetByUserId(userId);
            var budgetDtos = new List<BudgetDto>();

            foreach (var category in categories) {
                try {
                    var budget = budgetRepository.FindByCategoryId(category.Id);
                    if (budget != null) {
                        var spent = transactions
                            .Where(t => t.CategoryId == category.Id
                                        && t.Date >= budget.StartDate
                                        && t.Date <= budget.EndDate)
                            .Sum(t => t.Amount);

                        budgetDtos.Add(new BudgetDto(
                            budget.Id,
                            category.Name,
                            budget.StartDate,
                            budget.EndDate,
                            spent,
                            budget.Target,
                            category.Id
                        ));
                    }
                }
                catch (BudgetNotFoundException) { }
            }

            return budgetDtos;
        }
        catch (DatabaseException ex) {
            logger.LogError(ex, "Database error retrieving budgets for user with id: {UserId}", userId);
            throw new Exception($"Database error retrieving budgets for user with id: {userId}", ex);
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error retrieving budgets for user with id: {UserId}", userId);
            throw new Exception($"Error retrieving budgets for user with id: {userId}", ex);
        }
    }

    public BudgetDto GetByCategoryId(int categoryId) {
        try {
            var budget = budgetRepository.FindByCategoryId(categoryId);
            if (budget == null)
                throw new BudgetNotFoundException($"No budget with category id: {categoryId} found.");

            var category = categoryService.GetById(categoryId);
            var spent = transactionService
                .GetByUserId(category.UserId)
                .Where(t => t.CategoryId == category.Id
                            && t.Date >= budget.StartDate
                            && t.Date <= budget.EndDate)
                .Sum(t => t.Amount);

            return new BudgetDto(
                budget.Id,
                category.Name,
                budget.StartDate,
                budget.EndDate,
                spent,
                budget.Target,
                category.Id
            );
        }
        catch (BudgetNotFoundException ex) {
            logger.LogError(ex, "No budget with category id: {CategoryId} found.", categoryId);
            throw;
        }
        catch (DatabaseException ex) {
            logger.LogError(ex, "Database error retrieving budget with category id: {CategoryId}", categoryId);
            throw new Exception($"Database error retrieving budget with category id: {categoryId}", ex);
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error retrieving budget with category id: {CategoryId}", categoryId);
            throw new Exception($"Error retrieving budget with category id: {categoryId}", ex);
        }
    }

    public BudgetDto Add(DateOnly startDate, DateOnly endDate, double target, int categoryId) {
        try {
            var newBudget = new Budget(startDate, endDate, target, categoryId);
            var addedBudget = budgetRepository.Add(newBudget);
            var category = categoryService.GetById(categoryId);

            return new BudgetDto(
                addedBudget.Id,
                category.Name,
                addedBudget.StartDate,
                addedBudget.EndDate,
                0,
                addedBudget.Target,
                category.Id
            );
        }
        catch (ArgumentException ex) {
            logger.LogError(ex, "Invalid budget data: {Message}", ex.Message);
            throw new InvalidDataException("Invalid budget data: " + ex.Message, ex);
        }
        catch (DatabaseException ex) {
            logger.LogError(ex, "Database error while adding a budget.");
            throw new Exception("Database error while adding a budget.", ex);
        }
        catch (Exception ex) {
            logger.LogError(ex, "An unexpected error occurred while adding a budget.");
            throw new Exception("An unexpected error occurred.", ex);
        }
    }

    public bool Edit(Budget budget) {
        try {
            return budgetRepository.Edit(budget);
        }
        catch (BudgetNotFoundException ex) {
            logger.LogError(ex, "Budget with ID {BudgetId} was not found for update.", budget.Id);
            return false;
        }
        catch (DatabaseException ex) {
            logger.LogError(ex, "Database error while updating budget with ID {BudgetId}", budget.Id);
            return false;
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error updating budget with ID {BudgetId}", budget.Id);
            return false;
        }
    }

    public bool Delete(int id) {
        try {
            var budget = budgetRepository.FindById(id);
            return budgetRepository.Delete(budget);
        }
        catch (BudgetNotFoundException ex) {
            logger.LogError(ex, "Budget with ID {BudgetId} was not found for deletion.", id);
            return false;
        }
        catch (DatabaseException ex) {
            logger.LogError(ex, "Database error while deleting budget with ID {BudgetId}", id);
            return false;
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error deleting budget with ID {BudgetId}", id);
            return false;
        }
    }
}