using UnityEngine;
using System.Collections.Generic;
namespace Secrets.Gameplay
{
    public class QuantumPlaygroundGenerator : MonoBehaviour
    {
        [Header("Generation Settings")]
        [SerializeField] private int defaultSeed = 42;
        [SerializeField] private Vector2 playgroundSize = new Vector2(100f, 100f);
        [SerializeField] private float minRoomSize = 10f;
        [SerializeField] private float maxRoomSize = 25f;
        [SerializeField] private int minRooms = 5;
        [SerializeField] private int maxRooms = 12;

        [Header("Room Objects")]
        [SerializeField] private GameObject[] wallPrefabs;
        [SerializeField] private GameObject[] energyBarriers; // Walls that react to quantum state
        [SerializeField] private GameObject[] reflectiveSurfaces; // For light burst bouncing

        [Header("Visual Settings")]
        [SerializeField] private Material normalRoomMaterial;
        [SerializeField] private Material quantumRoomMaterial;
        [SerializeField] private GameObject backgroundPrefab;
        [SerializeField] private ParticleSystem quantumFieldParticles;

        private System.Random random;
        private Dictionary<int, List<Room>> universeCache = new Dictionary<int, List<Room>>();

        [System.Serializable]
        public class Room
        {
            public Vector2 position;
            public Vector2 size;
            public List<GameObject> walls = new List<GameObject>();
            public List<GameObject> objects = new List<GameObject>();
            public RoomType type;

            public enum RoomType
            {
                Normal,
                Quantum,
                Mixed
            }
        }

        public void GenerateUniverse(int seed)
        {
            if (universeCache.ContainsKey(seed))
            {
                LoadCachedUniverse(seed);
                return;
            }

            random = new System.Random(seed);
            ClearCurrentUniverse();

            List<Room> rooms = GenerateRooms();
            ConnectRooms(rooms);
            PopulateRooms(rooms);

            universeCache[seed] = rooms;
        }

        private void ClearCurrentUniverse()
        {
            // Find and destroy all generated objects
            GameObject[] generatedObjects = GameObject.FindGameObjectsWithTag("Generated");
            foreach (GameObject obj in generatedObjects)
            {
                Destroy(obj);
            }
        }

        private List<Room> GenerateRooms()
        {
            List<Room> rooms = new List<Room>();
            int roomCount = random.Next(minRooms, maxRooms + 1);

            for (int i = 0; i < roomCount; i++)
            {
                Room room = new Room
                {
                    size = new Vector2(
                        (float)random.NextDouble() * (maxRoomSize - minRoomSize) + minRoomSize,
                        (float)random.NextDouble() * (maxRoomSize - minRoomSize) + minRoomSize
                    ),
                    type = (Room.RoomType)random.Next(0, System.Enum.GetValues(typeof(Room.RoomType)).Length)
                };

                // Try to place room without overlap
                bool placed = false;
                int attempts = 0;
                while (!placed && attempts < 50)
                {
                    Vector2 testPos = new Vector2(
                        (float)random.NextDouble() * (playgroundSize.x - room.size.x) - playgroundSize.x / 2,
                        (float)random.NextDouble() * (playgroundSize.y - room.size.y) - playgroundSize.y / 2
                    );

                    if (!RoomOverlaps(testPos, room.size, rooms))
                    {
                        room.position = testPos;
                        placed = true;
                    }
                    attempts++;
                }

                if (placed)
                    rooms.Add(room);
            }

            return rooms;
        }

        private bool RoomOverlaps(Vector2 position, Vector2 size, List<Room> rooms)
        {
            Rect newRoom = new Rect(position - size / 2, size);
            float padding = 2f; // Space between rooms

            foreach (Room existingRoom in rooms)
            {
                Rect existing = new Rect(existingRoom.position - existingRoom.size / 2, existingRoom.size);
                existing.x -= padding;
                existing.y -= padding;
                existing.width += padding * 2;
                existing.height += padding * 2;

                if (newRoom.Overlaps(existing))
                    return true;
            }

            return false;
        }

        private void ConnectRooms(List<Room> rooms)
        {
            for (int i = 0; i < rooms.Count - 1; i++)
            {
                Room currentRoom = rooms[i];
                Room nextRoom = rooms[i + 1];

                // Create corridor between rooms
                Vector2 start = currentRoom.position;
                Vector2 end = nextRoom.position;

                CreateCorridor(start, end);
            }
        }

        private void CreateCorridor(Vector2 start, Vector2 end)
        {
            // Create corridor walls
            float corridorWidth = 3f;
            Vector2 direction = (end - start).normalized;
            Vector2 perpendicular = new Vector2(-direction.y, direction.x);

            // Create walls along the corridor
            GameObject wallParent = new GameObject("Corridor");
            wallParent.tag = "Generated";

            Vector2[] wallPoints = new Vector2[]
            {
            start + perpendicular * corridorWidth/2,
            end + perpendicular * corridorWidth/2,
            end - perpendicular * corridorWidth/2,
            start - perpendicular * corridorWidth/2
            };

            for (int i = 0; i < wallPoints.Length; i++)
            {
                Vector2 current = wallPoints[i];
                Vector2 next = wallPoints[(i + 1) % wallPoints.Length];

                CreateWall(current, next, wallParent.transform);
            }
        }

