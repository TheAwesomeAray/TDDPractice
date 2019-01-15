using System;

public class DisposablePattern
{
    public static Disposable<TDisposable> For<TDisposable>(TDisposable wrapAround)
        where TDisposable : class, IDisposable
    {
        Castle.DynamicProxy.ProxyGenerator generator = new Castle.DynamicProxy.ProxyGenerator();

        DisposeInterceptor interceptor = new DisposeInterceptor();
        TDisposable disposable = generator.CreateInterfaceProxyWithTarget(wrapAround, interceptor);

        return new Disposable<TDisposable>(disposable, () => interceptor.IsDisposed);
    }
}