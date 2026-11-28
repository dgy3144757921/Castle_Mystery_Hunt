using System.Text;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// 抓取诊断脚本（运行时事件监听版）
/// 用法：把本脚本拖到抓不起来的物体（如"画"）上，进入 Play。
/// 1) Start 时打印一次静态配置检查；
/// 2) 每 1 秒打印一次本物体与左/右控制器的实时距离、刚体休眠状态；
/// 3) 用手去抓时，Console 会实时打印 hover / select 事件是否触发。
/// 关键判读：
///   - 手已经贴到画但「一次 hover 都没有」→ 物理检测问题（碰撞体/触发器重叠、刚体休眠）
///   - 有 hover 但没有 select → 输入动作/抓取键绑定问题
///   - 有 select 但物体不动 → 刚体或抓取变换器问题
/// 确认问题后可删除本脚本。
/// </summary>
public class GrabDiagnostics : MonoBehaviour
{
    [Tooltip("Play 时自动执行静态检查")]
    public bool runOnStart = true;
    [Tooltip("每秒打印一次距离/状态")]
    public bool logRuntime = true;
    public float logInterval = 1f;
    public bool printInteractorDetails = true;

    private XRGrabInteractable grab;
    private Rigidbody rb;
    private XRBaseInteractor[] interactors;
    private float nextLog;

    private void Start()
    {
        grab = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        if (runOnStart)
            DiagnoseStatic();

        // 订阅运行时交互事件 —— 这是定位"抓不到"最关键的部分
        if (grab != null)
        {
            grab.hoverEntered.AddListener(OnHoverEntered);
            grab.hoverExited.AddListener(OnHoverExited);
            grab.selectEntered.AddListener(OnSelectEntered);
            grab.selectExited.AddListener(OnSelectExited);
            Debug.Log("[抓取诊断] 已开始监听交互事件。现在用手去抓这个物体，观察下面是否打印 hover / select。");
        }
        else
        {
            Debug.LogWarning("[抓取诊断] 本物体没有 XRGrabInteractable，事件监听无法启动。");
        }
    }

    private void OnDestroy()
    {
        if (grab == null) return;
        grab.hoverEntered.RemoveListener(OnHoverEntered);
        grab.hoverExited.RemoveListener(OnHoverExited);
        grab.selectEntered.RemoveListener(OnSelectEntered);
        grab.selectExited.RemoveListener(OnSelectExited);
    }

    private void OnHoverEntered(HoverEnterEventArgs e)
    {
        Debug.Log(">>> [抓取诊断] HOVER ENTER  被 [" + NameOf(e.interactorObject) + "] 悬停 —— 检测成功，试着按抓取键");
    }

    private void OnHoverExited(HoverExitEventArgs e)
    {
        Debug.Log("<<< [抓取诊断] HOVER EXIT   离开 [" + NameOf(e.interactorObject) + "]");
    }

    private void OnSelectEntered(SelectEnterEventArgs e)
    {
        Debug.Log("★★★ [抓取诊断] SELECT ENTER 抓取成功！由 [" + NameOf(e.interactorObject) + "] 抓起 —— 抓取链路完全正常");
    }

    private void OnSelectExited(SelectExitEventArgs e)
    {
        Debug.Log("☆☆☆ [抓取诊断] SELECT EXIT  松开 / 抓取中断");
    }

    private static string NameOf(IXRInteractor it)
    {
        return it == null ? "null" : it.GetType().Name + " on " + it.transform.name;
    }

    private void Update()
    {
        if (!logRuntime || Time.time < nextLog) return;
        nextLog = Time.time + logInterval;

        if (interactors == null || interactors.Length == 0)
            interactors = FindObjectsByType<XRBaseInteractor>(FindObjectsSortMode.None);

        var sb = new StringBuilder();
        sb.Append("[状态] ").Append(gameObject.name);
        if (rb != null)
            sb.Append(" 刚体休眠=").Append(rb.IsSleeping())
              .Append(" 位置=").Append(transform.position.ToString("F2"));
        sb.AppendLine();

        // 与每个交互器的实时距离 + 到碰撞体最近点的距离
        Collider[] myCols = GetComponentsInChildren<Collider>();
        foreach (var it in interactors)
        {
            if (it == null) continue;
            Vector3 p = it.transform.position;
            float centerDist = Vector3.Distance(p, transform.position);
            float surfaceDist = float.MaxValue;
            foreach (var c in myCols)
                if (c.enabled)
                    surfaceDist = Mathf.Min(surfaceDist, Vector3.Distance(p, c.ClosestPoint(p)));
            sb.Append("   ").Append(it.GetType().Name).Append(" on ").Append(it.transform.name)
              .Append(" 到中心=").Append(centerDist.ToString("F2"))
              .Append("m 到表面=").Append(surfaceDist.ToString("F2")).Append("m");
            if (it is XRDirectInteractor)
            {
                var d = it as XRDirectInteractor;
                var dc = d.GetComponent<Collider>();
                if (dc != null)
                    sb.Append(" [触发器半径=").Append(GetRadius(dc).ToString("F3"))
                      .Append("m trigger=").Append(dc.isTrigger).Append("]");
            }
            sb.AppendLine();
        }
        Debug.Log(sb.ToString());
    }

