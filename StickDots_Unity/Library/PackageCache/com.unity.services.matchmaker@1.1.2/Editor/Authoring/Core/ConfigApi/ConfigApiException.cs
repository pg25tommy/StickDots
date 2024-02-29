using System;

namespace Unity.Services.Matchmaker.Authoring.Core.ConfigApi
{
    class ConfigApiException : Exception
    {
        internal int ErrorCode { get; }

        internal ConfigApiException(int errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
