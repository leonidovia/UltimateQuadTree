// Copyright 2017 Igor' Leonidov
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;

namespace UltimateQuadTree
{
    public class QuadTree<T>
    {
        public int MaxObjects { get; }
        public int MaxLevel { get; }
        public QuadTreeRect MainRect { get; }
        private Sector _rootSector;
        private readonly IQuadTreeObjectBounds<T> _objectBounds;

        public QuadTree(double x, double y, double width, double height, IQuadTreeObjectBounds<T> objectBounds, int maxObjects = 10, int maxLevel = 5)
        {
            MaxObjects = maxObjects;
            MaxLevel = maxLevel;
            MainRect = new QuadTreeRect(x, y, width, height);
            _objectBounds = objectBounds;
            _rootSector = new LeafSector(0, MainRect, objectBounds, maxObjects, maxLevel);
        }

        public QuadTree(double width, double height, IQuadTreeObjectBounds<T> objectBounds, int maxObjects = 10, int maxLevel = 5)
            : this(0, 0, width, height, objectBounds, maxObjects, maxLevel)
        {
        }

        public void Clear()
        {
            _rootSector.Clear();
            _rootSector = new LeafSector(0, MainRect, _objectBounds, MaxObjects, MaxLevel);
        }

        public void Insert(T obj)
        {
            if (!IsObjectInMainRect(obj)) return;
            if (_rootSector.TryInsert(obj)) return;
            _rootSector = _rootSector.Quarter();
            _rootSector.TryInsert(obj);
        }

        private bool IsObjectInMainRect(T obj)
        {
            if (_objectBounds.GetTop(obj) > MainRect.Bottom) return false;
            if (_objectBounds.GetBottom(obj) < MainRect.Top) return false;
            if (_objectBounds.GetLeft(obj) > MainRect.Right) return false;
            if (_objectBounds.GetRight(obj) < MainRect.Left) return false;

            return true;
        }

        public void Insert(IEnumerable<T> objects)
        {
            foreach (var obj in objects) Insert(obj);
        }

        public IEnumerable<T> GetNearestObjects(T obj)
        {
            var result = new HashSet<T>();
            _rootSector.GetNearestObjects(result, obj);
            return result;
        }

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
            public abstract void GetNearestObjects(ISet<T> result, T obj);
            public abstract IEnumerable<QuadTreeRect> GetRects();
        }

        private class LeafSector : Sector
        {
            private readonly List<T> _objects = new List<T>();

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

            public override void GetNearestObjects(ISet<T> result, T obj)
            {
                foreach (var o in _objects) result.Add(o);
            }

            public override IEnumerable<QuadTreeRect> GetRects()
            {
                yield return Rect;
            }
        }

        private class NodeSector : Sector
        {
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
            }

            public override bool TryInsert(T obj)
            {
                if (IsLeft(obj))
                {
                    if (IsTop(obj)) Insert(ref _leftTopSector, obj);
                    if (IsBottom(obj)) Insert(ref _leftBottomSector, obj);
                }

                if (IsRight(obj))
                {
                    if (IsTop(obj)) Insert(ref _rightTopSector, obj);
                    if (IsBottom(obj)) Insert(ref _rightBottomSector, obj);
                }

                return true;
            }

            private static void Insert(ref Sector sector, T obj)
            {
                if (sector.TryInsert(obj)) return;
                sector = sector.Quarter();
                sector.TryInsert(obj);
            }

            public override Sector Quarter() => this;

            public override void GetNearestObjects(ISet<T> result, T obj)
            {
                foreach (var sector in GetSector(obj))
                    sector.GetNearestObjects(result, obj);
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
