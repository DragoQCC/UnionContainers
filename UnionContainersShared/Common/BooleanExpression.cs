using System.Linq.Expressions;

namespace UnionContainers.Shared.Common;

public class BooleanExpression
{
    private readonly Expression<Func<bool>> _expression;

    private BooleanExpression(Expression<Func<bool>> expression)
    {
        _expression = expression;
    }

    public static implicit operator BooleanExpression(bool value)
    {
        return new BooleanExpression(() => value);
    }

    public static implicit operator BooleanExpression(Expression<Func<bool>> expression)
    {
        return new BooleanExpression(expression);
    }

    public bool Evaluate()
    {
        return _expression.Compile().Invoke();
    }
}