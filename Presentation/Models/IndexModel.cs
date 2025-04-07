using System.Text.Json;
using Application.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Presentation.Models;

public class IndexModel : PageModel
{
    public User? LoggedInUser { get; set; }
    public List<SavingGoal> SavingGoals { get; set; }
    public double Balance { get; set; }
    public double Expenses { get; set; }
    public double Income { get; set; }
}