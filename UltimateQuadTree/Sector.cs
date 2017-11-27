using System.Collections.Generic;

namespace UltimateQuadTree
{
    internal abstract class Sector<T>
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
        public abstract Sector<T> Quarter();
        public abstract bool Remove(T obj);
        public abstract bool TryCollapse(out Sector<T> sector);
        public abstract IEnumerable<T> GetNearestObjects(T obj);
        public abstract IEnumerable<T> GetObjects();
        public abstract IEnumerable<QuadTreeRect> GetRects();
    }
}