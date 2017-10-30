// Copyright 2017 Igor' Leonidov
// Licensed under the Apache License, Version 2.0

namespace UltimateQuadTree
{
    /// <summary>Provides a set of methods for getting boundaries of the element.</summary>
    /// <typeparam name="T">The type of elements in the QuadTree.</typeparam>
    public interface IQuadTreeObjectBounds<in T>
    {
        /// <summary>Gets the x-coordinate of the left edge of the object.</summary>
        double GetLeft(T obj);
        /// <summary>Gets the x-coordinate of the right edge of the object.</summary>
        double GetRight(T obj);
        /// <summary>Gets the y-coordinate of the top edge of the object.</summary>
        double GetTop(T obj);
        /// <summary>Gets the y-coordinate of the bottom edge of the object.</summary>
        double GetBottom(T obj);
    }
}