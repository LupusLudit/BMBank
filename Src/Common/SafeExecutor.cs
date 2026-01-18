namespace BMBank.Src.Common
{
    /// <include file='../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="SafeExecutor"]/*'/>
    public static class SafeExecutor
    {

        private const string ErrorPrefix = "ER";

        /// <summary>
        /// Safely executes the specified action.
        /// </summary>
        /// <param name="action">The action to be executed (in the try block).</param>
        /// <returns>
        /// Nothing if the action completes successfully; otherwise, the formatted error message (string).
        /// </returns>
        public static string Execute(Func<string> action)
        {
            try
            {
                return action();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                LogError(ex);
                return FormatError(ex.Message);
            }
        }


        /// <summary>
        /// Safely executes an asynchronous action.
        /// </summary>
        /// <param name="action">The action to be executed (in the try block).</param>
        /// <returns>
        /// Nothing if the action completes successfully; otherwise, the formatted error message (string).
        /// </returns>
        public static async Task<string> ExecuteAsync(Func<Task<string>> action)
        {
            try
            {
                return await action();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                LogError(ex);
                return FormatError(ex.Message);
            }
        }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="ex">The exception to be logged.</param>
        private static void LogError(Exception ex)
        {
            Logger.Error($"{ex.Message}");
        }

        /// <summary>
        /// Formats the error message to include the error prefix.
        /// The formatted error is used to be displayed to the user.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>
        /// The formatted error message (string).
        /// </returns>
        private static string FormatError(string errorMessage)
        {
            return $"{ErrorPrefix} {errorMessage}";
        }
    }
}
