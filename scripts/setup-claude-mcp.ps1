# ============================================================
#  setup-claude-mcp.ps1
#  Adds the ProductCatalog MCP Server to Claude Desktop's config
#  (claude_desktop_config.json). Keeps any settings already there.
#  Works for both installs: Microsoft Store (MSIX) and normal.
# ============================================================
$ErrorActionPreference = 'Stop'
$root    = Split-Path $PSScriptRoot -Parent
$exe     = Join-Path $root 'src\McpServer\bin\Debug\net8.0\ProductCatalog.McpServer.exe'
$logDir  = Join-Path $root '.logs'
$log     = Join-Path $logDir 'setup-claude-mcp.log'
New-Item -ItemType Directory -Force -Path $logDir | Out-Null
"Run at $(Get-Date -Format s)" | Out-File $log -Encoding utf8

if (-not (Test-Path $exe)) {
    "ERROR: MCP Server not built yet: $exe. Run scripts\build-mcp.bat first." | Tee-Object -FilePath $log -Append
    exit 1
}

# Where Claude Desktop keeps its config
$targets = @()
$pkg = Get-ChildItem (Join-Path $env:LOCALAPPDATA 'Packages') -Directory -Filter 'Claude_*' -ErrorAction SilentlyContinue | Select-Object -First 1
if ($pkg) { $targets += (Join-Path $pkg.FullName 'LocalCache\Roaming\Claude') }
$classic = Join-Path $env:APPDATA 'Claude'
$targets += $classic   # always: some app versions read the normal (unvirtualized) folder
# Newer app versions move their data to %LOCALAPPDATA%\Claude-Data (or similar).
# Write there too, but only where a config file already exists.
foreach ($name in 'Claude-Data','Claude','Claude-3p') {
    $d = Join-Path $env:LOCALAPPDATA $name
    "--- check $d (top: $((Get-ChildItem $d -Name -ErrorAction SilentlyContinue) -join ', '))" | Out-File $log -Append -Encoding utf8
    $found = Get-ChildItem $d -Recurse -Depth 3 -Filter 'claude_desktop_config.json' -ErrorAction SilentlyContinue | Select-Object -First 3
    foreach ($f in $found) { "    found config: $($f.FullName)" | Out-File $log -Append -Encoding utf8; $targets += $f.DirectoryName }
}

$utf8NoBom = New-Object System.Text.UTF8Encoding $false

foreach ($dir in $targets) {
    New-Item -ItemType Directory -Force -Path $dir | Out-Null
    $file = Join-Path $dir 'claude_desktop_config.json'
    "--- $dir (files before: $((Get-ChildItem $dir -Name -ErrorAction SilentlyContinue) -join ', '))" | Out-File $log -Append -Encoding utf8

    if (Test-Path $file) {
        $raw = [System.IO.File]::ReadAllText($file)
        $config = if ($raw.Trim()) { $raw | ConvertFrom-Json } else { [pscustomobject]@{} }
        Copy-Item $file "$file.bak" -Force
    } else {
        $config = [pscustomobject]@{}
    }

    if (-not $config.PSObject.Properties['mcpServers']) {
        $config | Add-Member -NotePropertyName mcpServers -NotePropertyValue ([pscustomobject]@{})
    }
    $server = [pscustomobject]@{
        command = $exe
        env     = [pscustomobject]@{ ProductCatalogApi__BaseUrl = 'http://localhost:5233' }
    }
    if ($config.mcpServers.PSObject.Properties['product-catalog']) {
        $config.mcpServers.'product-catalog' = $server
    } else {
        $config.mcpServers | Add-Member -NotePropertyName 'product-catalog' -NotePropertyValue $server
    }

    $json = $config | ConvertTo-Json -Depth 10
    [System.IO.File]::WriteAllText($file, $json, $utf8NoBom)
    "OK: wrote $file" | Tee-Object -FilePath $log -Append
    $json | Out-File $log -Append -Encoding utf8
}

"DONE. Now quit Claude Desktop completely (tray icon > Quit) and open it again." | Tee-Object -FilePath $log -Append
