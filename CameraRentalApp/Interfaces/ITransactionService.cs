using System.Collections.Generic;
using System.Threading.Tasks;
using CameraRentalApp.Models;

namespace CameraRentalApp.Interfaces
{
    public interface ITransactionService
    {
        Task<IEnumerable<Transaction>> GetAllTransactionsAsync();
        Task<IEnumerable<Customer>> GetCustomers(); 
        Task<IEnumerable<Camera>> GetAvailableCameras(); 
        Task<bool> CreateTransactionAsync(Transaction transaction); 
        Task<bool> CompleteTransactionAsync(int transactionId);
    }
}
