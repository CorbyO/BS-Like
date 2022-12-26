using System;
using Cysharp.Threading.Tasks;

namespace Corby.Frameworks
{
    public interface IHandleable { }
    public interface IHandleable<T> : IHandleable
        where T : class, IHandle
    {
        T Handle { get; set; }
    }

    public interface IHandle
    {
    }

    public delegate void Signal();
    public delegate void Signal<in T>(T arg);
    public delegate void Signal<in T1, in T2>(T1 arg1, T2 arg2);
    public delegate void Signal<in T1, in T2, in T3>(T1 arg1, T2 arg2, T3 arg3);
    public delegate T Request<out T>();
    public delegate T Request<out T, in TA1>(TA1 arg1);
    public delegate T Request<out T, in TA1, in TA2>(TA1 arg1, TA2 arg2);
    public delegate T Request<out T, in TA1, in TA2, in TA3>(TA1 arg1, TA2 arg2, TA3 arg3);
    public delegate UniTask<T> RequestAsync<T>();
    public delegate UniTask<T> RequestAsync<T, in TA1>(TA1 arg1);
    public delegate UniTask<T> RequestAsync<T, in TA1, in TA2>(TA1 arg1, TA2 arg2);
    public delegate UniTask<T> RequestAsync<T, in TA1, in TA2, in TA3>(TA1 arg1, TA2 arg2, TA3 arg3);
}