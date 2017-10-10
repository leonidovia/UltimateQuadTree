# UltimateQuadTree
The .NET implementation of the QuadTree algorithm for generic types.

Wikipedia - https://en.wikipedia.org/wiki/Quadtree

Example for polygons defined by Point[]:

// implement IQuadTreeObjectBounds<T> interface for your object type 
public class MyPolygonBounds : IQuadTreeObjectBounds<Point[]>
{
    public double GetLeft(Point[] obj)
    {
        return obj.Min(p => p.X);
    }
    
    public double GetRight(Point[] obj)
    {
        return obj.Max(p => p.X);
    }
    
    public double GetTop(Point[] obj)
    {
        return obj.Min(p => p.Y);
    }
    
    public double GetBottom(Point[] obj)
    {
        return obj.Max(p => p.Y);
    }
}

// create a QuadTree and fill it with objects
var quadTree = new QuadTree<Point[]>(1920, 1080, new MyPolygonBounds());
quadTree.Insert(myPolygons); // "myPolygons" are an array of all your objects 

// find the nearest objects for your object "myPolygon"
var nearestObjects = quadTree.GetNearestObjects(myPolygon);

// find the intersecting objects among the nearest 
// bool IsIntersect(Point[] obj1, Point[] obj2) is your function for checking intersections
var intersectingObjects = nearestObjects.Where(nearest => IsIntersect(nearest, myPolygon);
