using System;
using System.Runtime.Serialization;

[Serializable]
internal class DisposablePatternException : Exception
{
    public DisposablePatternException()
    {
    }

    public DisposablePatternException(string message) : base(message)
    {
    }

    public DisposablePatternException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected DisposablePatternException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}