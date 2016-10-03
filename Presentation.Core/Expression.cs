using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace Presentation.Core
{
    /// <summary>
    /// Utility class for working with expressions
    /// </summary>
    public static class Expression
    {
        /// <summary>
        /// Converts a property Expression into a property string
        /// </summary>
        /// <typeparam name="TObj">The view model the property is on</typeparam>
        /// <typeparam name="T">The type of the property</typeparam>
        /// <param name="o">The view model</param>
        /// <param name="propertyExpression">A property expression of the property</param>
        /// <returns>The string representing the property name</returns>
        [DebuggerStepThrough]
        public static string NameOf<TObj, T>(this TObj o, Expression<Func<TObj, T>> propertyExpression) where
            TObj : IViewModel
        {
            if (propertyExpression == null)
                throw new ArgumentNullException(nameof(propertyExpression));

            MemberExpression property = null;

            // it's possible to end up with conversions, as the expressions are trying 
            // to convert to the same underlying type
            if (propertyExpression.Body.NodeType == ExpressionType.Convert)
            {
                var convert = propertyExpression.Body as UnaryExpression;
                if (convert != null)
                {
                    property = convert.Operand as MemberExpression;
                }
            }

            if (property == null)
            {
                property = propertyExpression.Body as MemberExpression;
            }
            if (property == null)
                throw new Exception(
                    "propertyExpression cannot be null and should be passed in the format x => x.PropertyName");

            return property.Member.Name;
        }

        /// <summary>
        /// Converts multiple property Expressions into a property string array
        /// </summary>
        /// <typeparam name="TObj">The view model the property is on</typeparam>
        /// <typeparam name="T">The type of the property</typeparam>
        /// <param name="o">The view model</param>
        /// <param name="propertyExpressions">One or more property expressions</param>
        /// <returns>The string array representing the property names</returns>
        [DebuggerStepThrough]
        public static string[] NameOf<TObj, T>(this TObj o, params Expression<Func<TObj, T>>[] propertyExpressions) where
            TObj : IViewModel
        {
            return propertyExpressions.Select(p => NameOf(o, p)).ToArray();
        }
    }
}