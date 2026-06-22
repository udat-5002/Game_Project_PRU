using UnityEngine;
using UnityEditor;

public class SnapToGround : Editor
{
    [MenuItem("Tools/Snap To Ground %g")] // Phím tắt: Ctrl + G (hoặc Cmd + G)
    public static void SnapSelectedToGround()
    {
        // 1. Tự động sửa lỗi: Quét qua tất cả object có chữ "plane" hoặc "ground" để gắn Collider nếu bị thiếu
        MeshRenderer[] renderers = Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None);
        int addedColliders = 0;
        foreach (MeshRenderer rend in renderers)
        {
            string name = rend.gameObject.name.ToLower();
            if (name.Contains("plane") || name.Contains("ground"))
            {
                if (rend.gameObject.GetComponent<Collider>() == null)
                {
                    rend.gameObject.AddComponent<MeshCollider>();
                    addedColliders++;
                }
            }
        }
        if (addedColliders > 0)
        {
            Debug.Log($"[Tự động sửa lỗi] Đã tự động gắn MeshCollider cho {addedColliders} mặt đất.");
        }

        // 2. Thực hiện Snap nhân vật
        int snappedCount = 0;

        foreach (GameObject obj in Selection.gameObjects)
        {
            Undo.RecordObject(obj.transform, "Snap to Ground");

            bool wasActive = obj.activeSelf;
            obj.SetActive(false);

            // Tính độ lệch từ tâm đến gót chân nhân vật
            float bottomOffset = 0f;
            Collider col = obj.GetComponentInChildren<Collider>();
            if (col != null)
            {
                bottomOffset = obj.transform.position.y - col.bounds.min.y;
            }
            else
            {
                Renderer rend = obj.GetComponentInChildren<Renderer>();
                if (rend != null)
                {
                    bottomOffset = obj.transform.position.y - rend.bounds.min.y;
                }
            }

            // Bắn tia xuống mặt đất
            RaycastHit hit;
            if (Physics.Raycast(obj.transform.position + Vector3.up * 100f, Vector3.down, out hit, 2000f))
            {
                obj.SetActive(wasActive);
                obj.transform.position = new Vector3(obj.transform.position.x, hit.point.y + bottomOffset, obj.transform.position.z);
                snappedCount++;
            }
            else
            {
                obj.SetActive(wasActive); 
                
                // NẾU TIA BẮN XUỐNG BỊ TRƯỢT: Có nghĩa là nhân vật đang đứng cách mặt đất quá xa theo chiều ngang (X, Z).
                // Khắc phục: Tìm mặt đất và dịch chuyển nhân vật thẳng ra chính giữa mặt đất luôn.
                GameObject groundObj = GameObject.Find("Ground");
                if (groundObj == null) groundObj = GameObject.Find("Plane");
                if (groundObj == null) groundObj = GameObject.Find("ground");
                
                if (groundObj != null)
                {
                    Collider groundCol = groundObj.GetComponent<Collider>();
                    if (groundCol != null)
                    {
                        float groundTop = groundCol.bounds.max.y;
                        obj.transform.position = new Vector3(groundCol.bounds.center.x, groundTop + bottomOffset, groundCol.bounds.center.z);
                        snappedCount++;
                        Debug.LogWarning($"Nhân vật {obj.name} đứng nằm ngoài khu vực có đất, mình đã tự động dịch chuyển nhân vật ra chính giữa mặt đất {groundObj.name}!");
                    }
                }
            }
        }

        if (snappedCount > 0)
        {
            Debug.Log($"Đã đặt thành công {snappedCount} object(s) xuống mặt đất.");
        }
        else
        {
            Debug.LogError("Vẫn không tìm thấy mặt đất. Bạn hãy kiểm tra xem có object nào tên là 'Ground' hay 'Plane' không nhé.");
        }
    }
}
