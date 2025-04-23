using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace EmployeeApp.Exceptions
{
    public class GlobalExceptionHandler
    {
        private readonly ILogger _logger;

        public GlobalExceptionHandler(ILogger logger)
        {
            _logger = logger;
            AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;
            TaskScheduler.UnobservedTaskException += HandleUnobservedTaskException;
        }

        public void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                _logger.Fatal(ex, "Unhandled exception occurred. Application will terminate: {IsTerminating}", e.IsTerminating);

                Thread.Sleep(1000);
            }
            else
            {
                _logger.Fatal("Unknown unhandled exception occurred. Application will terminate: {IsTerminating}", e.IsTerminating);
            }
        }

        public void HandleUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            _logger.Error(e.Exception, "Unobserved task exception occurred");
            e.SetObserved();
        }

        public void HandleException(Exception ex, string context = null)
        {
            switch (ex)
            {
                case EmployeeNotFoundException enfe:
                    _logger.Warning(enfe, "Employee not found: {EmployeeId}", enfe.EmployeeId);
                    break;

                case InvalidInputException iie:
                    _logger.Warning(iie, "Invalid input for {FieldName}: {InvalidValue}", iie.FieldName, iie.InvalidValue);
                    break;

                case IOException ioe:
                    _logger.Error(ioe, "I/O error occurred");
                    break;

                default:
                    _logger.Error(ex, "An unexpected error occurred" + (context != null ? $" in {context}" : ""));
                    break;
            }
        }
    }

}
