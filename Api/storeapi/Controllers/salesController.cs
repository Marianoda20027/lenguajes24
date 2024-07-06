using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using storeapi.Database;
using storeapi.Models;
using Microsoft.AspNetCore.Authorization;


namespace storeapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        [HttpGet("transactions"), Authorize(Roles ="Admin")]
        public async Task<IActionResult> GetTransactions([FromQuery] string date)
        {
           if (string.IsNullOrWhiteSpace(date))
    {
        return BadRequest("La fecha no puede estar vacía.");
    }

    // Intentar parsear la fecha en el formato esperado (yyyy-MM-dd)
    if (!DateTime.TryParseExact(date, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
    {
        return BadRequest("Formato de fecha inválido. La fecha debe estar en formato yyyy-MM-dd.");
    }

    var salesDB = new SalesDB();

    Task<IEnumerable<string[]>> taskTransactionDay = salesDB.GetForDayAsync(parsedDate);
    Task<IEnumerable<string[]>> taskTransactionWeek = salesDB.GetForWeekAsync(parsedDate);
    Console.WriteLine(taskTransactionDay);

    await Task.WhenAll(taskTransactionDay, taskTransactionWeek);

    IEnumerable<TransactionManager.TransactionRecord> transactionsDays =
        await TransactionManager.LoadTransactionsFromDayAsync(taskTransactionDay.Result);
    IEnumerable<TransactionManager.TransactionRecord> transactionsWeeks =
        await TransactionManager.LoadTransactionsFromWeekAsync(taskTransactionWeek.Result);
       

    var response = new Dictionary<string, IEnumerable<object>>
    {
        ["transactionsDays"] = transactionsDays,
        ["transactionsWeeks"] = transactionsWeeks
    };
    
    return Ok(response);
}
    }
}
