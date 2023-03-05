﻿namespace BW.EventSystem {
    public delegate void Callback();

    public delegate void Callback<T>(T arg1);

    public delegate void Callback<T, U>(T arg1, U arg2);

    public delegate void Callback<T, U, V>(T arg1, U arg2, V arg3);

    public delegate void Callback<T, U, V, N>(T arg1, U arg2, V arg3, N arg4);

    public delegate void Callback<T, U, V, N, M>(T arg1, U arg2, V arg3, N arg4, M arg5);
}
