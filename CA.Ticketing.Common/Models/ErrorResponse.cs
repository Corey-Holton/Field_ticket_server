namespace CA.Ticketing.Common.Models
{
    /// <summary>
    /// Error response
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Trace Id
        /// </summary>
        public string TraceId { get; set; }

        /// <summary>
        /// Status code of the error message
        /// </summary>
        /// <example>
        /// 500
        /// </example>
        public int StatusCode { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
