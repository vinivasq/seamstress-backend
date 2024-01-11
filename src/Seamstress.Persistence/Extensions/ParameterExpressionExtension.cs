using System.Linq.Expressions;

namespace Seamstress.Persistence.Extensions
{
  public class ParameterExpressionExtension
  {
    public class ParameterReplacer : ExpressionVisitor
    {
      private readonly ParameterExpression _source;
      private readonly ParameterExpression _target;

      public ParameterReplacer(ParameterExpression source, ParameterExpression target)
      {
        _source = source ?? throw new ArgumentNullException(nameof(source));
        _target = target ?? throw new ArgumentNullException(nameof(target));
      }

      protected override Expression VisitParameter(ParameterExpression node)
      {
        return node == _source ? _target : base.VisitParameter(node);
      }
    }
  }
}