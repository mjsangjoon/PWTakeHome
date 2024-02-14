using UnityEngine;

public class NavGridPathNode
{
    /// <summary>
    /// World position of the node
    /// </summary>
    public Vector3 Position;

    //A* information
    public NavGridPathNode Parent;
    public float fVal, gVal, hVal;

    public NavGridPathNode(Vector3 position, NavGridPathNode parent, float f, float g, float h){
        this.Position = position;
        this.Parent = parent;
        this.fVal = f;
        this.gVal = g;
        this.hVal = h;
    }

    public NavGridPathNode(Vector3 position){
        this.Position = position;
        this.Parent = null;
        this.fVal = 0.0f;
        this.gVal = 0.0f;
        this.hVal = 0.0f;
    }

    public NavGridPathNode(){
        this.Position = new Vector3(0,0,0);
        this.Parent = null;
        this.fVal = 0.0f;
        this.gVal = 0.0f;
        this.hVal = 0.0f;
    }

    public override bool Equals(System.Object obj){
        if (obj == null)
            return false;
        
        NavGridPathNode n = obj as NavGridPathNode;
        if ((System.Object)n == null)
            return false;

        return Position == n.Position;
    }

    public bool Equals(NavGridPathNode n){
        if ((object)n == null)
            return false;

        return Position == n.Position;
    }

    public override int GetHashCode(){
        return Position.GetHashCode();
    }

    public static bool operator ==(NavGridPathNode a, NavGridPathNode b){
        return a.Equals(b);
    }

    public static bool operator !=(NavGridPathNode a, NavGridPathNode b){
        return !a.Equals(b);
    }

    public override string ToString(){
        return Position.ToString();
    }
}