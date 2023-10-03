using Microsoft.EntityFrameworkCore;
using TestTask.Data;
using TestTask.Enums;
using TestTask.Models;
using TestTask.Services.Interfaces;
namespace TestTask.Services.Implementations;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context) => _context = context;

    public async Task<User> GetUser()
    {
        var userWithOrdersCount = await _context.Orders
            .GroupBy(o => o.UserId) // группируем записи по UserId
            .Select(u => new    // подсчитываем количество заказов для каждого пользователя
            {
                UserId = u.Key,
                OrdersCount = u.Count()
            })
            .OrderByDescending(u => u.OrdersCount)  // сортируем по убыванию
            .FirstOrDefaultAsync(); // берём первую запись

        var user = await _context.Users // фильтруем записи по полю Id, равному UserId пользователя с наибольшим количеством заказов
            .Where(u => u.Id
            .Equals(userWithOrdersCount.UserId))
            .FirstOrDefaultAsync();

        return user;
    }

    public async Task<List<User>> GetUsers() =>
        await _context.Users
            .Where(u => u.Status
            .Equals(UserStatus.Inactive))   // выбираем неактивных пользователей
            .ToListAsync();
}