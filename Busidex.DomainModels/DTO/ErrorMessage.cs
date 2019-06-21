namespace Busidex.DomainModels.DTO
{
    /// <summary>
    /// Base model for all non-successful API responses.
    /// </summary>
    public class ErrorMessage
    {
        public string Message { get; set; }

        public int ErrorCode { get; set; }

        /// <summary>
        /// Exception details.  This property is populated in debug mode only.
        /// </summary>
        public ErrorException Exception { get; set; }
    }
}