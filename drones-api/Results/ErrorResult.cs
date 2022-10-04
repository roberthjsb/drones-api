using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace drones_api.Results
{
    public class ErrorResult
    {
        public ErrorResult(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException($"\"{nameof(message)}\" no puede ser NULL ni un espacio en blanco.", nameof(message));
            }
            Message = message;
        }

        public string Message { get; }
    }
}
