using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceInteraction : MonoBehaviour
{
    private List<string> existing_force_modes = new List<string>();
    private Dictionary<string, string> force_mode_types = new Dictionary<string, string>();
    private int force_mode_idx;

    // Force config.
    public GameObject vfx_push_from_hit;
    public GameObject vfx_pull_from_hit;
    public GameObject vfx_pull_from_space;
    public GameObject vfx_push_from_space;
    bool vfx_push_pull_from_space_preview_created = false;
    GameObject vfx_push_pull_from_space_inst;
    float vfx_push_pull_from_space_dist;

    // Gravity config.
    Vector3 gravity_dir;
    public float gravity_strength;

    // Start is called before the first frame update
    void Start()
    {
        // Ini modes.
        force_mode_types.Add("PUSH_FROM_HIT", "Push from hit");
        force_mode_types.Add("PULL_FROM_HIT", "Pull from hit");
        force_mode_types.Add("PULL_FROM_CAMERA", "Pull from camera");
        force_mode_types.Add("PUSH_FROM_CAMERA", "Push from camera");
        force_mode_types.Add("PUSH_FROM_SPACE", "Push from space");
        force_mode_types.Add("PULL_FROM_SPACE", "Pull from space");
        force_mode_types.Add("CHANGE_GRAVITY", "Change gravity");
        foreach( KeyValuePair<string, string> kvp in force_mode_types )
        {
            existing_force_modes.Add(kvp.Value);
        }
        force_mode_idx = 0;

        // Ini force config.
        vfx_push_pull_from_space_dist = 30.0f;

        // Ini gravity config.
        gravity_dir = new Vector3(0.0f, -1.0f, 0.0f);
        gravity_strength = 10.0f;
    }

    void OnGUI()
    {
        if (GlobalInfo.global_info.current_interaction_mode == GlobalInfo.global_info.interaction_mode_types["FORCE_INTERACTION"])
        {
            GUI.Box(new Rect(30, 60, 300, 20), "MODE (q,e): "+existing_force_modes[force_mode_idx]);

            if(existing_force_modes[force_mode_idx] == force_mode_types["PULL_FROM_SPACE"] || existing_force_modes[force_mode_idx] == force_mode_types["PUSH_FROM_SPACE"])
            {
                GUI.Box(new Rect(30, 120, 300, 20), "Distance (scroll): "+vfx_push_pull_from_space_dist);
            }

            if(existing_force_modes[force_mode_idx] == force_mode_types["CHANGE_GRAVITY"])
            {
                GUI.Box(new Rect(30, 80, 300, 20), "Gravity dir: "+gravity_dir+". Gravity strength: " + gravity_strength);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GlobalInfo.global_info.current_interaction_mode == GlobalInfo.global_info.interaction_mode_types["FORCE_INTERACTION"])
        {
            // Check mode.
            if (Input.GetKeyDown("e"))
            {
                force_mode_idx += 1;
                if (force_mode_idx >= existing_force_modes.Count)
                {
                    force_mode_idx = existing_force_modes.Count-1;
                } 
            }
            if (Input.GetKeyDown("q"))
            {
                force_mode_idx -= 1;
                if (force_mode_idx < 0)
                {
                    force_mode_idx = 0;
                }
            }

            if(existing_force_modes[force_mode_idx] == force_mode_types["PUSH_FROM_HIT"])
            {
                float force_sign = -1.0f;
                float radius = 10.0f;
                float force_strength = 1000.0f;
                if (Input.GetMouseButtonDown(0))
                {
                    pull_push_from_hit(force_sign, force_strength, radius);
                }
            }

            if(existing_force_modes[force_mode_idx] == force_mode_types["PULL_FROM_HIT"])
            {
                float force_sign = 1.0f;
                float radius = 10.0f;
                float force_strength = 1000.0f;
                if (Input.GetMouseButtonDown(0))
                {
                    pull_push_from_hit(force_sign, force_strength, radius);
                }
            }

            if(existing_force_modes[force_mode_idx] == force_mode_types["PUSH_FROM_CAMERA"])
            {
                float force_sign = -1.0f;
                float radius = 50.0f;
                float force_strength = 1000.0f;
                if (Input.GetMouseButtonDown(0))
                {
                    push_pull_from_camera(force_sign, force_strength, radius);
                }
            }

            if(existing_force_modes[force_mode_idx] == force_mode_types["PULL_FROM_CAMERA"])
            {
                float force_sign = 1.0f;
                float radius = 50.0f;
                float force_strength = 1000.0f;
                if (Input.GetMouseButtonDown(0))
                {
                    push_pull_from_camera(force_sign, force_strength, radius);
                }
            }

            if(existing_force_modes[force_mode_idx] == force_mode_types["PUSH_FROM_SPACE"])
            {
                float force_sign = -1.0f;
                float radius = 50.0f;
                float force_strength = 1000.0f;
                push_pull_from_space(force_sign, force_strength, radius);

            }

            if(existing_force_modes[force_mode_idx] == force_mode_types["PULL_FROM_SPACE"])
            {
                float force_sign = 1.0f;
                float radius = 50.0f;
                float force_strength = 1000.0f;
                push_pull_from_space(force_sign, force_strength, radius);
            }

            if(existing_force_modes[force_mode_idx] == force_mode_types["CHANGE_GRAVITY"])
            {
                change_gravity();
            }
        }
    }

    void change_gravity()
    {
        // Change gravity strength.
        if(Input.mouseScrollDelta.y < 0.0f)
        {
            gravity_strength -= 1.0f;
            if (gravity_strength < 0.0)
            {
                gravity_strength = 0.0f;
            }
        }

        if(Input.mouseScrollDelta.y > 0.0f)
        {
            gravity_strength += 1.0f;
        }

        // Change gravity direction.
        if (Input.GetMouseButtonDown(0))
        {
            int screen_x = Screen.width / 2;
            int screen_y = Screen.height / 2;
            Vector2 screen_center = new Vector2(screen_x, screen_y);
            Ray ray = Camera.main.ScreenPointToRay(screen_center);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity)) // objects to be instanced must have "Ignore Raycast" layer!
            {
                gravity_dir = hit.normal * gravity_strength;
                Physics.gravity = -hit.normal * gravity_strength;
            }
        }
    }

    void push_pull_from_space(float force_sign, float force_strength, float radius)
    {
        int screen_x = Screen.width / 2;
        int screen_y = Screen.height / 2;
        Vector2 screen_center = new Vector2(screen_x, screen_y);
        Ray ray = Camera.main.ScreenPointToRay(screen_center);
        Vector3 point_in_space = ray.origin + ray.direction * vfx_push_pull_from_space_dist;

        if(Input.mouseScrollDelta.y < 0.0f)
        {
            vfx_push_pull_from_space_dist -= 1.0f;
        }

        if(Input.mouseScrollDelta.y > 0.0f)
        {
            vfx_push_pull_from_space_dist += 1.0f;
        }
        
        if (!vfx_push_pull_from_space_preview_created)
        {
            if (force_sign > 0.0f)
            {
                vfx_push_pull_from_space_inst = Instantiate(vfx_pull_from_space, point_in_space, Quaternion.identity);
            }
            else
            {
                vfx_push_pull_from_space_inst = Instantiate(vfx_push_from_space, point_in_space, Quaternion.identity);
            }
            vfx_push_pull_from_space_preview_created = true;
        }

        vfx_push_pull_from_space_inst.transform.position = point_in_space;
        
        if (Input.GetMouseButton(0))
        {
            Collider[] hitColliders = Physics.OverlapSphere(point_in_space, radius);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject.GetComponent<Rigidbody>() != null)
                {
                    Vector3 collider_position = hitCollider.gameObject.transform.position;
                    Vector3 force_dir = Vector3.Normalize(point_in_space - collider_position);
                    Rigidbody collider_rigidbody = hitCollider.gameObject.GetComponent<Rigidbody>();
                    collider_rigidbody.AddForce(force_sign * force_dir * force_strength);
                }
            }
            Destroy(vfx_push_pull_from_space_inst, 1);
            vfx_push_pull_from_space_preview_created = false;
        }
    }

    void pull_push_from_hit(float force_sign, float force_strength, float radius)
    {
        int screen_x = Screen.width / 2;
        int screen_y = Screen.height / 2;
        Vector2 screen_center = new Vector2(screen_x, screen_y);
        Ray ray = Camera.main.ScreenPointToRay(screen_center);
        RaycastHit hit;
        GameObject vfx_inst;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (force_sign > 0.0f)
            {
                vfx_inst = Instantiate(vfx_pull_from_hit, hit.point, Quaternion.identity);
            }
            else
            {
                vfx_inst = Instantiate(vfx_push_from_hit, hit.point, Quaternion.identity);
            }
            Collider[] hitColliders = Physics.OverlapSphere(hit.point, radius);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject.GetComponent<Rigidbody>() != null)
                {
                    Vector3 collider_position = hitCollider.gameObject.transform.position;
                    Vector3 force_dir = Vector3.Normalize(hit.point - collider_position);
                    Rigidbody colloder_rigidbody = hitCollider.gameObject.GetComponent<Rigidbody>();
                    colloder_rigidbody.AddForce(force_sign * force_dir * force_strength);
                }
            }
            Destroy(vfx_inst, 1);
        }
    }

    void push_pull_from_camera(float force_sign, float force_strength, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.GetComponent<Rigidbody>() != null)
            {
                Vector3 collider_position = hitCollider.gameObject.transform.position;
                Vector3 force_dir = Vector3.Normalize(transform.position - collider_position);
                Rigidbody colloder_rigidbody = hitCollider.gameObject.GetComponent<Rigidbody>();
                colloder_rigidbody.AddForce(force_sign * force_dir * force_strength);
            }
        }
    }
}
