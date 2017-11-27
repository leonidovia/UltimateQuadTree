// Copyright 2017 Igor' Leonidov
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;

namespace UltimateQuadTree
{
    /// <summary>
    /// Represents a tree data structure in which each internal node has exactly four children. 
    /// Used to partition a two-dimensional space by recursively subdividing it into four quadrants or regions.
    /// Allows to quickly find the nearest objects.
    /// </summary>
    /// <typeparam name="T">The type of elements in the QuadTree.</typeparam>
    public class QuadTree<T>
    {
        /// <summary> Gets the max number of elements in one rectangle. </summary>
        public int MaxObjects { get; }
        /// <summary> Gets the max depth level. </summary>
        public int MaxLevel { get; }
        /// <summary> Gets the number of elements contained in the <see cref="T:UltimateQuadTree.QuadTree`1"></see>. </summary>
        public int ObjectCount { get; private set; }
        /// <summary> Gets the boundary rectangle of the <see cref="T:UltimateQuadTree.QuadTree`1"></see>. </summary>
        public QuadTreeRect MainRect { get; }

        private Sector<T> _rootSector;
        private readonly IQuadTreeObjectBounds<T> _objectBounds;

        /// <summary> Initializes a new instance of the <see cref="T:UltimateQuadTree.QuadTree`1"></see> class with initial coordinates. </summary>
        /// <param name="x">The x-coordinate of the upper-left corner of the boundary rectangle.</param>
        /// <param name="y">The y-coordinate of the upper-left corner of the boundary rectangle.</param>
        /// <param name="width">The width of the boundary rectangle.</param>
        /// <param name="height">The height of the boundary rectangle.</param>
        /// <param name="objectBounds">The set of methods for getting boundaries of an element.</param>
        /// <param name="maxObjects">The max number of elements in one rectangle. The default is 10.</param>
        /// <param name="maxLevel">The max depth level. The default is 5. </param>
        public QuadTree(double x, double y, double width, double height, IQuadTreeObjectBounds<T> objectBounds, int maxObjects = 10, int maxLevel = 5)
        {
            if(objectBounds == null) throw new ArgumentNullException(nameof(objectBounds));

            MaxObjects = maxObjects;
            MaxLevel = maxLevel;
            MainRect = new QuadTreeRect(x, y, width, height);
            _objectBounds = objectBounds;
            _rootSector = new LeafSector<T>(0, MainRect, objectBounds, maxObjects, maxLevel);
        }

        /// <summary> Initializes a new instance of the <see cref="T:UltimateQuadTree.QuadTree`1"></see> class. </summary>
        /// <param name="width">The width of the boundary rectangle.</param>
        /// <param name="height">The height of the boundary rectangle.</param>
        /// <param name="objectBounds">The set of methods for getting boundaries of an element.</param>
        /// <param name="maxObjects">The max number of elements in one rectangle. The default is 10.</param>
        /// <param name="maxLevel">The max depth level. The default is 5. </param>
        public QuadTree(double width, double height, IQuadTreeObjectBounds<T> objectBounds, int maxObjects = 10, int maxLevel = 5)
            : this(0, 0, width, height, objectBounds, maxObjects, maxLevel)
        {
        }

        /// <summary> Removes all elements from the <see cref="T:UltimateQuadTree.QuadTree`1"></see>. </summary>
        public void Clear()
        {
            _rootSector.Clear();
            _rootSector = new LeafSector<T>(0, MainRect, _objectBounds, MaxObjects, MaxLevel);
            ObjectCount = 0;
        }

        /// <summary> Inserts an element into the <see cref="T:UltimateQuadTree.QuadTree`1"></see>. </summary>
        /// <param name="obj">The object to insert.</param>
        /// <returns>true if the element is added to the <see cref="T:UltimateQuadTree.QuadTree`1"></see>; false if failure.</returns>
        public bool Insert(T obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            if (!IsObjectInMainRect(obj)) return false;

            if (_rootSector.TryInsert(obj))
            {
                ObjectCount++;
                return true;
            }

            _rootSector = _rootSector.Quarter();
            _rootSector.TryInsert(obj);

            ObjectCount++;

            return true;
        }

        private bool IsObjectInMainRect(T obj)
        {
            if (_objectBounds.GetTop(obj) > MainRect.Bottom) return false;
            if (_objectBounds.GetBottom(obj) < MainRect.Top) return false;
            if (_objectBounds.GetLeft(obj) > MainRect.Right) return false;
            if (_objectBounds.GetRight(obj) < MainRect.Left) return false;

            return true;
        }

        /// <summary> Inserts the elements of a collection into the <see cref="T:UltimateQuadTree.QuadTree`1"></see>. </summary>
        /// <param name="objects">The objects to insert.</param>
        public void InsertRange(IEnumerable<T> objects)
        {
            if (objects == null) throw new ArgumentNullException(nameof(objects));
            foreach (var obj in objects) Insert(obj);
        }

        /// <summary> Removes the specified element from the <see cref="T:UltimateQuadTree.QuadTree`1"></see>. </summary>
        /// <param name="obj">The object to remove.</param>
        /// <returns>true if the element is successfully found and removed; false if failure.</returns>
        public bool Remove(T obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            if (!_rootSector.Remove(obj)) return false;

            ObjectCount--;

            if (ObjectCount >= MaxObjects) return true;

            if (_rootSector.TryCollapse(out var collapsed))
                _rootSector = collapsed;

            return true;
        }

        /// <summary> Removes a range of elements from the <see cref="T:UltimateQuadTree.QuadTree`1"></see>. </summary>
        /// <param name="objects">The objects to remove.</param> 
        public void RemoveRange(IEnumerable<T> objects)
        {
            if (objects == null) throw new ArgumentNullException(nameof(objects));
            foreach (var obj in objects) Remove(obj);
        }

        /// <summary> Returns the elements nearest to the object. </summary>
        /// <param name="obj">The object for search of the nearest elements.</param> 
        /// <returns> the enumeration of elements nearest to the object. </returns>
        public IEnumerable<T> GetNearestObjects(T obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            return _rootSector.GetNearestObjects(obj).Distinct();
        }

        /// <summary> Returns all elements from the <see cref="T:UltimateQuadTree.QuadTree`1"></see>. </summary>
        /// <returns> the enumeration of all elements of the <see cref="T:UltimateQuadTree.QuadTree`1"></see>. </returns>
        public IEnumerable<T> GetObjects()
        {
            return _rootSector.GetObjects().Distinct();
        }

        /// <summary> Returns all rectangles from the <see cref="T:UltimateQuadTree.QuadTree`1"></see>. </summary>
        /// <returns> the enumeration of all rectangles of the <see cref="T:UltimateQuadTree.QuadTree`1"></see>. </returns>
        public IEnumerable<QuadTreeRect> GetGrid() => _rootSector.GetRects();
    }
}