    private static float GetRadius(Collider c)
    {
        if (c is SphereCollider s) return s.radius;
        if (c is CapsuleCollider cap) return cap.radius;
        return 0f;
    }

    [ContextMenu("运行静态检查")]
    public void DiagnoseStatic()
    {
        var sb = new StringBuilder();
        sb.AppendLine("========== [抓取诊断-静态] " + gameObject.name + " ==========");

        var g = GetComponent<XRGrabInteractable>();
        sb.AppendLine("[1] XRGrabInteractable: " + (g != null ? "有" : "【缺失】"));
        if (g != null)
            sb.AppendLine("    enabled=" + g.enabled + ", 交互层=" + g.interactionLayers.value +
                ", 抓取模式=" + g.selectMode + ", 抛掷倍率=" + g.throwVelocityScale);

        sb.AppendLine("[1b] Static 静态标记: " + gameObject.isStatic +
            (gameObject.isStatic ? "  ←【致命】静态物体会禁用刚体物理：悬空不下落、抓取检测失效！Inspector 右上角取消 Static 勾选" : " (正常)"));

        var body = GetComponent<Rigidbody>();
        sb.AppendLine("[2] Rigidbody: " + (body != null ? "有" : "【缺失】"));
        if (body != null)
            sb.AppendLine("    isKinematic=" + body.isKinematic + ", useGravity=" + body.useGravity +
                ", 休眠=" + body.IsSleeping());

        var cols = GetComponentsInChildren<Collider>(true);
        sb.AppendLine("[3] 碰撞体数量(含子物体): " + cols.Length);
        foreach (var c in cols)
        {
            Vector3 lossy = c.transform.lossyScale;
            Vector3 ws = Vector3.zero;
            if (c is BoxCollider b) ws = Vector3.Scale(b.size, lossy);
            else if (c is SphereCollider s) ws = Vector3.one * s.radius * 2f * Mathf.Max(lossy.x, Mathf.Max(lossy.y, lossy.z));
            else if (c is CapsuleCollider cap) ws = Vector3.Scale(new Vector3(cap.radius * 2f, cap.height, cap.radius * 2f), lossy);
            float min = Mathf.Min(ws.x, Mathf.Min(ws.y, ws.z));
            sb.AppendLine("    - " + c.GetType().Name + " on " + c.gameObject.name +
                " trigger=" + c.isTrigger + " enabled=" + c.enabled +
                " 世界尺寸=" + ws.ToString("F3") + " 最小厚度=" + min.ToString("F3") + "m");
        }

        var all = FindObjectsByType<XRBaseInteractor>(FindObjectsSortMode.None);
        sb.AppendLine("[4] 交互器数量: " + all.Length);
        int myLayer = g != null ? g.interactionLayers.value : 0;
        foreach (var it in all)
        {
            bool overlap = g != null && (myLayer & it.interactionLayers.value) != 0;
            sb.AppendLine("    - " + it.GetType().Name + " on " + it.transform.name +
                " enabled=" + it.enabled + " 交互层=" + it.interactionLayers.value +
                " 重叠=" + overlap + (overlap ? "" : " ←【层不匹配】"));
            if (printInteractorDetails && it is XRDirectInteractor)
            {
                var dc = it.GetComponent<Collider>();
                sb.AppendLine("        直接抓取触发器=" + (dc != null ? dc.GetType().Name + " trigger=" + dc.isTrigger + " 半径=" + GetRadius(dc).ToString("F3") : "【无 Collider】"));
            }
        }
        sb.AppendLine("========== 静态检查结束 ==========");
        Debug.Log(sb.ToString());
    }
}
