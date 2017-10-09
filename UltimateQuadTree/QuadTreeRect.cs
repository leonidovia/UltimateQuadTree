// Copyright 2017 Igor' Leonidov
// Licensed under the Apache License, Version 2.0

namespace UltimateQuadTree
{
    public struct QuadTreeRect
    {
        public readonly double X;
        public readonly double Y;

        public readonly double Width;
        public readonly double Height;

        public readonly double MidX;
        public readonly double MidY;

        public readonly double Top;
        public readonly double Bottom;
        public readonly double Left;
        public readonly double Right;

        private readonly double _halfWidth;
        private readonly double _halfHeight;

        public QuadTreeRect LeftTopQuarter => new QuadTreeRect(X, Y, _halfWidth, _halfHeight);
        public QuadTreeRect LeftBottomQuarter => new QuadTreeRect(X, MidY, _halfWidth, _halfHeight);
        public QuadTreeRect RightTopQuarter => new QuadTreeRect(MidX, Y, _halfWidth, _halfHeight);
        public QuadTreeRect RightBottomQuarter => new QuadTreeRect(MidX, MidY, _halfWidth, _halfHeight);

        public QuadTreeRect(double x, double y, double width, double height)
        {
            Left = X = x;
            Top = Y = y;

            Width = width;
            Height = height;

            Right = X + Width;
            Bottom = Top + Height;

            _halfWidth = Width / 2;
            _halfHeight = Height / 2;

            MidX = X + _halfWidth;
            MidY = Y + _halfHeight;
        }
    }
}