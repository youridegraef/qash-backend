using Application.Domain;
using Application.Dtos;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class SavingGoalService(
    ISavingGoalRepository savingGoalRepository,
    ITransactionService transactionService,
    ILogger<SavingGoalService> logger)
    : ISavingGoalService
{
    public SavingGoalDto GetById(int id) {
        try {
            SavingGoal? savingGoal = savingGoalRepository.FindById(id);

            if (savingGoal != null!) {
                var amountSaved = CalculateAmountSavedForGoal(savingGoal.UserId);
                var dto = new SavingGoalDto(savingGoal.Id, savingGoal.Name, amountSaved,
                    savingGoal.Target, savingGoal.Deadline);
                return dto;
            }

            throw new SavingGoalNotFoundException($"Saving goal with id {id} not found.");
        }
        catch (SavingGoalNotFoundException ex) {
            logger.LogError(ex, "Saving goal with id {SavingGoalId} not found.", id);
            throw;
        }
        catch (DatabaseException ex) {
            logger.LogError(ex, "Database error retrieving saving goal with id {SavingGoalId}", id);
            throw new Exception($"Database error retrieving saving goal with id: {id}", ex);
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error retrieving saving goal with id {SavingGoalId}", id);
            throw new Exception($"Error retrieving saving goal with id: {id}", ex);
        }
    }

    public List<SavingGoalDto> GetByUserId(int userId) {
        try {
            var goals = savingGoalRepository.FindByUserId(userId);
            if (goals.Count == 0) {
                throw new SavingGoalNotFoundException($"Saving goal with user id {userId} not found.");
            }

            var dtos = goals.Select(goal =>
                new SavingGoalDto(
                    goal.Id,
                    goal.Name,
                    CalculateAmountSavedForGoal(userId),
                    goal.Target,
                    goal.Deadline
                )).ToList();

            return dtos;
        }
        catch (SavingGoalNotFoundException ex) {
            logger.LogError(ex, "Saving goal with id {SavingGoalId} not found.", userId);
            throw;
        }
        catch (KeyNotFoundException ex) {
            logger.LogError(ex, "No saving goals found for user_id: {UserId}", userId);
            throw new Exception($"No saving goals found for user_id: {userId}", ex);
        }
        catch (DatabaseException ex) {
            logger.LogError(ex, "Database error retrieving saving goals for user_id: {UserId}", userId);
            throw new Exception($"Database error retrieving saving goals for user_id: {userId}", ex);
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error retrieving saving goals for user_id: {UserId}", userId);
            throw new Exception($"Error retrieving saving goals for user_id: {userId}", ex);
        }
    }

    public List<SavingGoalDto> GetByUserIdPaged(int userId, int page, int pageSize) {
        try {
            var goals = savingGoalRepository.FindByUserIdPaged(userId, page, pageSize);
            if (goals.Count == 0) {
                throw new SavingGoalNotFoundException($"Saving goal with user id {userId} not found.");
            }

            var dtos = goals.Select(goal =>
                new SavingGoalDto(
                    goal.Id,
                    goal.Name,
                    CalculateAmountSavedForGoal(userId),
                    goal.Target,
                    goal.Deadline
                )).ToList();

            return dtos;
        }
        catch (SavingGoalNotFoundException) {
            logger.LogError("Saving goal with user id: {UserId} not found.", userId);
            throw;
        }
        catch (KeyNotFoundException ex) {
            logger.LogError(ex, "No saving goals found for user_id: {UserId}", userId);
            throw new Exception($"No saving goals found for user_id: {userId}", ex);
        }
        catch (DatabaseException ex) {
            logger.LogError(ex, "Database error retrieving paged saving goals for user_id: {UserId}", userId);
            throw new Exception($"Database error retrieving paged saving goals for user_id: {userId}", ex);
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error retrieving paged saving goals for user_id: {UserId}", userId);
            throw new Exception($"Error retrieving paged saving goals for user_id: {userId}", ex);
        }
    }

    public SavingGoalDto Add(string name, double target, DateOnly deadline, int userId) {
        try {
            var newSavingGoal = new SavingGoal(name, target, deadline, userId);
            var addedGoal = savingGoalRepository.Add(newSavingGoal);

            var amountSaved = CalculateAmountSavedForGoal(userId);

            var dto = new SavingGoalDto(addedGoal.Id, addedGoal.Name, amountSaved,
                addedGoal.Target, addedGoal.Deadline);

            return dto;
        }
        catch (ArgumentException ex) {
            logger.LogError(ex, "Invalid saving goal data: {Message}", ex.Message);
            throw new InvalidDataException("Invalid saving goal data: " + ex.Message, ex);
        }
        catch (DatabaseException ex) {
            logger.LogError(ex, "Database error while adding a saving goal.");
            throw new Exception("Database error while adding a saving goal.", ex);
        }
        catch (Exception ex) {
            logger.LogError(ex, "An unexpected error occurred while adding a saving goal.");
            throw new Exception("An unexpected error occurred.", ex);
        }
    }

    public bool Edit(SavingGoal savingGoal) {
        try {
            return savingGoalRepository.Edit(savingGoal);
        }
        catch (SavingGoalNotFoundException ex) {
            logger.LogError(ex, "Saving goal with ID {SavingGoalId} was not found for update.", savingGoal.Id);
            return false;
        }
        catch (DatabaseException ex) {
            logger.LogError(ex, "Database error while updating saving goal with ID {SavingGoalId}", savingGoal.Id);
            return false;
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error updating saving goal with ID {SavingGoalId}", savingGoal.Id);
            return false;
        }
    }

    public bool Delete(int id) {
        try {
            var savingGoal = savingGoalRepository.FindById(id);
            return savingGoalRepository.Delete(savingGoal);
        }
        catch (SavingGoalNotFoundException ex) {
            logger.LogError(ex, "Saving goal with ID {SavingGoalId} was not found for deletion.", id);
            return false;
        }
        catch (DatabaseException ex) {
            logger.LogError(ex, "Database error while deleting saving goal with ID {SavingGoalId}", id);
            return false;
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error deleting saving goal with ID {SavingGoalId}", id);
            return false;
        }
    }

    private double CalculateAmountSavedForGoal(int userId) {
        var goals = savingGoalRepository.FindByUserId(userId);
        return transactionService.GetBalance(userId) / goals.Count;
    }
}