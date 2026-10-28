param([string]$Node, [string]$Wrapper, [string]$Payload)
$ErrorActionPreference = 'Stop'
Add-Type -TypeDefinition @'
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
public static class CloudEdgeStepJob {
  [StructLayout(LayoutKind.Sequential)] struct BasicLimit {
    public long ProcessTime, JobTime;
    public uint Flags;
    public UIntPtr MinimumWorkingSet, MaximumWorkingSet;
    public uint ActiveProcesses;
    public UIntPtr Affinity;
    public uint PriorityClass, SchedulingClass;
  }
  [StructLayout(LayoutKind.Sequential)] struct IoCounters {
    public ulong ReadOperations, WriteOperations, OtherOperations, ReadBytes, WriteBytes, OtherBytes;
  }
  [StructLayout(LayoutKind.Sequential)] struct ExtendedLimit {
    public BasicLimit Basic;
    public IoCounters Io;
    public UIntPtr ProcessMemory, JobMemory, PeakProcessMemory, PeakJobMemory;
  }
  [DllImport("kernel32.dll", CharSet=CharSet.Unicode, SetLastError=true)] static extern IntPtr CreateJobObject(IntPtr attributes, string name);
  [DllImport("kernel32.dll", SetLastError=true)] static extern bool SetInformationJobObject(IntPtr job, int kind, ref ExtendedLimit limits, uint size);
  [DllImport("kernel32.dll", SetLastError=true)] static extern bool AssignProcessToJobObject(IntPtr job, IntPtr process);
  [DllImport("kernel32.dll")] static extern bool CloseHandle(IntPtr handle);
  static void Require(bool success) { if (!success) throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error()); }
  public static int Run(string node, string wrapper, string payload) {
    IntPtr job = CreateJobObject(IntPtr.Zero, null);
    Require(job != IntPtr.Zero);
    Process child = null;
    bool assigned = false;
    try {
      ExtendedLimit limits = new ExtendedLimit();
      limits.Basic.Flags = 0x2000; // JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE
      Require(SetInformationJobObject(job, 9, ref limits, (uint)Marshal.SizeOf(typeof(ExtendedLimit))));
      // Only the fixed wrapper path enters the Windows command line. The
      // requested executable and argv cross stdin as JSON after assignment.
      ProcessStartInfo start = new ProcessStartInfo(node, "\"" + wrapper + "\"");
      start.UseShellExecute = false; start.CreateNoWindow = true;
      start.RedirectStandardInput = true;
      child = Process.Start(start);
      Require(AssignProcessToJobObject(job, child.Handle));
      assigned = true;
      byte[] command = Convert.FromBase64String(payload);
      child.StandardInput.BaseStream.Write(command, 0, command.Length);
      child.StandardInput.Close();
      child.WaitForExit();
      return child.ExitCode;
    } finally {
      // Closing the last job handle also removes orphaned descendants after
      // normal completion, timeout, abort or abrupt owner termination.
      CloseHandle(job);
      if (child != null) {
        if (!assigned && !child.HasExited) child.Kill();
        child.Dispose();
      }
    }
  }
}
'@
exit [CloudEdgeStepJob]::Run($Node, $Wrapper, $Payload)
