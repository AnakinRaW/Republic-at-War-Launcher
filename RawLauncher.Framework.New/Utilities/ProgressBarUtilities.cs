using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace RawLauncher.Framework.Utilities
{
    public static class ProgressBarUtilities
    {
        public static async Task AnimateProgressBar<T>(double oldValue, double newValue,int time,T outobj, Expression<Func<T, double>> progress)
        {
            if (oldValue == newValue)
                return;

            var expr = (MemberExpression) progress.Body;
            var prop = (PropertyInfo) expr.Member;

            if (oldValue > newValue)
            {
                for (var i = oldValue; i > newValue - 1; i--)
                {
                    prop.SetValue(outobj, i, null);
                    await Task.Run(() => Thread.Sleep(time));
                }
            }
            else
                for (var i = oldValue; i <= newValue + 1; i++)
                {
                    prop.SetValue(outobj, i, null);
                    await Task.Run(() => Thread.Sleep(time));
                }
        }
    }
}
