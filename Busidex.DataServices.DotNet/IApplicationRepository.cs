﻿namespace Busidex.DataServices.DotNet
{
    public interface IApplicationRepository
    {
        Task SaveApplicationError(string error, string innerException, string stackTrace, long userId);
    }
}