        private void CreateWall(Vector2 start, Vector2 end, Transform parent)
        {
            Vector2 direction = end - start;
            float length = direction.magnitude;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            GameObject wall = Instantiate(wallPrefabs[random.Next(wallPrefabs.Length)],
                (Vector3)(start + direction / 2),
                Quaternion.Euler(0, 0, angle),
                parent);

            wall.transform.localScale = new Vector3(length, 1, 1);
        }

        private void PopulateRooms(List<Room> rooms)
        {
            foreach (Room room in rooms)
            {
                // Create room boundaries
                CreateRoomBoundaries(room);

                // Add room-specific objects based on type
                switch (room.type)
                {
                    case Room.RoomType.Normal:
                        PopulateNormalRoom(room);
                        break;

                    case Room.RoomType.Quantum:
                        PopulateQuantumRoom(room);
                        break;

                    case Room.RoomType.Mixed:
                        PopulateMixedRoom(room);
                        break;
                }

                // Add background effects
                CreateBackground(room);
            }
        }

        private void CreateRoomBoundaries(Room room)
        {
            // Create walls around room perimeter
            Vector2[] corners = new Vector2[]
            {
            room.position + new Vector2(-room.size.x/2, -room.size.y/2),
            room.position + new Vector2(room.size.x/2, -room.size.y/2),
            room.position + new Vector2(room.size.x/2, room.size.y/2),
            room.position + new Vector2(-room.size.x/2, room.size.y/2)
            };

            GameObject wallParent = new GameObject($"Room_Walls");
            wallParent.tag = "Generated";

            for (int i = 0; i < corners.Length; i++)
            {
                Vector2 start = corners[i];
                Vector2 end = corners[(i + 1) % corners.Length];

                CreateWall(start, end, wallParent.transform);
            }

            room.walls.Add(wallParent);
        }

        private void PopulateNormalRoom(Room room)
        {
            // Add reflective surfaces for light burst interactions
            int surfaceCount = random.Next(2, 5);
            for (int i = 0; i < surfaceCount; i++)
            {
                Vector2 position = GetRandomPositionInRoom(room);
                float angle = (float)random.NextDouble() * 360f;

                GameObject surface = Instantiate(reflectiveSurfaces[random.Next(reflectiveSurfaces.Length)],
                    (Vector3)position,
                    Quaternion.Euler(0, 0, angle));

                surface.tag = "Generated";
                room.objects.Add(surface);
            }
        }

        private void PopulateQuantumRoom(Room room)
        {
            // Add quantum-state-reactive barriers
            int barrierCount = random.Next(3, 6);
            for (int i = 0; i < barrierCount; i++)
            {
                Vector2 position = GetRandomPositionInRoom(room);
                float angle = (float)random.NextDouble() * 360f;

                GameObject barrier = Instantiate(energyBarriers[random.Next(energyBarriers.Length)],
                    (Vector3)position,
                    Quaternion.Euler(0, 0, angle));

                barrier.tag = "Generated";
                room.objects.Add(barrier);
            }
        }

        private void PopulateMixedRoom(Room room)
        {
            // Add both normal and quantum elements
            PopulateNormalRoom(room);
            PopulateQuantumRoom(room);
        }

        private Vector2 GetRandomPositionInRoom(Room room)
        {
            return room.position + new Vector2(
                ((float)random.NextDouble() - 0.5f) * room.size.x * 0.8f,
                ((float)random.NextDouble() - 0.5f) * room.size.y * 0.8f
            );
        }

        private void CreateBackground(Room room)
        {
            // Create quantum field effect
            if (quantumFieldParticles != null)
            {
                ParticleSystem particles = Instantiate(quantumFieldParticles,
                    (Vector3)room.position + Vector3.back,
                    Quaternion.identity);

                var shape = particles.shape;
                shape.scale = new Vector3(room.size.x, room.size.y, 1);

                particles.gameObject.tag = "Generated";
                room.objects.Add(particles.gameObject);
            }

            // Create background visual
            GameObject background = Instantiate(backgroundPrefab,
                (Vector3)room.position + Vector3.back * 2,
                Quaternion.identity);

            background.transform.localScale = new Vector3(room.size.x, room.size.y, 1);
            background.GetComponent<Renderer>().material =
                room.type == Room.RoomType.Quantum ? quantumRoomMaterial : normalRoomMaterial;

            background.tag = "Generated";
            room.objects.Add(background);
        }

        private void LoadCachedUniverse(int seed)
        {
            ClearCurrentUniverse();

            // Recreate the cached universe
            foreach (Room room in universeCache[seed])
            {
                PopulateRooms(new List<Room> { room });
            }
        }
    }
}