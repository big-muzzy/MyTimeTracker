using System;
using System.Threading;
using System.Windows;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
//using System.Diagnostics;

namespace MyTimeTracker
{
  public static class WpfSingleInstance
  {

    internal static void Make(String name, Application app)
    {

      EventWaitHandle eventWaitHandle = null;
      String eventName = Environment.MachineName + "-" + name;

      bool isFirstInstance = false;

      try
      {
        eventWaitHandle = EventWaitHandle.OpenExisting(eventName);
      }
      catch
      {
        // it's first instance
        isFirstInstance = true;
      }

      if (isFirstInstance)
      {
        eventWaitHandle = new EventWaitHandle(
            false,
            EventResetMode.AutoReset,
            eventName);

        ThreadPool.RegisterWaitForSingleObject(eventWaitHandle, waitOrTimerCallback, app, Timeout.Infinite, false);

        // not need more
        eventWaitHandle.Close();
      }
      else
      {
        eventWaitHandle.Set();

        //if ((app.MainWindow as MainWindow) != null)
        //    if ((app.MainWindow as MainWindow).MttSettings.MinimizeToTray == false)
        //        MessageBox.Show("Уже запущена одна копия программы", "Программа уже запущена",
        //                            MessageBoxButton.OK, MessageBoxImage.Information);

        IntPtr hTargetWnd = NativeMethods.FindWindow(null, "MyTimeTracker");
        if (hTargetWnd == IntPtr.Zero)
        {
          return;
        }

        NativeMethods.MyStruct myStruct;
        myStruct.Message = "ShowMyTimeTrackerMainWindow";
        int myStructSize = Marshal.SizeOf(myStruct);
        IntPtr pMyStruct = Marshal.AllocHGlobal(myStructSize);
        try
        {
          Marshal.StructureToPtr(myStruct, pMyStruct, true);

          NativeMethods.COPYDATASTRUCT cds = new NativeMethods.COPYDATASTRUCT();
          cds.cbData = myStructSize;
          cds.lpData = pMyStruct;
          NativeMethods.SendMessage(hTargetWnd, NativeMethods.WM_COPYDATA, new IntPtr(), ref cds);

          int result = Marshal.GetLastWin32Error();
          if (result != 0)
          {
          }
        }
        finally
        {
          Marshal.FreeHGlobal(pMyStruct);
        }

        // For that exit no interceptions
        Environment.Exit(0);
      }
    }

    private delegate void dispatcherInvoker();

    private static void waitOrTimerCallback(Object state, Boolean timedOut)
    {
      Application app = (Application)state;
      app.Dispatcher.BeginInvoke(
              new dispatcherInvoker(delegate()
      {
        Application.Current.MainWindow.Activate();
      }),
              null
          );
    }
  }

  [SuppressUnmanagedCodeSecurity]
  internal class NativeMethods
  {
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    public static extern IntPtr SendMessage(IntPtr hWnd, int Msg,
        IntPtr wParam, ref COPYDATASTRUCT lParam);


    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct MyStruct
    {
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
      public string Message;
    }

    internal const int WM_COPYDATA = 0x004A;
    [StructLayout(LayoutKind.Sequential)]
    internal struct COPYDATASTRUCT
    {
      public IntPtr dwData;       // Specifies data to be passed
      public int cbData;          // Specifies the data size in bytes
      public IntPtr lpData;       // Pointer to data to be passed
    }
  }
}
