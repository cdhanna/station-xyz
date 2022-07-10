using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldCellBehaviour : MonoBehaviour
{


    [Header("Config")]
    public MeshRenderer Renderer;
    public float Ttl = 2;
    public Color Color;

    
    [Header("internal data")]
    public WorldGridBehaviour World;
    public Vector3Int Cell;
    public MaterialPropertyBlock MaterialPropertyBlock;
    public float Presence;
    public float PresenceVel;
    private float _createdAt;

    private void Start()
    {
        MaterialPropertyBlock = new MaterialPropertyBlock();
        var transparent = new Color(Color.r, Color.g, Color.b, 0);
        MaterialPropertyBlock.SetColor("_Color", transparent);
        Renderer.SetPropertyBlock(MaterialPropertyBlock);
    }

    public void OnCreated()
    {
        _createdAt = Time.realtimeSinceStartup;
    }
    
    public void OnPersist()
    {
        _createdAt = Time.realtimeSinceStartup;
    }

    private void OnMouseEnter()
    {
        World.MouseEntered(this);
    }

    private void Update()
    {
        
        
        if (!World) return;

        var lifeTime = Time.realtimeSinceStartup - _createdAt;
        var timeLeft = Ttl - lifeTime;
        if (timeLeft < 0)
        {
            World.OnCellExpire(this);
        }

        if (!World.lastMouseEntered)
        {
            
            
            return;
        }
        
        var dist = (World.lastMouseEntered.Cell - Cell).sqrMagnitude;
        foreach (var poi in World.OtherPointsOfInterest)
        {
            var poiCell = World.Grid.WorldToCell(poi.position);
            poiCell.y = 0;
            dist = Mathf.Min(dist, (poiCell - Cell).sqrMagnitude);
        }
        
        var maxDist = 10f + 2.4f * Mathf.Abs(Mathf.Sin(Time.realtimeSinceStartup * Mathf.PI * .2f));
        var alpha = 1 - (dist / maxDist);
        alpha = Math.Clamp(alpha, 0, 1);
        // alpha = Mathf.SmoothStep(.01f, 1f, alpha);
        Presence = Mathf.SmoothDamp(Presence, alpha, ref PresenceVel, .1f);

        var color = new Color(Color.r, Color.g, Color.b, Presence);

        MaterialPropertyBlock.SetColor("_Color", color);
        Renderer.SetPropertyBlock(MaterialPropertyBlock);
    }

}
