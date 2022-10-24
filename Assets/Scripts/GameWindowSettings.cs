using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine.Events;

/// <summary>
///ǿ������Unity��Ϸ���ڵĳ���ȡ�����Ե������ڵĴ�С������ǿ�Ʊ���һ������
///ͨ�����ش��ڴ�С�����¼�(WindowProc�ص�)����Ӧ���޸�������ʵ�ֵ�
///Ҳ����������Ϊ����������С/����Ⱥ͸߶�
///����Ⱥ���С/���ֱ��ʶ��봰�������йأ��������ͱ߿򲻰�������
///�ýű�������Ӧ�ó�����ȫ��״̬ʱǿ�����ó���ȡ������л���ȫ����
///Ӧ�ó����Զ�����Ϊ��ǰ��ʾ���Ͽ��ܵ����ֱ��ʣ�����Ȼ���̶ֹ��ȡ������ʾ��û����ͬ�Ŀ�߱ȣ��������/�һ���/����Ӻ���
///ȷ������PlayerSetting�������ˡ�Resizable Window���������޷�������С
///���ȡ����֧�ֵĳ������PlayerSetting�����á�Supported Aspect Rations��
///ע��:��Ϊʹ����WinAPI������ֻ����Windows�Ϲ�������Windows 10�ϲ��Թ�
/// </summary>
public class GameWindowSettings : MonoBehaviour
{
    /// <summary>
    /// ÿ�����ڷֱ��ʸı���û��л�ȫ��ʱ�����ᴥ�����¼�
    ///  �������µĿ�ȡ��߶Ⱥ�ȫ��״̬(true��ʾȫ��)
    /// </summary>
    public ResolutionChangedEvent resolutionChangedEvent;
    [Serializable]
    public class ResolutionChangedEvent : UnityEvent<int, int, bool> { }

    // ���Ϊfalse������ֹ�л���ȫ��
    [SerializeField]
    private bool allowFullscreen = true;

    // ����ȵĿ�Ⱥ͸߶�
    [SerializeField]
    private float aspectRatioWidth = 16;
    [SerializeField]
    private float aspectRatioHeight = 9;

    // ��Сֵ�����ֵ�Ĵ��ڿ��/�߶�����
    [SerializeField]
    private int minWidthPixel = 512;
    [SerializeField]
    private int minHeightPixel = 512;
    [SerializeField]
    private int maxWidthPixel = 1920;
    [SerializeField]
    private int maxHeightPixel = 1920;

    // ��ǰ��������ȡ�
    private float aspect;

    // ���ڵĿ�Ⱥ͸߶ȡ��������߿�ʹ��ڱ�����
    // ���������ڴ�Сʱ���ͻ�������Щֵ
    private int setWidth = -1;
    private int setHeight = -1;

    // ���һ֡ȫ��״̬��
    private bool wasFullscreenLastFrame;

    // �Ƿ��ʼ����AspectRatioController
    // һ��ע����WindowProc�ص��������ͽ�������Ϊtrue
    private bool started;

    // ��ʾ���Ŀ�Ⱥ͸߶ȡ����Ǵ��ڵ�ǰ�򿪵ļ�����
    private int pixelHeightOfCurrentScreen;
    private int pixelWidthOfCurrentScreen;

    //һ���û�������ֹapplaction����������Ϊtrue
    private bool quitStarted;

    // WinAPI��ض���
    #region WINAPI

    // �����ڵ���ʱ,WM_SIZING��Ϣͨ��WindowProc�ص����͵�����
    private const int WM_SIZING = 0x214;

    // WM��С������Ϣ�Ĳ���
    private const int WMSZ_LEFT = 1;
    private const int WMSZ_RIGHT = 2;
    private const int WMSZ_TOP = 3;
    private const int WMSZ_BOTTOM = 6;

    // ��ȡָ��WindowProc������ָ��
    private const int GWLP_WNDPROC = -4;

    // ί������Ϊ�µ�WindowProc�ص�����
    private delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
    private WndProcDelegate wndProcDelegate;

    // ���������̵߳��̱߳�ʶ��
    [DllImport("kernel32.dll")]
    private static extern uint GetCurrentThreadId();

