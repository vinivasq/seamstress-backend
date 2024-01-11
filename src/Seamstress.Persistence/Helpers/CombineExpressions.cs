using System.Linq.Expressions;
using static Seamstress.Persistence.Extensions.ParameterExpressionExtension;

namespace Seamstress.Persistence.Helpers
{
  public class CombineEpressions
  {
    public static Expression<Func<T, bool>> CombineExpressions<T>(
      Expression<Func<T, bool>> left,
      Expression<Func<T, bool>> right)
    {
      var visitor = new ParameterReplacer(right.Parameters[0], left.Parameters[0]);
      right = (Expression<Func<T, bool>>)visitor.Visit(right);

      return Expression.Lambda<Func<T, bool>>(
          Expression.AndAlso(left.Body, right.Body), left.Parameters);
    }
  }
}