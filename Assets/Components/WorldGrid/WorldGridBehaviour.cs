using System;
using System.Collections;
using System.Collections.Generic;
using StationXYZ.Core;
using UnityEngine;
using UnityEngine.Pool;

public class WorldGridBehaviour : MonoBehaviour
{

    [Header("Config")]
    public Vector2Int WorldSize;
    public Grid Grid;
    public WorldCellBehaviour Prefab;
    public GameBehaviour Game;
    public List<Transform> OtherPointsOfInterest;

    [Header("Internal Data")] 
    public WorldCellBehaviour lastMouseEntered;
    private Vector3 _mousePos;
    private ObjectPool<WorldCellBehaviour> Cells;
    private Dictionary<Vector3Int, WorldCellBehaviour> CellToObject = new Dictionary<Vector3Int, WorldCellBehaviour>();
    private Queue<Vector3Int> CreatedCells = new Queue<Vector3Int>();

    [Header("Computed Data")] public Vector2Int position;

    // Start is called before the first frame update
    void Start()
    {
        Cells = new ObjectPool<WorldCellBehaviour>(() =>
        {
            var cell = Instantiate(Prefab, transform);
            return cell;
        }, cell => cell.gameObject.SetActive(true), cell => cell.gameObject.SetActive(false), cell => Destroy(cell.gameObject));
        // instantiate a big old grid.
        // var halfSize = new Vector2Int((int)(WorldSize.x * .5f), (int)(WorldSize.y * .5f));
        // for (var x = 0; x < WorldSize.x; x++)
        // {
        //     for (var y = 0; y < WorldSize.y; y++)
        //     {
        //         var cell = Instantiate(Prefab, transform);
        //         var pos = new Vector3Int(x - halfSize.x, 0, y - halfSize.y);
        //         cell.transform.localPosition = Grid.CellToLocal(pos);
        //         cell.transform.localScale = new Vector3(cell.transform.localScale.x * Grid.cellSize.x, 1, cell.transform.localScale.z * Grid.cellSize.z);
        //         cell.World = this;
        //         cell.Cell = pos;
        //         cell.OnCreated();
        //     }
        // }
        //
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_mousePos, .2f);
    }

    // Update is called once per frame
    void Update()
    {
        _mousePos = GetPlayerPlaneMousePos();


        var currPos = Grid.WorldToCell(_mousePos + Grid.cellSize*.5f);
        position = new Vector2Int(currPos.x, currPos.z);
        var size = 4;
        for (var x = -size; x <= size; x++)
        {
            for (var y = -size; y <= size; y++)
            {
                EnsureCell(currPos + new Vector3Int(x, 0, y));
                foreach (var poi in OtherPointsOfInterest)
                {
                    var poiCell = Grid.WorldToCell(poi.position);
                    poiCell.y = 0;

                    EnsureCell(poiCell + new Vector3Int(x, 0, y));
                }
            }
        }
        
        
        
    }

    public WorldCellBehaviour EnsureCell(Vector3Int currPos)
    {
        if (!CellToObject.TryGetValue(currPos, out var cell))
        {
            // create the cell.
            Cells.Get(out cell);
            cell.transform.localPosition = Grid.CellToLocal(currPos);
            cell.transform.localScale = new Vector3(.1f * Grid.cellSize.x, 1, .1f * Grid.cellSize.z);
            cell.Cell = currPos;
            cell.World = this;
            cell.OnCreated();
            CellToObject.Add(currPos, cell);
        }
        else
        {
            cell.OnPersist();
        }

        return cell;
    }

    public void MouseEntered(WorldCellBehaviour worldCellBehaviour)
    {
        lastMouseEntered = worldCellBehaviour;
        Game.HoverCell = worldCellBehaviour.Cell;
    }
    
    public Vector3 GetPlayerPlaneMousePos()
    {
        Plane plane = new Plane(Vector3.up, transform.position);
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float dist;
        if (plane.Raycast(ray, out dist))
        {
            return ray.GetPoint(dist);
        }
        return Vector3.zero;
    }

    public void OnCellExpire(WorldCellBehaviour worldCellBehaviour)
    {
        Cells.Release(worldCellBehaviour);
        
        CellToObject.Remove(worldCellBehaviour.Cell);
    }
}