    // ����ָ�����������������
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int GetClassName(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    // ͨ����������ݸ�ÿ�����ڣ����δ��ݸ�Ӧ�ó�����Ļص�������ö�����̹߳��������з��Ӵ���
    [DllImport("user32.dll")]
    private static extern bool EnumThreadWindows(uint dwThreadId, EnumWindowsProc lpEnumFunc, IntPtr lParam);
    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    // ����Ϣ��Ϣ���ݸ�ָ���Ĵ��ڹ���
    [DllImport("user32.dll")]
    private static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    // ����ָ�����ڵı߿�ĳߴ�
    // �ߴ�������Ļ�����и����ģ������������Ļ���Ͻǵ�
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool GetWindowRect(IntPtr hwnd, ref RECT lpRect);

    //�������ڿͻ���������ꡣ�ͻ�������ָ�����Ͻ�
    //�Լ��ͻ��������½ǡ���Ϊ�ͻ�����������������Ͻǵ�
    //�ڴ��ڵĿͻ�����Ľ��䣬���Ͻǵ�������(0,0)
    [DllImport("user32.dll")]
    private static extern bool GetClientRect(IntPtr hWnd, ref RECT lpRect);

    // ����ָ�����ڵ����ԡ��ú�������ָ��ƫ������32λ(��)ֵ���õ�����Ĵ����ڴ���
    [DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
    private static extern IntPtr SetWindowLong32(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    //����ָ�����ڵ����ԡ��ú������ڶ���Ĵ����ڴ���ָ����ƫ����������һ��ֵ
    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Auto)]
    private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    //���ڲ��Ҵ��ھ����Unity�����������
    private const string UNITY_WND_CLASSNAME = "UnityWndClass";

    // Unity���ڵĴ��ھ��
    private IntPtr unityHWnd;

    // ָ���WindowProc�ص�������ָ��
    private IntPtr oldWndProcPtr;

    // ָ�������Լ��Ĵ��ڻص�������ָ��
    private IntPtr newWndProcPtr;

    /// <summary>
    /// WinAPI���ζ��塣
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    #endregion

    void Start()
    {
        // ��Ҫ��Unity�༭����ע��WindowProc�ص�����������ָ��Unity�༭�����ڣ�������Game��ͼ

#if !UNITY_EDITOR
        //ע��ص���Ȼ��Ӧ�ó�����Ҫ�˳�
        Application.wantsToQuit += ApplicationWantsToQuit;
 
        // �ҵ���Unity���ڵĴ��ھ��
        EnumThreadWindows(GetCurrentThreadId(), (hWnd, lParam) =>
        {
            var classText = new StringBuilder(UNITY_WND_CLASSNAME.Length + 1);
            GetClassName(hWnd, classText, classText.Capacity);
 
            if (classText.ToString() == UNITY_WND_CLASSNAME)
            {
                unityHWnd = hWnd;
                return false;
            }
            return true;
        }, IntPtr.Zero);
 
        // �������Ӧ���ڵ�ǰ�ֱ���
        SetAspectRatio(aspectRatioWidth, aspectRatioHeight, true);
 
        // ���浱ǰ��ȫ��״̬
        wasFullscreenLastFrame = Screen.fullScreen;
 
        // Register (replace) WindowProc callback��ÿ��һ�������¼�������ʱ������������ᱻ����
        //���������С���ƶ�����
        //����ɵ�WindowProc�ص���������Ϊ������»ص������е�����
        wndProcDelegate = wndProc;
        newWndProcPtr = Marshal.GetFunctionPointerForDelegate(wndProcDelegate);
        oldWndProcPtr = SetWindowLong(unityHWnd, GWLP_WNDPROC, newWndProcPtr);
 
        // ��ʼ�����
        started = true;
#endif

    }

    /// <summary>
    ///��Ŀ�곤�������Ϊ�����ĳ���ȡ�
    /// </summary>
    /// <param name="newAspectWidth">��߱ȵ��¿��</param>
    /// <param name="newAspectHeight">�ݺ�ȵ��¸߶�</param>
    /// <param name="apply">true����ǰ���ڷֱ��ʽ�����������ƥ���µ��ݺ�� false����ֻ���´��ֶ��������ڴ�Сʱִ�д˲���</param>
    public void SetAspectRatio(float newAspectWidth, float newAspectHeight, bool apply)
    {
        //�����µ��ݺ��
        aspectRatioWidth = newAspectWidth;
        aspectRatioHeight = newAspectHeight;
        aspect = aspectRatioWidth / aspectRatioHeight;

        // �����ֱ�����ƥ�䳤���(����WindowProc�ص�)
        if (apply)
        {
            Screen.SetResolution(Screen.width, Mathf.RoundToInt(Screen.width / aspect), Screen.fullScreen);
        }
    }

    /// <summary>
    /// WindowProc�ص���Ӧ�ó�����ĺ��������������͵����ڵ���Ϣ 
    /// </summary>
    /// <param name="msg">���ڱ�ʶ�¼�����Ϣ</param>
    /// <param name="wParam">�������Ϣ��Ϣ���ò���������ȡ����uMsg������ֵ </param>
    /// <param name="lParam">������Ϣ����Ϣ���ò���������ȡ����uMsg������ֵ </param>
    /// <returns></returns>
    IntPtr wndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        // �����Ϣ����
        // resize�¼�
        if (msg == WM_SIZING)
        {
            // ��ȡ���ڴ�С�ṹ��
            RECT rc = (RECT)Marshal.PtrToStructure(lParam, typeof(RECT));

            // ���㴰�ڱ߿�Ŀ�Ⱥ͸߶�
            RECT windowRect = new RECT();
            GetWindowRect(unityHWnd, ref windowRect);

            RECT clientRect = new RECT();
            GetClientRect(unityHWnd, ref clientRect);

            int borderWidth = windowRect.Right - windowRect.Left - (clientRect.Right - clientRect.Left);
            int borderHeight = windowRect.Bottom - windowRect.Top - (clientRect.Bottom - clientRect.Top);

            // ��Ӧ�ÿ�߱�֮ǰɾ���߿�(�������ڱ�����)
            rc.Right -= borderWidth;
            rc.Bottom -= borderHeight;

            // ���ƴ��ڴ�С
            int newWidth = Mathf.Clamp(rc.Right - rc.Left, minWidthPixel, maxWidthPixel);
            int newHeight = Mathf.Clamp(rc.Bottom - rc.Top, minHeightPixel, maxHeightPixel);

            // �����ݺ�Ⱥͷ��������С
            switch (wParam.ToInt32())
            {
                case WMSZ_LEFT:
                    rc.Left = rc.Right - newWidth;
                    rc.Bottom = rc.Top + Mathf.RoundToInt(newWidth / aspect);
                    break;
                case WMSZ_RIGHT:
                    rc.Right = rc.Left + newWidth;
                    rc.Bottom = rc.Top + Mathf.RoundToInt(newWidth / aspect);
                    break;
                case WMSZ_TOP:
                    rc.Top = rc.Bottom - newHeight;
                    rc.Right = rc.Left + Mathf.RoundToInt(newHeight * aspect);
                    break;
                case WMSZ_BOTTOM:
                    rc.Bottom = rc.Top + newHeight;
                    rc.Right = rc.Left + Mathf.RoundToInt(newHeight * aspect);
                    break;
                case WMSZ_RIGHT + WMSZ_BOTTOM:
                    rc.Right = rc.Left + newWidth;
                    rc.Bottom = rc.Top + Mathf.RoundToInt(newWidth / aspect);
                    break;
                case WMSZ_RIGHT + WMSZ_TOP:
                    rc.Right = rc.Left + newWidth;
                    rc.Top = rc.Bottom - Mathf.RoundToInt(newWidth / aspect);
                    break;
                case WMSZ_LEFT + WMSZ_BOTTOM:
                    rc.Left = rc.Right - newWidth;
                    rc.Bottom = rc.Top + Mathf.RoundToInt(newWidth / aspect);
                    break;
                case WMSZ_LEFT + WMSZ_TOP:
                    rc.Left = rc.Right - newWidth;
                    rc.Top = rc.Bottom - Mathf.RoundToInt(newWidth / aspect);
                    break;
            }

            // ����ʵ�ʷֱ���,�������߽�
            setWidth = rc.Right - rc.Left;
            setHeight = rc.Bottom - rc.Top;

            // ��ӱ߽�
            rc.Right += borderWidth;
            rc.Bottom += borderHeight;

            // �����ֱ��ʸ����¼�
            resolutionChangedEvent.Invoke(setWidth, setHeight, Screen.fullScreen);

            // ��д���ĵĴ��ڲ���
            Marshal.StructureToPtr(rc, lParam, true);
        }

        // ����ԭʼ��WindowProc����
        return CallWindowProc(oldWndProcPtr, hWnd, msg, wParam, lParam);
    }

