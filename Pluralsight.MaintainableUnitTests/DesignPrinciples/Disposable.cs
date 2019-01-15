using System;

public class Disposable<TDisposable> where TDisposable : class, IDisposable
{
    public TDisposable Object { get; }
    private Func<bool> IsDisposed;

    public Disposable(TDisposable obj, Func<bool> isDisposed)
    {
        Object = obj;
        IsDisposed = isDisposed;
    }

    public void VerifyDisposed()
    {
        if (!IsDisposed())
            throw new DisposablePatternException("Object not disposed when expected.");
    }
}