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

        private Sector _rootSector;
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
            _rootSector = new LeafSector(0, MainRect, objectBounds, maxObjects, maxLevel);
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
            _rootSector = new LeafSector(0, MainRect, _objectBounds, MaxObjects, MaxLevel);
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

            if (_rootSector.TryCollapse(out Sector collapsed))
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

        private abstract class Sector
        {
            protected readonly int MaxObjects;
            protected readonly int MaxLevel;

            protected readonly int Level;
            protected readonly QuadTreeRect Rect;

            protected readonly IQuadTreeObjectBounds<T> ObjectBounds;

            protected Sector(int level, QuadTreeRect rect, IQuadTreeObjectBounds<T> objectBounds, int maxObjects, int maxLevel)
            {
                Level = level;
                Rect = rect;
                ObjectBounds = objectBounds;
                MaxObjects = maxObjects;
                MaxLevel = maxLevel;
            }

            public abstract void Clear();
            public abstract bool TryInsert(T obj);
            public abstract Sector Quarter();
            public abstract bool Remove(T obj);
            public abstract bool TryCollapse(out Sector sector);
            public abstract IEnumerable<T> GetNearestObjects(T obj);
            public abstract IEnumerable<T> GetObjects();
            public abstract IEnumerable<QuadTreeRect> GetRects();
        }

        private class LeafSector : Sector
        {
            private readonly HashSet<T> _objects = new HashSet<T>();

            public LeafSector(int level, QuadTreeRect rect, IQuadTreeObjectBounds<T> objectBounds, int maxObjects, int maxLevel)
                : base(level, rect, objectBounds, maxObjects, maxLevel)
            {
            }

            public override void Clear() => _objects.Clear();

            public override bool TryInsert(T obj)
            {
                if (_objects.Count >= MaxObjects && Level < MaxLevel) return false;
                _objects.Add(obj);
                return true;
            }

            public override Sector Quarter()
            {
                var node = new NodeSector(Level, Rect, ObjectBounds, MaxObjects, MaxLevel);
                foreach (var o in _objects) node.TryInsert(o);
                return node;
            }

            public override bool Remove(T obj)
            {
                return _objects.Remove(obj);
            }

            public override bool TryCollapse(out Sector sector)
            {
                sector = this;
                return false;
            }

            public override IEnumerable<T> GetNearestObjects(T obj)
            {
                return _objects;
            }

            public override IEnumerable<T> GetObjects()
            {
                return _objects;
            }

            public override IEnumerable<QuadTreeRect> GetRects()
            {
                yield return Rect;
            }
        }

        private class NodeSector : Sector
        {
            private int _objectsCount;

            private Sector _leftTopSector;
            private Sector _leftBottomSector;
            private Sector _rightTopSector;
            private Sector _rightBottomSector;

            public NodeSector(int level, QuadTreeRect rect, IQuadTreeObjectBounds<T> objectBounds, int maxObjects, int maxLevel)
                : base(level, rect, objectBounds, maxObjects, maxLevel)
            {
                CreateLeaves();
            }

            public override void Clear()
            {
                _leftTopSector.Clear();
                _leftTopSector = null;

                _leftBottomSector.Clear();
                _leftBottomSector = null;

                _rightTopSector.Clear();
                _rightTopSector = null;

                _rightBottomSector.Clear();
                _rightBottomSector = null;

                _objectsCount = 0;
            }

            public override bool TryInsert(T obj)
            {
                var result = false;

                if (IsLeft(obj))
                {
                    if (IsTop(obj)) result |= Insert(ref _leftTopSector, obj);
                    if (IsBottom(obj)) result |= Insert(ref _leftBottomSector, obj);
                }

                if (IsRight(obj))
                {
                    if (IsTop(obj)) result |= Insert(ref _rightTopSector, obj);
                    if (IsBottom(obj)) result |= Insert(ref _rightBottomSector, obj);
                }

                if (result) _objectsCount++;

                return result;
            }

            private static bool Insert(ref Sector sector, T obj)
            {
                if (sector.TryInsert(obj)) return true;
                sector = sector.Quarter();
                return sector.TryInsert(obj);
            }

            public override Sector Quarter() => this;

            public override bool Remove(T obj)
            {
                var result = false;

                if (IsLeft(obj))
                {
                    if (IsTop(obj)) result |= Remove(ref _leftTopSector, obj);
                    if (IsBottom(obj)) result |= Remove(ref _leftBottomSector, obj);
                }

                if (IsRight(obj))
                {
                    if (IsTop(obj)) result |= Remove(ref _rightTopSector, obj);
                    if (IsBottom(obj)) result |= Remove(ref _rightBottomSector, obj);
                }

                if (result) _objectsCount--;

                return result;
            }

            private static bool Remove(ref Sector sector, T obj)
            {
                var result = sector.Remove(obj);
                if (!result) return false;

                if (sector.TryCollapse(out Sector collapsed))
                    sector = collapsed;

                return true;
            }

            public override bool TryCollapse(out Sector sector)
            {
                if (_objectsCount >= MaxObjects)
                {
                    sector = this;
                    return false;
                }

                sector = new LeafSector(Level, Rect, ObjectBounds, MaxObjects, MaxLevel);
                foreach (var o in GetObjects().Distinct()) sector.TryInsert(o);

                Clear();

                return true;
            }

            public override IEnumerable<T> GetNearestObjects(T obj)
            {
                return GetSector(obj).SelectMany(s => s.GetNearestObjects(obj));
            }

            public override IEnumerable<T> GetObjects()
            {
                foreach (var obj in _leftTopSector.GetObjects()) yield return obj;
                foreach (var obj in _leftBottomSector.GetObjects()) yield return obj;
                foreach (var obj in _rightTopSector.GetObjects()) yield return obj;
                foreach (var obj in _rightBottomSector.GetObjects()) yield return obj;
            }

            public override IEnumerable<QuadTreeRect> GetRects()
            {
                yield return Rect;

                foreach (var rect in _leftTopSector.GetRects()) yield return rect;
                foreach (var rect in _leftBottomSector.GetRects()) yield return rect;
                foreach (var rect in _rightTopSector.GetRects()) yield return rect;
                foreach (var rect in _rightBottomSector.GetRects()) yield return rect;
            }

            private void CreateLeaves()
            {
                var nextLevel = Level + 1;

                _leftTopSector = new LeafSector(nextLevel, Rect.LeftTopQuarter, ObjectBounds, MaxObjects, MaxLevel);
                _leftBottomSector = new LeafSector(nextLevel, Rect.LeftBottomQuarter, ObjectBounds, MaxObjects, MaxLevel);
                _rightTopSector = new LeafSector(nextLevel, Rect.RightTopQuarter, ObjectBounds, MaxObjects, MaxLevel);
                _rightBottomSector = new LeafSector(nextLevel, Rect.RightBottomQuarter, ObjectBounds, MaxObjects, MaxLevel);
            }

            private IEnumerable<Sector> GetSector(T obj)
            {
                if (IsLeft(obj))
                {
                    if (IsTop(obj)) yield return _leftTopSector;
                    if (IsBottom(obj)) yield return _leftBottomSector;
                }

                if (IsRight(obj))
                {
                    if (IsTop(obj)) yield return _rightTopSector;
                    if (IsBottom(obj)) yield return _rightBottomSector;
                }
            }

            private bool IsTop(T obj) => ObjectBounds.GetTop(obj) <= Rect.MidY;
            private bool IsBottom(T obj) => ObjectBounds.GetBottom(obj) >= Rect.MidY;
            private bool IsLeft(T obj) => ObjectBounds.GetLeft(obj) <= Rect.MidX;
            private bool IsRight(T obj) => ObjectBounds.GetRight(obj) >= Rect.MidX;
        }
    }
}
