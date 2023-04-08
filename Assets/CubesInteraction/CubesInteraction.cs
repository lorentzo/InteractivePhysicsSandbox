using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubesInteraction : MonoBehaviour
{
    // Cubes data.
    public GameObject[] cubes_to_instance;
    int n_instanced_cubes;
    private List<GameObject> instanced_cubes = new List<GameObject>();  
    int cube_mode_idx;
    int instance_cube_random_scale;
    private List<string> existing_cube_modes = new List<string>();
    Dictionary<string, string> cube_mode_types = new Dictionary<string, string>();
    public float cubes_grow_shrink_all_factor = 1.0f;
    private Vector3 cubes_grow_shrink_all_vector;
    public float cubes_grow_shrink_particular_factor = 3.0f;
    private Vector3 cubes_grow_shrink_particular_vector;

    void OnGUI()
    {
        if (GlobalInfo.global_info.current_interaction_mode == GlobalInfo.global_info.interaction_mode_types["CUBE_INTERACTION"])
        {
            GUI.Box(new Rect(30, 80, 300, 20), "MODE (q,e): "+existing_cube_modes[cube_mode_idx]);
            GUI.Box(new Rect(30, 100, 300, 20), "N created instances: "+n_instanced_cubes);
            GUI.Box(new Rect(30, 140, 300, 20), "Instance with random scale :"+instance_cube_random_scale+" (i).");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Ini modes.
        cube_mode_idx = 1;
        cube_mode_types.Add("INSTANCE_EXPLOSION", "Instance Explosion");
        cube_mode_types.Add("INSTANCE_SPRAY_HIT", "Instance Spray from hit");
        cube_mode_types.Add("INSTANCE_SPRAY_CAMERA", "Instance Spray from camera");
        cube_mode_types.Add("GROW_ALL", "Grow all");
        cube_mode_types.Add("SHRINK_ALL", "Shrink all");
        cube_mode_types.Add("GROW_PARTICULAR", "Grow particular");
        cube_mode_types.Add("SHRINK_PARTICULAR", "Shrink particular");
        cube_mode_types.Add("DESTROY_ALL", "Destroy All");
        foreach( KeyValuePair<string, string> kvp in cube_mode_types )
        {
            existing_cube_modes.Add(kvp.Value);
        }
        
        // Grow/shrink ini.
        
        cubes_grow_shrink_all_vector = new Vector3(cubes_grow_shrink_all_factor, cubes_grow_shrink_all_factor, cubes_grow_shrink_all_factor);
        cubes_grow_shrink_particular_vector = new Vector3(cubes_grow_shrink_particular_factor, cubes_grow_shrink_particular_factor, cubes_grow_shrink_particular_factor);
        instance_cube_random_scale = 1;
    }

    void FixedUpdate()
    {
        if (GlobalInfo.global_info.current_interaction_mode == GlobalInfo.global_info.interaction_mode_types["CUBE_INTERACTION"])
        {
            spray_cubes_from_camera();
            n_instanced_cubes = instanced_cubes.Count;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GlobalInfo.global_info.current_interaction_mode == GlobalInfo.global_info.interaction_mode_types["CUBE_INTERACTION"])
        {
            // Check cube_mode_idx.
            if (Input.GetKeyDown("e"))
            {
                cube_mode_idx += 1;
                if (cube_mode_idx >= existing_cube_modes.Count) cube_mode_idx = existing_cube_modes.Count-1;
            }
            if (Input.GetKeyDown("q"))
            {
                cube_mode_idx -= 1;
                if (cube_mode_idx < 0) cube_mode_idx = 0;
            }

            // Check instancing scale.
            if (Input.GetKeyDown("i"))
            {
                if (instance_cube_random_scale == 1) instance_cube_random_scale = 0;
                else instance_cube_random_scale = 1;
            }

            if (existing_cube_modes[cube_mode_idx] == cube_mode_types["GROW_PARTICULAR"])
            {
                grow_particular();
            }

            if (existing_cube_modes[cube_mode_idx] == cube_mode_types["SHRINK_PARTICULAR"])
            {
                shrink_particular();
            }

            if (existing_cube_modes[cube_mode_idx] == cube_mode_types["GROW_ALL"])
            {
                grow_all();
            }

            if (existing_cube_modes[cube_mode_idx] == cube_mode_types["SHRINK_ALL"])
            {
                shrink_all();
            }

            if (existing_cube_modes[cube_mode_idx] == cube_mode_types["INSTANCE_EXPLOSION"])
            {
               instance_explosion(); 
            }

            if (existing_cube_modes[cube_mode_idx] == cube_mode_types["INSTANCE_SPRAY_HIT"])
            {
                instance_hit();
            }

             if (existing_cube_modes[cube_mode_idx] == cube_mode_types["DESTROY_ALL"])
            {
                destory_all();
            }

            n_instanced_cubes = instanced_cubes.Count;
        }
    }

    void spray_cubes_from_camera()
    {
        // Instance as spray from scene.
        if (existing_cube_modes[cube_mode_idx] == cube_mode_types["INSTANCE_SPRAY_CAMERA"])
        {
            if (Input.GetMouseButton(0))
            {
                int screen_x_center = Screen.width / 2;
                int screen_y_center = Screen.height / 2;
                Vector2 screen_center = new Vector2(screen_x_center, screen_y_center);
                Ray ray_center = Camera.main.ScreenPointToRay(screen_center);

                int screen_x_right = Screen.width / 2 + Screen.width / 4;
                Vector2 screen_right = new Vector2(screen_x_right, screen_y_center);
                Ray ray_right = Camera.main.ScreenPointToRay(screen_right);

                Vector3 screen_right_vec = ray_right.origin - ray_center.origin;

                for (var i = 0; i < 3; i++)
                {
                    Vector3 instance_origin = ray_center.origin + screen_right_vec.normalized * 5.0f; 
                    Vector3 start_position = instance_origin + ray_center.direction * 5.0f + ray_center.direction * 3.0f * i; // bit in front of camera, move each instance bit further
                    int cubes_to_instance_idx = Random.Range(0, cubes_to_instance.Length);
                    GameObject inst = Instantiate(cubes_to_instance[cubes_to_instance_idx], start_position, Quaternion.identity);
                    if (instance_cube_random_scale == 1)
                    {
                        float rand_scale = Random.Range(1.0f, 3.0f);
                        inst.transform.localScale = new Vector3(rand_scale, rand_scale, rand_scale);
                    }
                    inst.GetComponent<Rigidbody>().AddForce(ray_center.direction * 2000.0f);
                    instanced_cubes.Add(inst);
                }
            }
        }
    }

    void grow_particular()
    {
        // Grow particular.
        // TODO: physics raycast layers!
        if (Input.GetMouseButtonDown(0))
        {
            int screen_x = Screen.width / 2;
            int screen_y = Screen.height / 2;
            Vector2 screen_center = new Vector2(screen_x, screen_y);
            Ray ray = Camera.main.ScreenPointToRay(screen_center);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Collider[] hitColliders = Physics.OverlapSphere(hit.point, 10.0f);
                foreach (var hitCollider in hitColliders)
                {
                    hitCollider.gameObject.transform.localScale += cubes_grow_shrink_particular_vector;
                }
            }
        }
    }

    void shrink_particular()
    {
        // Shrink particular.
        // TODO: physics raycast layers!
        if (Input.GetMouseButtonDown(0))
        {
            int screen_x = Screen.width / 2;
            int screen_y = Screen.height / 2;
            Vector2 screen_center = new Vector2(screen_x, screen_y);
            Ray ray = Camera.main.ScreenPointToRay(screen_center);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Collider[] hitColliders = Physics.OverlapSphere(hit.point, 10.0f);
                foreach (var hitCollider in hitColliders)
                {
                    Vector3 curr_scale = hitCollider.gameObject.transform.localScale;
                    curr_scale -= cubes_grow_shrink_particular_vector;
                    if (curr_scale.x <= 0.0f)
                    {
                        hitCollider.gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f); 
                    }
                    else
                    {
                        hitCollider.gameObject.transform.localScale = curr_scale;
                    }
                }
            }
        }
    }

    void grow_all()
    {
        if (Input.GetMouseButton(0))
        {
            for (int i = 0; i < n_instanced_cubes; ++i)
            {
                instanced_cubes[i].transform.localScale += cubes_grow_shrink_all_vector * Time.deltaTime;
            }
        }
    }

    void shrink_all()
    {
        if (Input.GetMouseButton(0))
        {
            for (int i = 0; i < n_instanced_cubes; ++i)
            {
                if (instanced_cubes[i].transform.localScale.x > 0.1f)
                {
                    instanced_cubes[i].transform.localScale -= cubes_grow_shrink_all_vector * Time.deltaTime;
                }
            }
        }
    }

    void instance_explosion()
    {
        if (Input.GetMouseButtonDown(0))
        {
            int screen_x = Screen.width / 2;
            int screen_y = Screen.height / 2;
            Vector2 screen_center = new Vector2(screen_x, screen_y);
            Ray ray = Camera.main.ScreenPointToRay(screen_center);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                for (var i = 0; i < 50; i++)
                {
                    Vector3 rand = new Vector3(Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f));
                    int cubes_to_instance_idx = Random.Range(0, cubes_to_instance.Length);
                    GameObject inst = Instantiate(cubes_to_instance[cubes_to_instance_idx], hit.point + rand, Quaternion.identity);
                    if (instance_cube_random_scale == 1)
                    {
                        float rand_scale = Random.Range(1.0f, 3.0f);
                        inst.transform.localScale = new Vector3(rand_scale, rand_scale, rand_scale);
                    }
                    instanced_cubes.Add(inst);
                }
            }
        }
    }

    void instance_hit()
    {
        if (Input.GetMouseButton(0))
        {
            int screen_x = Screen.width / 2;
            int screen_y = Screen.height / 2;
            Vector2 screen_center = new Vector2(screen_x, screen_y);
            Ray ray = Camera.main.ScreenPointToRay(screen_center);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity)) // objects to be instanced must have "Ignore Raycast" layer!
            {
                for (int i = 0; i < 10; i++)
                {
                    Vector3 position = hit.point;
                    int cubes_to_instance_idx = Random.Range(0, cubes_to_instance.Length);
                    GameObject inst = Instantiate(cubes_to_instance[cubes_to_instance_idx], position, Quaternion.identity);
                    if (instance_cube_random_scale == 1)
                    {
                        float rand_scale = Random.Range(1.0f, 3.0f);
                        inst.transform.localScale = new Vector3(rand_scale, rand_scale, rand_scale);
                    }
                    instanced_cubes.Add(inst);
                }
            }
        }
    }

    void destory_all()
    {
        if (Input.GetMouseButton(0))
        {
            for (int i = 0; i < n_instanced_cubes; ++i)
            {
                Destroy(instanced_cubes[i], 1);
            }
        }
    }
}
