// Copyright 2017 Igor' Leonidov
// Licensed under the Apache License, Version 2.0

namespace UltimateQuadTree
{
    /// <summary>Stores a set of four values of a Double that represent the location and size of a rectangle</summary>
    public struct QuadTreeRect
    {
        /// <summary>Gets the x-coordinate of the upper-left corner of this <see cref="T:UltimateQuadTree.QuadTreeRect"></see> structure.</summary>
        /// <returns>The x-coordinate of the upper-left corner of this <see cref="T:UltimateQuadTree.QuadTreeRect"></see> structure.</returns>
        public readonly double X;
        /// <summary>Gets the y-coordinate of the upper-left corner of this <see cref="T:UltimateQuadTree.QuadTreeRect"></see> structure.</summary>
        /// <returns>The y-coordinate of the upper-left corner of this <see cref="T:UltimateQuadTree.QuadTreeRect"></see> structure.</returns>
        public readonly double Y;
        /// <summary>Gets the width of this <see cref="T:UltimateQuadTree.QuadTreeRect"></see> structure.</summary>
        /// <returns>The width of this <see cref="T:UltimateQuadTree.QuadTreeRect"></see> structure.</returns>
        public readonly double Width;
        /// <summary>Gets the height of this <see cref="T:UltimateQuadTree.QuadTreeRect"></see> structure.</summary>
        /// <returns>The height of this <see cref="T:UltimateQuadTree.QuadTreeRect"></see> structure.</returns>
        public readonly double Height;
        
        /// <summary>Gets the y-coordinate of the top edge of this <see cref="T:UltimateQuadTree.QuadTreeRect"></see> structure.</summary>
        /// <returns>The y-coordinate of the top edge of this <see cref="T:UltimateQuadTree.QuadTreeRect"></see> structure.</returns>
        public double Top => Y;
        /// <summary>Gets the y-coordinate that is the sum of the <see cref="P:UltimateQuadTree.QuadTreeRect.Y"></see> and <see cref="P:UltimateQuadTree.QuadTreeRect.Height"></see> property values of this <see cref="T:UltimateQuadTree.QuadTreeRect"></see> structure.</summary>
        /// <returns>The y-coordinate that is the sum of <see cref="P:UltimateQuadTree.QuadTreeRect.Y"></see> and <see cref="P:UltimateQuadTree.QuadTreeRect.Height"></see> of this <see cref="T:UltimateQuadTree.QuadTreeRect"></see>.</returns>
        public double Bottom => Y + Height;
        /// <summary>Gets the x-coordinate of the left edge of this <see cref="T:UltimateQuadTree.QuadTreeRect"></see> structure.</summary>
        /// <returns>The x-coordinate of the left edge of this <see cref="T:UltimateQuadTree.QuadTreeRect"></see> structure.</returns>
        public double Left => X;
        /// <summary>Gets the x-coordinate that is the sum of <see cref="P:UltimateQuadTree.QuadTreeRect.X"></see> and <see cref="P:UltimateQuadTree.QuadTreeRect.Width"></see> property values of this <see cref="T:UltimateQuadTree.QuadTreeRect"></see> structure.</summary>
        /// <returns>The x-coordinate that is the sum of <see cref="P:UltimateQuadTree.QuadTreeRect.X"></see> and <see cref="P:UltimateQuadTree.QuadTreeRect.Width"></see> of this <see cref="T:UltimateQuadTree.QuadTreeRect"></see>.</returns>
        public double Right => X + Width;
        
        internal double MidX => X + HalfWidth;
        internal double MidY => Y + HalfHeight;

        internal QuadTreeRect LeftTopQuarter => new QuadTreeRect(X, Y, HalfWidth, HalfHeight);
        internal QuadTreeRect LeftBottomQuarter => new QuadTreeRect(X, MidY, HalfWidth, HalfHeight);
        internal QuadTreeRect RightTopQuarter => new QuadTreeRect(MidX, Y, HalfWidth, HalfHeight);
        internal QuadTreeRect RightBottomQuarter => new QuadTreeRect(MidX, MidY, HalfWidth, HalfHeight);

        private double HalfWidth => Width * 0.5;
        private double HalfHeight => Height * 0.5;

        /// <summary>Initializes a new instance of the <see cref="T:UltimateQuadTree.QuadTreeRect"></see> class with the specified location and size.</summary>
        /// <param name="x">The x-coordinate of the upper-left corner of the rectangle.</param>
        /// <param name="y">The y-coordinate of the upper-left corner of the rectangle.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        public QuadTreeRect(double x, double y, double width, double height)
        {
            X = x;
            Y = y;

            Width = width;
            Height = height;
        }
    }
}