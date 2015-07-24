﻿using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace RawLauncherWPF.Utilities
{
    public static class ProgressBarUtilities
    {
        async public static Task AnimateProgressBar<T>(int oldValue, int newValue,int time,T outobj, Expression<Func<T, int>> progress)
        {
            if (oldValue == newValue)
                return;

            var expr = (MemberExpression) progress.Body;
            var prop = (PropertyInfo) expr.Member;

            if (oldValue > newValue)
            {
                for (int i = oldValue; i > newValue - 1; i--)
                {
                    prop.SetValue(outobj, i, null);
                    await Task.Run(() => Thread.Sleep(time));
                }
            }
            else
                for (int i = oldValue; i < newValue + 1; i++)
                {
                    prop.SetValue(outobj, i, null);
                    await Task.Run(() => Thread.Sleep(time));
                }
        }
    }
}