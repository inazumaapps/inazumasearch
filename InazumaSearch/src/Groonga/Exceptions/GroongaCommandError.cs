using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InazumaSearch.Groonga.Exceptions
{
    public class GroongaCommandError : Exception
    {
        public virtual int ReturnCode { get; set; }
        public virtual string CommandErrorMessage { get; set; }

        public GroongaCommandError(string message, int returnCode, string commandErrorMessage) : base(message)
        {
            ReturnCode = returnCode;
            CommandErrorMessage = CommandErrorMessage;
        }
    }
}