    void Update()
    {
        // ���������ȫ��������ֹ�л���ȫ��
        if (!allowFullscreen && Screen.fullScreen)
        {
            Screen.fullScreen = false;
        }

        if (Screen.fullScreen && !wasFullscreenLastFrame)
        {
            //�л���ȫ�����,����Ϊ�����Ļ�ֱ��ʣ�ͬʱ���ֳ����
            int height;
            int width;

            //���ݵ�ǰ����Ⱥ���ʾ���ı������бȽϣ����»�������Ӻڱ�
            bool blackBarsLeftRight = aspect < (float)pixelWidthOfCurrentScreen / pixelHeightOfCurrentScreen;

            if (blackBarsLeftRight)
            {
                height = pixelHeightOfCurrentScreen;
                width = Mathf.RoundToInt(pixelHeightOfCurrentScreen * aspect);
            }
            else
            {
                width = pixelWidthOfCurrentScreen;
                height = Mathf.RoundToInt(pixelWidthOfCurrentScreen / aspect);
            }

            Screen.SetResolution(width, height, true);
            resolutionChangedEvent.Invoke(width, height, true);
        }
        else if (!Screen.fullScreen && wasFullscreenLastFrame)
        {
            // ��ȫ���л�����⵽�Ĵ��ڡ�������һ�����ڵķֱ��ʡ�
            Screen.SetResolution(setWidth, setHeight, false);
            resolutionChangedEvent.Invoke(setWidth, setHeight, false);
        }
        else if (!Screen.fullScreen && setWidth != -1 && setHeight != -1 && (Screen.width != setWidth || Screen.height != setHeight))
        {
            //���ݸ߶����ÿ�ȣ���ΪAero Snap���ᴥ��WM_SIZING��
            setHeight = Screen.height;
            setWidth = Mathf.RoundToInt(Screen.height * aspect);

            Screen.SetResolution(setWidth, setHeight, Screen.fullScreen);
            resolutionChangedEvent.Invoke(setWidth, setHeight, Screen.fullScreen);
        }
        else if (!Screen.fullScreen)
        {
            // ���浱ǰ��Ļ�ķֱ���
            // �´��л���ȫ��ʱ���˷ֱ��ʽ�������Ϊ���ڷֱ���
            // ֻ�и߶ȣ������Ҫ����Ƚ����ݸ߶Ⱥͳ�������ã���ȷ������ȱ�����ȫ��ģʽ
            pixelHeightOfCurrentScreen = Screen.currentResolution.height;
            pixelWidthOfCurrentScreen = Screen.currentResolution.width;
        }

        //������һ֡��ȫ��״̬
        wasFullscreenLastFrame = Screen.fullScreen;

        // ����Ϸ���ڵ�����Сʱ���ڱ༭���д����ֱ��ʸı��¼���
#if UNITY_EDITOR
        if (Screen.width != setWidth || Screen.height != setHeight)
        {
            setWidth = Screen.width;
            setHeight = Screen.height;
            resolutionChangedEvent.Invoke(setWidth, setHeight, Screen.fullScreen);
        }
#endif
    }

