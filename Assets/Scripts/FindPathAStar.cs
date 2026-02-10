using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PathMarker
{
    public MapLocation location;
    public float G, H, F;
    public GameObject marker;
    public PathMarker parent;

    public PathMarker(MapLocation l, float g, float h, float f, GameObject m, PathMarker p)
    {
        location = l; G = g; H = h; F = f; marker = m; parent = p;
    }

    public override bool Equals(object obj) => obj is PathMarker pm && location.Equals(pm.location);
    public override int GetHashCode() => location.GetHashCode();
}

public class FindPathAStar : MonoBehaviour
{
    public Maze maze;
    public Material closedMaterial;
    public GameObject startPrefab;
    public GameObject endPrefab;
    public GameObject pathP;

    private List<PathMarker> open = new List<PathMarker>();
    private List<PathMarker> closed = new List<PathMarker>();
    private List<GameObject> activeMarkers = new List<GameObject>();
    private PathMarker goalNode;
    private PathMarker lastPos;
    private GameObject currentPlayer;
    private GameObject currentGoal;
    private bool done = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            StopAllCoroutines();
            ResetScene();
            StartCoroutine(BeginSearchRoutine());
        }
    }

    void ResetScene()
    {
        done = false;
        foreach (GameObject m in activeMarkers) Destroy(m);
        activeMarkers.Clear();
        if (currentPlayer != null) Destroy(currentPlayer);
        if (currentGoal != null) Destroy(currentGoal);
        open.Clear();
        closed.Clear();
    }

    IEnumerator BeginSearchRoutine()
    {
        // 1. Setup Start en Goal
        Vector3 sPos = new Vector3(maze.scale, 0.5f, maze.scale);
        currentPlayer = Instantiate(startPrefab, sPos, Quaternion.identity);

        int ex = Random.Range(5, maze.width - 1);
        int ez = Random.Range(5, maze.depth - 1);
        Vector3 ePos = new Vector3(ex * maze.scale, 0.5f, ez * maze.scale);
        currentGoal = Instantiate(endPrefab, ePos, Quaternion.identity);

        PathMarker startNode = new PathMarker(new MapLocation(1, 1), 0, 0, 0, null, null);
        goalNode = new PathMarker(new MapLocation(ex, ez), 0, 0, 0, null, null);

        open.Add(startNode);
        lastPos = startNode;

        // 2. Stap voor stap zoeken (Visueel)
        while (open.Count > 0 && !done)
        {
            SearchStep();
            yield return new WaitForSeconds(0.05f); // Snelheid van het algoritme-bezoek
        }

        // 3. Als we er zijn, loop het pad af
        if (done)
        {
            List<PathMarker> finalPath = ReconstructPath(lastPos);
            yield return StartCoroutine(MovePlayerRoutine(finalPath));
        }
    }

    void SearchStep()
    {
        open = open.OrderBy(p => p.F).ToList();
        PathMarker current = open[0];
        open.RemoveAt(0);
        closed.Add(current);

        if (current.Equals(goalNode))
        {
            done = true;
            lastPos = current;
            return;
        }

        foreach (MapLocation dir in maze.directions)
        {
            MapLocation neighbour = dir + current.location;
            if (neighbour.x < 1 || neighbour.x >= maze.width || neighbour.z < 1 || neighbour.z >= maze.depth) continue;
            if (maze.map[neighbour.x, neighbour.z] == 1) continue;
            if (closed.Any(p => p.location.Equals(neighbour))) continue;

            float g = current.G + 1;
            float h = Vector2.Distance(neighbour.ToVector(), goalNode.location.ToVector());
            float f = g + h;

            if (!UpdateMarker(neighbour, g, h, f, current))
            {
                GameObject mb = Instantiate(pathP, new Vector3(neighbour.x * maze.scale, 0.1f, neighbour.z * maze.scale), Quaternion.identity);
                activeMarkers.Add(mb);
                open.Add(new PathMarker(neighbour, g, h, f, mb, current));
            }
        }
        lastPos = current;
    }

    bool UpdateMarker(MapLocation pos, float g, float h, float f, PathMarker prt)
    {
        foreach (PathMarker p in open)
        {
            if (p.location.Equals(pos))
            {
                if (g < p.G) { p.G = g; p.F = f; p.parent = prt; }
                return true;
            }
        }
        return false;
    }

    List<PathMarker> ReconstructPath(PathMarker current)
    {
        List<PathMarker> path = new List<PathMarker>();
        while (current != null) { path.Add(current); current = current.parent; }
        path.Reverse();
        return path;
    }

    IEnumerator MovePlayerRoutine(List<PathMarker> path)
    {
        foreach (PathMarker p in path)
        {
            if (currentPlayer == null) yield break;

            // Verplaats de player
            currentPlayer.transform.position = new Vector3(p.location.x * maze.scale, 0.5f, p.location.z * maze.scale);

            // Kleur de marker alleen als hij een Renderer heeft
            if (p.marker != null)
            {
                Renderer rend = p.marker.GetComponent<Renderer>();
                if (rend == null) rend = p.marker.GetComponentInChildren<Renderer>(); // Check ook kinderen

                if (rend != null && closedMaterial != null)
                {
                    rend.material = closedMaterial;
                }
            }

            yield return new WaitForSeconds(0.5f); // De 0.5 seconde vertraging per stap
        }
    }
}