using Busidex.DataServices.DotNet;
using System;
using System.Threading.Tasks;

namespace Busidex.Functions.DotNetCore
{
    public class FunctionBase
    {
        protected static async Task LogError(Exception ex, long userId)
        {
            var connStr = Environment.GetEnvironmentVariable("busidexConnectionString");

            ICardRepository cardRepository = new CardRepository(connStr);
            await cardRepository.SaveApplicationError(ex, userId);
        }
    }
}