    /// <summary>
    /// ����SetWindowLong32��SetWindowLongPtr64��ȡ���ڿ�ִ���ļ���32λ����64λ��
    /// ���������ǾͿ���ͬʱ����32λ��64λ�Ŀ�ִ���ļ��������������⡣
    /// </summary>
    /// <param name="hWnd">The window handle.</param>
    /// <param name="nIndex">Ҫ���õ�ֵ�Ĵ��㿪ʼ��ƫ����</param>
    /// <param name="dwNewLong">The replacement value.</param>
    /// <returns>����ֵ��ָ��ƫ������ǰһ��ֵ��������.</returns>
    private static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
    {
        //32λϵͳ
        if (IntPtr.Size == 4)
        {
            return SetWindowLong32(hWnd, nIndex, dwNewLong);
        }
        return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
    }

    /// <summary>
    /// �˳�ʱ���á� ����false����ֹ��ʹӦ�ó��򱣳ֻ��True�������˳���
    /// </summary>
    /// <returns></returns>
    private bool ApplicationWantsToQuit()
    {
        //��������Ӧ�ó����ʼ�����˳���
        if (!started)
            return false;

        //�ӳ��˳���clear up
        if (!quitStarted)
        {
            StartCoroutine("DelayedQuit");
            return false;
        }

        return true;
    }

    /// <summary>
    /// �ָ��ɵ�WindowProc�ص���Ȼ���˳���
    /// </summary>
    IEnumerator DelayedQuit()
    {

        // �������þɵ�WindowProc�ص�,�����⵽WM_CLOSE,�⽫���µĻص����������, 64λû���⣬32λ���ܻ��������

        SetWindowLong(unityHWnd, GWLP_WNDPROC, oldWndProcPtr);

        yield return new WaitForEndOfFrame();

        quitStarted = true;
        Application.Quit();
    }
}