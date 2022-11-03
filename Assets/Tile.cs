using BoardGame;
using UnityEngine;
using UnityEngine.UI;

public class Tile : TileParent {
    
    // 1. TileParent extends MonoBehavior, so you can add member variables here
    // to store data.
    public Material regularMaterial;
    public Material blockedMaterial;
    public Material walkable;
    public int id;
    public float movementCost;
    private Transform portalDest;
    
    // This function is called when something has changed on the board. All 
    // tiles have been created before it is called.
    public override void OnSetup(Board board) {
        // 2. Each tile has a unique 'coordinate'
        Vector2Int key = Coordinate;
        
        // 3. Tiles can have different modifiers
        if (IsBlocked) {
            
        }
        
        if (IsObstacle(out int penalty))
        {
            transform.GetChild(1).gameObject.SetActive(true);
            transform.GetChild(1).GetChild(0).GetComponent<Text>().text = penalty.ToString();
        }
        else
        {
            transform.GetChild(1).gameObject.SetActive(false);
        }
        
        if (IsCheckPoint) {
            transform.GetChild(3).gameObject.SetActive(true);
        }
        else
        {
            transform.GetChild(3).gameObject.SetActive(false);
        }
        
        if (IsStartPoint) {
            transform.GetChild(2).gameObject.SetActive(true);
        }
        else
        {
            transform.GetChild(2).gameObject.SetActive(false);
        }
        
        if (IsPortal(out Vector2Int destination))
        {
            if (board.TryGetTile(destination, out Tile other))
            {
                portalDest = other.transform;
            }

            transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            portalDest = null;
            transform.GetChild(0).gameObject.SetActive(false);
        }
        
        
        // 4. Other tiles can be accessed through the 'board' instance
        if (board.TryGetTile(new Vector2Int(2, 1), out Tile otherTile)) {
            
        }
    }

    // This function is called during the regular 'Update' step, but also gives
    // you access to the 'board' instance.
    public override void OnUpdate(Board board) {
        
        // 5. Change the material color if this tile is blocked
        if (TryGetComponent<MeshRenderer>(out var meshRenderer)) {
            if (IsBlocked) {
                meshRenderer.material = blockedMaterial;
            } else {
                meshRenderer.material = regularMaterial;
            }
        }

        if (portalDest != null)
        {
            
        }
        
        
    }
}