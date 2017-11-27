using System.Collections.Generic;
using System.Linq;

namespace UltimateQuadTree
{
    internal class NodeSector<T> : Sector<T>
    {
        private int _objectsCount;

        private Sector<T> _leftTopSector;
        private Sector<T> _leftBottomSector;
        private Sector<T> _rightTopSector;
        private Sector<T> _rightBottomSector;

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

        private static bool Insert(ref Sector<T> sector, T obj)
        {
            if (sector.TryInsert(obj)) return true;
            sector = sector.Quarter();
            return sector.TryInsert(obj);
        }

        public override Sector<T> Quarter() => this;

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

        private static bool Remove(ref Sector<T> sector, T obj)
        {
            var result = sector.Remove(obj);
            if (!result) return false;

            if (sector.TryCollapse(out Sector<T> collapsed))
                sector = collapsed;

            return true;
        }

        public override bool TryCollapse(out Sector<T> sector)
        {
            if (_objectsCount >= MaxObjects)
            {
                sector = this;
                return false;
            }

            sector = new LeafSector<T>(Level, Rect, ObjectBounds, MaxObjects, MaxLevel);
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

            _leftTopSector = new LeafSector<T>(nextLevel, Rect.LeftTopQuarter, ObjectBounds, MaxObjects, MaxLevel);
            _leftBottomSector = new LeafSector<T>(nextLevel, Rect.LeftBottomQuarter, ObjectBounds, MaxObjects, MaxLevel);
            _rightTopSector = new LeafSector<T>(nextLevel, Rect.RightTopQuarter, ObjectBounds, MaxObjects, MaxLevel);
            _rightBottomSector = new LeafSector<T>(nextLevel, Rect.RightBottomQuarter, ObjectBounds, MaxObjects, MaxLevel);
        }

        private IEnumerable<Sector<T>> GetSector(T obj)
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