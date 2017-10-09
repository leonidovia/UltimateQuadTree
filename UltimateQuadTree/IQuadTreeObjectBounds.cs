// Copyright 2017 Igor' Leonidov
// Licensed under the Apache License, Version 2.0

namespace UltimateQuadTree
{
    public interface IQuadTreeObjectBounds<in T>
    {
        double GetLeft(T obj);
        double GetRight(T obj);
        double GetTop(T obj);
        double GetBottom(T obj);
    }
}