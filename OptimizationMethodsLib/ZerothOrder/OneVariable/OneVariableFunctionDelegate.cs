//-----------------------------------------------------------------------
// <copyright file="OneVariableFunctionDelegate.cs" company="Home Corporation">
//     Copyright (c) Home Corporation 2009. All rights reserved.
// </copyright>
// <author>Sergii Pechenizkyi</author>
//-----------------------------------------------------------------------

namespace OptimizationMethods.ZerothOrder.OneVariable
{
    /// <summary>
    /// Ссылка на функциональную зависимость f(x).
    /// </summary>
    /// <param name="x">Переменная х.</param>
    /// <returns>Значение функции в точке х.</returns>
    public delegate double OneVariableFunction(double x);
}