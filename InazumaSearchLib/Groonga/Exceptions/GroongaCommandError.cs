using System;

namespace InazumaSearchLib.Groonga.Exceptions
{
    public class GroongaCommandError : Exception
    {
        public virtual int ReturnCode { get; set; }
        public virtual string CommandErrorMessage { get; set; }

        public GroongaCommandError(int returnCode, string commandErrorMessage) : base($"Groongaコマンドエラー : returnCode={returnCode}, message={commandErrorMessage}")
        {
            ReturnCode = returnCode;
            CommandErrorMessage = CommandErrorMessage;
        }
    }
}
