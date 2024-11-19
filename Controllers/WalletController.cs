using Meshwark.Data;
using Meshwark.DTOs;
using Meshwark.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Meshwark.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetWallet(Guid userId)
        {
            var wallet = await _walletService.GetWalletByUserId(userId);
            if (wallet == null) return Ok(new List<Wallet>());

            return Ok(wallet);
        }

        [HttpGet("transactions/{userId}/{month}")]
        public async Task<IActionResult> GetTransactions(Guid userId, int month)
        {
            var transactions = await _walletService.GetTransactionsByUserIdAndMonth(userId, month);
            if (transactions == null) return Ok(new List<Transaction>());

            return Ok(transactions);
        }
        [HttpPost("send-transaction")]
        public async Task<IActionResult> SendTransaction([FromBody] TransactionDto transactionDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _walletService.SendTransaction(transactionDto.SenderId, transactionDto.ReceiverId, transactionDto.Amount, transactionDto.Description);
            if (!result)
                return BadRequest("Transaction failed. Please check the details.");

            return Ok("Transaction successful.");
        }

        [HttpPost("addTransaction/{userId}")]
        public async Task<IActionResult> AddTransaction(Guid userId, [FromBody] Transaction transaction)
        {
            if (transaction == null) return BadRequest("Transaction is null");

            await _walletService.AddTransaction(userId, transaction);
            return Ok("Transaction added successfully.");
        }

        [HttpGet("sumTransactions/{userId}/{period}")]
        public async Task<IActionResult> SumTransactions(Guid userId, string period)
        {
            decimal total = await _walletService.SumTransactions(userId, period);
            return Ok(total);
        }
    }

    public interface IWalletService
    {
        Task<Wallet> GetWalletByUserId(Guid userId);
        Task<List<Transaction>> GetTransactionsByUserIdAndMonth(Guid userId, int month);
        Task AddTransaction(Guid userId, Transaction transaction);
        Task<decimal> SumTransactions(Guid userId, string period);
        Task<bool> SendTransaction(Guid senderId, Guid receiverId, decimal amount, string description);
    }

    public class WalletService : IWalletService
    {
        private readonly ApiContext _context;

        public WalletService(ApiContext context)
        {
            _context = context;
        }

        public async Task<Wallet> GetWalletByUserId(Guid userId)
        {
            return await _context.Wallets
                .Include(w => w.Transactions)
                .FirstOrDefaultAsync(w => w.UserId == userId);
        }

        public async Task<List<Transaction>> GetTransactionsByUserIdAndMonth(Guid userId, int month)
        {
            var wallet = await GetWalletByUserId(userId);
            if (wallet == null) return null;

            return wallet.Transactions
                .Where(t => t.Date.Month == month)
                .ToList();
        }

        public async Task AddTransaction(Guid userId, Transaction transaction)
        {
            // Ensure the transaction is valid
            transaction.Date = DateTime.Now; // Set the current date
            await _context.Transactions.AddAsync(transaction); // Add the transaction to the context

            // Update the wallet
            var wallet = await GetWalletByUserId(userId);
            if (wallet != null)
            {
                wallet.Transactions.Add(transaction);
                wallet.Balance += transaction.Amount; // Update balance if necessary
                await _context.SaveChangesAsync(); // Save changes to the database
            }
        }

        public async Task<decimal> SumTransactions(Guid userId, string period)
        {
            var wallet = await GetWalletByUserId(userId);
            if (wallet == null) return 0;

            DateTime startDate = DateTime.Now;

            switch (period.ToLower())
            {
                case "1day":
                    startDate = DateTime.Now.AddDays(-1);
                    break;
                case "3days":
                    startDate = DateTime.Now.AddDays(-3);
                    break;
                case "week":
                    startDate = DateTime.Now.AddDays(-7);
                    break;
                case "month":
                    startDate = DateTime.Now.AddMonths(-1);
                    break;
                case "3months":
                    startDate = DateTime.Now.AddMonths(-3);
                    break;
                case "6months":
                    startDate = DateTime.Now.AddMonths(-6);
                    break;
                case "9months":
                    startDate = DateTime.Now.AddMonths(-9);
                    break;
                case "year":
                    startDate = DateTime.Now.AddYears(-1);
                    break;
                case "alltime":
                    startDate = DateTime.MinValue;
                    break;
                default:
                    throw new ArgumentException("Invalid period specified");
            }

            // Sum the transactions within the given period
            return wallet.Transactions
                .Where(t => t.Date >= startDate)
                .Sum(t => t.Amount);
        }

        public async Task<bool> SendTransaction(Guid senderId, Guid receiverId, decimal amount, string description)
        {
            var senderWallet = await _context.Wallets.Include(w => w.Transactions).FirstOrDefaultAsync(w => w.UserId == senderId);
            var receiverWallet = await _context.Wallets.Include(w => w.Transactions).FirstOrDefaultAsync(w => w.UserId == receiverId);

            if (senderWallet == null || receiverWallet == null || senderWallet.Balance < amount)
            {
                return false; // Insufficient funds or wallet not found
            }

            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                SenderId = senderId,
                ReceiverId = receiverId,
                Amount = amount,
                Date = DateTime.Now,
                Description = description
            };

            // Update balances
            senderWallet.Balance -= amount;
            receiverWallet.Balance += amount;

            // Add transaction to both wallets
            senderWallet.Transactions.Add(transaction);
            receiverWallet.Transactions.Add(transaction);

            await _context.SaveChangesAsync();
            return true;
        }

    }
}
