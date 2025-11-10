#if UNITY_EDITOR
using System;
using com.IvanMurzak.Unity.MCP;
using UnityEditor;

/// <summary>
/// Auto-configures the Unity MCP host from environment variables when the editor starts.
/// Prevents the plugin from spamming localhost when the CLI proxies via a different port.
/// </summary>
[InitializeOnLoad]
public static class McpHostBootstrap
{
    private const string PrimaryEnvVar = "UNITY_MCP_HOST";
    private const string SecondaryEnvVar = "MCP_SERVER_URL";

    static McpHostBootstrap()
    {
        TryConfigureHost();
    }

    static void TryConfigureHost()
    {
        string host = Environment.GetEnvironmentVariable(PrimaryEnvVar);
        if (string.IsNullOrWhiteSpace(host))
        {
            host = Environment.GetEnvironmentVariable(SecondaryEnvVar);
        }

        if (string.IsNullOrWhiteSpace(host))
        {
            UnityMcpPlugin.KeepConnected = false;
            return;
        }

        host = host.Trim();
        if (!host.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            host = $"http://{host}";
        }

        if (string.Equals(UnityMcpPlugin.Host, host, StringComparison.OrdinalIgnoreCase) &&
            UnityMcpPlugin.KeepConnected)
        {
            return;
        }

        UnityMcpPlugin.Host = host;
        UnityMcpPlugin.KeepConnected = true;
        UnityMcpPlugin.Save();
    }
}
#endif
