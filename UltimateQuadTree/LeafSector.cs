using System.Collections.Generic;

namespace UltimateQuadTree
{
    internal class LeafSector<T> : Sector<T>
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

        public override Sector<T> Quarter()
        {
            var node = new NodeSector<T>(Level, Rect, ObjectBounds, MaxObjects, MaxLevel);
            foreach (var o in _objects) node.TryInsert(o);
            return node;
        }

        public override bool Remove(T obj)
        {
            return _objects.Remove(obj);
        }

        public override bool TryCollapse(out Sector<T> sector)
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
}