using Castle.DynamicProxy;

public class DisposeInterceptor : IInterceptor
{
    public bool IsDisposed { get; private set; }

    public void Intercept(IInvocation invocation)
    {
        if (invocation.Method.Name == "Dispose")
        {
            IsDisposed = true;
        }
        else if (IsDisposed)
        {
            throw new DisposablePatternException("Member invoked on a disposed object.");
        }

        invocation.Proceed();
    }
}